using System.Collections.Generic;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Provides information about the documentation comments of a method
    /// </summary>
    public class MethodCommentInformation
    {
        #region Public Properties

        /// <summary>
        /// The method's name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// The summary comments
        /// </summary>
	    public string SummaryComments { get; set; }

        /// <summary>
        /// The parameters that are used in the <see cref="SummaryComments"/>
        /// </summary>
        public IEnumerable<ParameterCommentInformation> SummaryCommentParameters { get; set; }

        /// <summary>
        /// The parameters' data
        /// </summary>
        public IEnumerable<ParameterCommentInformation> Parameters { get; set;}

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MethodCommentInformation()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Testing the specified <paramref name="comments"/>
        /// </summary>
        /// <param name="comments">Thee comments <paramref name="oof"/> <paramref name="parameterDatas"/></param>
        /// <param name="test">The test rgdrg
        /// drgrdgdrgdrgt</param>
        /// <param name="oof">Aaaaaaaa</param>
        /// <param name="parameterDatas">ola kala OPA</param>
        public void CreateCommentsData (string comments, string test, int oof, IEnumerable<ParameterCommentInformation> parameterDatas)
        {

        }

        public void GetCommentsData()
        {

        }

       
        /// <param name="comments"></param>
        /// <param name="test"></param>
        /// <param name="oof"></param>
        /// <param name="parameterDatas"></param>
        public void SetCommentsData(string comments, string test, int oof, IEnumerable<ParameterCommentInformation> parameterDatas)
        {

        }

        /// <summary>
        /// dwdwdwdw
        /// </summary>
        public void DeleteCommentsData(string comments, string test, int oof, IEnumerable<ParameterCommentInformation> parameterDatas)
        {

        }

        /// <summary>
        /// The comments datatatatatata
        /// </summary>
        /// <param name="comments">Thee comments <paramref name="oof"/> <paramref name="parameterDatas"/></param>
        /// <param name="test">The test rgdrg
        /// drgrdgdrgdrgt</param>
        /// <param name="oof">Aaaaaaaa</param>
        /// <param name="parameterDatas">ola kala OPA</param>
        public void DroseraComments(string comments, string test, int oof, IEnumerable<ParameterCommentInformation> parameterDatas)
        {

        }

        #endregion
    }

}
