/**  --------------------------------------------------------  *
 *   FixedSize.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/19/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client.Gui
{
	public class FixedSize : IGuiSize
	{
		private Vector2 mSize;
		
		public Vector2 Size
		{
			get { return mSize; }
			set { mSize = value; }
		}

		public FixedSize(Vector2 size)
		{
			mSize = size;
		}

		public FixedSize(float x, float y) : this(new Vector2(x, y)) { }

		public Vector2 GetSize(IGuiElement widget)
		{
			return mSize;
		}
	}
}
