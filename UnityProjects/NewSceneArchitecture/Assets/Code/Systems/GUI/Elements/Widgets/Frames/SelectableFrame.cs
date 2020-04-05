/**  --------------------------------------------------------  *
 *   SelectableFrame.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class SelectableFrame : GuiFrame
	{
		private IGuiStyle mUnselectedStyle;
		private IGuiStyle mSelectedStyle;

		private Hangout.Shared.Action mOnSelected = null;
		private Hangout.Shared.Action mOnUnselected = null;

		public SelectableFrame(string name,
								IGuiSize size,
								IEnumerable<KeyValuePair<IWidget, IGuiPosition>> widgets,
								IGuiStyle unselectedStyle,
								IGuiStyle selectedStyle)
			: base(name, size, widgets, unselectedStyle)
		{
			mSelectedStyle = selectedStyle;
			mUnselectedStyle = unselectedStyle;
		}

		public void AddOnSelectedAction(Hangout.Shared.Action onSelected)
		{
			mOnSelected += onSelected;
		}

		public void AddOnUnselectedAction(Hangout.Shared.Action onUnselected)
		{
			mOnUnselected += onUnselected;
		}

		public void ClearOnSelectedActions()
		{
			mOnSelected = null;
		}

		public void ClearOnUnselectedActions()
		{
			mOnUnselected = null;
		}

		private void ExecuteOnSelected()
		{
			this.Style = mSelectedStyle;

			if (mOnSelected != null)
			{
				mOnSelected();
			}
		}

		private void ExecuteOnUnselected()
		{
			this.Style = mUnselectedStyle;

			if (mOnUnselected != null)
			{
				mOnUnselected();
			}
		}
	}
}
