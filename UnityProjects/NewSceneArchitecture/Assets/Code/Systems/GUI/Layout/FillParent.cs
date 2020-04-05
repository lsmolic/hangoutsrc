/**  --------------------------------------------------------  *
 *   FillParent.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client.Gui 
{
	
	/// Use this layout if you want a child widget to expand to fill all the parent's available space
	/// Only works for widgets drawn at (0, 0)
	public class FillParent : IGuiSize, IGuiPosition 
	{
		private Layout mMode;
		private Vector2 mFixedSize;     // Used for the axes not controlled by 'Layout'
		private Vector2 mFixedPosition; // Used for the axes not controlled by 'Layout'
		
		public override string ToString() 
		{
			string result = "FillParent ";
			if( mMode == Layout.BothAxes ) 
			{
				result += "Both Axes";
			} 
			else if( mMode == Layout.Horizontal )
			{
				result += "(Fill, " + mFixedSize.x + ")";
			} 
			else if( mMode == Layout.Vertical )
			{
				result += "(" + mFixedSize.x + ", Fill)";
			}
			return result;
		}
		
		public FillParent(Layout mode, Vector2 fixedPosition, Vector2 fixedSize) 
		{
			mMode = mode;
			mFixedPosition = fixedPosition;
			mFixedSize = fixedSize;
		}
		
		public FillParent(Layout mode, Vector2 fixedSize) 
		: this(mode, Vector2.zero, fixedSize) { }
		
		public FillParent()
		: this(Layout.BothAxes, Vector2.zero, Vector2.zero) { } 
		
		/// Keep the Widget at the origin along the axes required by 'Layout'
		public Vector2 GetPosition( IGuiElement element ) 
		{
			Vector2 result = mFixedPosition;
			if( mMode == Layout.Horizontal || mMode == Layout.BothAxes ) 
			{
				result.x = 0.0f;
			}
			if( mMode == Layout.Vertical || mMode == Layout.BothAxes ) 
			{
				result.y = 0.0f;
			}
			
			return result; 
		}
		
		public void UpdatePosition( IGuiElement element, Vector2 position ) 
		{
			Console.LogError("Cannot update the position of a widget (" + element.Name + ") that's filling it's parent (" + element.Parent.Name + ").");
		}
		
		/// Make the Widget the same size as the parent along the axes required by 'Layout'
		public Vector2 GetSize( IGuiElement element )
		{ 
			if( element == null ) 
			{
				throw new ArgumentNullException("element");
			}
			
			Vector2 result = mFixedSize;
			if( mMode == Layout.Horizontal || mMode == Layout.BothAxes ) 
			{
				if( element.Parent == null ) 
				{
					result.x = Screen.width;
				} 
				else 
				{
					result.x = element.Parent.InternalSize.x;
				}
				
				if( element.Style != null ) 
				{
					result.x -= element.Style.ExternalMargins.GetSizeDifference().x;
				}
			}
			
			if( mMode == Layout.Vertical || mMode == Layout.BothAxes ) 
			{
				if( element.Parent == null ) 
				{
					result.y = Screen.height;
				} 
				else 
				{
					result.y = element.Parent.InternalSize.y;
				}
				
				if( element.Style != null ) 
				{
					result.y -= element.Style.ExternalMargins.GetSizeDifference().y;
				}
			}
			
			return result;
		}
	}
}
