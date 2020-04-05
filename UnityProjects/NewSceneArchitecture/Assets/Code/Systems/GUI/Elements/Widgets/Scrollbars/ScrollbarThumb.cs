/**  --------------------------------------------------------  *
 *   ScrollbarThumb.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/06/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class ScrollbarThumb : Widget
	{
		private readonly Scrollbar mScrollbar;
		public override object Clone()
		{
			return new ScrollbarThumb(this);
		}

		public ScrollbarThumb(ScrollbarThumb copy)
		: base(copy.Name, copy.GuiSize, copy.Style)
		{
			mScrollbar = copy.mScrollbar;
		}

		public ScrollbarThumb(string name,
						IGuiStyle style,
						IGuiSize size,
						Scrollbar scrollbar)
						
		: base(name, size, style) 
		{
			if( style == null )
			{
				throw new ArgumentNullException("style");
			}
			if( scrollbar == null )
			{
				throw new ArgumentNullException("scrollbar");
			}
			mScrollbar = scrollbar;
		}

		private readonly static GUIContent mEmptyGuiContent = new GUIContent();
		public override void Draw(IGuiContainer container, Vector2 position)
		{
			Rect coords = new Rect(position.x, position.y, this.Size.x, this.Size.y);
			
			GUI.Box(coords, mEmptyGuiContent, Style.GenerateUnityGuiStyle());
		}

		public override IGuiStyle Style
		{
			get { return base.Style; }
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException("value");
				}
				base.Style = value;
			}
		}
	}	
}

