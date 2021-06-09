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
        public string Name { get; set; }

        /// <summary>
        /// The comments
        /// </summary>
        public string Comments { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseCommentInformation()
        {

        }

        #endregion

    }
}
