
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the documentation comments of a parameter of a method
    /// </summary>
    public class ParameterCommentInformation : BaseCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The parameters that are used in the <see cref="BaseCommentInformation.Summary"/>
        /// </summary>
        public IEnumerable<ParameterCommentInformation> CommentParameters { get; set; } = Enumerable.Empty<ParameterCommentInformation>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="summary">The summary comment</param>
        public ParameterCommentInformation(string name, string summary) : base(DeclarationSyntaxType.Parameter, name, summary)
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="declarationSyntaxType">The declaration syntax type</param>
        /// <param name="name">The name</param>
        /// <param name="summary">The summary comment</param>
        private protected ParameterCommentInformation(DeclarationSyntaxType declarationSyntaxType, string name, string summary) : base(declarationSyntaxType, name, summary)
        {

        }

        #endregion
    }
}
