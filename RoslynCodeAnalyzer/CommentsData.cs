using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Class commmmetns
    /// </summary>
    public class CommentsData
    {
        #region Public Properties

        /// <summary>
        /// The summary comments
        /// </summary>
	    public string SummaryComments { get; set; }

        /// <summary>
        /// The parameters' data
        /// </summary>
        public IEnumerable<ParameterData> Parameters { get; set;}

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CommentsData()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Testing comments dataa
        /// fffffff
        /// </summary>
        /// <param name="comments">Thee comments</param>
        /// <param name="test">The test rgdrg
        /// drgrdgdrgdrgt</param>
        /// <param name="oof">Aaaaaaaa</param>
        /// <param name="parameterDatas">ola kala OPA</param>
        public static void CreateCommentsData (string comments, string test, int oof, IEnumerable<ParameterData> parameterDatas)
        {

        }

        #endregion
    }

}
