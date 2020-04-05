/**  --------------------------------------------------------  *
 *   CenteredAutoLayout.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/8/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class CenteredAutoLayout : IAutoLayout
	{
		private float mHeight;

		public CenteredAutoLayout()
			: this(0.0f)
		{
		}

		public CenteredAutoLayout(float height)
		{
			mHeight = height;
		}

		public void UpdatePosition(IGuiElement element, Vector2 newPosition)
		{
			// Ignores X
			mHeight = newPosition.y;
		}

		public Vector2 GetPosition(IGuiElement element)
		{
			Vector2? position = null;
			Rect widgetCoords;
			if (element is IWidget && mWidgetCache.TryGetValue((IWidget)element, out widgetCoords))
			{
				position = new Vector2(widgetCoords.x, widgetCoords.y);
			}

			if (position == null)
			{
				throw new Exception("Element(" + element.Name + ")'s position has not been calculated in CenteredAutoLayout.NextPosition");
			}
			return (Vector2)position;
		}

		private IList<KeyValuePair<IWidget, IAutoLayout>> mCachedWidgetCollection = null; // Source collection that mWidgetCache was generated from
		private readonly Dictionary<IWidget, Rect> mWidgetCache = new Dictionary<IWidget, Rect>();

		public Rect NextPosition(IWidget nextWidget, IEnumerable<Rect> reservedSpaces, Vector2 parentSize, IList<KeyValuePair<IWidget, IAutoLayout>> autolayoutWidgets)
		{
			if (mCachedWidgetCollection != autolayoutWidgets)
			{
				mWidgetCache.Clear();

				// Calculate the left extent
				float centerX = parentSize.x * 0.5f;
				float widgetWidth = 0.0f;
				foreach (KeyValuePair<IWidget, IAutoLayout> widget in autolayoutWidgets)
				{
					widgetWidth += widget.Key.ExternalSize.x;
				}
				float leftX = centerX - (widgetWidth * 0.5f);

				// Layout in relation to the left extent
				float nextPosition = leftX;
				foreach (KeyValuePair<IWidget, IAutoLayout> widget in autolayoutWidgets)
				{
					Vector2 widgetSize = widget.Key.ExternalSize;
					Rect widgetCoords = new Rect(nextPosition, mHeight, widgetSize.x, widgetSize.y);
					nextPosition += widgetSize.x;

					mWidgetCache.Add(widget.Key, widgetCoords);
				}

				mCachedWidgetCollection = autolayoutWidgets;
			}

			return mWidgetCache[nextWidget];
		}
	}
}
