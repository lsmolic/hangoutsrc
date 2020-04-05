/**  --------------------------------------------------------  *
 *   ToggleButton.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/22/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;


using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class ToggleButton : Button
	{
		private bool mActivated = false;

		public bool Activated
		{
			get { return mActivated; }

			set
			{
				if (mActivated != value)
				{
					Toggle();
				}
			}
		}

		private void Toggle()
		{
			mActivated = !mActivated; // Toggle

			// Callbacks for the different toggle state
			if (mActivated && mOnActivated != null)
			{
				mOnActivated();
			}
			else if (!mActivated && mOnDeactivated != null)
			{
				mOnDeactivated();
			}
		}

		private IGuiStyle mDeactivatedStyle;

		public ToggleButton(string name,
							 IGuiSize size,
							 IGuiStyle activatedStyle,
							 IGuiStyle deactivatedStyle,
							 Texture image)
			: base(name, size, activatedStyle, image)
		{
			if (activatedStyle == null)
			{
				throw new ArgumentNullException("activatedStyle");
			}

			if (deactivatedStyle == null)
			{
				throw new ArgumentNullException("deactivatedStyle");
			}

			mDeactivatedStyle = deactivatedStyle;
			this.AddOnPressedAction(this.Toggle);
		}

		private Hangout.Shared.Action mOnActivated;
		private Hangout.Shared.Action mOnDeactivated;

		public void AddOnActivatedAction(Hangout.Shared.Action actionOnActivated)
		{
			mOnActivated += actionOnActivated;
		}

		public void AddOnDeactivatedAction(Hangout.Shared.Action actionOnDeactivated)
		{
			mOnDeactivated += actionOnDeactivated;
		}

		public void ClearOnActivatedActions()
		{
			mOnActivated = null;
		}

		public void ClearOnDeactivatedActions()
		{
			mOnDeactivated = null;
		}

		protected override IGuiStyle GetButtonStyle()
		{
			IGuiStyle result;

			if (mActivated)
			{
				result = base.GetButtonStyle();
			}
			else
			{
				result = mDeactivatedStyle;
			}

			return result;
		}
	}
}
