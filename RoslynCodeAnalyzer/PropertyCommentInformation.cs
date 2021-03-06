using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the document comments of a property
    /// </summary>
    public class PropertyCommentInformation : BaseCommentInformation
    {
        #region Public Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="summary">The summary comment</param>
        public PropertyCommentInformation(string name, string summary) : base(DeclarationSyntaxType.Property, name, summary)
        {

        }

        #endregion
    }
}
