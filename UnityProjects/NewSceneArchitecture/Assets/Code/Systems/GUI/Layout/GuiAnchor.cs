/**  --------------------------------------------------------  *
 *   GuiAnchor.cs  
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
	public class GuiAnchor
	{
		// Shortcuts
		public static GuiAnchor TopLeft = new GuiAnchor(X.Left, Y.Top);
		public static GuiAnchor TopCenter = new GuiAnchor(X.Center, Y.Top);
		public static GuiAnchor TopRight = new GuiAnchor(X.Right, Y.Top);
		public static GuiAnchor CenterLeft = new GuiAnchor(X.Left, Y.Center);
		public static GuiAnchor CenterCenter = new GuiAnchor(X.Center, Y.Center);
		public static GuiAnchor CenterRight = new GuiAnchor(X.Right, Y.Center);
		public static GuiAnchor BottomLeft = new GuiAnchor(X.Left, Y.Bottom);
		public static GuiAnchor BottomCenter = new GuiAnchor(X.Center, Y.Bottom);
		public static GuiAnchor BottomRight = new GuiAnchor(X.Right, Y.Bottom);

		public enum X { Left, Center, Right }
		public enum Y { Top, Center, Bottom }

		private X mHorizontal;
		private Y mVertical;

		public GuiAnchor(X horizontal, Y vertical)
		{
			mHorizontal = horizontal;
			mVertical = vertical;
		}

		public GuiAnchor(GuiAnchor copy)
		{
			mHorizontal = copy.Horizontal;
			mVertical = copy.Vertical;
		}

		public X Horizontal
		{
			get { return mHorizontal; }
			set { mHorizontal = value; }
		}

		public Y Vertical
		{
			get { return mVertical; }
			set { mVertical = value; }
		}

		public Vector2 OffsetFromTopLeft(Vector2 size)
		{
			Vector2 result = Vector2.zero;

			if (Horizontal == GuiAnchor.X.Center)
			{
				result.x += size.x * 0.5f;
			}
			else if (Horizontal == GuiAnchor.X.Right)
			{
				result.x += size.x;
			}

			if (Vertical == GuiAnchor.Y.Center)
			{
				result.y += size.y * 0.5f;
			}
			else if (Vertical == GuiAnchor.Y.Bottom)
			{
				result.y += size.y;
			}

			return result;
		}

		public UnityEngine.TextAnchor ToUnityTextAnchor()
		{
			TextAnchor result = TextAnchor.LowerLeft;
			switch (mHorizontal)
			{
				case X.Left:
					switch (mVertical)
					{
						case Y.Top:
							result = TextAnchor.UpperLeft;
							break;
						case Y.Center:
							result = TextAnchor.MiddleLeft;
							break;
						case Y.Bottom:
							result = TextAnchor.LowerLeft;
							break;
					}
					break;
				case X.Center:
					switch (mVertical)
					{
						case Y.Top:
							result = TextAnchor.UpperCenter;
							break;
						case Y.Center:
							result = TextAnchor.MiddleCenter;
							break;
						case Y.Bottom:
							result = TextAnchor.LowerCenter;
							break;
					}
					break;
				case X.Right:
					switch (mVertical)
					{
						case Y.Top:
							result = TextAnchor.UpperRight;
							break;
						case Y.Center:
							result = TextAnchor.MiddleRight;
							break;
						case Y.Bottom:
							result = TextAnchor.LowerRight;
							break;
					}
					break;
			}

			return result;
		}

		public override string ToString()
		{
			return "GuiAnchor(" + Horizontal + " " + Vertical + ")";
		}
	}
}
