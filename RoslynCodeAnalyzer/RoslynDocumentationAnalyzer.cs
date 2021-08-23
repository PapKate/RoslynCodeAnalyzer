using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

using Atom.Core;


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
        /// Analyzers the files of the directory with the specified <paramref name="directoryPath"/> and
        /// extracts all the documentation information
        /// </summary>
        /// <param name="types"></param>
        /// <param name="directoryPath">The directory path</param>
        public IEnumerable<ClassCommentInformation> Analyze(IEnumerable<Type> types, string directoryPath)
        {
            // Gets the tree's members
            var members = GetAllFilesFromDirectory(directoryPath);

            var result = new List<ClassCommentInformation>();

            foreach(var member in members)
            {
                if(member is ClassDeclarationSyntax classSyntax)
                {
                    var name = classSyntax.Identifier.ToString();
                    if (types.Any(x => x.FullName.Contains(name)))
                    {
                        result.AddRange(AnalyzeClass(classSyntax, members));
                    }
                }
                else if(member is PropertyDeclarationSyntax propertySyntax)
                {
                    var name = propertySyntax.Identifier.ToString();
                    if (types.Any(x => x.FullName.Contains(name)))
                    {
                        AnalyzeProperty(propertySyntax, result);
                    }
                }
                // Else if the member is of type MethodDeclarationSyntax...
                else if (member is MethodDeclarationSyntax methodSyntax)
                {
                    var name = methodSyntax.Identifier.ToString();
                    if (types.Any(x => x.FullName.Contains(name)))
                    {
                        AnalyzeMethod(methodSyntax, result);
                    }
                }
            }

            foreach (var classCommentInformation in result)
            {
                foreach (var classType in classCommentInformation.BaseClasses)
                {
                    var baseClassCommentInformation = result.First(x => x.ClassType == classType);
                    classCommentInformation.AddRange(baseClassCommentInformation.Properties);
                    classCommentInformation.AddRange(baseClassCommentInformation.Methods);
                }
            }

            Console.WriteLine(result.FirstOrDefault().Name);
            result.FirstOrDefault().Properties.ForEach(x => Console.WriteLine(x.Name));
            result.FirstOrDefault().Methods.ForEach(x => Console.WriteLine(x.Name));

            return result;
        }

        private IEnumerable<ClassCommentInformation> AnalyzeClass(ClassDeclarationSyntax classSyntax, IEnumerable<MemberDeclarationSyntax> members)
        {
            var result = new List<ClassCommentInformation>();

            // Gets the xml node for the comments
            var xml = GetXml(classSyntax, classSyntax.Identifier, Constants.ClassTag);

            var summaryComments = string.Empty;

            // If the class has no comments...
            if (xml == null)
                // Prints in the out put console a message
                HelperMethods.MissingSummaryCommentsOutputError(classSyntax.Identifier.ToString(), Constants.ClassTag);
            // Else...
            else
            {
                // Gets the summary comments
                var summarySyntax = GetSummary(xml, classSyntax.Identifier, Constants.ClassTag);

                // Formats correctly the comments
                summaryComments = HelperMethods.CleanCommentString(summarySyntax.Content.ToString());
            }

            // Gets the class' name
            var className = classSyntax.Identifier.ToString();

            // Searches in the solution and gets the first type with name the class' name
            var classType = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .FirstOrDefault(t => t.Name == className);

            var baseClasses = GetBaseClasses(classType);

            // Creates a new class comment information object
            var classCommentInformation = new ClassCommentInformation(classType, summaryComments);

            baseClasses.ForEach(x => classCommentInformation.Add(x));

            foreach(var member in members)
            {
                if(baseClasses.Count() > 0)
                {
                    foreach(var baseClass in baseClasses)
                    {
                        if (member is ClassDeclarationSyntax classSyntaxT)
                        {
                            
                            var name = classSyntaxT.Identifier.ToString();
                            var memberType = AppDomain.CurrentDomain
                                            .GetAssemblies()
                                            .SelectMany(x => x.GetTypes())
                                            .FirstOrDefault(t => t.Name == name);
                            if (memberType.FullName.Contains(baseClass.FullName))
                            {
                                result.AddRange(AnalyzeClass(classSyntaxT, members));
                            }
                        }
                        //else if (member is PropertyDeclarationSyntax propertySyntaxT)
                        //{
                        //    var name = propertySyntaxT.Identifier.ToString();

                        //    if (propertySyntaxT.Identifier.ToString().Contains(name))
                        //    {
                        //        AnalyzeProperty(propertySyntaxT, result);
                        //    }
                        //}
                        //// Else if the member is of type MethodDeclarationSyntax...
                        //else if (member is MethodDeclarationSyntax methodSyntaxT)
                        //{
                        //    var name = methodSyntaxT.Identifier.ToString();
                        //    if (methodSyntaxT.Identifier.ToString().Contains(name))
                        //    {
                        //        AnalyzeMethod(methodSyntaxT, result);
                        //    }
                        //}
                    }    
                }
            }

            // Adds to the classes list the class
            result.Add(classCommentInformation);

            // Returns the result
            return result;
        }

        /// <summary>
        /// Gets all the base classes of a class
        /// </summary>
        /// <param name="classType">The class type</param>
        private IEnumerable<Type> GetBaseClasses(Type classType)
        {
            var result = new List<Type>();

            // Gets the base type of the given class
            var baseClass = classType.BaseType;
            // If the base class is NOT null...
            if (baseClass != null && baseClass.Name != Constants.ObjectClass)
            {
                result.Add(baseClass);
                // The method calls itself with the base class this time
                result.AddRange(GetBaseClasses(baseClass));
            }
            // Returns the result
            return result;
        }

        /// <summary>
        /// Analyzes a property
        /// </summary>
        /// <param name="member"></param>
        /// <param name="classes"></param>
        private void AnalyzeProperty(PropertyDeclarationSyntax member, IEnumerable<ClassCommentInformation> classes)
        {
            var propertyName = member.Identifier.ToString();

            var propertySummary = string.Empty;

            // A new list for the comments in the property's summary
            var propertyCommentCrefs = new List<CRef>();

            // Gets the comments above the method
            var xmlCommentTrivia = member.GetLeadingTrivia().FirstOrDefault(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);


            // If there are no comments for the method...
            if (xmlCommentTrivia == null || xmlCommentTrivia.Token == None)
            {
                HelperMethods.MissingSummaryCommentsOutputError(propertyName, Constants.PropertyTag);
            }
            else
            {
                // Gets the comments' xml structure
                var xml = xmlCommentTrivia.GetStructure();
                // Sets as summary the first xml's child of type XmlElementSyntax that has a start tag <summary>
                var summary = xml.ChildNodes()
                                .OfType<XmlElementSyntax>()
                                .FirstOrDefault(i => i.StartTag.Name.ToString().Equals(Constants.SummaryTag));
                // If there are no summary comments for the method...
                if (summary == null)
                {
                    // Prints message to the output console
                    HelperMethods.MissingSummaryCommentsOutputError(propertyName, Constants.PropertyTag);
                }
                else
                {
                    // Gets the text inside the <summary> </sumarry> area and...
                    // Filters the string and removes the specified strings
                    var clean = HelperMethods.CleanCommentString(summary.Content.ToString());

                    // Replaces the multiple spaces with a single one
                    propertySummary = HelperMethods.CleanStringFromExtraSpaces(clean);
                }

                // Creates a new property comment information
                var propertyCommentInformation = new PropertyCommentInformation(propertyName, propertySummary);

                propertyCommentCrefs = GetCRefFromParent(summary, propertyCommentInformation);

                // Sets as comment crefs the crefs found before 
                propertyCommentInformation.CommentCrefs = propertyCommentCrefs;

                // Finds the first class that its name the property's full name contains and...
                classes.First(x => member.Identifier.ToFullString().Contains(x.ClassType.FullName))
                                    // Adds the property comment info to its properties' list
                                    .Add(propertyCommentInformation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="classes"></param>
        private void AnalyzeMethod(MethodDeclarationSyntax member, IEnumerable<ClassCommentInformation> classes)
        {
            // Sets as the method's name the member's identifier
            var methodName = member.Identifier.ToString();

            var methodSummary = string.Empty;

            // Gets the comments above the method
            var xmlCommentTrivia = member.GetLeadingTrivia().FirstOrDefault(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);


            // If there are no comments for the method...
            if (xmlCommentTrivia == null || xmlCommentTrivia.Token == None)
            {
                HelperMethods.MissingSummaryCommentsOutputError(methodName, Constants.MethodTag);
            }
            else
            {
                // Gets the comments' xml structure
                var xml = xmlCommentTrivia.GetStructure();
                // Sets as summary the first xml's child of type XmlElementSyntax that has a start tag <summary>
                var summary = xml.ChildNodes()
                                .OfType<XmlElementSyntax>()
                                .FirstOrDefault(i => i.StartTag.Name.ToString().Equals(Constants.SummaryTag));
                // If there are no summary comments for the method...
                if (summary == null)
                {
                    // Prints message to the output console
                    HelperMethods.MissingSummaryCommentsOutputError(methodName, Constants.MethodTag);
                }
                else
                {
                    // Gets the text inside the <summary> </sumarry> area and...
                    // Filters the string and removes the specified strings
                    var clean = HelperMethods.CleanCommentString(summary.Content.ToString());

                    // Replaces the multiple spaces with a single one
                    methodSummary = HelperMethods.CleanStringFromExtraSpaces(clean);

                    // Creates a new method comment information with name the methodName and summary comments the summary
                    var methodCommentInformation = new MethodCommentInformation(methodName, methodSummary);

                    // Gets all the child nodes the have the start tag <param> and adds them to a list
                    var allParamNameAttributes = xml.ChildNodes().OfType<XmlElementSyntax>()
                                                    .Where(i => i.StartTag.Name.ToString().Equals(Constants.ParameterTag))
                                                    .ToList();

                    // If no comments for parameters is found...
                    if (allParamNameAttributes.Count == 0
                        // And the actual method has parameters...
                        && member.ParameterList.Parameters.Count != 0)
                    {
                        // Prints message to the output console
                        HelperMethods.MissingParamCommentsOutputError(methodName);
                    }

                    var tempParameters = new List<ParameterCommentInformation>();

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
                        var parameterType = member.ParameterList.Parameters.First(x => x.Identifier.ToString() == parameterName)
                                                                                        .Type.GetText();

                        // Creates and adds to the list the parameter data
                        tempParameters.Add(new ParameterCommentInformation(parameterName, cleanParamComments));

                        // Sets as the parameters of the comments data as the paramDataList
                        methodCommentInformation.Parameters = tempParameters;

                        methodCommentInformation.SummaryCommentParameters = GetParamRefsFromMethodSummary(summary, tempParameters);

                        methodCommentInformation.CommentCrefs = GetCRefFromParent(summary, methodCommentInformation);

                        // Finds the first class that its name the method's full name contains and...
                        classes.First(x => member.Identifier.ToFullString().Contains(x.Name))
                                            // Adds the method comment info to its methods' list
                                            .Add(methodCommentInformation);
                    }
                }
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
            var clean = HelperMethods.CleanCommentString(summary.Content.ToString());

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
        public List<ParameterCommentInformation> GetParamRefsFromMethodSummary(XmlElementSyntax summary, List<ParameterCommentInformation> parameterComments)
        {
            // Gets the empty elements of the summary
            var emptyElements = summary.ChildNodes().OfType<XmlEmptyElementSyntax>().ToList();

            // New List
            var summaryCommentParameters = new List<ParameterCommentInformation>();

            // For each empty element found in summary...
            foreach (var emptyElement in emptyElements)
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

            return summaryCommentParameters;
        }

        /// <summary>
        /// Filters through summary and gets the empty elements
        /// </summary>
        /// <param name="summary">The summary</param>
        /// <param name="parent">The parent</param>
        public List<CRef> GetCRefFromParent(XmlElementSyntax summary, BaseCommentInformation parent)
        {
            // Gets the empty elements of the summary
            var emptyElements = summary.ChildNodes().OfType<XmlEmptyElementSyntax>().ToList();

            // New List
            var summaryCommentParameters = new List<CRef>();

            // For each empty element found in summary...
            foreach (var emptyElement in emptyElements)
            {
                // Gets the empty element of type Cref attribute syntax if exists...
                var cref = emptyElement.ChildNodes().OfType<XmlCrefAttributeSyntax>().FirstOrDefault()
                    // Then the child node of type member Cref...
                    ?.ChildNodes().OfType<NameMemberCrefSyntax>().First()
                    // Then the child node of type Identifier name 
                    ?.ChildNodes().OfType<IdentifierNameSyntax>().First();

                // If a cref node exists...
                if (cref != null)
                    // Adds to the summary comments parameters a new parameterCommentsInformation with name the cref's identifier
                    summaryCommentParameters.Add(new CRef(cref.Identifier.ToString(), parent));
            }

            return summaryCommentParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<MemberDeclarationSyntax> GetAllFilesFromDirectory(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            var assemblyFile = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories).First(x => x.Name == "AssemblyInfo.cs");

            var netFile = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories).First(x => x.Name.Contains("NETFramework"));

            var files = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);

            var members = new List<MemberDeclarationSyntax>();

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
                members.AddRange(tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>());
            }
            return members;
        }

        #endregion

    }
}
