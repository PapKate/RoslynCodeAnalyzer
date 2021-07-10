﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Responsible for analyzing .cs files with Roslyn 
    /// </summary>
    public class RoslynDocumentationAnalyzer : IDocumentationAnalyzer
    {
        #region Public Properties

        /// <summary>
        /// The none syntax token
        /// </summary>
        public static SyntaxToken None { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RoslynDocumentationAnalyzer()
        {

        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Analyzes a .cs file with Roslyn
        /// </summary>
        public void AnalyzeFile()
        {
            GetAllFilesFromDirectory();
            // C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer\MethodCommentInformation.cs
            var implementationFilePath = Constants.CsFilePath;

            // Reads all the text in the file path
            var implementation = File.ReadAllText(implementationFilePath);

            // Gets the syntax tree
            var tree = CSharpSyntaxTree.ParseText(implementation);

            // Gets the tree's root
            var root = tree.GetCompilationUnitRoot();

            // Gets the tree's members
            var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
            
            var classes = new List<ClassCommentInformation>();

            var multipleMethodCommentsData = new List<MethodCommentInformation>();

            var multiplePropertyCommentsData = new List<PropertyCommentInformation>();

            foreach(var member in members)
            {
                if (member is ClassDeclarationSyntax classSyntax)
                {
                    var xml = GetXml(classSyntax, classSyntax.Identifier, Constants.ClassTag);
                    var summaryComments = string.Empty;
                    // If the class has no comments...
                    if (xml == null)
                        // Prints in the out put console a message
                        HelperMethods.MissingSummaryCommentsOutputError(classSyntax.Identifier.ToString(), Constants.ClassTag);
                    else
                    {
                        var summarySyntax = GetSummary(xml, classSyntax.Identifier, Constants.ClassTag);

                        summaryComments = HelperMethods.CleanedCommentsString(summarySyntax.Content.ToString());
                    }

                    // Creates a new class comment information object
                    var classCommentInformation = new ClassCommentInformation()
                    {
                        Name = classSyntax.Identifier.ToString(),
                        Comments = summaryComments
                    };

                    // Adds to the classes list the class
                    classes.Add(classCommentInformation);
                }
            }

            foreach (var member in members)
            {
                if(member is ClassDeclarationSyntax classSyntax)
                {
                    var xml = GetXml(classSyntax, classSyntax.Identifier, Constants.ClassTag);
                    var baseClasses = xml.ChildNodes()
                                    .OfType<XmlElementSyntax>()
                                    .FirstOrDefault(i => i.GetType() == typeof(BaseListSyntax));
                    //classes.First(x => x.Name == classSyntax.Identifier.ToString()).Add();
                }
                else if (member is PropertyDeclarationSyntax propertySyntax)
                {
                    // Gets the xml node
                    var xml = GetXml(propertySyntax, propertySyntax.Identifier, Constants.PropertyTag);

                    // Gets the summary element
                    var clean = GetSummary(xml, propertySyntax.Identifier, Constants.PropertyTag);

                    // Gets the first child node of type XmlElementSyntax and filter's through it and gets all the cref elements
                    var list = FilterThroughEmptyElements(xml.ChildNodes().OfType<XmlElementSyntax>().First());

                    // Creates and adds to the multiple properties list a property comment information with...
                    // Name the property's identifier, 
                    // Comments the clean version of summary
                    // CommentCref the list with the cref elements
                    multiplePropertyCommentsData.Add(new PropertyCommentInformation()
                    { 
                        Name = propertySyntax.Identifier.ToString(),
                        Comments = clean.ToString(),
                        CommentParameters = list
                    });
                }

                // Else if the member is of type MethodDeclarationSyntax...
                else if (member is MethodDeclarationSyntax methodSyntax)
                {
                    // Gets the xml node
                    var xml = GetXml(methodSyntax, methodSyntax.Identifier, Constants.MethodTag);

                    if (xml == null)
                        continue;

                    // Gets the summary element
                    var summary = GetSummary(xml, methodSyntax.Identifier, Constants.MethodTag);

                    if (summary == null)
                        continue;

                    var methodCommentsData = new MethodCommentInformation()
                    {
                        Name = methodSyntax.Identifier.ToString(),
                        Comments = GetSummaryComments(summary)
                    };

                    // Gets all the child nodes the have the start tag <param> and adds them to a list
                    var allParamNameAttributes = xml.ChildNodes().OfType<XmlElementSyntax>()
                                                    .Where(i => i.StartTag.Name.ToString().Equals(Constants.ParameterTag))
                                                    .ToList();

                    // If no comments for parameters is found...
                    if (allParamNameAttributes.Count == 0
                        // And the actual method has parameters...
                        && methodSyntax.ParameterList.Parameters.Count != 0)
                    {
                        // Prints message to the output console
                        HelperMethods.MissingParamCommentsOutputError(methodSyntax.Identifier.ToString());
                        // Returns
                        continue;
                    }

                    var paramDataList = new List<ParameterCommentInformation>();

                    // For each parameter data in allParamNameAttributes...
                    foreach (var paramData in allParamNameAttributes)
                    {
                        // Filters the paramData's Content and removes the specified strings
                        var cleanParamComments = HelperMethods.FilterString(paramData.Content.ToString(), Constants.CarriageReturn, Constants.NewLine, Constants.TripleSlashes);
                        // Replaces the multiple spaces with a single one
                        cleanParamComments = HelperMethods.CleanStringFromExtraSpaces(cleanParamComments);

                        // Sets as parameter name from paramData gets the first object with start tag that has a type of XmlNameAttributeSyntax and gets its identifier
                        var parameterName = paramData.StartTag.Attributes.OfType<XmlNameAttributeSyntax>()
                                            .FirstOrDefault().Identifier.ToString();

                        // Gets the parameter's type as string
                        // TODO: fix -> get type
                        var parameterType = methodSyntax.ParameterList.Parameters.First(x => x.Identifier.ToString() == parameterName)
                                                                                        .Type.GetText();

                        // Creates and adds to the list the parameter data
                        paramDataList.Add(new ParameterCommentInformation()
                        {
                            Name = parameterName,
                            Comments = cleanParamComments,
                            XmlElement = paramData
                        });
                    }

                    paramDataList.ForEach(x => x.CommentParameters = FilterThroughEmptyElements(x.XmlElement, paramDataList));

                    // Sets as the parameters of the comments data as the paramDataList
                    methodCommentsData.Parameters = paramDataList;

                    methodCommentsData.SummaryCommentParameters = FilterThroughEmptyElements(summary, paramDataList);

                    // Adds to the methodCommentsData the commentsData
                    multipleMethodCommentsData.Add(methodCommentsData);
                }
            }

            foreach (var methodData in multipleMethodCommentsData)
            {
                Console.WriteLine($"Method Name : {methodData.Name}\nSummary : {methodData.Comments}\n");
            }
        }

        /// <summary>
        /// Gets the xml node for the comments
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="identifier">The member's name </param>
        /// <param name="declarationSyntaxType">Whether a method or a property</param>
        /// <returns></returns>
        public SyntaxNode GetXml(MemberDeclarationSyntax member, SyntaxToken identifier, string declarationSyntaxType)
        {
            // Gets the comments above the method
            var xmlCommentTrivia = member.GetLeadingTrivia().FirstOrDefault(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);


            // If there are no comments for the method...
            if (xmlCommentTrivia == null || xmlCommentTrivia.Token == None)
            {
                // Prints message to the output console
                Debug.WriteLine($"The {declarationSyntaxType} with name: {identifier} does NOT have any comments!");
                
                return null;
            }

            // Gets the comments' xml structure
            var xml = xmlCommentTrivia.GetStructure();

            // Returns the xml node
            return xml;
        }

        /// <summary>
        /// Gets the summary comments
        /// </summary>
        /// <param name="xml">The xml node</param>
        /// <param name="identifier">The member's name </param>
        /// <param name="declarationSyntaxType">Whether a method or a property</param>
        public XmlElementSyntax GetSummary(SyntaxNode xml, SyntaxToken identifier, string declarationSyntaxType)
        {
            // Sets as summary the first xml's child of type XmlElementSyntax that has a start tag <summary>
            var summary = xml.ChildNodes()
                            .OfType<XmlElementSyntax>()
                            .FirstOrDefault(i => i.StartTag.Name.ToString().Equals(Constants.SummaryTag));

            // If there are no summary comments for the method...
            if (summary == null)
            {
                // Prints message to the output console
                HelperMethods.MissingSummaryCommentsOutputError(xml.ToString(), declarationSyntaxType);
            }

            // Returns the summary syntax element
            return summary;
        }

        /// <summary>
        /// Gets the summary comments and formats them correctly
        /// </summary>
        /// <param name="summary">The summary</param>
        /// <returns></returns>
        public string GetSummaryComments(XmlElementSyntax summary)
        {
            // Gets the text inside the <summary> </sumarry> area and...
            // Filters the string and removes the specified strings
            var clean = HelperMethods.CleanedCommentsString(summary.Content.ToString());

            // Replaces the multiple spaces with a single one
            clean = HelperMethods.CleanStringFromExtraSpaces(clean);

            // Returns the "cleaned" version of the summary comments
            return clean;
        }

        /// <summary>
        /// Filters through summary and gets the empty elements
        /// </summary>
        /// <param name="summary">The summary</param>
        /// <param name="parameterComments"></param>
        public List<ParameterCommentInformation> FilterThroughEmptyElements(XmlElementSyntax summary, List<ParameterCommentInformation> parameterComments)
        {
            // Gets the empty elements of the summary
            var emptyElements = summary.ChildNodes().OfType<XmlEmptyElementSyntax>().ToList();

            // New List
            var summaryCommentParameters = new List<ParameterCommentInformation>();

            // For each empty element found in summary...
            foreach (var emptyElement in emptyElements)
            {
                // Gets the empty element of type Cref attribute syntax if exists...
                var cref = emptyElement.ChildNodes().OfType<XmlCrefAttributeSyntax>().FirstOrDefault()
                    // Then the child node of type member Cref...
                    ?.ChildNodes().OfType<NameMemberCrefSyntax>().First()
                    // Then the child node of type Identifier name 
                    .ChildNodes().OfType<IdentifierNameSyntax>().First();

                // If a cref node exists...
                if (cref != null)
                    // Adds to the summary comments parameters a new parameterCommentsInformation with name the cref's identifier
                    summaryCommentParameters.Add(new ParameterCommentInformation() { Name = cref.Identifier.ToString() });
                // Else...
                else
                {
                    // Gets the empty element's child node of type Name attribute if exists...
                    var paramref = emptyElement.ChildNodes().OfType<XmlNameAttributeSyntax>().FirstOrDefault()
                        // Then the child node of type Identifier name 
                        ?.ChildNodes().OfType<IdentifierNameSyntax>().First();

                    // If a param ref exists...
                    if (paramref != null)
                            // Adds to the summary comments parameters a new parameterCommentsInformation with name the param ref's identifier
                            summaryCommentParameters.Add(parameterComments.First(x => x.Name == paramref.Identifier.ToString()));
                }
            }

            return summaryCommentParameters;
        }

        /// <summary>
        /// Filters through summary and gets the empty elements
        /// </summary>
        /// <param name="summary">The summary</param>
        public List<ParameterCommentInformation> FilterThroughEmptyElements(XmlElementSyntax summary)
        {
            // Gets the empty elements of the summary
            var emptyElements = summary.ChildNodes().OfType<XmlEmptyElementSyntax>().ToList();

            // New List
            var summaryCommentParameters = new List<ParameterCommentInformation>();

            // For each empty element found in summary...
            foreach (var emptyElement in emptyElements)
            {
                // Gets the empty element of type Cref attribute syntax if exists...
                var cref = emptyElement.ChildNodes().OfType<XmlCrefAttributeSyntax>().FirstOrDefault()
                    // Then the child node of type member Cref...
                    ?.ChildNodes().OfType<NameMemberCrefSyntax>().First()
                    // Then the child node of type Identifier name 
                    .ChildNodes().OfType<IdentifierNameSyntax>().First();

                // If a cref node exists...
                if (cref != null)
                    // Adds to the summary comments parameters a new parameterCommentsInformation with name the cref's identifier
                    summaryCommentParameters.Add(new ParameterCommentInformation() { Name = cref.Identifier.ToString() });
            }

            return summaryCommentParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetAllFilesFromDirectory()
        {
            var directoryInfo = new DirectoryInfo(@"C:\Users\PapKate\Documents\PersonalProjects\C#\RoslynCodeAnalyzer\RoslynCodeAnalyzer");

            var assemblyFile = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories).First(x => x.Name == "AssemblyInfo.cs");

            var netFile = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories).First(x => x.Name.Contains("NETFramework"));

            var files = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);

            foreach (var file in directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                var implementationFilePath = file.FullName;

                // Reads all the text in the file path
                var implementation = File.ReadAllText(implementationFilePath);

                // Gets the syntax tree
                var tree = CSharpSyntaxTree.ParseText(implementation);

                // Gets the tree's root
                var root = tree.GetCompilationUnitRoot();

                // Gets the tree's members
                var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
                Console.WriteLine(file.Name);
            }
        }

        public void Analyze(IEnumerable<Type> types, string directoryPath)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}