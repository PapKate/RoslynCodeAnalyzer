﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// A class with helper methods
    /// </summary>
    public static class HelperMethods
    {
        #region Public Methods

        /// <summary>
        /// Filters a string and replaces the specified strings with <see cref="string.Empty"/>
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="stringsToReplace">The string values to be replaced</param>
        public static string FilterString(string value, params string[] stringsToReplace)
        {
            // For each string value...
            foreach (var stringToreplace in stringsToReplace)
                // Sets the value as the value with the string value replaced with empty
                value = value.Replace(stringToreplace, string.Empty);

            // Returns the edited value
            return value;
        }

        /// <summary>
        /// Removes extra spaces and replaces them with a single space
        /// </summary>
        /// <param name="value">The string</param>
        /// <returns></returns>
        public static string CleanStringFromExtraSpaces(string value)
        {
            // The new regEx with the array of spaces
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            // Replaces the multiple spaces with a single one
            value = regex.Replace(value, " ");
            
            // Returns the edited value
            return value;
        }
        
        #endregion

    }
}