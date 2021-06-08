
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

            var properties = new List<PropertyCommentInformation>();
            var methods = new List<MethodCommentInformation>();

            // For each member in the xml file...
            foreach (var member in members)
            {
                // Parses the member from object to XmlNode
                var memberNode = (XmlNode)member;

                // If the name attribute of the member starts with "P:"...
                // Meaning if the member is a property...
                if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("P:"))
                {
                    var propertyName = memberNode.GetAttributeName();

                    // Sets as summary the inner xml of the member
                    var propertySummary = memberNode.InnerXml.Replace(Constants.SummaryXmlStartMember, string.Empty)
                                                             .Replace(Constants.SummaryXmlEndMember, string.Empty);

                    // Filters and cleans the text
                    propertySummary = HelperMethods.CleanedCommentsString(propertySummary);

                    // Creates a new property comment information
                    var propertyCommentInformation = new PropertyCommentInformation
                    {
                        Name = propertyName,
                        Comments = propertySummary
                    };

                    // A new list for the comments in the property's summary
                    var propertyCommentCrefs = new List<ParameterCommentInformation>();

                    // For each child node in the property's summary node
                    foreach(var propertyChildData in memberNode.ChildNodes[0].ChildNodes)
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

                            // Creates a new param comment information with name the cref's name
                            propertyCommentCrefs.Add(new ParameterCommentInformation() { Name = crefName });
                        }
                    }

                    // Sets as comment crefs the crefs found before 
                    propertyCommentInformation.CommentCref = propertyCommentCrefs;

                    // Adds the property comment information
                    properties.Add(propertyCommentInformation);

                    Console.WriteLine(propertySummary);
                }
                // Else if the name attribute of the member starts with "M:"...
                // Meaning if the member is a method OR constructor...
                else if (memberNode.Attributes.GetNamedItem(Constants.NameTag).InnerXml.StartsWith("M:"))
                {
                    // Sets as the method's name from the member node the name attribute's value formatted accordingly
                    var methodName = FormatMethodName(memberNode.GetAttributeName());

                    // Gets the member node's inner text
                    var methodSummary = memberNode.InnerXml.Substring(0, memberNode.InnerXml.LastIndexOf(Constants.SummaryXmlEndMember))
                                        .Replace(Constants.SummaryXmlStartMember, string.Empty).Replace(Constants.SummaryXmlEndMember, string.Empty);

                    // Filters and cleans the text
                    methodSummary = HelperMethods.CleanedCommentsString(methodSummary);

                    // Creates a new method comment information with name the methodName and summary comments the summary
                    var methodCommentInformation = new MethodCommentInformation
                    {
                        MethodName = methodName,
                        SummaryComments = methodSummary
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
                                Comments = node.InnerXml
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

                            // For each child node...
                            foreach (var summaryChildData in summaryChildNodes)
                            {
                                // Parses the child from object to xml node
                                var summaryChildNode = (XmlNode)summaryChildData;

                                // If the node is of type Element...
                                if (summaryChildNode.NodeType == XmlNodeType.Element)
                                {
                                    // Gets the element's name
                                    var nodeName = GetNodeElementName(summaryChildNode);
                                    // Adds the parameter information to the list
                                    commentParameters.Add(
                                            // Gets the first param info that has the same name as the node if exists...
                                            tempParameters.FirstOrDefault(x => x.Name == nodeName)
                                            // Else creates a new param info with name the node's name
                                            ?? new ParameterCommentInformation() { Name = nodeName }
                                        );
                                }
                            }

                            // Sets as summary comments the references found
                            methodCommentInformation.SummaryCommentParameters = commentParameters;
                        }
                        // If the node's name is "param"...
                        else if (node.Name == Constants.ParameterTag)
                        {
                            // For each param in the nodes...
                            foreach (var paramData in paramNodes)
                            {
                                // Parses the paramData from obj to Xml node
                                var paramNode = (XmlNode)paramData;

                                // If the node is of type Element...
                                if (paramNode.NodeType == XmlNodeType.Element)
                                {
                                    var nodeName = GetNodeElementName(paramNode);
                                    commentParameters.Add(
                                            tempParameters.FirstOrDefault(x => x.Name == nodeName)
                                            ?? new ParameterCommentInformation() { Name = nodeName }
                                        );
                                }
                            }

                            tempParameters.First(x => x.Name == node.GetAttributeName()).CommentParameters = commentParameters;
                        }
                    }

                    Console.WriteLine(methodCommentInformation.SummaryComments);
                }
            }
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
            value = value.Substring(value.LastIndexOf(Constants.Dot) + 1);

            // Returns the formatted value
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string GetNodeElementName(XmlNode node)
        {
            if (node.Name == Constants.ParameterReferenceTag)
                return node.Attributes.GetNamedItem(Constants.NameTag).Value;
            else if (node.Name == Constants.SeeTag)
            {
                var crefName = node.GetAttributeName(Constants.CrefTag);

                // Sets as method's name the character set after the last '.'
                return crefName.Substring(crefName.LastIndexOf(Constants.Dot) + 1);
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="multipleParameterCommentInformation"></param>
        /// <param name="commentParameters"></param>
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
                    commentParameters.Add(
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
