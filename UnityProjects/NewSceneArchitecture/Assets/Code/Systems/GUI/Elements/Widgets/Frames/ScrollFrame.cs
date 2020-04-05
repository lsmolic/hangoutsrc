/**  --------------------------------------------------------  *
 *   ScrollFrame.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/02/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class ScrollFrame : GuiFrame
	{	
		private Vector2 mScrollOffset = Vector2.zero;
		private Scrollbar mScrollbar = null;
		private IReceipt mScrollCallbackReceipt = null;
		
		public Scrollbar Scrollbar
		{
			get { return mScrollbar; }
			set
			{
				mScrollbar = value;

				if( mScrollCallbackReceipt != null )
				{
					mScrollCallbackReceipt.Exit();
				}
				
				mScrollCallbackReceipt = mScrollbar.AddOnPercentChangedCallback(delegate(float scrollPosition)
				{
					float widgetOversize = RectUtility.BoundingRect(Vector2.zero, this.CalculateWidgetBounds()).height - InternalSize.y;
					if (widgetOversize > 0.0f)
					{
						mScrollOffset.y = -Mathf.Lerp(0.0f, widgetOversize, scrollPosition);
					}
				});

				IgnoreModifyPosition(mScrollbar);
			}
		}

		public void ResetScroll()
		{
			mScrollOffset = Vector2.zero;
		}

		public ScrollFrame(string name,
							IGuiSize size,
							IEnumerable<KeyValuePair<IWidget, IGuiPosition>> widgets,
							IGuiStyle frameStyle)
			: base(name, size, widgets, frameStyle)
		{
			AddModifyPositionCallback(delegate(Vector2 widgetPosition)
			{
				return widgetPosition + mScrollOffset;
			});
		}
	}
}
