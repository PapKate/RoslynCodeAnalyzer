using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

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

        /// <summary>
        /// Cleans and formats the summary comments
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanCommentString(string value)
        {
            // Filters the string and removes the specified strings
            value = FilterString(value, Constants.CarriageReturn, Constants.NewLine, Constants.TripleSlashes);

            // Replaces multiple spaces with one
            value = CleanStringFromExtraSpaces(value);

            // Returns the new value
            return value;
        }

        /// <summary>
        /// Returns the node's name attribute's value
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <returns></returns>
        public static string GetAttributeName(this XmlNode node) => node.Attributes.GetNamedItem(Constants.NameTag).Value;

        /// <summary>
        /// Returns the node's tag attribute's value
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="tag">The attributes's name</param>
        /// <returns></returns>
        public static string GetAttributeName(this XmlNode node, string tag) => node.Attributes.GetNamedItem(tag).Value;

        /// <summary>
        /// Gets the last character set that are after the last "."
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static string GetLastNameAfterDot(this string value)
        {
            // Sets as value the character set after the last "."
            return value.Substring(value.LastIndexOf(Constants.Dot) + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetFullClassName(this string value)
        {
            // Sets as value the character set after the last "."
            var newValue = value.Substring(value.IndexOf(':') + 1);
            
            // If it is a class' name
            if (value.StartsWith("T:"))
                // Returns the value
                return newValue;
            else
            {
                var splits = newValue.Split('.');
                newValue = splits[0] + '.' + splits[1];
                return newValue; 
            }
        }

        #region Errors

        /// <summary>
        /// Prints message to the output console
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <returns></returns>
        public static string MissingParamCommentsOutputError(string identifier)
        {
            var value = $"The method with name: {identifier} does NOT have any <param> comments </param> for its parameters";
          
            Debug.WriteLine(value);
            
            return value;
        }

        /// <summary>
        /// Prints message to the output console
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="declarationSyntaxType">Whether a method or a property</param>
        /// <returns></returns>
        public static string MissingSummaryCommentsOutputError(string name, string declarationSyntaxType)
        {
            var value = $"The {declarationSyntaxType} with name: {name} does NOT have any <summary> comments </summary>";

            Debug.WriteLine(value);

            return value;
        }

        #endregion

        #endregion

    }
}
