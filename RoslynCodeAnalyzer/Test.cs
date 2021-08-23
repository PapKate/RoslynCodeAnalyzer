using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// A class for tests 
    /// Inherits from <see cref="ClassCommentInformation"/>
    /// </summary>
    public class Test : ClassCommentInformation
    {
        /// <summary>
        /// Test string <see cref="ClassCommentInformation.Properties"/>
        /// </summary>
        public string StringTest { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Test(Type classType, string summary) : base(classType, summary)
        {

        }

        /// <summary>
        /// Testing the specified <paramref name="comments"/> yes <see cref="BaseCommentInformation.Summary"/>
        /// </summary>
        /// <param name="comments">Thee comments <paramref name="oof"/> <see cref="BaseCommentInformation.Name"/> <paramref name="comments"/></param>
        /// <param name="test">The test rgdrg
        /// drgrdgdrgdrgt</param>
        /// <param name="oof">Aaaaaaaa <paramref name="comments"/></param>
        /// <param name="parameterDatas">ola kala OPA</param>
        public string CreateCommentsData(string comments, string test, int oof, IEnumerable<ParameterCommentInformation> parameterDatas)
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
    }
}
