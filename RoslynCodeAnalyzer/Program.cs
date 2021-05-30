using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoslynCodeAnalyzer
{
    public class Program
    {
        public static Microsoft.CodeAnalysis.SyntaxToken None { get; private set; }

        static void Main(string[] args)
        {

            // C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\CommentsData.cs
            var implementationFilePath = @"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\CommentsData.cs";

            var implementation = File.ReadAllText(implementationFilePath);

            var tree = CSharpSyntaxTree.ParseText(implementation);
            var root = tree.GetCompilationUnitRoot();

            var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();

            var methodCommentsData = new List<MethodCommentInformation>();

            foreach (var member in members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                   
                }

                // If the member is of type MethodDeclarationSyntax...
                if (member is MethodDeclarationSyntax method)
                {
                    // Gets the comments above the method
                    var xmlCommentTrivia = method.GetLeadingTrivia().FirstOrDefault(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

                    // If there are no comments for the method...
                    if (xmlCommentTrivia == null || xmlCommentTrivia.Token == None)
                    {
                        // Prints message to the output console
                        Debug.WriteLine($"The method with name: {method.Identifier} does NOT have any comments!");
                        // Returns
                        continue;
                    }

                    // Gets the comments' xml structure
                    var xml = xmlCommentTrivia.GetStructure();

                    // Sets as summary the first xml's child of type XmlElementSyntax that has a start tag <summary>
                    var summary = xml.ChildNodes()
                                    .OfType<XmlElementSyntax>()
                                    .FirstOrDefault(i => i.StartTag.Name.ToString().Equals("summary"));
                    
                    // If there are no summary comments for the method...
                    if(summary == null)
                    {
                        // Prints message to the output console
                        Debug.WriteLine($"The method with name: {method.Identifier}  does NOT have any <summary> comments </summary>");
                        // Returns
                        continue;
                    }

                    // Gets the text inside the <summary> </sumarry> area
                    var summaryComments = summary.ChildNodes().OfType<XmlTextSyntax>().FirstOrDefault().GetText();

                    // Filters the string and removes the specified strings
                    var clean = HelperMethods.FilterString(summaryComments.ToString(), "\r", "\n", "///");

                    // Replaces the multiple spaces with a single one
                    clean = HelperMethods.CleanStringFromExtraSpaces(clean);

                    var commentsData = new MethodCommentInformation() 
                    {
                        MethodName = method.Identifier.ToString(),
                        SummaryComments = clean 
                    };

                    // Gets all the child nodes the have the start tag <param> and adds them to a list
                    var allParamNameAttributes = xml.ChildNodes().OfType<XmlElementSyntax>()
                                                    .Where(i => i.StartTag.Name.ToString().Equals("param"))
                                                    .ToList();
                    // If not comments for parameters is found...
                    if(allParamNameAttributes.Count == 0 
                        // And the actual method has parameters...
                        && method.ParameterList.Parameters.Count != 0)
                    {
                        // Prints message to the output console
                        Debug.WriteLine($"The method with name: {method.Identifier} does NOT have any <param> comments </param> for its parameters");
                        // Returns
                        continue;
                    }

                    var paramDataList = new List<ParameterCommentInformation>();

                    // For each parameter data in allParamNameAttributes...
                    foreach (var paramData in allParamNameAttributes)
                    {
                        // Filters the paramData's Content and removes the specified strings
                        var cleanParamComments = HelperMethods.FilterString(paramData.Content.ToString(), "\r", "\n", "///");
                        // Replaces the multiple spaces with a single one
                        cleanParamComments = HelperMethods.CleanStringFromExtraSpaces(cleanParamComments);
                       
                        // Sets as parameter name from paramData gets the first object with start tag that has a type of XmlNameAttributeSyntax and gets its identifier
                        var parameterName = paramData.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault().Identifier.ToString();
                        
                        // Gets the parameter's type
                        var parameterType = method.ParameterList.Parameters.FirstOrDefault(x => x.Identifier.ToString() == parameterName).Type.GetText();

                        // Creates and adds to the list the parameter data
                        paramDataList.Add(new ParameterCommentInformation()
                        { 
                            Name = parameterName,
                            Comments = cleanParamComments,
                        });
                    }

                    // Sets as the parameters of the comments data as the paramDataList
                    commentsData.Parameters = paramDataList;
                    
                    // Adds to the methodCommentsData the commentsData
                    methodCommentsData.Add(commentsData);
                }
            }

            foreach (var methodData in methodCommentsData)
            {
                Console.WriteLine($"Method Name : {methodData.MethodName}\nSummary : {methodData.SummaryComments}\n");
            }

            Console.ReadLine();
        }

        public void GetSummaryComments(MemberDeclarationSyntax member, SyntaxToken identifier)
        {
            // Gets the comments above the method
            var xmlCommentTrivia = member.GetLeadingTrivia().FirstOrDefault(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

            // If there are no comments for the method...
            if (xmlCommentTrivia == null || xmlCommentTrivia.Token == None)
            {
                // Prints message to the output console
                Debug.WriteLine($"The method with name: {identifier} does NOT have any comments!");
                // Returns
                //continue;
            }

            // Gets the comments' xml structure
            var xml = xmlCommentTrivia.GetStructure();

            // Sets as summary the first xml's child of type XmlElementSyntax that has a start tag <summary>
            var summary = xml.ChildNodes()
                            .OfType<XmlElementSyntax>()
                            .FirstOrDefault(i => i.StartTag.Name.ToString().Equals("summary"));

            // If there are no summary comments for the method...
            if (summary == null)
            {
                // Prints message to the output console
                Debug.WriteLine($"The method with name: {identifier}  does NOT have any <summary> comments </summary>");
                // Returns
                //continue;
            }

            // Gets the text inside the <summary> </sumarry> area
            var summaryComments = summary.ChildNodes().OfType<XmlTextSyntax>().FirstOrDefault().GetText();

            // Filters the string and removes the specified strings
            var clean = HelperMethods.FilterString(summaryComments.ToString(), "\r", "\n", "///");

            // Replaces the multiple spaces with a single one
            clean = HelperMethods.CleanStringFromExtraSpaces(clean);
        }
    }


}
