using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the document comments of a parameter of a method
    /// </summary>
    public class ParameterCommentInformation : BaseCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The xml element
        /// </summary>
        public XmlElementSyntax XmlElement { get; set; }

        /// <summary>
        /// The parameters that are used in the <see cref="BaseCommentInformation.Comments"/>
        /// </summary>
        public IEnumerable<ParameterCommentInformation> CommentParameters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParameterCommentInformation()
        {

        }

	    #endregion
    }
}
