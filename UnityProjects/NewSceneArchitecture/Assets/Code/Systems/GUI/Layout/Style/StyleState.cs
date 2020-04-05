/**  --------------------------------------------------------  *
 *   StyleState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/05/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class StyleState
	{
		private Texture2D mBackground;
		private Color mTextColor;
		
		private bool mIsDirty = true;
		private UnityEngine.GUIStyleState mCachedUnityStyleState = null;

		public Color TextColor
		{
			get { return mTextColor; }
			set 
			{ 
				mIsDirty = true;
				mTextColor = value; 
			}
		}
		public Texture2D Background
		{
			get { return mBackground; }
			set 
			{ 
				mIsDirty = true;
				mBackground = value; 
			}
		}

		public override string ToString()
		{
			return "StyleState(Background: " + (mBackground == null ? "null" : mBackground.name) + ", TextColor: " + ColorUtility.ColorToHex(mTextColor) + ")";
		}

		public StyleState()
			: this(null, Color.black)
		{
		}

		public StyleState(Texture2D background, Color textColor)
		{
			mBackground = background;
			mTextColor = textColor;
			
			// Generate the cache at construction
			ToUnityEngineGuiStyleState();
		}

		public StyleState(StyleState copy)
		: this(copy.Background, copy.TextColor)
		{
		}

		public UnityEngine.GUIStyleState ToUnityEngineGuiStyleState()
		{
			if( mIsDirty )
			{
				mCachedUnityStyleState = new UnityEngine.GUIStyleState();
				
				mCachedUnityStyleState.background = this.Background;
				mCachedUnityStyleState.textColor = this.TextColor;
				
				mIsDirty = false;
			}

			return mCachedUnityStyleState;
		}
	}
}
