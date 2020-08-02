﻿/*
* Copyright (c) 2014, Furore (info@furore.com) and contributors
* See the file CONTRIBUTORS for details.
*
* This file is licensed under the BSD 3-Clause license
*/

using System;
using System.Linq;
using System.Text;


namespace Fhir.Metrics
{
    /// <summary>
    /// Extensions to convert Quantity components to a searchable string
    /// </summary>
    public static class SearchExtensions
    {
        ///<summary>
        /// Creates a string from a decimal that allows compare-from-left string searching 
        /// for finding values that fall within a the precision of a given string representing a decimal.
        /// Each Digit caries the significance of the original digits more to the right.
        ///</summary>
        private static string LeftSearchableNumberString(string original, string exponent)
        {
            StringBuilder leftSearchable = new StringBuilder(original);
            int reminder = 0;
            var exponentValue = int.Parse(exponent);

            for (int i = leftSearchable.Length - 1; i >= 0; i--)
            {
                if (leftSearchable[i] == '.') continue; // End of the decimal fraction part of the quantity

                // Add the reminder to the number on the left side
                int n = (int)char.GetNumericValue(leftSearchable[i]);
                n += reminder;

                reminder = n / 10; // Reset the reminder to 0 in case n < 10
                n = n % 10;
                if (n > 5)
                    reminder += 1;

                leftSearchable[i] = Convert.ToString(n)[0];
            }

            // Check if we still have a reminder that we need to carry to the left
            if (reminder != 0 && leftSearchable[0] == '0')
                exponentValue++;

            return $"E{exponentValue}x{leftSearchable}";
        }

        /// <summary>
        /// Transforms the value of a quantity to a string that can be compared from the left.
        /// Each Digit caries the significance of the original digits more to the right.
        /// </summary>
        public static string ValueAsSearchablestring(this Quantity quantity)
        {
            quantity = quantity.UnPrefixed();
            var value = quantity.Value.SignificandText;
            var exponent = quantity.Value.Exponent.ToString();
            var leftSearchable = LeftSearchableNumberString(value, exponent);
            return leftSearchable;
        }

        /// <summary>
        /// Transforms the units and the value of a quantity to a single string that can be compared from the left.
        /// Each Digit caries the significance of the original digits more to the right.
        /// </summary>
        public static string LeftSearchableString(this Quantity quantity)
        {
            quantity = quantity.UnPrefixed();
            var value = quantity.Value.SignificandText;
            var exponent = quantity.Value.Exponent.ToString();
            var leftSearchable = LeftSearchableNumberString(value, exponent);

            StringBuilder b = new StringBuilder();
            b.Append(quantity.Metric);
            b.Append(leftSearchable);
            return b.ToString();
        }
    }
}
