using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
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
        public static string CleanedCommentsString(string value)
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



        #region Errors

        /// <summary>
        /// Prints message to the output console
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <returns></returns>
        public static string MissingParamCommentsOutputError(SyntaxToken identifier)
            => $"The method with name: {identifier} does NOT have any <param> comments </param> for its parameters";

        /// <summary>
        /// Prints message to the output console
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <param name="declarationSyntaxType">Whether a method or a property</param>
        /// <returns></returns>
        public static string MissingSummaryCommentsOutputError(SyntaxToken identifier, string declarationSyntaxType)
            => $"The {declarationSyntaxType} with name: {identifier} does NOT have any <summary> comments </summary>";

        #endregion

        #endregion

    }
}
