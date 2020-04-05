/**  --------------------------------------------------------  *
 *   XmlWidgetFactory.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/22/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections.Generic;

using System.Xml;
using System.Text;
using System.Reflection;

namespace Hangout.Client.Gui
{
	public partial class XmlGuiFactory : IGuiFactory
	{
		private const string BUILTIN_UNITY_STYLE_NAME = "UnityDefault";
		private class StyleLoadTreeNode
		{
			public StyleLoadTreeNode(XmlNode xml)
			{
				Xml = xml;
			}

			public XmlNode Xml;
			public List<StyleLoadTreeNode> Children = new List<StyleLoadTreeNode>();
			public StyleLoadTreeNode Parent;
			public string Name
			{
				get
				{
					return BuildName(Xml);
				}
			}
			public string ParentName
			{
				get
				{
					if (Xml.Attributes["parent"] != null)
					{
						return Xml.Attributes["parent"].InnerText;
					}

					return null;
				}
			}
		}

		private static StyleLoadTreeNode FindElementByName(IEnumerable<StyleLoadTreeNode> allTheNodes, string name)
		{
			foreach (StyleLoadTreeNode node in allTheNodes)
			{
				if (name == node.Name)
				{
					return node;
				}
			}
			throw new GuiConstructionException("Unable to find a StyleLoadTreeNode named " + name);
		}

		private static void RemoveAllNodesFrom(StyleLoadTreeNode root, List<StyleLoadTreeNode> list)
		{
			if (list.Contains(root))
			{
				list.Remove(root);
			}

			foreach (StyleLoadTreeNode child in root.Children)
			{
				RemoveAllNodesFrom(child, list);
			}
		}

		private void BuildStylesInOrder(StyleLoadTreeNode rootNode)
		{
			IGuiStyle parent = null;
			if (rootNode.Parent != null)
			{
				if(rootNode.Parent.Name == BUILTIN_UNITY_STYLE_NAME)
				{
					throw new GuiConstructionException("Style (" + rootNode.Name + ") cannot be based on (" + BUILTIN_UNITY_STYLE_NAME + ")");
				}
				parent = mStyles[rootNode.Parent.Name];
			}

			IGuiStyle newStyle = BuildStyle(rootNode.Name, rootNode.Xml, parent);
			mStyles[newStyle.Name] = newStyle;

			foreach (StyleLoadTreeNode child in rootNode.Children)
			{
				BuildStylesInOrder(child);
			}
		}

		public IEnumerable<IGuiStyle> ConstructAllStyles()
		{
			if (mStyles == null)
			{
				if (mGuiDoc == null)
				{
					throw new Exception("No XMLDocument Loaded");
				}

				mStyles = new Dictionary<string, IGuiStyle>();

				// Add the default unity style (null)
				mStyles.Add(BUILTIN_UNITY_STYLE_NAME, null);

				List<StyleLoadTreeNode> allUnconstructedNodes = new List<StyleLoadTreeNode>();
				List<StyleLoadTreeNode> rootNodes = new List<StyleLoadTreeNode>();
				List<StyleLoadTreeNode> orphanNodes = new List<StyleLoadTreeNode>();

				// Styles aren't hierarchical in the XML, so we can just grab them all
				XmlNodeList styleNodes = mGuiDoc.SelectNodes("//Style");
				foreach (XmlNode styleNode in styleNodes)
				{
					StyleLoadTreeNode newNode = new StyleLoadTreeNode(styleNode);
					if (styleNode.Attributes["parent"] == null)
					{
						rootNodes.Add(newNode);
					}
					else
					{
						orphanNodes.Add(newNode);
					}
					allUnconstructedNodes.Add(newNode);
				}

				foreach (StyleLoadTreeNode orphanNode in orphanNodes)
				{
					orphanNode.Parent = FindElementByName(allUnconstructedNodes, orphanNode.ParentName);
					orphanNode.Parent.Children.Add(orphanNode);
				}

				foreach (StyleLoadTreeNode root in rootNodes)
				{
					RemoveAllNodesFrom(root, orphanNodes);
				}

				if (orphanNodes.Count != 0)
				{
					StringBuilder realOrphans = new StringBuilder();
					foreach (StyleLoadTreeNode orphanNode in orphanNodes)
					{
						realOrphans.Append(orphanNode.Name);
						realOrphans.Append(", ");
					}
					realOrphans.Remove(realOrphans.Length - ", ".Length, ", ".Length);

					throw new GuiConstructionException("Cannot find a parent for element(s): " + realOrphans);
				}

				foreach (StyleLoadTreeNode root in rootNodes)
				{
					BuildStylesInOrder(root);
				}

				// Plus 1 for the built in unity style
				if (mStyles.Count != (styleNodes.Count + 1))
				{
					throw new GuiConstructionException("Not all the Styles in this factory's XML were constructed.");
				}
			}

			return mStyles.Values;
		}

		private IGuiStyle BuildStyle(string name, XmlNode node, IGuiStyle basedOn)
		{
			GuiStyle result = null;
			if (basedOn == null)
			{
				result = new GuiStyle(name);
			}
			else
			{
				result = new GuiStyle(basedOn, name);
			}

			// DefaultFor
			if (node.Attributes["defaultFor"] != null)
			{
				string[] defaultTypes = node.Attributes["defaultFor"].InnerText.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string defaultType in defaultTypes)
				{
					System.Type t = System.Type.GetType("Hangout.Client.Gui." + defaultType, true);
					result.SetAsDefaultFor(t);
				}
			}

			// Normal, Hover, Disbaled and Active
			foreach (XmlNode styleStateNode in node.SelectNodes("StyleState"))
			{
				string styleStateName = BuildName(styleStateNode);

				switch (styleStateName)
				{
					case "NormalState":
						if (basedOn == null)
						{
							result.Normal = BuildStyleState(styleStateNode, null);
						}
						else
						{
							result.Normal = BuildStyleState(styleStateNode, basedOn.Normal);
						}
						break;
					case "HoverState":
						if (basedOn == null)
						{
							result.Hover = BuildStyleState(styleStateNode, null);
						}
						else
						{
							result.Hover = BuildStyleState(styleStateNode, basedOn.Normal);
						}
						break;
					case "DisabledState":
						if (basedOn == null)
						{
							result.Disabled = BuildStyleState(styleStateNode, null);
						}
						else
						{
							result.Disabled = BuildStyleState(styleStateNode, basedOn.Normal);
						}
						break;
					case "ActiveState":
						if (basedOn == null)
						{
							result.Active = BuildStyleState(styleStateNode, null);
						}
						else
						{
							result.Active = BuildStyleState(styleStateNode, basedOn.Normal);
						}
						break;
					default:
						throw new GuiConstructionException("Unknown style state found in style (" + name + "): " + styleStateName);
				}
			}

			// InternalMargins, ExternalMargins, NinePartScale
			foreach (XmlNode marginNode in node.SelectNodes("Margin"))
			{
				string marginName = BuildName(marginNode);
				GuiMargin newMargins = BuildMargin(marginNode);
				switch (marginName)
				{
					case "InternalMargins":
						result.InternalMargins = newMargins;
						break;
					case "ExternalMargins":
						result.ExternalMargins = newMargins;
						break;
					case "NinePartScale":
						result.NinePartScale = newMargins;
						break;
					default:
						throw new GuiConstructionException("Unknown margin found in style (" + name + "): " + marginName);
				}
			}

			// DefaultFont
			if (node.SelectSingleNode("Font") != null)
			{
				result.DefaultFont = BuildFont(node.SelectSingleNode("Font"), "path");
			}

			// DefaultTextAnchor
			if (node.SelectSingleNode("Anchor") != null)
			{
				result.DefaultTextAnchor = BuildAnchor(node.SelectSingleNode("Anchor"), "value");
			}

			// WordWrap
			try
			{
				XmlNode wordWrapNode = node.SelectSingleNode("WordWrap");
				if (wordWrapNode != null)
				{
					XmlAttribute valueAttrib = wordWrapNode.Attributes["value"];
					if (valueAttrib != null)
					{
						result.WordWrap = bool.Parse(valueAttrib.InnerText);
					}
				}
			}
			catch (FormatException e)
			{
				throw new GuiConstructionException("Unable to parse the 'value' attribute on " + name + "'s WordWrap node.", e);
			}

			// ClipText
			try
			{
				XmlNode clipTextNode = node.SelectSingleNode("ClipText");
				if (clipTextNode != null)
				{
					XmlAttribute valueAttrib = clipTextNode.Attributes["value"];
					if (valueAttrib != null)
					{
						result.ClipText = bool.Parse(valueAttrib.InnerText);
					}
				}
			}
			catch (FormatException e)
			{
				throw new GuiConstructionException("Unable to parse the 'value' attribute on " + name + "'s ClipText node.", e);
			}

			return result;
		}

		/**
		 * Style State Nodes are expected to be in the form:
		 * <StyleState name="NormalState">
		 * 	  <TextColor color="0xF3B211" />
		 * 	  <Background texture="./Images/Buttons/DefaultNormal.psd" />
		 * </StyleState>
		 */
		public StyleState BuildStyleState(XmlNode styleStateNode, StyleState basedOn)
		{
			StyleState result = new StyleState();

			XmlNode textColorNode = styleStateNode.SelectSingleNode("TextColor");
			if (textColorNode != null)
			{
				result.TextColor = ColorUtility.HexToColor(textColorNode.Attributes["color"].InnerText);
			}
			else if (basedOn != null)
			{
				result.TextColor = basedOn.TextColor;
			}

			XmlNode backgroundNode = styleStateNode.SelectSingleNode("Background");
			if (backgroundNode != null)
			{
				result.Background = BuildTexture(backgroundNode, "texture");
			}
			else if (basedOn != null)
			{
				result.Background = basedOn.Background;
			}

			return result;
		}

		/**
		 * Margin Nodes are expected to be in the form:
		 * <Margin name="InternalMargins" value="2 2 2 2" />
		 */
		public GuiMargin BuildMargin(XmlNode marginNode)
		{
			if (marginNode.Attributes["value"] == null)
			{
				throw new GuiConstructionException("Margin Node must have a 'value' attribute.");
			}

			string[] values = marginNode.Attributes["value"].InnerText.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return new GuiMargin(float.Parse(values[0]),
								  float.Parse(values[1]),
								  float.Parse(values[2]),
								  float.Parse(values[3]));
		}

		public IGuiStyle GetStyle(XmlNode widgetWithStyle, System.Type defaultWidgetType)
		{
			return GetNamedStyle(widgetWithStyle, "style", defaultWidgetType);
		}

		public IGuiStyle GetNamedStyle(XmlNode widgetWithStyle, string styleName, System.Type defaultWidgetType)
		{
			IGuiStyle result = null;

			if (mStyles == null)
			{
				ConstructAllStyles();
			}

			XmlAttribute styleAttribute = widgetWithStyle.Attributes[styleName];
			if (styleAttribute != null)
			{
				if (mStyles.ContainsKey(styleAttribute.InnerText))
				{
					result = mStyles[styleAttribute.InnerText];
				}
			}
			else
			{
				result = mManager.GetDefaultStyle(defaultWidgetType);
			}

			return result;
		}

		/// Constructs the appropriate IGuiPosition from the given node
		public static IGuiPosition BuildPosition(XmlNode node)
		{
			IGuiPosition result = null;

			string positionX = GetStringAttributeNoThrow(node, "positionX");
			string positionY = GetStringAttributeNoThrow(node, "positionY");
			string position = GetStringAttributeNoThrow(node, "position");

			// If there's no position attributes listed, just use the origin as a default
			if (positionX == null && positionY == null && position == null)
			{
				return new FixedPosition(0.0f, 0.0f);
			}

			// If there's a position attribute, copy it into the positionX and positionY values
			if (position != null)
			{
				// The delimitter for a size attribute with different height and width values is space
				string[] splitPosition = position.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (splitPosition.Length == 2)
				{ // Space was found (ex. "45 30"), use first value for positionX, second for positionY
					positionX = splitPosition[0];
					positionY = splitPosition[1];
				}
				else if (splitPosition.Length == 1)
				{ // Single value was found, use it for both positionX and positionY
					positionX = splitPosition[0];
					positionY = splitPosition[0];
				}
				else
				{
					throw new GuiConstructionException("Unable to parse the position attribute from the node (" + node.Name + "): " + position);
				}
			}

			if (IsFixedPair(positionX, positionY))
			{
				result = BuildFixedPosition(positionX, positionY, node);
			}
			else if (IsFillParent(positionX, positionY))
			{
				result = BuildFillParent(positionX, positionY);
			}
			else if (IsHorizontalAutoLayout(position))
			{
				result = BuildHorizontalAutoLayout();
			}
			else if (IsMainFrame(positionX, positionY))
			{
				result = BuildMainFrameSizePosition();
			}
			else if (IsHeaderFrame(positionX, positionY))
			{
				result = BuildHeaderFrameSizePosition(positionY);
			}
			else if (IsProcedural(position))
			{
				result = new ProceduralPosition();
			}
			else
			{
				throw new GuiConstructionException("Unable to create a position from the attributes on node (" + node.Name + "): " + positionX + ", " + positionY);
			}

			return result;
		}

		public static bool IsProcedural(string position)
		{
			return position == "Procedural";
		}

		public static bool IsHorizontalAutoLayout(string position)
		{
			return position == "HorizontalAutoLayout";
		}

		public static HorizontalAutoLayout BuildHorizontalAutoLayout()
		{
			return new HorizontalAutoLayout();
		}

		public static FixedPosition BuildFixedPosition(string x, string y, XmlNode node)
		{
			FixedPosition result = null;

			if (x == null)
			{
				throw new ArgumentNullException("x");
			}
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			try
			{
				GuiAnchor localAnchor = BuildAnchor(node, "anchor");
				if (node.Attributes["parentAnchor"] != null)
				{
					GuiAnchor parentAnchor = BuildAnchor(node, "parentAnchor");
					result = new FixedPosition(new Vector2(float.Parse(x), float.Parse(y)), localAnchor, parentAnchor);
				}
				else
				{
					result = new FixedPosition(new Vector2(float.Parse(x), float.Parse(y)), localAnchor);
				}
			}
			catch (FormatException)
			{
				throw new GuiConstructionException("Attempted to create a FixedPosition IGuiPosition from a node that doesn't parse to (float) X, (float) Y.");
			}

			return result;
		}

		/// Constructs the appropriate IGuiSize from the given node
		public static IGuiSize BuildSize(XmlNode node)
		{
			IGuiSize result = null;

			string width = GetStringAttributeNoThrow(node, "width");
			string height = GetStringAttributeNoThrow(node, "height");
			string size = GetStringAttributeNoThrow(node, "size");

			if (width == null && height == null && size == null)
			{
				throw new GuiConstructionException("No size attributes (width and height or a size attribute) were found on node (" + node.Name + ")");
			}

			// If there's a size attribute, copy it into the width and height values
			if (size != null)
			{
				// The delimitter for a size attribute with different height and width values is space
				string[] splitSize = size.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (splitSize.Length == 2)
				{ // Space was found (ex. "45 30"), use first value for width, second for height
					width = splitSize[0];
					height = splitSize[1];
				}
				else if (splitSize.Length == 1)
				{ // Single value was found, use it for both width and height
					width = splitSize[0];
					height = splitSize[0];
				}
				else
				{
					throw new GuiConstructionException("Unable to parse the size attribute from the node (" + node.Name + "): " + size);
				}
			}

			if (width == null || height == null)
			{
				string foundAttrib = width == null ? "height" : "width";
				string missingAttrib = width != null ? "height" : "width";
				throw new GuiConstructionException(foundAttrib + " was found, but no " + missingAttrib + " was found on node (" + node.Name + ")");
			}

			if (IsFixedPair(width, height))
			{
				result = BuildFixedSize(width, height);
			}
			else if (IsFillParent(width, height))
			{
				result = BuildFillParent(width, height);
			}
			else if (IsMainFrame(width, height))
			{
				result = BuildMainFrameSizePosition();
			}
			else if (IsHeaderFrame(width, height))
			{
				result = BuildHeaderFrameSizePosition(height);
			}
			else if (IsExpandText(width, height))
			{
				result = BuildExpandText(width, height);
			}
			else if( IsProcedural(size) )
			{
				result = new ProceduralSize();
			}
			else
			{
				throw new GuiConstructionException("Unable to create a size from the attributes on node (" + node.Name + ")");
			}

			return result;
		}

		private static HeaderFrameSizePosition BuildHeaderFrameSizePosition(string arg2)
		{
			float headerHeight;
			HeaderFrameSizePosition result = null;

			if (arg2 == "HeaderFrame")
			{
				result = new HeaderFrameSizePosition();
			}
			else if (float.TryParse(arg2, out headerHeight))
			{
				result = new HeaderFrameSizePosition(headerHeight);
			}

			if (result == null)
			{
				throw new GuiConstructionException("Unable to parse the 2nd parameter to the HeaderFrame size or position (" + arg2 + ")");
			}

			return result;
		}

		private static bool IsHeaderFrame(string arg1, string arg2)
		{
			return arg1 == "HeaderFrame";
		}

		private static bool IsExpandText(string arg1, string arg2)
		{
			return arg1 == "ExpandText";
		}

		private static ExpandText BuildExpandText(string arg1, string arg2)
		{
			float lineWrapWidth;
			if (float.TryParse(arg2, out lineWrapWidth))
			{
				return new ExpandText(lineWrapWidth);
			}
			return new ExpandText();
		}

		private static MainFrameSizePosition BuildMainFrameSizePosition()
		{
			return new MainFrameSizePosition();
		}

		private static bool IsMainFrame(string width, string height)
		{
			return width == "MainFrame" && height == "MainFrame";
		}

		/// Is this a set of values that can be used for FixedSize or FixedPosition
		// This may be faster if a regex were used.
		public static bool IsFixedPair(string width, string height)
		{
			float widthf;
			float heightf;
			return float.TryParse(width, out widthf) && float.TryParse(height, out heightf);
		}

		public static FixedSize BuildFixedSize(string width, string height)
		{
			FixedSize result = null;

			try
			{
				result = new FixedSize(new Vector2(float.Parse(width), float.Parse(height)));
			}
			catch (FormatException)
			{
				throw new GuiConstructionException("Attempted to create a FixedSize IGuiSize from a node that doesn't parse to (float) width, (float) height.");
			}

			return result;
		}

		public static bool IsFillParent(string width, string height)
		{
			return width == "FillParent" || height == "FillParent";
		}

		public static FillParent BuildFillParent(string width, string height)
		{
			Vector2 fixedSize = Vector2.zero;
			Layout mode;

			float? widthf = null;
			float? heightf = null;

			try
			{
				widthf = float.Parse(width);
			}
			catch (FormatException) { } // widthf will be null

			try
			{
				heightf = float.Parse(height);
			}
			catch (FormatException) { } // heightf will be null

			try
			{
				if (width == "FillParent")
				{
					if (height == "FillParent")
					{
						mode = Layout.BothAxes;
					}
					else
					{
						mode = Layout.Horizontal;

						if (heightf == null)
						{
							throw new GuiConstructionException("Found a FillParent width the only valid values for the height node are 'FillParent' or a number. Found: " + height);
						}

						fixedSize.y = (float)heightf;
					}
				}
				else if (height == "FillParent")
				{
					mode = Layout.Vertical;

					if (widthf == null)
					{
						throw new GuiConstructionException("Found a FillParent height the only valid values for the width node are 'FillParent' or a number. Found: " + width);
					}

					fixedSize.x = (float)widthf;
				}
				else
				{
					throw new GuiConstructionException("Attempted to create a FillParent IGuiSize from a node that doesn't have a width, height or size=\"FillParent\"");
				}
			}
			catch (FormatException e)
			{
				throw new GuiConstructionException("Unable to parse the FillParent IGuiSize", e);
			}

			return new FillParent(mode, fixedSize);
		}

		public static GuiAnchor BuildAnchor(XmlNode widgetNode, string anchorAttributeName)
		{
			string anchorData = GetStringAttributeNoThrow(widgetNode, anchorAttributeName);
			if (anchorData == null)
			{
				return GuiAnchor.TopLeft;
			}

			string[] anchorDataSplit = anchorData.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (anchorDataSplit.Length != 2)
			{
				throw new GuiConstructionException("Unable to parse the anchor data in " + anchorAttributeName + ", should be in the form 'Top Left', 'Center Right', etc. This error happened in node (" + widgetNode.Name + ")");
			}

			try
			{
				GuiAnchor.X horizontal = (GuiAnchor.X)Enum.Parse(typeof(GuiAnchor.X), anchorDataSplit[0]);
				GuiAnchor.Y vertical = (GuiAnchor.Y)Enum.Parse(typeof(GuiAnchor.Y), anchorDataSplit[1]);
				return new GuiAnchor(horizontal, vertical);
			}
			catch (ArgumentException e)
			{ // Thrown from Enum.Parse
				throw new GuiConstructionException("Unrecognized GuiAnchor value in " + anchorData + ", the 1st position must be Left, Center or Right, the 2nd position must be Top, Center or Bottom. This error happened in node (" + widgetNode.Name + ")", e);
			}
		}
	}
}
