/**  --------------------------------------------------------  *
 *   RuntimeGuiManager.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

namespace Hangout.Client.Gui
{

	/// <summary>
	/// This is the GUI manager that handles all the draw calls for GUIs in the game.
	/// </summary>
	public class RuntimeGuiManager : Mediator, IMediator, IGuiManager
	{
		private static readonly string mDefaultStylePath = "resources://GUI/Styles/Default.style";
		private readonly bool mLoadDefaults;
		private readonly ILogger mLogger;

		public T SelectSingleElement<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectSingleElement<T>(this);
		}

		public IEnumerable<T> SelectElements<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectElements<T>(this);
		}

		private class StyleTypeNode
		{
			private System.Type mRegisteredType;
			private List<StyleTypeNode> mChildren = new List<StyleTypeNode>();
			private IGuiStyle mDefaultStyle = null;
			private StyleTypeNode mParent = null;

			public StyleTypeNode Parent
			{
				get { return mParent; }
				set { mParent = value; }
			}
			public IGuiStyle DefaultStyle
			{
				get { return mDefaultStyle; }
				set { mDefaultStyle = value; }
			}
			public System.Type RegisteredType
			{
				get { return mRegisteredType; }
			}
			public List<StyleTypeNode> Children
			{
				get { return mChildren; }
			}

			
			//Currently a linear search, can easily be optimized if necessary 
			public StyleTypeNode Find(System.Type t)
			{
				if (t == mRegisteredType)
				{
					return this;
				}
				else
				{
					foreach (StyleTypeNode child in this.Children)
					{
						StyleTypeNode result = child.Find(t);
						if (result != null)
						{
							return result;
						}
					}
				}
				return null;
			}

			public StyleTypeNode(System.Type t, IGuiStyle style)
			{
				mRegisteredType = t;
				mDefaultStyle = style;
			}

			public StyleTypeNode(StyleTypeNode copy)
			{
				mRegisteredType = copy.RegisteredType;
				mDefaultStyle = copy.DefaultStyle;
				mChildren = new List<StyleTypeNode>(copy.Children);
				mParent = copy.Parent;
			}
		}

		// Root of the tree of style defaults for each type
		private StyleTypeNode mDefaultStylesRoot = null;

		public void LoadDefaultStylesFromXml(XmlDocument styleData)
		{
			XmlGuiFactory factory = new XmlGuiFactory(styleData, this);
			LoadDefaultStylesFromFactory(factory);
		}

		private void LoadDefaultStylesFromResources()
		{
			XmlGuiFactory factory = new XmlGuiFactory(mDefaultStylePath, this);
			LoadDefaultStylesFromFactory(factory);
		}

		private void LoadDefaultStylesFromFactory(XmlGuiFactory factory)
		{
			IEnumerable<IGuiStyle> styles = factory.ConstructAllStyles();

			// Find the root style
			foreach (IGuiStyle style in styles)
			{
				if (style != null)
				{
					foreach (System.Type t in style.DefaultFor())
					{
						if (t == typeof(GuiElement))
						{
							mDefaultStylesRoot = new StyleTypeNode(t, style);
							break;
						}
					}

					if (mDefaultStylesRoot != null)
					{
						break;
					}
				}
			}

			if (mDefaultStylesRoot == null)
			{
				throw new Exception("Cannot load default styles, no default for GuiElement was defined.");
			}

			// Insert each style into the tree
			foreach (IGuiStyle style in styles)
			{
				if (style != null)
				{
					foreach (System.Type t in style.DefaultFor())
					{
						if (t != typeof(GuiElement))
						{ // GuiElement should have already been inserted
							InsertStyleDefault(mDefaultStylesRoot, t, style);
						}
					}
				}
			}

			/*
			// Debug.Log the loaded heiarchy of style defaults
			StringBuilder sb = new StringBuilder("\n"); 
			foreach( string line in ListDefaults(mDefaultStylesRoot, 0) ) 
			{
				sb.AppendLine(line);
			}
			Debug.Log(sb);
			*/
		}

		private List<string> ListDefaults(StyleTypeNode root, uint level)
		{
			StringBuilder thisLine = new StringBuilder();
			for (uint i = 0; i < level; ++i)
			{
				thisLine.Append("\t");
			}
			thisLine.Append(root.RegisteredType.Name);
			thisLine.Append(" => ");
			if (root.DefaultStyle != null)
			{
				thisLine.Append(root.DefaultStyle.Name);
			}
			else
			{
				thisLine.Append("null");
			}
			List<string> result = new List<string>();

			result.Add(thisLine.ToString());
			foreach (StyleTypeNode child in root.Children)
			{
				result.AddRange(ListDefaults(child, level + 1));
			}

			return result;
		}

		private static StyleTypeNode FindClosestAncestor(StyleTypeNode root, System.Type type)
		{
			if (root.RegisteredType == type.BaseType || root.Children.Count == 0)
			{
				return root;
			}

			foreach (StyleTypeNode child in root.Children)
			{
				if (type.IsSubclassOf(child.RegisteredType))
				{
					return FindClosestAncestor(child, type);
				}
			}

			return root;
		}

		private static void InsertStyleDefault(StyleTypeNode root, System.Type type, IGuiStyle style)
		{
			if (type.IsSubclassOf(root.RegisteredType))
			{
				// Construct the branch to this type
				StyleTypeNode newBranchRoot = root.Find(type);
				if (newBranchRoot == null)
				{
					newBranchRoot = new StyleTypeNode(type, style);

					// Create empty elements up a known point in the tree
					StyleTypeNode ancestor = FindClosestAncestor(root, type);

					while (newBranchRoot.RegisteredType.BaseType != ancestor.RegisteredType)
					{
						StyleTypeNode oldBranchRoot = new StyleTypeNode(newBranchRoot);
						newBranchRoot = new StyleTypeNode(oldBranchRoot.RegisteredType.BaseType, null);
						oldBranchRoot.Parent = newBranchRoot;
						newBranchRoot.Children.Add(oldBranchRoot);
					}

					newBranchRoot.Parent = ancestor;
					ancestor.Children.Add(newBranchRoot);
				}

			}
			else
			{
				throw new ArgumentException("Argument type(" + type.Name + ") must be a subclass of root.RegisteredType(" + root.RegisteredType.Name + ")");
			}
		}

		public ILogger Logger
		{
			get { return mLogger; }
		}

		public RuntimeGuiManager(ILogger logger)
			: this(true, logger) 
		{
		}

		public RuntimeGuiManager(bool loadDefaults, ILogger logger)
			: base()
		{
			mLoadDefaults = loadDefaults;
			if( logger == null )
			{
				throw new ArgumentNullException("logger");
			}
			mLogger = logger;
		}

		public IGuiStyle GetDefaultStyle(System.Type elementType)
		{
			if (mLoadDefaults && mDefaultStylesRoot == null)
			{
				LoadDefaultStylesFromResources();
			}

			return FindDefaultStyle(mDefaultStylesRoot, elementType);
		}

		private IGuiStyle FindDefaultStyle(StyleTypeNode root, System.Type elementType)
		{
			IGuiStyle result = null;
			
			if( root != null )
			{
				if (root.RegisteredType == elementType)
				{
					result = root.DefaultStyle;
				}
				else
				{
					foreach (StyleTypeNode child in root.Children)
					{
						if (elementType.IsSubclassOf(child.RegisteredType))
						{
							result = FindDefaultStyle(child, elementType);
						}
						else if (elementType == child.RegisteredType)
						{
							result = child.DefaultStyle;
						}
					}

					if (result == null)
					{
						result = root.DefaultStyle;
					}
				}
			}
			
			return result;
		}

		public string ContainerName
		{
			get { return this.GetType().Name; }
		}

		// Dictionary of topLevels and topLevel position
		private IDictionary<ITopLevel, IGuiPosition> mRegisteredGuiList = new Dictionary<ITopLevel, IGuiPosition>();

		public int ActiveTopLevelCount
		{
			get { return mRegisteredGuiList.Count; }
		}

		public ICollection<ITopLevel> OccludingTopLevels(Vector2 screenPoint)
		{
			List<ITopLevel> result = new List<ITopLevel>();
			Vector2 pnt = new Vector2(screenPoint.x, ((float)Screen.height) - screenPoint.y);
			foreach (KeyValuePair<ITopLevel, IGuiPosition> topLevel in mRegisteredGuiList)
			{
				if (!topLevel.Key.Showing)
				{
					continue;
				}

				Vector2 size = topLevel.Key.ExternalSize;
				Vector2 position = topLevel.Value.GetPosition(topLevel.Key);
				Rect coords = new Rect(position.x, position.y, size.x, size.y);

				if (coords.Contains(pnt))
				{
					result.Add(topLevel.Key);
				}
			}

			return result;
		}

		public IEnumerable<IGuiElement> Children
		{
			get 
			{
				foreach(ITopLevel topLevel in mRegisteredGuiList.Keys)
				{
					yield return topLevel;
				}
			}
		}
		
		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			return GetChildGuiPosition(childElement).GetPosition(childElement);
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			foreach(KeyValuePair<ITopLevel, IGuiPosition> gui in mRegisteredGuiList)
			{
				if( childElement == gui.Key )
				{
					return gui.Value;
				}
			}

			throw new Exception("GuiManager(" + this.ContainerName + ") does not contain Element(" + childElement.Name + ")");
		}



		public void RegisterTopLevel(ITopLevel gui, IGuiPosition position)
		{
			mRegisteredGuiList.Add(gui, position);
		}

		public void SetTopLevelPosition(ITopLevel topLevel, IGuiPosition position)
		{
			if (!mRegisteredGuiList.ContainsKey(topLevel))
			{
				throw new ArgumentException("Unable to set position for unknown ITopLevel(" + topLevel.Name + ")", "topLevel");
			}

			mRegisteredGuiList[topLevel] = position;
		}

        public IGuiPosition GetTopLevelPosition(ITopLevel topLevel)
        {
            if (!mRegisteredGuiList.ContainsKey(topLevel))
            {
                throw new ArgumentException("Unable to get position for unknown ITopLevel(" + topLevel.Name + ")", "topLevel");
            }

            return mRegisteredGuiList[topLevel];
        }

		public void UnregisterTopLevel(ITopLevel gui)
		{
			mRegisteredGuiList.Remove(gui);
		}

		/// <summary>
		/// This should be called once per Unity OnGUI call.
		/// </summary>
		public void Draw()
		{
			foreach (KeyValuePair<ITopLevel, IGuiPosition> topLevel in mRegisteredGuiList)
			{
				try
				{
					Vector2 position = topLevel.Value.GetPosition(topLevel.Key);
					// TODO: This is causing some layout bugs, so it's being disabled until it can be addressed
					//position = GetValidInScreenPosition(topLevel.Key, position);

					//mRegisteredGuiList[topLevel.Key].UpdatePosition(topLevel.Key, position);
					if (topLevel.Key.Showing)
					{
						topLevel.Key.Draw(this, position);
					}
				}
				catch (Exception e)
				{
					mLogger.Log(e.ToString(), LogLevel.Error);
				}
			}
		
			mLogger.Flush();	
		}

		public void UpdateChildPosition(IGuiElement element, UnityEngine.Vector2 position)
		{
			if (!(element is ITopLevel))
			{
				throw new ArgumentException("The GUI Manager cannot contain any non-ITopLevel IGuiElements. This happend on element (" + element.Name + ")", "element");
			}
			ITopLevel topLevel = (ITopLevel)element;
			if (!mRegisteredGuiList.ContainsKey(topLevel))
			{
				throw new ArgumentException("Cannot update child topLevel that hasn't been registered. This happend on element (" + element.Name + ")", "element");
			}

			mRegisteredGuiList[topLevel].UpdatePosition(element, position);
		}

		private Vector2 GetValidInScreenPosition(ITopLevel topLevel, Vector2 currentPosition)
		{
			ScreenEdgeTest edgeTest = new ScreenEdgeTest
			(
				new Rect
				(
					currentPosition.x,
					currentPosition.y,
					topLevel.ExternalSize.x,
					topLevel.ExternalSize.y
				)
			);

			// If this window is larger than the screen horizontally or vertically, don't force into viewport... it won't fit
			if (edgeTest.IsEitherDimensionLargerThanScreen())
			{
				return currentPosition;
			}

			Vector2 result = currentPosition;
			if (!edgeTest.Right)
			{
				result.x = Screen.width - topLevel.ExternalSize.x;
			}
			else if (!edgeTest.Left)
			{
				result.x = 0.0f;
			}

			if (!edgeTest.Bottom)
			{
				result.y = Screen.height - topLevel.ExternalSize.y;
			}
			else if (!edgeTest.Top)
			{
				result.y = 0.0f;
			}

			return result;
		}
	}
}