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

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public partial class XmlGuiFactory : IGuiFactory
	{
		private List<Action<IWidget>> mOnNewWidget = new List<Action<IWidget>>();
		public IReceipt RegisterOnBuildWidgetProcessor(Action<IWidget> onNewWidget)
		{
			if (onNewWidget == null)
			{
				throw new ArgumentNullException("onNewWidget");
			}

			mOnNewWidget.Add(onNewWidget);
			return new Receipt(delegate()
			{
				mOnNewWidget.Remove(onNewWidget);
			});
		}

		public virtual Pair<IWidget, IGuiPosition> BuildWidget(XmlNode widgetNode)
		{
			Pair<IWidget, IGuiPosition> result = new Pair<IWidget, IGuiPosition>(null, BuildPosition(widgetNode));
			switch (widgetNode.Name)
			{ // Element System.Type
				case "Frame":
					result.First = BuildFrame(widgetNode);
					break;
				case "ScrollFrame":
					result.First = BuildScrollFrame(widgetNode);
					break;
				case "Scrollbar":
					result.First = BuildScrollbar(widgetNode);
					break;
				case "Button":
					result.First = BuildButton(widgetNode);
					break;
				case "PushButton":
					result.First = BuildPushButton(widgetNode);
					break;
				case "Label":
					result.First = BuildLabel(widgetNode);
					break;
				case "Image":
					result.First = BuildImage(widgetNode);
					break;
				case "Textbox":
					result.First = BuildTextbox(widgetNode);
					break;
				case "TabView":
					result.First = BuildTabView(widgetNode);
					break;
				case "TabButton":
					result.First = BuildTabButton(widgetNode);
					break;
				case "GridView":
					result.First = BuildGridLayout(widgetNode);
					break;
				case "ProgressIndicator":
					result.First = BuildProgressIndicator(widgetNode);
					break;
				case "Animation":
					result.First = BuildAnimation(widgetNode);
					break;
			}

			foreach (Action<IWidget> onNewWidget in mOnNewWidget)
			{
				onNewWidget(result.First);
			}

			return result;
		}

		public IWidget BuildAnimation(XmlNode widgetNode)
		{
			string name = BuildName(widgetNode);
			IGuiSize size = BuildSize(widgetNode);
			IGuiStyle style = GetStyle(widgetNode, typeof(GuiAnimation));

			float? playbackFpsDuration = null;
			XmlNode playbackFpsNode = widgetNode.SelectSingleNode("@playbackFps");
			if (playbackFpsNode != null)
			{
				playbackFpsDuration = 1.0f / float.Parse(playbackFpsNode.InnerText);
			}

			List<GuiAnimation.Frame> frames = new List<GuiAnimation.Frame>();
			foreach (XmlNode animationFrameNode in widgetNode.SelectNodes("Frame"))
			{
				float time = 0.0f;
				XmlNode durationNode = animationFrameNode.SelectSingleNode("@duration");
				if (durationNode == null && playbackFpsDuration == null)
				{
					throw new GuiConstructionException("Animation(" + name + ") requires duration information for each of it's Frames (either a playbackFps attribute on the Animation node, or a duration attribute on each of the Frame nodes.");
				}
				else if(durationNode != null)
				{
					time = float.Parse(durationNode.InnerText);
				}
				else
				{
					time = (float)playbackFpsDuration;
				}

				
				Texture2D image = BuildTexture(animationFrameNode, "path");
				frames.Add(new GuiAnimation.Frame(image, time));
			}

			if( frames.Count == 0 )
			{
				throw new GuiConstructionException("Animation(" + name + ") has no Frame nodes.");
			}

			return new GuiAnimation(name, size, style, frames);
		}

		public ProgressIndicator BuildProgressIndicator(XmlNode widgetNode)
		{
			string name = BuildName(widgetNode);
			IGuiSize size = BuildSize(widgetNode);
			IGuiStyle troughStyle = GetNamedStyle(widgetNode, "troughStyle", typeof(ProgressIndicator));
			IGuiStyle progresStyle = GetNamedStyle(widgetNode, "progressStyle", typeof(ProgressIndicator));

			XmlNode orientationAttrib = widgetNode.SelectSingleNode("@orientation");
			if (orientationAttrib == null)
			{
				throw new GuiConstructionException("Cannot build a ProgressIndicator without an 'orientation' attribute");
			}

			ProgressIndicator.Orientation orientation =
				orientationAttrib.InnerText == "Horizontal"
				? ProgressIndicator.Orientation.Horizontal :
				  ProgressIndicator.Orientation.Vertical;

			return new ProgressIndicator(name, size, troughStyle, progresStyle, orientation);
		}

		//We can later add some more information to construct default values for the grid such as default row and column amounts.
		//For now everything is done procedurally. 
		public GridView BuildGridLayout(XmlNode widgetNode)
		{
			GridView result = null;
			string name = BuildName(widgetNode);
			IGuiSize size = BuildSize(widgetNode);
			IGuiStyle style = GetStyle(widgetNode, typeof(GridView));

			List<KeyValuePair<IWidget, IGuiPosition>> widgets = new List<KeyValuePair<IWidget, IGuiPosition>>();
			foreach (XmlNode childNode in widgetNode.ChildNodes)
			{
				Pair<IWidget, IGuiPosition> newWidget = BuildWidget(childNode);
				if (newWidget.First == null)
				{
					throw new GuiConstructionException("Error, all XML nodes under a Frame node should be valid Widgets.");
				}
				widgets.Add(newWidget);
			}

			result = new GridView(name, size, widgets, style);
			return result;
		}

		public GuiFrame BuildFrame(XmlNode frameNode)
		{
			string name = BuildName(frameNode);
			IGuiSize size = BuildSize(frameNode);
			IGuiStyle style = GetStyle(frameNode, typeof(GuiFrame));

			List<KeyValuePair<IWidget, IGuiPosition>> widgets = new List<KeyValuePair<IWidget, IGuiPosition>>();
			foreach (XmlNode widgetNode in frameNode.ChildNodes)
			{
				Pair<IWidget, IGuiPosition> newWidget = BuildWidget(widgetNode);
				if (newWidget.First == null)
				{
					throw new GuiConstructionException("Error, all XML nodes under a Frame node should be valid Widgets.");
				}
				widgets.Add(newWidget);
			}

			return new GuiFrame(name, size, widgets, style);
		}

		public Scrollbar BuildScrollbar(XmlNode scrollbarNode)
		{
			string name = BuildName(scrollbarNode);
			IGuiSize size = BuildSize(scrollbarNode);
			IGuiStyle troughStyle = GetNamedStyle(scrollbarNode, "troughStyle", typeof(Scrollbar));
			IGuiStyle thumbStyle = GetNamedStyle(scrollbarNode, "thumbStyle", typeof(ScrollbarThumb));
			string pathToFrame = GetStringAttribute(scrollbarNode, "scrollFrame");


			if (troughStyle == null)
			{
				throw new GuiConstructionException("UnityDefault style is not supported for Scrollbar Troughs. Scrollbar(" + name + ")\n" + scrollbarNode.OuterXml);
			}
			if (thumbStyle == null)
			{
				throw new GuiConstructionException("UnityDefault style is not supported for Scrollbar Thumbs. Scrollbar(" + name + ")\n" + scrollbarNode.OuterXml);
			}

			if (String.IsNullOrEmpty(pathToFrame))
			{
				throw new GuiConstructionException("Cannot build Scrollbar(" + name + ") without a valid 'scrollFrame' attribute.");
			}
			return new Scrollbar(name, troughStyle, thumbStyle, size, pathToFrame);
		}

		public GuiFrame BuildScrollFrame(XmlNode frameNode)
		{
			string name = BuildName(frameNode);
			IGuiSize size = BuildSize(frameNode);
			IGuiStyle style = GetStyle(frameNode, typeof(GuiFrame));

			List<KeyValuePair<IWidget, IGuiPosition>> widgets = new List<KeyValuePair<IWidget, IGuiPosition>>();
			foreach (XmlNode widgetNode in frameNode.ChildNodes)
			{
				Pair<IWidget, IGuiPosition> newWidget = BuildWidget(widgetNode);
				if (newWidget.First == null)
				{
					throw new GuiConstructionException("Error, all XML nodes under a Frame node should be valid Widgets.");
				}
				widgets.Add(newWidget);
			}

			return new ScrollFrame(name, size, widgets, style);
		}

		public Button BuildButton(XmlNode buttonNode)
		{
			string name = BuildName(buttonNode);
			IGuiSize size = BuildSize(buttonNode);
			IGuiStyle style = GetStyle(buttonNode, typeof(Button));

			Texture image = null;
			if (buttonNode.Attributes["image"] != null)
			{
				image = BuildTexture(buttonNode, "image");
			}
			string text = BuildText(buttonNode);

			return new Button(name, size, style, image, text);
		}

		public Button BuildPushButton(XmlNode buttonNode)
		{
			string name = BuildName(buttonNode);
			IGuiSize size = BuildSize(buttonNode);
			IGuiStyle style = GetStyle(buttonNode, typeof(Button));

			Texture image = null;
			if (buttonNode.Attributes["image"] != null)
			{
				image = BuildTexture(buttonNode, "image");
			}
			string text = BuildText(buttonNode);

			return new PushButton(name, size, style, image, text);
		}

		public Button BuildTabButton(XmlNode buttonNode)
		{
			string name = BuildName(buttonNode);
			IGuiSize size = BuildSize(buttonNode);
			IGuiStyle activatedStyle = GetNamedStyle(buttonNode, "activatedStyle", typeof(TabButton));
			IGuiStyle deactivatedStyle = GetNamedStyle(buttonNode, "deactivatedStyle", typeof(TabButton));

			Texture image = null;
			if (buttonNode.Attributes["image"] != null)
			{
				image = BuildTexture(buttonNode, "image");
			}

			IGuiFrame frame = null;
			XmlNode frameNode = buttonNode.SelectSingleNode("Frame");
			XmlNode scrollFrameNode = buttonNode.SelectSingleNode("ScrollFrame");
			if (frameNode != null)
			{
				frame = BuildFrame(frameNode);
			}
			if (scrollFrameNode != null)
			{
				frame = BuildScrollFrame(scrollFrameNode);
			}

			if (frame == null)
			{
				throw new GuiConstructionException("Cannot create TabButton (" + name + "), cannot find frame child node.");
			}
			return new TabButton(name, size, activatedStyle, deactivatedStyle, image, frame);
		}

		public Label BuildLabel(XmlNode labelNode)
		{
			string name = BuildName(labelNode);
			IGuiSize size = BuildSize(labelNode);
			IGuiStyle style = GetStyle(labelNode, typeof(Label));

			string text = BuildText(labelNode);

			return new Label(name, size, style, text);
		}

		public Image BuildImage(XmlNode imageNode)
		{
			string name = BuildName(imageNode);
			Texture2D texture = BuildTextureOptional(imageNode, "image");
			IGuiStyle style = GetStyle(imageNode, typeof(Image));
			IGuiSize size = BuildSize(imageNode);

			Image result;

			if (texture != null)
			{
				result = new Image(name, style, size, texture);
			}
			else
			{
				result = new Image(name, style, size);
			}
			return result;
		}

		public Textbox BuildTextbox(XmlNode textBoxNode)
		{
			//Console.WriteLine(textBoxNode.OuterXml);
			string name = BuildName(textBoxNode);
			IGuiSize size = BuildSize(textBoxNode);
			IGuiStyle style = GetStyle(textBoxNode, typeof(Textbox));

			string text = BuildText(textBoxNode);

			bool singleLine = true;
			bool userEditable = true;
			int? maxLength = null;

			try
			{
				singleLine = GetBoolAttributeNothrow(textBoxNode, "singleLine", singleLine);
			}
			catch (FormatException)
			{
				// Not Found, using default
			}

			try
			{
				userEditable = GetBoolAttributeNothrow(textBoxNode, "userEditable", userEditable);
			}
			catch (FormatException)
			{
				// Not Found, using default
			}

			try
			{
				maxLength = GetIntAttribute(textBoxNode, "maxLength", maxLength);
			}
			catch (FormatException)
			{
				// Not Found, using default
			}

			return new Textbox(name, size, style, text, singleLine, userEditable, maxLength);
		}

		public TabView BuildTabView(XmlNode tabViewNode)
		{
			string name = BuildName(tabViewNode);

			XmlNode buttonsFrameNode = tabViewNode.SelectSingleNode("ButtonsFrame");
			if (buttonsFrameNode == null)
			{
				throw new GuiConstructionException("Unable to find the ButtonsFrame node that should be a child of TabView (" + name + ")");
			}

			GuiFrame buttonFrame = BuildFrame(buttonsFrameNode);
			IGuiPosition buttonFramePosition = BuildPosition(buttonsFrameNode);


			XmlNode contextFrameNode = tabViewNode.SelectSingleNode("ContextFrame");
			if (contextFrameNode == null)
			{
				throw new GuiConstructionException("Unable to find the ContextFrame node that should be a child of TabView (" + name + ")");
			}

			GuiFrame contextFrame = BuildFrame(contextFrameNode);
			IGuiPosition contextFramePosition = BuildPosition(contextFrameNode);

			IGuiSize size = BuildSize(tabViewNode);
			bool allowEmpty = GetBoolAttributeNothrow(tabViewNode, "allowEmpty", false);

			return new TabView
			(
				name,
				size,
				buttonFrame,
				buttonFramePosition,
				contextFrame,
				contextFramePosition,
				allowEmpty
			);
		}

		public Texture2D BuildTexture(XmlNode widgetNode, string attributeName)
		{
			if (widgetNode == null)
			{
				throw new ArgumentNullException("widgetNode");
			}

			if (widgetNode.Attributes[attributeName] != null)
			{
				Texture2D result = null;

				// attributeName="" can be specified to use an empty (null) texture
				if (widgetNode.Attributes[attributeName].InnerText != String.Empty)
				{
					result = GetAssetAtPath<Texture2D>(widgetNode.Attributes[attributeName].InnerText);
					if (result == null)
					{
						throw new GuiConstructionException("Unable to load texture from Node(" + widgetNode.Name + "), Attribute(" + attributeName + "), Path(" + widgetNode.Attributes[attributeName].InnerText + ")");
					}
				}

				return result;
			}
			else
			{
				throw new GuiConstructionException("attributeName (" + attributeName + ") wasn't found on XML Node (" + widgetNode.Name + ")");
			}
		}

		/// <summary>
		/// Build Texture, can return null if there is no attribute with the given name
		/// </summary>
		public Texture2D BuildTextureOptional(XmlNode widgetNode, string attributeName)
		{
			Texture2D result = null;

			XmlNode pathNode = widgetNode.SelectSingleNode("@" + attributeName);
			if (pathNode != null)
			{
				// attributeName="" can be specified to use an empty (null) texture
				if (pathNode.InnerText != String.Empty)
				{
					result = GetAssetAtPath<Texture2D>(pathNode.InnerText);
					if (result == null)
					{
						throw new GuiConstructionException
						(
							"Unable to load texture from Node(" +
							widgetNode.Name +
							"), Attribute(" +
							attributeName +
							"), Path(" +
							pathNode.InnerText +
							")"
						);
					}
				}
			}

			return result;
		}

		public Font BuildFont(XmlNode widgetNode, string attributeName)
		{
			if (widgetNode == null)
			{
				throw new ArgumentNullException("widgetNode");
			}

			if (widgetNode.Attributes[attributeName] != null)
			{
				Font result = GetAssetAtPath<Font>(widgetNode.Attributes[attributeName].InnerText);
				if (result == null)
				{
					throw new GuiConstructionException("Unable to load font from Node(" + widgetNode.Name + "), Attribute(" + attributeName + "), Path(" + widgetNode.Attributes[attributeName].InnerText + ")");
				}
				return result;
			}
			else
			{
				throw new GuiConstructionException("attributeName (" + attributeName + ") wasn't found on XML Node (" + widgetNode.Name + ")");
			}
		}

		/// In the default XmlGuiFactory, paths are relative to the 'Resources' directory.
		public virtual T GetAssetAtPath<T>(string path) where T : UnityEngine.Object
		{
			return (T)Resources.Load(path, typeof(T));
		}

		public string BuildText(XmlNode widgetNode)
		{
			string result = null;
			if (widgetNode.Attributes["text"] != null)
			{
				result = widgetNode.Attributes["text"].InnerText;
			}
			return result;
		}
	}
}



