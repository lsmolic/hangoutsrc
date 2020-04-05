/**  --------------------------------------------------------  *
 *   GuiStyle.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class GuiStyle : IGuiStyle
	{
		private string mName;
		private StyleState mNormal = new StyleState();
		private StyleState mHover = new StyleState();
		private StyleState mActive = new StyleState();
		private StyleState mDisabled = new StyleState();
		private GuiMargin mInternalMargins = new GuiMargin();
		private GuiMargin mExternalMargins = new GuiMargin();
		private GuiMargin mNinePartScale = new GuiMargin();
		private UnityEngine.Font mFont = null;
		private GuiAnchor mTextAnchor = new GuiAnchor(GuiAnchor.X.Left, GuiAnchor.Y.Bottom);
		private bool mWordWrap = false;
		private bool mClipText = true;

		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		public StyleState Normal
		{
			get { return mNormal; }
			set 
			{ 
				mDirty = true;
				mNormal = value; 
			}
		}
		public StyleState Hover
		{
			get { return mHover; }
			set
			{
				mDirty = true; 
				mHover = value;
			}
		}
		public StyleState Active
		{
			get { return mActive; }
			set
			{
				mDirty = true; 
				mActive = value;
			}
		}
		public StyleState Disabled
		{
			get { return mDisabled; }
			set
			{
				mDirty = true; 
				mDisabled = value;
			}
		}
		public GuiMargin InternalMargins
		{
			get { return mInternalMargins; }
			set
			{
				mDirty = true; 
				mInternalMargins = value;
			}
		}
		public GuiMargin ExternalMargins
		{
			get { return mExternalMargins; }
			set
			{
				mDirty = true; 
				mExternalMargins = value;
			}
		}
		public GuiMargin NinePartScale
		{
			get { return mNinePartScale; }
			set
			{
				mDirty = true; 
				mNinePartScale = value;
			}
		}
		public Font DefaultFont
		{
			get { return mFont; }
			set
			{
				mDirty = true; 
				mFont = value;
			}
		}
		public GuiAnchor DefaultTextAnchor
		{
			get { return mTextAnchor; }
			set
			{
				mDirty = true; 
				mTextAnchor = value;
			}
		}
		public bool WordWrap
		{
			get { return mWordWrap; }
			set
			{
				mDirty = true; 
				mWordWrap = value;
			}
		}
		public bool ClipText
		{
			get { return mClipText; }
			set
			{
				mDirty = true; 
				mClipText = value;
			}
		}

		public GuiStyle(string name)
		{
			mName = name;
		}
		public GuiStyle(IGuiStyle copy, string name)
			: this(name)
		{
			if (copy == null)
			{
				throw new ArgumentNullException("copy");
			}

			this.Normal = new StyleState(copy.Normal);
			this.Hover = new StyleState(copy.Hover);
			this.Active = new StyleState(copy.Active);
			this.Disabled = new StyleState(copy.Disabled);
			this.InternalMargins = copy.InternalMargins;
			this.ExternalMargins = copy.ExternalMargins;
			this.NinePartScale = copy.NinePartScale;
			this.DefaultFont = copy.DefaultFont;
			this.DefaultTextAnchor = new GuiAnchor(copy.DefaultTextAnchor);
			this.WordWrap = copy.WordWrap;
			this.ClipText = copy.ClipText;
		}

		private List<System.Type> mDefaultFor = new List<System.Type>();
		public IEnumerable<System.Type> DefaultFor()
		{
			return mDefaultFor;
		}

		public void SetAsDefaultFor(System.Type t)
		{
			if (!mDefaultFor.Contains(t))
			{
				mDefaultFor.Add(t);
			}
		}

		public override string ToString()
		{
			return String.Format("GuiStyle({0})", this.Name);
		}

		private bool mDirty = true;
		private bool mCacheEnabled = false;
		private GUIStyle mCache = null;

		public GUIStyle GenerateUnityGuiStyle()
		{
			return GenerateUnityGuiStyle(true);
		}

		public GUIStyle GenerateUnityGuiStyle(bool enabled)
		{
			if (mDirty || enabled != mCacheEnabled)
			{
				mCache = new GUIStyle();

				if (enabled)
				{
					mCache.normal = this.Normal.ToUnityEngineGuiStyleState();
					mCache.active = this.Active.ToUnityEngineGuiStyleState();
					mCache.hover = this.Hover.ToUnityEngineGuiStyleState();
				}
				else
				{
					mCache.normal = this.Disabled.ToUnityEngineGuiStyleState();
					mCache.active = this.Disabled.ToUnityEngineGuiStyleState();
					mCache.hover = this.Disabled.ToUnityEngineGuiStyleState();
				}

				mCache.font = this.DefaultFont;
				mCache.padding = this.InternalMargins.ToRectOffset();
				mCache.margin = this.ExternalMargins.ToRectOffset();
				mCache.border = this.NinePartScale.ToRectOffset();
				mCache.alignment = this.DefaultTextAnchor.ToUnityTextAnchor();
				mCache.wordWrap = this.WordWrap;
				mCache.clipping = this.ClipText ? TextClipping.Clip : TextClipping.Overflow;

				mCacheEnabled = enabled;
				mDirty = false;
			}

			return mCache;
		}
	}
}
