/**  --------------------------------------------------------  *
 *   GuiRange.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/06/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public abstract class GuiRange : Widget
	{
		private Rangef mBounds;
		private float mPercent;
		private readonly List<Action<float>> mPercentChangedCallbacks = new List<Action<float>>();

		public float Percent
		{
			get { return mPercent; }
			set 
			{
				float t = mBounds.ParametricValue(value);
				if( t > 1.0f || t < 0.0f )
				{
					throw new ArgumentOutOfRangeException("value");
				}
				mPercent = value;

				foreach (Action<float> percentChangedCallback in new List<Action<float>>(mPercentChangedCallbacks))
				{
					percentChangedCallback(mPercent);
				}
			}
		}

		/// <summary>
		/// Adds a callback for when the percent changes (ex. a scrollbar changes it's scroll amount).
		/// </summary>
		public IReceipt AddOnPercentChangedCallback(Action<float> onPercentChanged)
		{
			mPercentChangedCallbacks.Add(onPercentChanged);
			return new Receipt(delegate()
			{
				mPercentChangedCallbacks.Remove(onPercentChanged);
			});
		}
		
		public GuiRange( GuiRange copy )
		: base(copy.Name, copy.GuiSize, copy.Style)
		{
			mBounds = copy.mBounds;
			mPercent = copy.mPercent;
		}

		public GuiRange(string name,
						IGuiStyle style,
						IGuiSize size,
						Rangef bounds,
						float position)
		: base(name, size, style) 
		{
			if(bounds == null)
			{
				throw new ArgumentNullException("position");
			}
			mBounds = bounds;
			Percent = position;
		}

		public GuiRange(string name,
						IGuiStyle style,
						IGuiSize size)
			: base(name, size, style)
		{
			mBounds = new Rangef(0.0f, 1.0f);
			Percent = 0.0f;
		}

		/// <summary>
		/// Sends the percentage to all callbacks again
		/// </summary>
		public void Refresh()
		{
			foreach (Action<float> percentChangedCallback in new List<Action<float>>(mPercentChangedCallbacks))
			{
				percentChangedCallback(mPercent);
			}
		}
	}	
}

