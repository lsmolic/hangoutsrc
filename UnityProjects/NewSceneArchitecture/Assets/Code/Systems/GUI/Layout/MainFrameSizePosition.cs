/**  --------------------------------------------------------  *
 *   MainFrameSizePosition.cs  
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
	public class MainFrameSizePosition : IGuiSize, IGuiPosition
	{
		public void UpdatePosition(IGuiElement element, Vector2 position)
		{
			throw new Exception("Nothing should update the position of a top level's main frame.");
		}

		public Vector2 GetPosition(IGuiElement element)
		{
			if (!(element is IGuiFrame && element.Parent is ITopLevel))
			{
				throw new ArgumentException("MainFrameSizePosition only works with IGuiFrames that are children of ITopLevels.");
			}

			Vector2 result = Vector2.zero;

			ITopLevel parent = (ITopLevel)element.Parent;
			if (parent.Style != null)
			{
				result.x = parent.Style.InternalMargins.Left;
				result.y = parent.Style.InternalMargins.Top;
			}

			if (parent is Window)
			{
				Window parentWindow = (Window)parent;
				if (parentWindow.HeaderFrame.Key != null)
				{
					result.y += parentWindow.HeaderFrame.Key.ExternalSize.y;
					if (parent.Style != null)
					{
						result.y += parent.Style.InternalMargins.Top + parent.Style.InternalMargins.Bottom;
					}
				}
			}

			return result;
		}

		public Vector2 GetSize(IGuiElement element)
		{
			if (element.Parent == null)
			{
				throw new ArgumentException("MainFrameSizePosition does not work with null parents.");
			}
			if (!(element is IGuiFrame))
			{
				throw new ArgumentException("MainFrameSizePosition only works with IGuiFrames.");
			}
			if (!(element.Parent is ITopLevel))
			{
				throw new ArgumentException("MainFrameSizePosition only works with ITopLevels. (" + element.Parent.Name + ")");
			}

			ITopLevel parent = (ITopLevel)element.Parent;
			Vector2 result = parent.InternalSize;

			if (parent is Window)
			{
				Window parentWindow = (Window)parent;
				if (parentWindow.HeaderFrame.Key != null)
				{
					result.y -= parentWindow.HeaderFrame.Key.ExternalSize.y;
					if (parent.Style != null)
					{
						result.y -= parent.Style.InternalMargins.Top + parent.Style.InternalMargins.Bottom;
					}
				}
			}

			return result;
		}
	}
}

