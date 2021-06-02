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
        /// The none syntax token
        /// </summary>
        public static SyntaxToken None { get; private set; }

        static void Main(string[] args)
        {
            // C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\MethodCommentInformation.cs
            var implementationFilePath = @"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\MethodCommentInformation.cs";

            // Reads all the text in the file path
            var implementation = File.ReadAllText(implementationFilePath);

            // Gets the syntax tree
            var tree = CSharpSyntaxTree.ParseText(implementation);
            
            // Gets the tree's root
            var root = tree.GetCompilationUnitRoot();

            // Gets the tree's members
            var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();

            var methodCommentsData = new List<MethodCommentInformation>();

            foreach (var member in members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                    // Gets the xml node
                    var xml = GetXml(property, property.Identifier, "method");

                    // Gets the summary element
                    var clean = GetSummary(xml, property.Identifier, "property");
                }

                // If the member is of type MethodDeclarationSyntax...
                if (member is MethodDeclarationSyntax method)
                {
                    // Gets the xml node
                    var xml = GetXml(method, method.Identifier, "method");

                    // Gets the summary element
                    var summary = GetSummary(xml, method.Identifier, "method");

                    var commentsData = new MethodCommentInformation() 
                    {
                        MethodName = method.Identifier.ToString(),
                        SummaryComments = GetSummaryComments(summary)
                    };

                    // Gets all the child nodes the have the start tag <param> and adds them to a list
                    var allParamNameAttributes = xml.ChildNodes().OfType<XmlElementSyntax>()
                                                    .Where(i => i.StartTag.Name.ToString().Equals(Constants.ParameterTag))
                                                    .ToList();

                    // If no comments for parameters is found...
                    if(allParamNameAttributes.Count == 0 
                        // And the actual method has parameters...
                        && method.ParameterList.Parameters.Count != 0)
                    {
                        // Prints message to the output console
                        HelperMethods.MissingParamCommentsOutputError(method.Identifier);
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

        /// <summary>
        /// Gets the xml node for the comments
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="identifier">The member's name </param>
        /// <param name="declarationSyntaxType">Whether a method or a property</param>
        /// <returns></returns>
        public static SyntaxNode GetXml(MemberDeclarationSyntax member, SyntaxToken identifier, string declarationSyntaxType)
        {
            // Gets the comments above the method
            var xmlCommentTrivia = member.GetLeadingTrivia().FirstOrDefault(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

            // Gets the comments' xml structure
            var xml = xmlCommentTrivia.GetStructure();

            // If there are no comments for the method...
            if (xmlCommentTrivia == null || xmlCommentTrivia.Token == None)
            {
                // Prints message to the output console
                Debug.WriteLine($"The {declarationSyntaxType} with name: {identifier} does NOT have any comments!");
                
                // Returns null
                return null;
            }

            // Returns the xml node
            return xml;
        }

        /// <summary>
        /// Gets the summary comments
        /// </summary>
        /// <param name="xml">The xml node</param>
        /// <param name="identifier">The member's name </param>
        /// <param name="declarationSyntaxType">Whether a method or a property</param>
        public static XmlElementSyntax GetSummary(SyntaxNode xml, SyntaxToken identifier, string declarationSyntaxType)
        {
            // Sets as summary the first xml's child of type XmlElementSyntax that has a start tag <summary>
            var summary = xml.ChildNodes()
                            .OfType<XmlElementSyntax>()
                            .FirstOrDefault(i => i.StartTag.Name.ToString().Equals(Constants.SummaryTag));

            // If there are no summary comments for the method...
            if (summary == null)
            {
                // Prints message to the output console
                HelperMethods.MissingSummaryCommentsOutputError(identifier, declarationSyntaxType);
                // Returns
                return null;
            }

            // Returns the summary syntax element
            return summary;
        }

        /// <summary>
        /// Gets the summary comments and formats them correctly
        /// </summary>
        /// <param name="summary">The summary</param>
        /// <returns></returns>
        public static string GetSummaryComments(XmlElementSyntax summary)
        {
            // Gets the text inside the <summary> </sumarry> area and...
            // Filters the string and removes the specified strings
            var clean = HelperMethods.FilterString(summary.Content.ToString(), Constants.CarriageReturn, Constants.NewLine, Constants.TripleSlashes);

            // Replaces the multiple spaces with a single one
            clean = HelperMethods.CleanStringFromExtraSpaces(clean);

            // Returns the "cleaned" version of the summary comments
            return clean;
        }

        /// <summary>
        /// Filters through summary and gets the empty elements
        /// </summary>
        /// <param name="summary">The summary</param>
        public static void FilterThroughEmptyElements(XmlElementSyntax summary)
        {
            var emptyElements = summary.ChildNodes().OfType<XmlEmptyElementSyntax>().ToList();

            var summaryCommentParameters = new List<ParameterCommentInformation>();

            foreach (var emptyElement in emptyElements)
            {
                var cref = emptyElement.ChildNodes().OfType<XmlCrefAttributeSyntax>().FirstOrDefault()
                    ?.ChildNodes().OfType<NameMemberCrefSyntax>().FirstOrDefault()
                    ?.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
                if (cref != null)
                    summaryCommentParameters.Add(new ParameterCommentInformation() { Name = cref.Identifier.ToString() });
                else
                {
                    var paramref = emptyElement.ChildNodes().OfType<XmlNameAttributeSyntax>().FirstOrDefault()
                        ?.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
                    if (paramref != null)
                        summaryCommentParameters.Add(new ParameterCommentInformation() { Name = paramref.Identifier.ToString() });
                }
            }
        }
    }


}
