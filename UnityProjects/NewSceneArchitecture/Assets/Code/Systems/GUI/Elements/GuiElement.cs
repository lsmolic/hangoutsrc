/**  --------------------------------------------------------  *
 *   GuiElement.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public abstract class GuiElement : IGuiElement
	{
		private List<Action<bool>> mOnShowing = new List<Action<bool>>();

		private IGuiElement mParent; /// Used for calculating relative positions, null in ITopLevels
		public virtual IGuiElement Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}

		private IGuiSize mSize;
		public Vector2 Size
		{
			get { return mSize.GetSize(this); }
		}

		private IGuiStyle mStyle;
		public virtual IGuiStyle Style
		{
			get { return mStyle; }
			set { mStyle = value; }
		}

		public IGuiSize GuiSize
		{
			get { return mSize; }
			set { mSize = value; }
		}

		private bool mShowing = true;
		public bool Showing
		{
			get { return mShowing; }
			set { SetShowing(value); }
		}

		private void SetShowing(bool showing)
		{
			mShowing = showing;
			foreach (Action<bool> showingAction in mOnShowing)
			{
				showingAction(showing);
			}
		}

		private string mName;
		public string Name
		{
			get { return mName; }
		}

		public IReceipt OnShowing(Action<bool> callback)
		{
			mOnShowing.Add(callback);
			return new Receipt(delegate() { mOnShowing.Remove(callback); });
		}

		public virtual Vector2 ExternalSize
		{
			get
			{
				Vector2 result = mSize.GetSize(this);
				if (mStyle != null)
				{
					result += mStyle.ExternalMargins.GetSizeDifference();
				}
				return result;
			}
		}

		public virtual Vector2 InternalSize
		{
			get
			{
				Vector2 result = mSize.GetSize(this);
				if (mStyle != null)
				{
					result -= mStyle.InternalMargins.GetSizeDifference();
				}
				return result;
			}
		}

		public GuiElement(string name, IGuiSize size, IGuiStyle style)
		{
			if (size == null)
			{
				throw new ArgumentNullException("size");
			}

			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			mStyle = style;
			mName = name;
			mSize = size;
		}


		public virtual ITopLevel GetTopLevel()
		{
			IGuiElement i = this;
			while (!(i is ITopLevel))
			{
				i = i.Parent;
				if (i == null)
				{
					throw new Exception("The only IGuiElements that don't have a parent should be ITopLevels.");
				}
			}

			return (ITopLevel)i;
		}

		public Vector2 GetScreenPosition()
		{
			TopLevel topLevel;
			Vector2 position = Vector2.zero;
			if (this is TopLevel)
			{
				topLevel = (TopLevel)this;
			}
			else
			{
				IGuiElement curChild = this;
				IGuiElement curParent = curChild.Parent;
				position += ((IGuiContainer)curParent).GetChildPosition(curChild);
				while (!(curParent is ITopLevel))
				{
					curChild = curParent;
					curParent = curChild.Parent;
					if (curParent == null)
					{
						throw new System.Exception("The parent structure for this GUI Element (" + this.Name + ") is corrupted, Widgets should always be in a TopLevel.");
					}
					position += ((IGuiContainer)curParent).GetChildPosition(curChild);
				}

				topLevel = (TopLevel)curParent;
			}

			position += topLevel.Manager.GetTopLevelPosition(topLevel).GetPosition(topLevel);
			return position;
		}

		/// <summary>
		/// Returns a point in widget space from screen space.
		/// </summary>
		/// <param name="screenCoords"></param>
		/// <returns></returns>
		public Vector2 GetWidgetPosition(Vector2 screenCoords)
		{
			TopLevel topLevel;
			screenCoords.y = Screen.height - screenCoords.y;
			if( this is TopLevel )
			{
				topLevel = (TopLevel)this;
			}
			else
			{
				IGuiElement curChild = this;
				IGuiElement curParent = curChild.Parent;
				
				while (!(curParent is ITopLevel))
				{
					if (curParent == null)
					{
						throw new System.Exception("The parent structure for this GUI Element (" + this.Name + ") is corrupted, Widgets should always be in a TopLevel.");
					}

					screenCoords -= ((IGuiContainer)curParent).GetChildPosition(curChild);
					
					curChild = curParent;
					curParent = curChild.Parent;
				}
				screenCoords -= ((IGuiContainer)curParent).GetChildPosition(curChild);
				topLevel = (TopLevel)curParent;
			}
			screenCoords -= topLevel.Manager.GetTopLevelPosition(topLevel).GetPosition(topLevel);
			return screenCoords;
		}

		public abstract object Clone();

		public override string ToString()
		{
			return this.Name + " (" + GetType().Name + ")";
		}
	}
}