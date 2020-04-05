/**  --------------------------------------------------------  *
 *   Label.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/01/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using System.Collections;

namespace Hangout.Client.Gui
{
	public class Label : Widget, ITextGuiElement
	{
		
		public override object Clone()
		{
			return new Label(this);
		}
		
		private string mText = "";

		private bool mDropShadowEnabled = false;
		public bool DropShadowEnabled
		{
			get { return mDropShadowEnabled; }
			set { mDropShadowEnabled = value; }
		}

		public Label( Label copy )
		: base(copy.Name, copy.GuiSize, copy.Style)
		{
			mText = copy.Text;
			mDropShadowEnabled = copy.DropShadowEnabled;
		}
		
		public Label( string name,
					  IGuiSize size,
					  IGuiStyle style,
					  string text )
		: base(name, size, style) 
		{
			mText = text;
		}


		public Label(string name,
					  IGuiStyle style,
					  string text)
			: this(name, new ExpandText(), style, text)
		{
		}
		
		public string Text 
		{
			get { return mText; }
			set { mText = value; }
		}
		
		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			Rect coords = new Rect(position.x, position.y, this.ExternalSize.x, this.ExternalSize.y);
			if( this.Style != null )
			{

				if (mDropShadowEnabled)
				{
					Rect shadowCoords = new Rect(coords);
					shadowCoords.x += 1.0f;
					shadowCoords.y += 1.0f;
					Color shadowColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
					IGuiStyle shadowStyle = new GuiStyle(this.Style, "Shadow");
					shadowStyle.Normal.TextColor = shadowColor;
					shadowStyle.Hover.TextColor = shadowColor;
					shadowStyle.Active.TextColor = shadowColor;

					GUI.Label(shadowCoords, mText, shadowStyle.GenerateUnityGuiStyle());
				}
				GUI.Label( coords, mText, this.Style.GenerateUnityGuiStyle() );
			}
			else 
			{
				GUI.Label( coords, mText );
			}
		}
	}	
}

