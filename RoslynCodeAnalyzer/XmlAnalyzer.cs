
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Responsible for analyzing a .xml file
    /// </summary>
    public class XmlAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes an XML file
        /// </summary>
        public void AnalyzeFile()
        {
            // Creates a new Xml document
            var doc = new XmlDocument();
            // Loads the xml from the file path
            doc.Load(Constants.XmlFilePath);

            // Gets all the elements with tag <member>
            var members = doc.GetElementsByTagName(Constants.MemberTag);

            var classes = new List<ClassCommentInformation>();

            // For each member in the xml file...
            foreach (var member in members)
            {
                // Parses the member from object to XmlNode
                var memberNode = (XmlNode)member;

                // If the name attribute of the member starts with "T:"...
                // Meaning if the member is a class...
                if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("T:"))
                {
                    var className = memberNode.GetAttributeName().GetLastNameAfterDot();

                    // If the class has no comments...
                    if (memberNode.InnerText == string.Empty)
                        // Prints in the out put console a message
                        HelperMethods.MissingSummaryCommentsOutputError(className, "class");

                    // Sets as summary the inner xml of the member
                    var classSummary = memberNode.InnerXml.Replace(Constants.SummaryXmlStartMember, string.Empty)
                                                          .Replace(Constants.SummaryXmlEndMember, string.Empty);

                    // Filters and cleans the text
                    classSummary = HelperMethods.CleanedCommentsString(classSummary);

                    // Creates a new class comment information object
                    var classCommentInformation = new ClassCommentInformation()
                    {
                        Name = className,
                        Comments = classSummary
                    };

                    // Adds to the classes list the class
                    classes.Add(classCommentInformation);
                }
                // Else if the name attribute of the member starts with "P:"...
                // Meaning if the member is a property...
                else if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("P:"))
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
                        propertySummary = HelperMethods.CleanedCommentsString(propertySummary);
                    }
                   
                    // Creates a new property comment information
                    var propertyCommentInformation = new PropertyCommentInformation
                    {
                        Name = propertyName,
                        Comments = propertySummary
                    };

                    // A new list for the comments in the property's summary
                    var propertyCommentCrefs = new List<ParameterCommentInformation>();
                    
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
                            propertyCommentCrefs.Add(new ParameterCommentInformation() { Name = crefName });
                        }
                    }

                    // Sets as comment crefs the crefs found before 
                    propertyCommentInformation.CommentParameters = propertyCommentCrefs;

                    // Finds the first class that its name the property's full name contains and...
                    classes.First(x => memberNode.GetAttributeName().Contains(x.Name))
                                         // Adds the property comment info to its properties' list
                                        .Properties.Add(propertyCommentInformation);
                }
                // Else if the name attribute of the member starts with "M:"...
                // Meaning if the member is a method OR constructor...
                else if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("M:"))
                {
                    // Sets as the method's name from the member node the name attribute's value formatted accordingly
                    var methodName = FormatMethodName(memberNode.GetAttributeName());

                    var methodSummary = string.Empty;
                    
                    if(memberNode.InnerText == string.Empty)
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
                        methodSummary = HelperMethods.CleanedCommentsString(methodSummary);
                    }

                    // Creates a new method comment information with name the methodName and summary comments the summary
                    var methodCommentInformation = new MethodCommentInformation
                    {
                        Name = methodName,
                        Comments = methodSummary
                    };

                    var tempParameters = new List<ParameterCommentInformation>();

                    // For each child node of the member node...
                    foreach (var childNode in memberNode.ChildNodes)
                    {
                        // Parses the child to an Xml node
                        var node = (XmlNode)childNode;
                        
                        // If the node's name is "param"...
                        if(node.Name == Constants.ParameterTag)
                        {
                            //  Creates a new parameter comment information
                            var parameterCommentInformation = new ParameterCommentInformation
                            {
                                Name = node.GetAttributeName(),
                                Comments = HelperMethods.CleanedCommentsString(node.InnerXml)
                            };

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
                    
                    methodCommentInformation.Parameters = tempParameters;

                    // Finds the first class that its name the method's full name contains and...
                    classes.First(x => memberNode.GetAttributeName().Contains(x.Name))
                                        // Adds the method comment info to its methods' list
                                        .Methods.Add(methodCommentInformation);

                    Console.WriteLine(methodCommentInformation.Comments);
                }
            }
            Console.WriteLine(classes.First(x => x.Name == "MethodCommentInformation").Name);
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
                            ?? new ParameterCommentInformation() { Name = nodeName }
                        );
                }
            }
        }

        #endregion

    }
}
