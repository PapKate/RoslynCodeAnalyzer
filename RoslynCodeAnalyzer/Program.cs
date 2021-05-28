using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoslynCodeAnalyzer
{
    public class Program
    {
        /// <summary>
        /// The main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            // C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\CommentsData.cs
            var implementationFilePath = @"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\CommentsData.cs";

            var implementation = File.ReadAllText(implementationFilePath);

            var tree = CSharpSyntaxTree.ParseText(implementation);
            var root = tree.GetCompilationUnitRoot();

            var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();

            foreach (var member in members)
            {
                if (member is PropertyDeclarationSyntax property)
                    Console.WriteLine("Property: " + property.Identifier);

                if (member is MethodDeclarationSyntax method)
                {
                    Console.WriteLine("Method: " + method.Identifier);
                    var xmlCommentTrivia = method.GetLeadingTrivia().Single(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
                    var xml = xmlCommentTrivia.GetStructure();

                    var summary = xml.ChildNodes()
                                    .OfType<XmlElementSyntax>()
                                    .FirstOrDefault(i => i.StartTag.Name.ToString().Equals("summary"));

                    var children = summary.ChildNodes().OfType<XmlTextSyntax>().Single().GetText();

                    var clean = string.Empty;

                    clean = children.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("///", string.Empty);

                    var options = RegexOptions.None;
                    var regex = new Regex("[ ]{2,}", options);
                    clean = regex.Replace(clean, " ");

                    var allParamNameAttributes = xml.ChildNodes().OfType<XmlElementSyntax>()
                                                    .Where(i => i.StartTag.Name.ToString().Equals("param"))
                                                    .SelectMany(i => i.StartTag.Attributes.OfType<XmlNameAttributeSyntax>());

                    // Gets the parameter's type
                    var type = method.ParameterList.Parameters[3].Type.GetText();
                    var paramName = method.ParameterList.Parameters[3].Identifier;


                    Console.WriteLine(xml);

                }
            }


            Console.ReadLine();
        }

        private static void WriteLine(string v)
        {
            throw new NotImplementedException();
        }
    }
}
