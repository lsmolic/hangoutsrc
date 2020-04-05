using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hangout.Shared
{
	public static class XmlUtil
	{
		public static bool TryGetAttributeFromXml(string attributeName, XmlNode xmlNode, out string returnString)
		{
			returnString = string.Empty;
			try
			{
				returnString = xmlNode.Attributes[attributeName].Value;
			}
			catch (System.Exception)
			{
				return false;
			}
			return true;
		}


        /// <summary>
        /// Get the XML document Attribute from the request XmlDocument.
        /// </summary>
        /// <param name="xmlDoc">XMLDocument request document.</param>
        /// <param name="xPath">The XPath for the parameter</param>
        /// <param name="attribute">The parameter name to retrieve.</param>
        /// <param name="defaultValue">If an error occurs or value not found the default value to return.</param>
        /// <returns>string value that is the parameter or default value if the attribute is not found.</returns>
        public static string GetAttributeFromXml(XmlDocument xmlDoc, string xPath, string attribute, string defaultValue)
        {
            string value = defaultValue;
            try
            {
                value = xmlDoc.SelectSingleNode(xPath).Attributes[attribute].InnerText;
            }
            catch { }

            return value;
        }

        /// <summary>
        /// Add Attribute to the XMLDocument
        /// </summary>
        /// <param name="xmlDoc">XMLDocument to add the atttibute to</param>
        /// <param name="xPath">XPath of the node to add the attribute to.</param>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="attributeValue">The attribut value</param>
        public static void AddAttribute(XmlDocument xmlDoc, string xPath, string attributeName, string attributeValue)
        {
            XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
            XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            xmlDocNode.Attributes.Append(attribute);
        }

        /// <summary>
        /// Remove Attribute from an XmlElement
        /// </summary>
        /// <param name="xmlDoc">XMLDocument to add the atttibute to</param>
        /// <param name="xPath">XPath of the node for the attribute to remove</param>
        /// <param name="attributeName">The attribute name to remove</param>
        public static void RemoveAttribute(XmlDocument xmlDoc, string xPath, string attributeName)
        {
            XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
            XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
            xmlDocNode.Attributes.RemoveNamedItem(attributeName);
        }
	}
}
