using System.Collections.Generic;
using System.Linq;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the documentation comments of a method
    /// </summary>
    public class MethodCommentInformation : BaseCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The parameters that are used in the <see cref="BaseCommentInformation.Summary"/> 
        /// </summary>
        public IEnumerable<ParameterCommentInformation> SummaryCommentParameters { get; set; } = Enumerable.Empty<ParameterCommentInformation>();

        /// <summary>
        /// The parameters that are used in the <see cref="BaseCommentInformation.Summary"/> 
        /// </summary>
        public IEnumerable<CRef> SummaryCommentCRefs { get; set; } = Enumerable.Empty<CRef>();


        /// <summary>
        /// The parameters' data
        /// </summary>
        public IEnumerable<ParameterCommentInformation> Parameters { get; set;} = Enumerable.Empty<ParameterCommentInformation>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="summary">The summary comment</param>
        public MethodCommentInformation(string name, string summary) : base(DeclarationSyntaxType.Method, name, summary)
        {

        }

        #endregion

        #region Public Methods

      

        #endregion
    }

}
