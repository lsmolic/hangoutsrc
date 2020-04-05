/**  --------------------------------------------------------  *
 *   FixedPosition.cs  
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
	public class FixedPosition : IGuiPosition
	{
		private GuiAnchor mLocalAnchor;
		private GuiAnchor mParentAnchor;
		private bool mLockPosition = false;
		private Vector2 mFixedPosition;

		/// Can anything move elements that use this position?
		public bool LockPosition
		{
			get { return mLockPosition; }
			set { mLockPosition = value; }
		}

		public GuiAnchor LocalAnchor
		{
			get { return mLocalAnchor; }
			set { mLocalAnchor = value; }
		}
		public GuiAnchor ParentAnchor
		{
			get { return mParentAnchor; }
			set { mParentAnchor = value; }
		}

		/// Get the position value, unfiltered by the anchor
		public Vector2 RelativePosition
		{
			get { return mFixedPosition; }
		}

		public FixedPosition(Vector2 fixedPosition, GuiAnchor localAnchor, GuiAnchor parentAnchor)
		{
			if (localAnchor == null)
			{
				throw new ArgumentNullException("localAnchor");
			}
			if (parentAnchor == null)
			{
				throw new ArgumentNullException("parentAnchor");
			}

			mFixedPosition = fixedPosition;
			mLocalAnchor = localAnchor;
			mParentAnchor = parentAnchor;
		}

		public FixedPosition(float x, float y, GuiAnchor localAnchor, GuiAnchor parentAnchor)
			: this(new Vector2(x, y), localAnchor, parentAnchor)
		{
		}

		public FixedPosition(Vector2 fixedPosition, GuiAnchor anchor)
			: this(fixedPosition, anchor, anchor)
		{
		}

		public FixedPosition(float x, float y, GuiAnchor anchor)
			: this(new Vector2(x, y), anchor)
		{
		}

		public FixedPosition(Vector2 fixedPosition)
			: this(fixedPosition, GuiAnchor.TopLeft, GuiAnchor.TopLeft)
		{
		}

		public FixedPosition(float x, float y)
			: this(new Vector2(x, y))
		{
		}

		public Vector2 GetPosition(IGuiElement element)
		{
			Vector2 topLeftPosition = mFixedPosition;

			// Translate for Local Anchor
			topLeftPosition -= mLocalAnchor.OffsetFromTopLeft(element.ExternalSize);

			// Translate for Parent Anchor
			Vector2 parentSize = Vector2.zero;
			if (element.Parent != null)
			{
				// Parent Space
				parentSize = element.Parent.Size;
			}
			else
			{
				// Screen Space
				parentSize = new Vector2(Screen.width, Screen.height);
			}

			topLeftPosition += mParentAnchor.OffsetFromTopLeft(parentSize);

			return topLeftPosition;
		}

		public void UpdatePosition(IGuiElement element, Vector2 position)
		{
			if (!this.LockPosition)
			{
				mFixedPosition = position;
			}
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.Append("FixedPosition at ");
			sb.Append(mFixedPosition);
			sb.Append(", localAnchor(");
			if (mLocalAnchor == null)
			{
				sb.Append("null");
			}
			else
			{
				sb.Append(mLocalAnchor);
			}
			sb.Append(") parentAnchor(");
			if (mParentAnchor == null)
			{
				sb.Append("null");
			}
			else
			{
				sb.Append(mParentAnchor);
			}
			sb.Append(")");

			return sb.ToString();
		}
	}
}
