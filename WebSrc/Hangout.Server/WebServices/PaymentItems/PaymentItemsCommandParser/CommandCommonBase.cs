using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Configuration;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class CommandCommonBase : CommandHandlerBase
    {
        public CommandCommonBase(string loggerName) : base (loggerName) {}
        public CommandCommonBase(Type loggerType) : base(loggerType) { }

        protected string GetCurrencyIdsFromName(string currencyName)
        {
            string currencyId = "";
            if (currencyName == "CASH")
            {
                currencyId = ConfigurationManager.AppSettings["CashCurrencyId"];
            }
            else if (currencyName == "COIN")
            {
                currencyId = ConfigurationManager.AppSettings["CoinCurrencyId"];
            }

            return currencyId;
        }

        /// <summary>
        /// Remove all child nodes except the nodeName passed in.
        /// </summary>
        /// <param name="responseXML">XML Document to remove the nodes from.</param>
        /// <param name="xPath">The XPath for the nodes to remove.</param>
        /// <param name="nodeName">The name of the node to keep.</param>
        /// <returns>Modified XML document with the nodes removed.</returns>
        protected XmlDocument RemoveResponseNodes(XmlDocument responseXML, string xPath, string nodeName)
        {
            XmlNode nodeUser = responseXML.SelectSingleNode(xPath);

            if (nodeUser != null)
            {
                XmlNodeList nodes = nodeUser.ChildNodes;

                for (int i = nodes.Count - 1; i >= 0; --i)
                {
                    XmlNode node = nodes[i];
                    if (node.LocalName != nodeName)
                    {
                        nodeUser.RemoveChild(node);
                    }
                }
            }

            return responseXML;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc">Add a node to the xml document</param>
        /// <param name="xPath">The XPath of the node to add</param>
        /// <param name="nodeName">The name of the node to add.</param>
        /// <param name="attributeNameValue">The attribute in the form of 'key=value' </param>
        /// <returns>The XMLDocument with the node added.  If an error occurs the orignal node is returned.</returns>
        protected XmlDocument AddNode(XmlDocument xmlDoc, string xPath, string nodeName, string attributeNameValue)
        {
            try
            {
                XmlNode newNode = xmlDoc.CreateNode("element", nodeName, "");
                newNode.InnerText = attributeNameValue;

                XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
                xmlDocNode.AppendChild(newNode);
            }

            catch (Exception ex)
            {
                logError("GetNodeInnerText", ex);
            }

            return xmlDoc;
        }


        /// <summary>
        /// Append a node to the XML document
        /// </summary>
        /// <param name="doc">XMLdocument to append node to</param>
        /// <param name="xPath">XPath of parent node</param>
        /// <param name="childNode">child node to add</param>
        protected void AppendNode(XmlDocument doc, string xPath, XmlNode childNode)
        {
            XmlNode newChildNode = doc.ImportNode(childNode, true);
            XmlNode parentNode = doc.SelectSingleNode(xPath);

            parentNode.AppendChild(newChildNode);
        }

        /// <summary>
        /// Add Attribute to the XMLDocument
        /// </summary>
        /// <param name="xmlDoc">XMLDocument to add the atttibute to</param>
        /// <param name="xPath">XPath of the node to add the attribute to.</param>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="attributeValue">The attribut value</param>
        protected void AddAttribute(XmlDocument xmlDoc, string xPath, string attributeName, string attributeValue)
        {
            XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
            XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            xmlDocNode.Attributes.Append(attribute);
        }

        /// <summary>
        /// Update the Response node noun and verb.  
        /// Used to convert twoFish noun and verb to Hangout noun and verb.
        /// </summary>
        /// <param name="responseXML">Response XML document</param>
        /// <param name="noun">The command noun</param>
        /// <param name="verb">The command verb</param>
        /// <returns>The updated XML document</returns>
        protected XmlDocument UpdateResponseNounVerb(XmlDocument responseXML, string noun, string verb)
        {
            XmlNode node = responseXML.SelectSingleNode("/Response");
            node.Attributes["noun"].InnerText = noun;
            node.Attributes["verb"].InnerText = verb;

            node.Attributes.RemoveNamedItem("request");

            return responseXML;
        }


        /// <summary>
        /// Given an XMLNode 
        /// </summary>
        /// <param name="node">XMLNode</param>
        /// <param name="xPath">The XPath of the value to return</param>
        /// <param name="defaultValue">The default value returned, if the does not exist or an error occurs</param>
        /// <returns>The innerText string of the specified node, if the does not exist or an error occurs returns the default value</returns>
        protected string GetNodeInnerText(XmlNode node, string xPath, string defaultValue)
        {
            string returnValue = defaultValue;

            try
            {
                returnValue = node.SelectSingleNode(xPath).InnerText;
            }

            catch (Exception ex)
            {
                logError("GetNodeInnerText", ex);
            }

            return returnValue;
        }

        
        /// <summary>
        /// Convert Twofish error response to a Hangout error response
        /// </summary>
        /// <param name="response">Twofish xml response document</param>
        /// <param name="attributes">Key Value pair of attributes to add to the '/Response/errors/error' node.</param>
        /// <returns>Hangout error response XML document</returns>
        protected XmlDocument UpdateResponseForErrors(XmlDocument response, NameValueCollection attributes)
        {
            try
            {
                XmlNode node = response.SelectSingleNode("/Response/errors");
                if (node != null)
                {
                    if (attributes != null)
                    {

                        foreach (KeyValuePair<string, string> attribute in attributes)
                        {
                            AddAttribute(response, "/Response/errors/error", attribute.Key, attribute.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logError("UpdateResponseForErrors", ex);
            }

            return response;
        }

        /*protected void WriteXMLtoFile(string fileName, XmlDocument docToWrite)
         {
            StreamWriter swResponse = new StreamWriter(fileName);
            swResponse.Write(docToWrite.InnerXml);
            swResponse.Close();
        }*/
    }
}
