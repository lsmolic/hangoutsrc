/**  --------------------------------------------------------  *
 *   GuiMargin.cs  
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
	public struct GuiMargin
	{
		private float mTop;
		private float mBottom;
		private float mLeft;
		private float mRight;

		public float Top
		{
			get { return mTop; }
			set { mTop = value; }
		}
		public float Bottom
		{
			get { return mBottom; }
			set { mBottom = value; }
		}
		public float Left
		{
			get { return mLeft; }
			set { mLeft = value; }
		}
		public float Right
		{
			get { return mRight; }
			set { mRight = value; }
		}

		public GuiMargin(float top, float bottom, float left, float right)
		{
			mTop = top;
			mBottom = bottom;
			mLeft = left;
			mRight = right;
		}

		public override string ToString()
		{
			return "Top: " + mTop.ToString("f2") +
				   " Bottom: " + mBottom.ToString("f2") +
				   " Left: " + mLeft.ToString("f2") +
				   " Right: " + mRight.ToString("f2");
		}

		public Vector2 GetSizeDifference()
		{
			return new Vector2(mLeft + mRight, mTop + mBottom);
		}

		public UnityEngine.RectOffset ToRectOffset()
		{
			RectOffset result = new RectOffset();
			result.left = (int)this.Left;
			result.right = (int)this.Right;
			result.top = (int)this.Top;
			result.bottom = (int)this.Bottom;

			return result;
		}

		public static GuiMargin operator *(GuiMargin margin, float scalar)
		{
			margin.Top *= scalar;
			margin.Bottom *= scalar;
			margin.Left *= scalar;
			margin.Right *= scalar;
			return margin;
		}
	}
}