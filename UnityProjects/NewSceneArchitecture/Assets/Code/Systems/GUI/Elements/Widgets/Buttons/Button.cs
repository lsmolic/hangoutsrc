/**  --------------------------------------------------------  *
 *   Button.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class Button : Widget, ITextGuiElement
	{
		private Texture mImage;
		private string mText;
		private bool mEnabled = true;
		
		public override object Clone()
		{
			Button result = new Button
			(
				this.Name, 
				this.GuiSize, 
				this.Style,
				mImage,
				mText
			);
			result.Enabled = mEnabled;
			return result;
		}
		
		public Button( string name,
					   IGuiSize size,
					   IGuiStyle style, 
					   Texture image,
					   string text )
		: base( name, size, style ) 
		{
			mImage = image;
			mText = text;
		}
		
		public Button( string name,
					   IGuiSize size, 
					   IGuiStyle style, 
					   string text )
		: this( name, size, style, null, text ) { }
		
		public Button( string name,
					   IGuiSize size, 
					   IGuiStyle style, 
					   Texture image )
		: this( name, size, style, image, null ) { }
		
		public Button( string name,
					   IGuiSize size,
					   IGuiStyle style )
		: this( name, size, style, null, null ) { }
		
		/// Enabled is the default mode for the button
		/// Disabled is the 'greyed out' state for the button (button clicks don't register)
		public bool Enabled 
		{
			get { return mEnabled; }
			set { mEnabled = value; }
		}
		
		public void Disable() 
		{
			Enabled = false;
		}

		public void Enable()
		{
			Enabled = true;
		}
		
		public Texture Image 
		{
			get { return mImage; }
			set { mImage = value; }
		}

		public string Text 
		{
			get { return mText; }
			set { mText = value; }
		}

		private Hangout.Shared.Action mOnPressed;
		public void AddOnPressedAction( Hangout.Shared.Action actionOnPressed ) 
		{
			mOnPressed += actionOnPressed;
		}
		
		public void ClearOnPressedActions() 
		{
			mOnPressed = null;
		}

		protected GUIContent BuildButtonContent() 
		{ 
			GUIContent buttonContent;
			if ( mImage != null && mText != null ) 
			{
				buttonContent = new GUIContent( mText, mImage );
			} 
			else if ( mText != null ) 
			{
				buttonContent = new GUIContent( mText );
			} 
			else if ( mImage != null) 
			{
				buttonContent = new GUIContent( mImage );
			} 
			else 
			{
				buttonContent = new GUIContent();
			}

			return buttonContent;
		}

		protected virtual void OnPressed() 
		{
			if( mOnPressed != null ) 
			{
				mOnPressed();
			}
		}

		protected virtual IGuiStyle GetButtonStyle() 
		{
			return this.Style;
		}

		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			GUIContent buttonContent = BuildButtonContent();
			
			Vector2 size = this.Size;
			Rect coords = new Rect(position.x, position.y, size.x, size.y);
			bool buttonClicked = false;
			IGuiStyle style = this.GetButtonStyle();
			if( style != null ) 
			{
				GUIStyle style2 = style.GenerateUnityGuiStyle(mEnabled);
				buttonClicked = GUI.Button( coords, buttonContent, style2 );
			} 
			else 
			{
				buttonClicked = GUI.Button( coords, buttonContent );
			}
			
			if ( buttonClicked && mEnabled ) 
			{
				OnPressed();
			}
		}
	}
}
