/**  --------------------------------------------------------  *
 *   HeaderFrameSizePosition.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/16/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class HeaderFrameSizePosition : IGuiSize, IGuiPosition
	{
		private const float mDefaultHeaderHeight = 24.0f;
		private float mHeaderHeight;
		public float HeaderHeight
		{
			get { return mHeaderHeight; }
			set { mHeaderHeight = value; }
		}

		public HeaderFrameSizePosition()
			: this(mDefaultHeaderHeight)
		{
		}
		public HeaderFrameSizePosition(float headerHeight)
		{
			mHeaderHeight = headerHeight;
		}

		public void UpdatePosition(IGuiElement element, Vector2 position)
		{
			throw new Exception("Nothing should update the position of a Window's header frame.");
		}

		public Vector2 GetPosition(IGuiElement element)
		{
			if (!(element is IGuiFrame && element.Parent is ITopLevel))
			{
				throw new ArgumentException("HeaderFrameSizePosition only works with IGuiFrames that are children of ITopLevels.");
			}

			Vector2 result = Vector2.zero;
			ITopLevel parent = (ITopLevel)element.Parent;
			if (parent.Style != null)
			{
				result.x = parent.Style.InternalMargins.Left;
				result.y = parent.Style.InternalMargins.Top;
			}

			return result;
		}

		public Vector2 GetSize(IGuiElement element)
		{
			if (!(element is IGuiFrame && element.Parent is ITopLevel))
			{
				throw new ArgumentException("HeaderFrameSizePosition only works with IGuiFrames that are children of ITopLevels.");
			}

			ITopLevel parent = (ITopLevel)element.Parent;
			Vector2 result = new Vector2(parent.InternalSize.x, mHeaderHeight);

			return result;
		}
	}
}

