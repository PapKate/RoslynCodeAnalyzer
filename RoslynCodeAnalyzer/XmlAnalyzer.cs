
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RoslynCodeAnalyzer
{
    /// <summary>
    /// Responsible for analyzing a .xml file
    /// </summary>
    public static class XmlAnalyzer
    {
        #region Public Methods

        /// <summary>
        /// Analyzes an XML file
        /// </summary>
        public static void AnalyzeFile()
        {
            // Creates a new Xml document
            var doc = new XmlDocument();
            // Loads the xml from the file path
            doc.Load(Constants.XmlFilePath);

            // Gets all the elements with tag <member>
            var members = doc.GetElementsByTagName("member");

            // For each member in the xml file...
            foreach (var member in members)
            {
                // Parses the member from object to XmlNode
                var memberNode = (XmlNode)member;

                // If the name attribute of the member starts with "P:"...
                // Meaning if the member is a property...
                if (memberNode.Attributes.GetNamedItem("name").InnerXml.StartsWith("P:"))
                {
                    // Sets as summary the inner xml of the member
                    var summary = memberNode.InnerXml;

                    Console.WriteLine(summary);
                }
                // Else if the name attribute of the member starts with "M:"...
                // Meaning if the member is a method OR constructor...
                else if (memberNode.Attributes.GetNamedItem("name").InnerXml.StartsWith("M:"))
                {
                    // Sets as the method's name from the member node the name attribute's value
                    var methodName = memberNode.Attributes.GetNamedItem("name").Value;
                    
                    // If the method name contains a (
                    // Meaning, if the method has parameters...
                    if(methodName.Contains("("))
                        // Sets as the method's name the methods name up to before the (
                        methodName = methodName.Substring(0, methodName.IndexOf("("));

                    // Sets as method's name the character set after the last .
                    methodName = methodName.Substring(methodName.LastIndexOf(".") + 1);

                    // Gets the member node's inner xml and removes the summary open and close tags
                    var methodSummary = memberNode.InnerXml.Replace("<summary>", string.Empty).Replace("</summary>", string.Empty);

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
                        
                        // If the node's name is "summary"
                        if(node.Name == Constants.SummaryTag)
                        {
                            var summaryChildNodes = node.ChildNodes;
                            
                            var summaryRefs = new List<string>();

                            foreach (var summaryChildData in summaryChildNodes)
                            {
                                var summaryChildNode = (XmlNode)summaryChildData;

                                // If the node is of type Element...
                                if (summaryChildNode.NodeType == XmlNodeType.Element)
                                    if(summaryChildNode.Name == "paramref")
                                        summaryRefs.Add(summaryChildNode.Attributes.GetNamedItem("name").Value);
                                    else if(summaryChildNode.Name == "see")
                                    {
                                        var crefName = summaryChildNode.Attributes.GetNamedItem("cref").Value;

                                        // Sets as method's name the character set after the last .
                                        crefName = crefName.Substring(crefName.LastIndexOf(".") + 1);
                                        
                                        summaryRefs.Add(crefName);
                                    }
                            }
                        }
                        // Else if the node's name is "param"...
                        else if(node.Name == Constants.ParameterTag)
                        {
                            //  Creates a new parameter comment information
                            var parameterCommentInformation = new ParameterCommentInformation
                            {
                                Name = node.Attributes.GetNamedItem("name").Value,
                                Comments = node.InnerXml
                            };

                            // Gets the child nodes of the node
                            var paramNodes = node.ChildNodes;
                            
                            var paramRefs = new List<string>();

                            // For each param in the nodes...
                            foreach (var paramData in paramNodes)
                            {
                                // Parses the paramData from obj to Xml node
                                var paramNode = (XmlNode)paramData;
                                
                                // If the node is of type Element...
                                if (paramNode.NodeType == XmlNodeType.Element)
                                    paramRefs.Add(paramNode.Attributes.GetNamedItem("name").Value);
                            }

                            // Sets as references the found parmRefs
                            // Meaning the names of the parameters mentioned in the comments
                            parameterCommentInformation.References = paramRefs;

                            // Adds to the parameters the parameter
                            tempParameters.Add(parameterCommentInformation);
                        }
                    }

                    var paramRefrences = new List<ParameterCommentInformation>();

                    foreach(var tempParam in tempParameters)
                    {
                        paramRefrences.Add(new ParameterCommentInformation()
                        {
                            Name = tempParam.Name,
                            Comments = tempParam.Comments,
                        });

                        var commentParameters = new List<ParameterCommentInformation>();

                        tempParam.References.ForEach(x => 
                                    commentParameters.Add(tempParameters.FirstOrDefault(y => y.Name == x) 
                                                        ?? new ParameterCommentInformation() { Name = x })
                                                    );
                        tempParam.CommentParameters = commentParameters;
                    }


                    Console.WriteLine(methodCommentInformation.SummaryComments);
                }
            }
        }

        #endregion

    }
}
