using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// The main class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var xmlAnalyzer = new XmlDocumentationAnalyzer();
            
            var result = xmlAnalyzer.Analyze(new List<Type>() { typeof(Test) }, Constants.XmlFilePath);

            var roslynAnalyzer = new RoslynDocumentationAnalyzer();

            //roslynAnalyzer.AnalyzeFile();

            Console.ReadLine();
        }

    }


}
