/**  --------------------------------------------------------  *
 *   TextBubble.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/12/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class TextBubble : IDisposable
	{
		private const string mResourcePath = "resources://GUI/Chat/Talk Bubbles/Text Bubble.gui";
		private const string mTweakablesPath = "resources://Tweaks/Textbubble.tweaks";

		private Window mBubbleWindow = null;

		[Tweakable("Bubble Max Width")]
		private float mMaxWidth = 200.0f;

		[Tweakable("Bubble Min Width")]
		private float mMinWidth = 100.0f;

		private readonly TweakablesHandler mTweakablesHandler;
		private readonly IGuiManager mGuiManager;

		private FollowWorldSpaceObject mFollowWorldSpaceObject;

		public bool Showing
		{
			get
			{
				bool result = false;
				if (mBubbleWindow != null)
				{
					result = mBubbleWindow.Showing;
				}
				return result;
			}
			set
			{
				if (mBubbleWindow != null)
				{
					mBubbleWindow.Showing = value;
				}
			}
		}

		~TextBubble()
		{
			mGuiManager.UnregisterTopLevel(mBubbleWindow);
		}

		public void Dispose()
		{
			mFollowWorldSpaceObject.Dispose();
			//originally the bubble window was being unregistered with the gui in the next frame
			// apparently it was done that way as a fix to a bug that we dont remember anymore... 
			// ...but we need this object to clean itself up this frame in case the avatar needs to be deleted while the next bubble is being displayed
			// if some bug regresses with the bubble window being deleted this frame, we should probably try and find another way to fix it rather than 
			// waiting a frame to clean it up.
			mGuiManager.UnregisterTopLevel(mBubbleWindow);
		}
		public TextBubble(IGuiManager guiManager, string text, Transform worldPointToFollow, Camera camera)
		{
			mTweakablesHandler = new TweakablesHandler(mTweakablesPath, this);
			IgnoreUnusedVariableWarning(mTweakablesHandler);

			if (guiManager == null)
			{
				throw new ArgumentNullException("guiManager");
			}
			mGuiManager = guiManager;
			XmlGuiFactory loadTextBubble = new XmlGuiFactory(mResourcePath, guiManager);
			IGuiStyle bubbleStyle = null;
			IGuiStyle textStyle = null;
			foreach (IGuiStyle style in loadTextBubble.ConstructAllStyles())
			{
				if (style == null)
				{
					continue;
				}
				if (style.Name == "TextBubbleStyleWindow")
				{
					bubbleStyle = style;
				}
				else if (style.Name == "TextBubbleStyleBase")
				{
					textStyle = style;
				}
			}

			if (bubbleStyle == null)
			{
				throw new Exception("Unable to load TextBubbleStyleWindow from xml file at 'Resources/" + mResourcePath + ".xml'");
			}

			if (textStyle == null)
			{
				throw new Exception("Unable to load TextBubbleStyleBase from xml file at 'Resources/" + mResourcePath + ".xml'");
			}

			foreach (IGuiElement element in loadTextBubble.ConstructAllElements())
			{
				if (element is Window && element.Name == "TextBubbleWindow")
				{
					mBubbleWindow = (Window)element;
				}
			}

			if (mBubbleWindow == null)
			{
				throw new Exception("Unable to load TextBubbleWindow from xml file at 'Resources/" + mResourcePath + ".xml'");
			}

			mFollowWorldSpaceObject = new FollowWorldSpaceObject
				(
					camera,
					worldPointToFollow,
					GuiAnchor.BottomLeft,
					new Vector2(0.0f, 0),        // SDN: TODO:  make these offsets configurable
					new Vector3(0.0f, 1.8f, 0.0f)
				);

			guiManager.SetTopLevelPosition
			(
				mBubbleWindow,
				mFollowWorldSpaceObject
			);

			Textbox bubbleText = mBubbleWindow.SelectSingleElement<Textbox>("**/TextBubbleTextbox");
			mBubbleWindow.Style = bubbleStyle;
			bubbleText.Text = text;

			Image bubbleImage = mBubbleWindow.SelectSingleElement<Image>("**/TextBubbleImage");
			GuiFrame mainFrame = mBubbleWindow.SelectSingleElement<GuiFrame>("**/TextBubbleFrame");
			mainFrame.RemoveChildWidget(bubbleImage);

			// Set the size of the window to be the smallest that draws the entire chat message
			UnityEngine.GUIStyle unityStyle = bubbleText.Style.GenerateUnityGuiStyle();
			unityStyle.wordWrap = false;
			GUIContent textContent = new GUIContent(text);

			Vector2 size = unityStyle.CalcSize(textContent);
			if (size.x > mMaxWidth)
			{
				size.x = mMaxWidth;
				unityStyle.wordWrap = true;
				size.y = unityStyle.CalcHeight(textContent, mMaxWidth);
			}
			if (size.x < mMinWidth)
			{
				size.x = mMinWidth;
			}
			size.x += mBubbleWindow.Style.InternalMargins.Left + mBubbleWindow.Style.InternalMargins.Right;
			size.y += mBubbleWindow.Style.InternalMargins.Top + mBubbleWindow.Style.InternalMargins.Bottom;

			mBubbleWindow.GuiSize = new FixedSize(size);
		}

		public TextBubble(IGuiManager guiManager, Texture2D texture, Transform worldPointToFollow, Camera camera)
		{
			mTweakablesHandler = new TweakablesHandler(mTweakablesPath, this);
			IgnoreUnusedVariableWarning(mTweakablesHandler);

			if (guiManager == null)
			{
				throw new ArgumentNullException("guiManager");
			}
			mGuiManager = guiManager;
			XmlGuiFactory loadTextBubble = new XmlGuiFactory(mResourcePath, guiManager);
			IGuiStyle bubbleStyle = null;
			IGuiStyle textStyle = null;
			foreach (IGuiStyle style in loadTextBubble.ConstructAllStyles())
			{
				if (style == null)
				{
					continue;
				}
				if (style.Name == "TextBubbleStyleWindow")
				{
					bubbleStyle = style;
				}
				else if (style.Name == "TextBubbleStyleBase")
				{
					textStyle = style;
				}
			}

			if (bubbleStyle == null)
			{
				throw new Exception("Unable to load TextBubbleStyleWindow from xml file at 'Resources/" + mResourcePath + ".xml'");
			}

			if (textStyle == null)
			{
				throw new Exception("Unable to load TextBubbleStyleBase from xml file at 'Resources/" + mResourcePath + ".xml'");
			}

			foreach (IGuiElement element in loadTextBubble.ConstructAllElements())
			{
				if (element is Window && element.Name == "TextBubbleWindow")
				{
					mBubbleWindow = (Window)element;
				}
			}

			if (mBubbleWindow == null)
			{
				throw new Exception("Unable to load TextBubbleWindow from xml file at 'Resources/" + mResourcePath + ".xml'");
			}

			mFollowWorldSpaceObject = new FollowWorldSpaceObject
			(
				camera,
				worldPointToFollow,
				GuiAnchor.BottomLeft,
				new Vector2(0.0f, 0.0f),        // SDN: TODO:  make these offsets configurable
				new Vector3(0.0f, 1.8f, 0.0f)
			);

			guiManager.SetTopLevelPosition
			(
				mBubbleWindow,
				mFollowWorldSpaceObject
			);


			mBubbleWindow.Style = bubbleStyle;

			Image bubbleImage = mBubbleWindow.SelectSingleElement<Image>("**/TextBubbleImage");
			bubbleImage.Texture = texture;

			GuiFrame mainFrame = mBubbleWindow.SelectSingleElement<GuiFrame>("**/TextBubbleFrame");
			Textbox bubbleText = mBubbleWindow.SelectSingleElement<Textbox>("**/TextBubbleTextbox");
			mainFrame.RemoveChildWidget(bubbleText);

			Vector2 size = new Vector2(texture.width, texture.height);

			size.x += mBubbleWindow.Style.InternalMargins.Left + mBubbleWindow.Style.InternalMargins.Right;
			size.y += mBubbleWindow.Style.InternalMargins.Top + mBubbleWindow.Style.InternalMargins.Bottom;

			mBubbleWindow.GuiSize = new FixedSize(size);
		}

		private static void IgnoreUnusedVariableWarning(object o)
		{

		}
	}
}
