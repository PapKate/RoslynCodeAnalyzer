
using Atom.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Responsible for analyzing a .xml file
    /// </summary>
    public class XmlDocumentationAnalyzer : IDocumentationAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzers the files of the directory with the specified <paramref name="directoryPath"/> and
        /// extracts all the documentation information
        /// </summary>
        /// <param name="types"></param>
        /// <param name="directoryPath">The directory path</param>
        public IEnumerable<ClassCommentInformation> Analyze(IEnumerable<Type> types, string directoryPath)
        {
            // Creates a new Xml document
            var doc = new XmlDocument();
            // Loads the xml from the file path
            doc.Load(directoryPath);

            // Gets all the elements with tag <member>
            var members = doc.GetElementsByTagName(Constants.MemberTag);

            var result = new List<ClassCommentInformation>();

            // For each member in the xml file...
            foreach (var member in members)
            {
                // Parses the member from object to XmlNode
                var memberNode = (XmlNode)member;

                var xmlNodeFullName = memberNode.Attributes.GetNamedItem(Constants.NameTag).Value;

                var fullClassName = xmlNodeFullName.GetFullClassName();

                if (types.Any(x => x.FullName.Contains(fullClassName)))
                {
                    // If the name attribute of the member starts with "T:"...
                    // Meaning if the member is a class...
                    if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("T:"))
                    {
                        result.AddRange(AnalyzeClass(memberNode, members));
                        
                    }
                    // Else if the name attribute of the member starts with "P:"...
                    // Meaning if the member is a property...
                    else if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("P:"))
                    {
                        AnalyzeProperty(memberNode, result);
                    }
                    // Else if the name attribute of the member starts with "M:"...
                    // Meaning if the member is a method OR constructor...
                    else if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("M:"))
                    {
                        AnalyzeMethod(memberNode, result);
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

        /// <summary>
        /// Gets all the base classes of a class
        /// </summary>
        /// <param name="classType">The class type</param>
        public IEnumerable<Type> GetBaseClasses(Type classType)
        {
            var result = new List<Type>();

            // Gets the base type of the given class
            var baseClass = classType.BaseType;
            // If the base class is NOT null...
            if(baseClass != null && baseClass.Name != Constants.ObjectClass)
            {
                result.Add(baseClass);
                // The method calls itself with the base class this time
                result.AddRange(GetBaseClasses(baseClass));
            
            }

            return result;
        }

        /// <summary>
        /// Analyzes a class
        /// </summary>
        /// <param name="memberNode">The class' XML node</param>
        /// <param name="members"></param>
        public IEnumerable<ClassCommentInformation> AnalyzeClass(XmlNode memberNode, XmlNodeList members)
        {
            var result = new List<ClassCommentInformation>();

            // Sets as summary the inner xml of the member
            var classSummary = memberNode.InnerXml.Replace(Constants.SummaryXmlStartMember, string.Empty)
                                                  .Replace(Constants.SummaryXmlEndMember, string.Empty);

            // Filters and cleans the text
            classSummary = HelperMethods.CleanCommentString(classSummary);

            // Gets the class' Type
            var classType = Type.GetType(memberNode.GetAttributeName().Replace("T:", string.Empty));

            // Gets the base classes of the classType
            var baseClasses = GetBaseClasses(classType);

            // Creates a new class comment information object
            var classCommentInformation = new ClassCommentInformation(classType, classSummary);

            baseClasses.ForEach(x => classCommentInformation.Add(x));

            foreach(var member in members)
            {
                if(baseClasses.Count() > 0)
                {
                    foreach(var baseClass in baseClasses)
                    {
                        // Parses the member from object to XmlNode
                        var node = (XmlNode)member;

                        var xmlNodeFullName = node.Attributes.GetNamedItem(Constants.NameTag).Value;
                    
                        if(xmlNodeFullName.Contains(baseClass.FullName))
                        {
                            // If the name attribute of the member starts with "T:"...
                            // Meaning if the member is a class...
                            if (node.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("T:"))
                            {
                                result.AddRange(AnalyzeClass(node, members));
                            }
                            // Else if the name attribute of the member starts with "P:"...
                            // Meaning if the member is a property...
                            else if (node.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("P:"))
                            {
                                AnalyzeProperty(node, result);
                            }
                            // Else if the name attribute of the member starts with "M:"...
                            // Meaning if the member is a method OR constructor...
                            else if (node.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("M:"))
                            {
                                AnalyzeMethod(node, result);
                            }
                        }
                    }
                }
            }

            // Adds to the classes list the class
            result.Add(classCommentInformation);

            return result;
        }

        /// <summary>
        /// Analyzes a property
        /// </summary>
        /// <param name="memberNode">The property member node</param>
        /// <param name="classes">The list with all the <see cref="ClassCommentInformation"/></param>
        public void AnalyzeProperty(XmlNode memberNode, IEnumerable<ClassCommentInformation> classes)
        {
            // Gets the property's name
            var propertyName = memberNode.GetAttributeName().GetLastNameAfterDot();

            var propertySummary = string.Empty;

            // If the property does not have any comments...
            if (memberNode.InnerText == string.Empty)
            {
                // Prints in the out put console a message
                HelperMethods.MissingSummaryCommentsOutputError(propertyName, Constants.PropertyTag);
            }
            else
            {
                // Sets as summary the inner xml of the member
                propertySummary = memberNode.InnerXml.Replace(Constants.SummaryXmlStartMember, string.Empty)
                                                     .Replace(Constants.SummaryXmlEndMember, string.Empty);

                // Filters and cleans the text
                propertySummary = HelperMethods.CleanCommentString(propertySummary);
            }

            // Creates a new property comment information
            var propertyCommentInformation = new PropertyCommentInformation(propertyName, propertySummary);

            // A new list for the comments in the property's summary
            var propertyCommentCrefs = new List<CRef>();

            // For each child node in the property's summary node
            foreach (var propertyChildData in memberNode.ChildNodes[0].ChildNodes)
            {
                // Parses it to an xml node
                var node = (XmlNode)propertyChildData;

                // If the node's name is "see"
                if (node.Name == Constants.SeeTag)
                {
                    // Gets the cref's name
                    var crefName = node.GetAttributeName(Constants.CrefTag);

                    // Sets as method's name the character set after the last '.'
                    crefName = crefName.Substring(crefName.LastIndexOf(Constants.Dot) + 1);

                    crefName = GetNodeElementName(node);

                    // Creates a new param comment information with name the cref's name
                    // TODO:
                    propertyCommentCrefs.Add(new CRef(crefName, propertyCommentInformation));
                }
            }

            // Sets as comment crefs the crefs found before 
            propertyCommentInformation.CommentCrefs = propertyCommentCrefs;

            // Finds the first class that its name the property's full name contains and...
            classes.First(x => memberNode.GetAttributeName().Contains(x.ClassType.FullName))
                                // Adds the property comment info to its properties' list
                                .Add(propertyCommentInformation);
        }

        /// <summary>
        /// Analyzes a method
        /// </summary>
        /// <param name="memberNode">The method's XML node</param>
        /// <param name="classes">The list with all the <see cref="ClassCommentInformation"/></param>
        public void AnalyzeMethod(XmlNode memberNode, IEnumerable<ClassCommentInformation> classes)
        {
            // Sets as the method's name from the member node the name attribute's value formatted accordingly
            var methodName = FormatMethodName(memberNode.GetAttributeName());

            var methodSummary = string.Empty;

            if (memberNode.InnerText == string.Empty)
            {
                // Prints in the out put console a message
                HelperMethods.MissingSummaryCommentsOutputError(methodName, "method");
            }
            else
            {
                // Gets the member node's inner text
                methodSummary = memberNode.InnerXml.Substring(0, memberNode.InnerXml.LastIndexOf(Constants.SummaryXmlEndMember))
                                    .Replace(Constants.SummaryXmlStartMember, string.Empty)
                                    .Replace(Constants.SummaryXmlEndMember, string.Empty);
                // Filters and cleans the text
                methodSummary = HelperMethods.CleanCommentString(methodSummary);
            }

            // Creates a new method comment information with name the methodName and summary comments the summary
            var methodCommentInformation = new MethodCommentInformation(methodName, methodSummary);

            var tempParameters = new List<ParameterCommentInformation>();

            // For each child node of the member node...
            foreach (var childNode in memberNode.ChildNodes)
            {
                // Parses the child to an Xml node
                var node = (XmlNode)childNode;

                // If the node's name is "param"...
                if (node.Name == Constants.ParameterTag)
                {
                    //  Creates a new parameter comment information
                    var parameterCommentInformation = new ParameterCommentInformation(node.GetAttributeName(), HelperMethods.CleanCommentString(node.InnerXml));

                    // Adds to the parameters the parameter
                    tempParameters.Add(parameterCommentInformation);
                }
            }

            // For each child node of the member node...
            foreach (var childNode in memberNode.ChildNodes)
            {
                // Parses the child to an Xml node
                var node = (XmlNode)childNode;

                // Gets the child nodes of the node
                var paramNodes = node.ChildNodes;

                // Creates a list for the references in the comments
                var commentParameters = new List<ParameterCommentInformation>();

                // If the node's name is "summary"
                if (node.Name == Constants.SummaryTag)
                {
                    // Gets the summary's child nodes
                    var summaryChildNodes = node.ChildNodes;

                    SetCommentReferences(summaryChildNodes, commentParameters, tempParameters);

                    // Sets as summary comments the references found
                    methodCommentInformation.SummaryCommentParameters = commentParameters;
                }
                // If the node's name is "param"...
                else if (node.Name == Constants.ParameterTag)
                {
                    SetCommentReferences(paramNodes, commentParameters, tempParameters);

                    tempParameters.First(x => x.Name == node.GetAttributeName()).CommentParameters = commentParameters;
                }
            }

            // Sets as the method's parameters the tempParameters
            methodCommentInformation.Parameters = tempParameters;

            // Finds the first class that its name the method's full name contains and...
            classes.First(x => memberNode.GetAttributeName().Contains(x.Name))
                                // Adds the method comment info to its methods' list
                                .Add(methodCommentInformation);
        }

        /// <summary>
        /// Formats a method's full name
        /// </summary>
        /// <param name="value">The method's full name</param>
        /// <returns></returns>
        public string FormatMethodName(string value)
        {
            // If the method name contains a (
            // Meaning, if the method has parameters...
            if (value.Contains(Constants.LeftParenthesis))
                // Sets as the method's name the methods name up to before the (
                value = value.Substring(0, value.IndexOf(Constants.LeftParenthesis));

            // Sets as method's name the character set after the last .
            value = value.GetLastNameAfterDot();

            // Returns the formatted value
            return value;
        }

        /// <summary>
        /// Gets the name from a node of type element
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns></returns>
        public string GetNodeElementName(XmlNode node)
        {
            // If the node is a paramref...
            if (node.Name == Constants.ParameterReferenceTag)
                // Returns the name attribute's value
                return node.GetAttributeName();
            // Else if the node is a see...
            else if (node.Name == Constants.SeeTag)
            {
                // Gets the cref attribute's value
                var crefName = node.GetAttributeName(Constants.CrefTag).GetLastNameAfterDot();
                // Gets the cref's class name
                var className = node.GetAttributeName(Constants.CrefTag)
                                // Gets from the start up to the last "."
                                .Substring(0, node.GetAttributeName(Constants.CrefTag).LastIndexOf(Constants.Dot))
                                // Returns the last char set after the last "."
                                .GetLastNameAfterDot();

                // Sets as method's name the character set after the last '.'
                return $"{className}.{crefName}";
            }
            // Returns an empty string
            return string.Empty;
        }

        /// <summary>
        /// Gets from the given <paramref name="nodeList"/> the empty elements' names of the each node's summary
        /// Searches through the <paramref name="multipleParameterCommentInformation"/> if an object with the same name already exists...
        /// If yes adds it to the <paramref name="commentParameters"/>...
        /// Else creates a new object and adds that to the list
        /// </summary>
        /// <param name="nodeList">The list of nodes</param>
        /// <param name="multipleParameterCommentInformation">The parameters of a method</param>
        /// <param name="commentParameters">The references in the comments</param>
        public void SetCommentReferences(XmlNodeList nodeList, List<ParameterCommentInformation> multipleParameterCommentInformation, List<ParameterCommentInformation> commentParameters)
        {
            // For each param in the nodes...
            foreach (var data in nodeList)
            {
                // Parses the child from object to xml node
                var node = (XmlNode)data;

                // If the node is of type Element...
                if (node.NodeType == XmlNodeType.Element)
                {
                    // Gets the element's name
                    var nodeName = GetNodeElementName(node);
                    // Adds the parameter information to the list
                    multipleParameterCommentInformation.Add(
                            // Gets the first param info that has the same name as the node if exists...
                            commentParameters.FirstOrDefault(x => x.Name == nodeName)
                            // Else creates a new param info with name the node's name
                            ?? new ParameterCommentInformation(nodeName, null)
                        );
                }
            }
        }

        #endregion

    }
}
