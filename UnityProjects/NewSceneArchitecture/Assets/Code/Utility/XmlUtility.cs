/**  --------------------------------------------------------  *
 *   XmlUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/23/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public static class XmlUtility
	{

		/// <summary>
		/// Find all the 'Include' elements and replaces the element with 
		/// all the elements in the XML document defined in the include path.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="rootNode">Name of the root element, this is stripped out during the include. If null, nothing will be stripped out.</param>
		/// <returns></returns>
		public static XmlDocument ResolveIncludes(XmlDocument doc, string rootNode)
		{
			ResolveIncludesRecurse(doc, rootNode, new List<string>());
			return doc;
		}

		public static XmlDocument Resolve(XmlDocument doc, string rootNode)
		{
			ResolveIncludes(doc, rootNode);
			ResolveXRefs(doc);
			return doc;
		}

		public static void CopyAttributes(XmlNode sourceNode, XmlNode destNode)
		{
			foreach (XmlAttribute attribute in sourceNode.Attributes)
			{
				destNode.Attributes.Append((XmlAttribute)attribute.CloneNode(true));
			}
		}

		public static XmlDocument ResolveXRefs(XmlDocument rootNode)
		{
			Hangout.Shared.Func<List<XmlNode>, List<XmlNode>, object> xmlNodeListToRealList = 
			delegate(List<XmlNode> result, object node)
			{
				result.Add((XmlNode)node);
				return result;
			};

			List<XmlNode> xrefNodes = Functionals.Reduce<List<XmlNode>>
			(
				xmlNodeListToRealList,
				rootNode.SelectNodes(".//XRef")
			);

			while(xrefNodes.Count != 0)
			{
				XmlNode xrefNode = xrefNodes[0];
				xrefNodes.RemoveAt(0);

				XmlAttribute pathAttrib = xrefNode.Attributes["path"];
				XmlElement resultNode = (XmlElement)rootNode.SelectSingleNode(pathAttrib.InnerText).CloneNode(true);

				xrefNodes.AddRange
				(
					Functionals.Reduce<List<XmlNode>>
					(
						xmlNodeListToRealList,
						resultNode.SelectNodes(".//XRef")
					)
				);

				xrefNode.Attributes.Remove(pathAttrib);
				CopyAttributes(xrefNode, resultNode);

				xrefNode.ParentNode.ReplaceChild(resultNode, xrefNode);
			}

			return rootNode;
		}

		private static XmlDocument ResolveIncludesRecurse(XmlDocument doc, string rootNode, List<string> includedDocuments)
		{
			List<XmlNode> toRemove = new List<XmlNode>();

			foreach (XmlNode node in doc.FirstChild.SelectNodes("//Include"))
			{
				string newDocLocation = node.Attributes["path"].InnerText;

				// Ignore multiple includes and avoid crash from circular reference
				if (includedDocuments.Contains(newDocLocation))
				{
					continue;
				}
				includedDocuments.Add(newDocLocation);

				XmlDocument unresolvedDoc = LoadXmlDocument(newDocLocation);
				XmlDocument resolvedDoc = ResolveIncludesRecurse(unresolvedDoc, rootNode, includedDocuments);

				// Add everything in the included xml at the same level as the 'Include' node
				foreach (XmlNode resolvedNode in resolvedDoc.SelectSingleNode(rootNode).ChildNodes)
				{
					node.ParentNode.AppendChild(doc.ImportNode(resolvedNode, true));
				}

				toRemove.Add(node);
			}

			// Clean up all the 'Include' nodes
			foreach (XmlNode nodeToRemove in toRemove)
			{
				nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
			}

			return doc;
		}

		public static XmlDocument LoadXmlDocument(string documentPath)
		{
			Pair<string, string> split = ProtocolUtility.SplitProtocol(documentPath);
			string protocol = split.First;
			string path = split.Second;

			if (protocol == "assets")
			{
				throw new Exception("LoadXmlDocument does not support asynchronous protocols (" + protocol + ").  Use Client Asset Repository.");
			}

			XmlDocument result = new XmlDocument();
			switch (protocol)
			{
				case "resources":
					TextAsset styleTextAsset = (TextAsset)Resources.Load(path);
					if (styleTextAsset == null)
					{
						throw new FileNotFoundException("Unable to load the TextFile from resource path: " + path);
					}
					result.LoadXml(styleTextAsset.text);
					break;

				case "file":
					FileInfo file = new FileInfo(path);
					if (!file.Exists)
					{
						throw new FileNotFoundException(documentPath + " must point to an existing file on disk.", file.FullName);
					}
					result.Load(file.FullName);
					break;

				default:
					throw new Exception("Unrecognized protocol (" + protocol + ")");
			}

			// Remove all the comment nodes (this feels like it shouldn't be necessary, but the comments end up getting parsed in weird places as Nodes)
			List<XmlNode> toRemove = new List<XmlNode>();
			foreach (XmlNode node in result.SelectNodes("//comment()"))
			{
				toRemove.Add(node);
			}
			foreach (XmlNode node in toRemove)
			{
				node.ParentNode.RemoveChild(node);
			}

			return result;
		}

		public static bool XmlNodeHasAttribute(XmlNode node, string attribute)
		{
			return node.Attributes[attribute] != null;
		}

		public static string GetStringAttribute(XmlNode node, string attribute)
		{
			if (!XmlNodeHasAttribute(node, attribute))
			{
				throw new ArgumentException("node (" + node.Name + ") does not contain an attribute named " + attribute);
			}

			return node.Attributes[attribute].InnerText;
		}

		public static bool GetBoolAttribute(XmlNode node, string attribute)
		{
			if (!XmlNodeHasAttribute(node, attribute))
			{
				throw new ArgumentException("node (" + node.Name + ") does not contain an attribute named " + attribute);
			}
			return bool.Parse(node.Attributes[attribute].InnerText);
		}

		public static int GetIntAttributeOrReturn(XmlNode node, string attribute, int defaultValue)
		{
			int result = defaultValue;
			if (XmlNodeHasAttribute(node, attribute))
			{
				result = int.Parse(node.Attributes[attribute].InnerText);
			}
			return result;
		}

		public static int GetIntAttribute(XmlNode node, string attribute)
		{
			if (!XmlNodeHasAttribute(node, attribute))
			{
				throw new ArgumentException("node (" + node.Name + ") does not contain an attribute named " + attribute);
			}
			return int.Parse(node.Attributes[attribute].InnerText);
		}

		public static long GetLongAttribute(XmlNode node, string attribute)
		{
			if (!XmlNodeHasAttribute(node, attribute))
			{
				throw new ArgumentException("node (" + node.Name + ") does not contain an attribute named " + attribute);
			}
			return long.Parse(node.Attributes[attribute].InnerText);
		}

		public static uint GetUintAttribute(XmlNode node, string attribute)
		{
			if (!XmlNodeHasAttribute(node, attribute))
			{
				throw new ArgumentException("node (" + node.Name + ") does not contain an attribute named " + attribute);
			}
			return uint.Parse(node.Attributes[attribute].InnerText);
		}

		public static float GetFloatAttribute(XmlNode node, string attribute)
		{
			if (!XmlNodeHasAttribute(node, attribute))
			{
				throw new ArgumentException("node (" + node.Name + ") does not contain an attribute named " + attribute + ".  Full XmlNode: " + node.OuterXml);
			}
			return float.Parse(node.Attributes[attribute].InnerText);
		}

		public static Pair<Vector3> ParsePositionDirection(XmlNode root)
		{
			XmlNode positionNode = root.SelectSingleNode("Position");
			if (positionNode == null)
			{
				throw new Exception("Expecting a node(Position) as a child of node(" + root.Name + ")");
			}

			XmlNode directionNode = root.SelectSingleNode("Direction");
			if (directionNode == null)
			{
				throw new Exception("Expecting a node(Direction) as a child of node(" + root.Name + ")");
			}

			return new Pair<Vector3>
			(
				ParseVector3FromNode(positionNode, "value"),
				AngleToUnitVector(ParseFloatFromNode(directionNode, "value"))
			);
		}

		/// <summary>
		/// Converts a rotation around the Y axis into a direction vector
		/// </summary>
		private static Vector3 AngleToUnitVector(float degrees)
		{
			return new Vector3(Mathf.Cos(degrees * Mathf.Deg2Rad), 0.0f, Mathf.Sin(degrees * Mathf.Deg2Rad));
		}

		public static float ParseFloatFromNode(XmlNode node, string attributeName)
		{
			float result = 0.0f;

			try
			{
				string feature = node.Attributes[attributeName].InnerText;
				result = float.Parse(feature);
			}
			catch (FormatException e)
			{
				throw new Exception("Unable to parse a float from XmlNode(" + node.Name + "), attribute(" + attributeName + ")", e);
			}
			catch (NullReferenceException e)
			{
				throw new Exception("Unable to parse a float from XmlNode(" + node.Name + "), attribute(" + attributeName + ")", e);
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw new Exception("Unable to parse a float from XmlNode(" + node.Name + "), attribute(" + attributeName + ")", e);
			}

			return result;
		}

		public static Vector3 ParseVector3FromNode(XmlNode node, string attributeName)
		{
			Vector3 result = Vector3.zero;

			try
			{
				string[] featureStrings = node.Attributes[attributeName].InnerText.Split();
				result.x = float.Parse(featureStrings[0]);
				result.y = float.Parse(featureStrings[1]);
				result.z = float.Parse(featureStrings[2]);
			}
			catch (FormatException e)
			{
				throw new Exception("Unable to parse a Vector3 from XmlNode(" + node.Name + "), attribute(" + attributeName + ")", e);
			}
			catch (NullReferenceException e)
			{
				throw new Exception("Unable to parse a Vector3 from XmlNode(" + node.Name + "), attribute(" + attributeName + ")", e);
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw new Exception("Unable to parse a Vector3 from XmlNode(" + node.Name + "), attribute(" + attributeName + ")", e);
			}

			return result;
		}
	}
}