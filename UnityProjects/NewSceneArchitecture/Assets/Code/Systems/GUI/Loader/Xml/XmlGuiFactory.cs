/**  --------------------------------------------------------  *
 *   XmlGuiFactory.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/20/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

namespace Hangout.Client.Gui 
{
	public class GuiConstructionException : Exception 
	{
		public GuiConstructionException(string message) : base(message) { }
		public GuiConstructionException(string message, Exception innerException) : base(message, innerException) { }
	}
	
	public partial class XmlGuiFactory : IGuiFactory 
	{
		private readonly XmlDocument mGuiDoc;
		protected readonly IGuiManager mManager;
		private Dictionary<string, IGuiStyle> mStyles = null; // Constructed styles: Key = style name, Value = style

		public IGuiManager Manager 
		{
			get { return mManager; }
		}
		
		/// Load a GUI from XML
		public XmlGuiFactory(XmlDocument xml, IGuiManager manager)
		{
		 	if( manager == null )
		 	{
		 	   throw new ArgumentNullException("manager");
		 	}
			mManager = manager;
			
		 	if( xml == null )
		 	{
		 	   throw new ArgumentNullException("xml");
		 	}
			mGuiDoc = XmlUtility.Resolve(xml, "GUI");
			
			this.ConstructAllStyles(); // set up mStyles
		}
		
		
		/// Load a GUI from the given path
		public XmlGuiFactory(string path, IGuiManager manager)
		: this(XmlUtility.LoadXmlDocument(path), manager) 
		{	
		}		
		/// Constructs all the gui elements that are in the /GUI/* location in the XML.
		public IEnumerable<IGuiElement> ConstructAllElements() 
		{
			if( mStyles == null ) 
			{
				throw new Exception("Expected ConstructAllStyles to cache the styles in mStyles.");
			}
			
			List<IGuiElement> result = new List<IGuiElement>();
			
			XmlNode rootNode = mGuiDoc.SelectSingleNode("GUI");
			if( rootNode == null ) 
			{
				throw new GuiConstructionException("Unable to load XML, no root 'GUI' element found.");
			}
			
			// Windows and Widgets can exist at the root level (although only windows can be displayed)
			for( XmlNode childNode = rootNode.FirstChild; childNode != null; childNode = childNode.NextSibling ) 
			{
				if( childNode.Name == "Window" ) 
				{
					result.Add(BuildWindow(childNode));
				}
				else if( childNode.Name != "Style") 
				{
					// Widgets at the top level are positionless (they need to be added to a window to be displayed)
					result.Add(BuildWidget(childNode).First); 
				}
			}
			
			return result.ToArray();
		}
        
        protected static int? GetIntAttribute(XmlNode node, string attributeName, int? defaultValue)
        {
            if (node != null)
            {
                if (node.Attributes != null && node.Attributes.Count > 0)
                {
                    // This is a terrible way of doing this but the Attributes array accessors seem bugged
                    foreach (XmlAttribute a in node.Attributes)
                    {
                        if (a.Name == attributeName)
                        {
                            return int.Parse(a.InnerText);
                        }
                    }
                }
            }
            return defaultValue;
        }		
        
		protected static float GetFloatAttribute(XmlNode node, string attributeName) 
		{
			float result = 0.0f;
			
			try 
			{
				if( node.Attributes[attributeName] != null ) 
				{
					result = float.Parse(node.Attributes[attributeName].InnerText);
				}
				else 
				{
					throw new GuiConstructionException("Unable to find width or height from node (" + node.Name + ")");
				}
			} 
			catch (FormatException e) 
			{
				throw new GuiConstructionException("Unable to parse width or height from node (" + node.Name + ")", e);
			}
			
			return result;
		}
		
		protected static string GetStringAttribute(XmlNode node, string attributeName) 
		{
			string result = null;
			
			try 
			{
				if( node.Attributes[attributeName] != null ) 
				{
					result = node.Attributes[attributeName].InnerText;
				} 
				else 
				{
					throw new GuiConstructionException("Unable to find attribute (" + attributeName + ") node (" + node.Name + ")");
				}
			} 
			catch (FormatException e) 
			{
				throw new GuiConstructionException("Unable to parse attribute (" + attributeName + ") node (" + node.Name + ")", e);
			}
			
			return result;
		}
		
		protected static string GetStringAttributeNoThrow(XmlNode node, string attributeName) 
		{
			string result = null;
			
			if( node == null || attributeName == null )
			{
				return result;
			}
			
			try 
			{
				if( node.Attributes[attributeName] != null ) 
				{
					result = node.Attributes[attributeName].InnerText;
				}
			} 
			catch (FormatException) 
			{
				// result will be null
			}
			
			return result;
		}
		
		/// Throws: GuiConstructionException
		private static bool GetBoolAttribute(XmlNode node, string attributeName) 
		{
			string attribute = GetStringAttributeNoThrow(node, attributeName);
			return bool.Parse(attribute);
		}
		
		private static bool GetBoolAttributeNothrow(XmlNode node, string attributeName, bool defaultValue) 
		{
			try {
				return GetBoolAttribute(node, attributeName);
			} 
			catch(FormatException) 
			{
			} 
			catch(ArgumentException) 
			{
			}
			
			return defaultValue;
		}
		
		public static string BuildName(XmlNode node) 
		{
			string result = GetStringAttributeNoThrow(node, "name");
			if( result == null ) 
			{
				throw new GuiConstructionException("Expected a 'name' attribute on " + node.Name);
			}
			return result;
		}
		
		public static DragBehavior BuildDragBehavior(XmlNode node) 
		{
			DragBehavior result;

			if( node.Attributes["dragBehavior"] != null ) 
			{
				try
				{
					result = (DragBehavior)Enum.Parse(typeof(DragBehavior), node.Attributes["dragBehavior"].InnerText, true);
				} 
				catch (ArgumentException e) 
				{
					throw new GuiConstructionException("Unrecognized DragBehavior type (" + node.Attributes["dragBehavior"].InnerText + ") in node (" + node.Name + ")", e);
				}
			} 
			else 
			{
				result = Window.DefaultDragBehavior;
			}

			return result;
		}
		
		protected virtual Window BuildWindow(XmlNode windowNode) 
		{
			string name = BuildName(windowNode);
			IGuiSize size = BuildSize(windowNode);
			
			IGuiFrame mainFrame = null;
			IGuiPosition mainFramePosition = null;
			XmlNode mainFrameNode = windowNode.SelectSingleNode("MainFrame");
			if( mainFrameNode != null ) 
			{
				mainFramePosition = BuildPosition( mainFrameNode );
				mainFrame = BuildFrame( mainFrameNode );
				if( mainFrame == null ) 
				{
					throw new GuiConstructionException("Found MainFrame node in Window (" + name + "), but was unable to construct it.");
				}
			}
			
			IGuiFrame headerFrame = null;
			IGuiPosition headerFramePosition = null;
			XmlNode headerFrameNode = windowNode.SelectSingleNode("HeaderFrame");
			if( headerFrameNode != null ) 
			{
				headerFramePosition = BuildPosition(headerFrameNode);
				headerFrame = BuildFrame(headerFrameNode);
				if( headerFrame == null ) 
				{
					throw new GuiConstructionException("Found HeaderFrame node in Window (" + name + "), but was unable to construct it.");
				}
			}
			
			DragBehavior dragBehavior = BuildDragBehavior( windowNode );

			IGuiStyle style = GetStyle(windowNode, typeof(Window));
			
			if( windowNode.Attributes["position"] != null ) 
			{
				IGuiPosition position = BuildPosition(windowNode);
				return new Window
				(
					name, 
					size, 
					position, 
					mManager,  
					new KeyValuePair<IGuiFrame, IGuiPosition>(
						mainFrame, 
						mainFramePosition
					), 
					new KeyValuePair<IGuiFrame, IGuiPosition>(
						headerFrame, 
						headerFramePosition
					),
					dragBehavior, 
					style
				);
			}
			else 
			{
				return new Window
				(
					name, 
					size, 
					mManager, 
					new KeyValuePair<IGuiFrame, IGuiPosition>
					(
						mainFrame, 
						mainFramePosition
					), 
					new KeyValuePair<IGuiFrame, IGuiPosition>
					(
						headerFrame, 
						headerFramePosition
					),
					dragBehavior, 
					style
				);
			}
		}
	}
}
