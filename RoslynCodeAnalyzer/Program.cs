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
            var xmlAnalyzer = new XmlAnalyzer();
            
            xmlAnalyzer.AnalyzeFile();

            var roslynAnalyzer = new RoslynAnalyzer();

            //roslynAnalyzer.AnalyzeFile();

            Console.ReadLine();
        }

    }


}
