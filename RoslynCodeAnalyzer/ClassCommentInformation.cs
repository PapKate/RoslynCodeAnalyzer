using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassCommentInformation : BaseCommentInformation
    {
        #region Internal Members

        /// <summary>
        /// The methods
        /// </summary>
        private readonly List<MethodCommentInformation> mMethodCommentInformations = new List<MethodCommentInformation>();

        /// <summary>
        /// The properties
        /// </summary>
        private readonly List<PropertyCommentInformation> mPropertyCommentInformations = new List<PropertyCommentInformation>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The methods
        /// </summary>
        public IEnumerable<MethodCommentInformation> Methods 
        {
            get { return mMethodCommentInformations; }
        }

        /// <summary>
        /// The properties
        /// </summary>
        public IEnumerable<PropertyCommentInformation> Properties 
        {
            get { return mPropertyCommentInformations; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClassCommentInformation()
        {

        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Adds a <paramref name="value"/> to the <see cref="mMethodCommentInformations"/>
        /// </summary>
        /// <param name="value">The value</param>
        internal void Add(MethodCommentInformation value)
        {
            // Adds to the member the value
            mMethodCommentInformations.Add(value);
        }

        /// <summary>
        /// Adds a <paramref name="value"/> to the <see cref="mPropertyCommentInformations"/>
        /// </summary>
        /// <param name="value">The value</param>
        internal void Add(PropertyCommentInformation value)
        {
            // Adds to the member the value
            mPropertyCommentInformations.Add(value);
        }

        #endregion
    }
}
