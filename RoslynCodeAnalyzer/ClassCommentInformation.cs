using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassCommentInformation : BaseCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The methods
        /// </summary>
        public List<MethodCommentInformation> Methods { get; set; } = new List<MethodCommentInformation>();

        /// <summary>
        /// The properties
        /// </summary>
        public List<PropertyCommentInformation> Properties { get; set; } = new List<PropertyCommentInformation>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClassCommentInformation()
        {

        }

        #endregion
    }
}
