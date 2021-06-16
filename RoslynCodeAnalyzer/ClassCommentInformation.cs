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
        internal List<MethodCommentInformation> mMethods = new List<MethodCommentInformation>();

        /// <summary>
        /// The properties
        /// </summary>
        internal List<PropertyCommentInformation> mProperties = new List<PropertyCommentInformation>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The methods
        /// </summary>
        public IEnumerable<MethodCommentInformation> Methods 
        {
            get { return mMethods; }
            set { mMethods = (List<MethodCommentInformation>)value; } 
        }

        /// <summary>
        /// The properties
        /// </summary>
        public IEnumerable<PropertyCommentInformation> Properties 
        {
            get { return mProperties; }
            set { mProperties = (List<PropertyCommentInformation>)value; } 
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
    }
}
