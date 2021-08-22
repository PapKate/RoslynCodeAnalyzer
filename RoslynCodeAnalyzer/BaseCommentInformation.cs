using Atom.Core;

using System;
using System.Diagnostics;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the basic document comments
    /// </summary>
    public abstract class BaseCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The summary comment
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// The declaration syntax type
        /// </summary>
        public DeclarationSyntaxType DeclarationSyntaxType { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="declarationSyntaxType">The declaration syntax type</param>
        /// <param name="name">The name</param>
        /// <param name="summary">The summary comment</param>
        public BaseCommentInformation(DeclarationSyntaxType declarationSyntaxType, string name, string summary)
        {
            Name = name.NotNullOrEmpty();
            Summary = summary;

            Debug.WriteLine($"The are no comments for the {declarationSyntaxType} with name: {name} does NOT have any <summary> comments </summary>");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;

        #endregion
    }
}
