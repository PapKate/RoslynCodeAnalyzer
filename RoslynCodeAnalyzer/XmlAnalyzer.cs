using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Responsible for analyzing a .xml file
    /// </summary>
    public class XmlAnalyzer
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public XmlAnalyzer()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads and analyzes an xml file
        /// </summary>
        public void AnalyzeFile()
        {
            var doc = new XmlDocument();
            doc.Load(@"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\RoslynCodeAnalyzer.xml");

            XmlNode test = doc.DocumentElement.SelectSingleNode("/book/title");

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                string text = node.InnerText; //or loop through its children as well
                string attr = node.Attributes["theattributename"]?.InnerText;
            }

        }

        #endregion

    }
}
