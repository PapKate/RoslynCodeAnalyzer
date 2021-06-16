namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the documentation comments of a cref
    /// </summary>
    public class CrefCommentInformation : BaseCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The class
        /// </summary>
        public ClassCommentInformation Class { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrefCommentInformation()
        {

        }

        #endregion
    }
}
