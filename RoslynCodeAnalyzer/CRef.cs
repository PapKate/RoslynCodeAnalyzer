using Atom.Core;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the documentation comments of a  see cref 
    /// </summary>
    public class CRef
    {
        #region Public Properties

        /// <summary>
        /// The name of the cref
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The comment information that contains the cref
        /// </summary>
        public BaseCommentInformation Parent { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CRef(string name, BaseCommentInformation parent)
        {
            Name = name.NotNullOrEmpty();
            Parent = parent;
        }

        #endregion
    }
}
