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
    public class PropertyCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The parameter's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parameter's description
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// The cref that are used in the <see cref="Comments"/>
        /// </summary>
        public IEnumerable<ParameterCommentInformation> CommentCref { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropertyCommentInformation()
        {

        }

        #endregion
    }
}
