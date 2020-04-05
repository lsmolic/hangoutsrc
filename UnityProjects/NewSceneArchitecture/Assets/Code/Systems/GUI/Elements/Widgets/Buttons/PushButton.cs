/**  --------------------------------------------------------  *
 *   PushButton.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui	
{
	public class PushButton : Button
	{
		public override object Clone()
		{
			return new PushButton(this);
		}

		public PushButton( string name,
					   IGuiSize size,
					   IGuiStyle style, 
					   Texture image,
					   string text )
		: base( name, size, style, image, text )
		{
		}
		
		public PushButton( string name,
					   IGuiSize size, 
					   IGuiStyle style, 
					   string text )
		: this( name, size, style, null, text ) { }
		
		public PushButton( string name,
					   IGuiSize size, 
					   IGuiStyle style, 
					   Texture image )
		: this( name, size, style, image, null ) { }

		public PushButton(string name,
					   IGuiSize size,
					   IGuiStyle style )
		: this( name, size, style, null, null ) { }

		public PushButton(PushButton copy)
			: this(copy.Name, copy.GuiSize, copy.Style, copy.Image, copy.Text)
		{
			Enabled = copy.Enabled;
		}

		private readonly List<Action<Vector2>> mMouseDragCallbacks = new List<Action<Vector2>>();
		public IReceipt AddMouseDragCallback(Action<Vector2> onMouseDrag)
		{
			if (onMouseDrag == null)
			{
				throw new ArgumentNullException("onMouseDrag");
			}

			mMouseDragCallbacks.Add(onMouseDrag);
			return new Receipt(delegate()
			{
				mMouseDragCallbacks.Remove(onMouseDrag);
			});
		}

		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			if( Event.current.type == EventType.mouseDrag )
			{
				foreach (Action<Vector2> callback in mMouseDragCallbacks)
				{
					callback(Event.current.mousePosition);
				}
			}

			GUIContent buttonContent = BuildButtonContent();
			
			Vector2 size = this.Size;
			Rect coords = new Rect(position.x, position.y, size.x, size.y);
			bool buttonClicked = false;
			IGuiStyle style = this.GetButtonStyle();
			if( style != null ) 
			{
				GUIStyle style2 = style.GenerateUnityGuiStyle(this.Enabled);
				buttonClicked = GUI.RepeatButton( coords, buttonContent, style2 );
			} 
			else 
			{
				buttonClicked = GUI.RepeatButton(coords, buttonContent);
			}
			
			if ( buttonClicked && this.Enabled ) 
			{
				OnPressed();
			}
		}
	}
}
