﻿using System.Collections.Generic;
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
        /// The parameters that are used in the <see cref="BaseCommentInformation.Comments"/> <see cref="BaseCommentInformation.Name"/>
        /// </summary>
        public IEnumerable<ParameterCommentInformation> SummaryCommentParameters { get; set; } = Enumerable.Empty<ParameterCommentInformation>();

        /// <summary>
        /// The parameters' data
        /// </summary>
        public IEnumerable<ParameterCommentInformation> Parameters { get; set;} = Enumerable.Empty<ParameterCommentInformation>();

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
        /// Testing the specified <paramref name="comments"/> yes <see cref="BaseCommentInformation.Comments"/>
        /// </summary>
        /// <param name="comments">Thee comments <paramref name="oof"/> <see cref="BaseCommentInformation.Name"/> <paramref name="comments"/></param>
        /// <param name="test">The test rgdrg
        /// drgrdgdrgdrgt</param>
        /// <param name="oof">Aaaaaaaa <paramref name="comments"/></param>
        /// <param name="parameterDatas">ola kala OPA</param>
        public string CreateCommentsData (string comments, string test, int oof, IEnumerable<ParameterCommentInformation> parameterDatas)
        {
            return string.Empty;
        }

        /// <summary>
        /// The comments data
        /// </summary>
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
