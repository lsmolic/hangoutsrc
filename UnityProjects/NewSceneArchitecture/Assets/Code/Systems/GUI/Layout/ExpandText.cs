/**  --------------------------------------------------------  *
 *   ExpandText.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/29/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	// Make the size of an element expand around the text in it
	public class ExpandText : IGuiSize
	{
		private float mLineWrapWidth;
		public float LineWrapWidth
		{
			get { return mLineWrapWidth; }
			set { mLineWrapWidth = value; }
		}
		
		public ExpandText(float lineWrapWidth)
		{
			if( lineWrapWidth <= 0.0f )
			{
				throw new ArgumentOutOfRangeException("lineWrapWidth must be a positive non-zero value");
			}
			
			mLineWrapWidth = lineWrapWidth;
		}
		
		public ExpandText()
		{
			mLineWrapWidth = float.PositiveInfinity;
		}
		
		public Vector2 GetSize(IGuiElement element)
		{
			if( element == null )
			{
				throw new ArgumentNullException("element");
			}
			if( !(element is ITextGuiElement) )
			{
				throw new ArgumentException("ExpandText only works on ITextGuiElements", "element");
			}
			if( element.Style == null )
			{
				throw new Exception("ExpandText does not work with the built in unity style");
			}
			
			ITextGuiElement textElement = (ITextGuiElement)element;
			UnityEngine.GUIStyle unityStyle = textElement.Style.GenerateUnityGuiStyle();
			
			unityStyle.wordWrap = false;
			GUIContent textContent = new GUIContent(textElement.Text);
			
			Vector2 size = unityStyle.CalcSize(textContent);
			
			if( size.x > mLineWrapWidth )
			{
				size.x = mLineWrapWidth;
				unityStyle.wordWrap = true;
				size.y = unityStyle.CalcHeight(textContent, mLineWrapWidth);
			}
			
			size.x += textElement.Style.InternalMargins.Left + textElement.Style.InternalMargins.Right;
			size.y += textElement.Style.InternalMargins.Top + textElement.Style.InternalMargins.Bottom;

			return size;
		}
	}	
}
