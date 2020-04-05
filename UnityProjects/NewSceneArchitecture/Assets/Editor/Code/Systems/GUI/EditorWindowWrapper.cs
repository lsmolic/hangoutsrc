/**  --------------------------------------------------------  *
 *   EditorWindowWrapper.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using UnityEditor;

using System;

using Hangout.Client.Gui;

public class EditorWindowWrapper : EditorWindow
{
	private ITopLevel mInnerTopLevel = null;
	private WeakReference<EditorGuiManager> mManager = WeakReference<EditorGuiManager>.Create(null);

	public EditorGuiManager Manager
	{
		get
		{
			EditorGuiManager result = null;
			if (mManager != null)
			{
				result = mManager.Target;
			}
			return result;
		}
		set { mManager = WeakReference<EditorGuiManager>.Create(value); }
	}

	public ITopLevel InnerTopLevel
	{
		get { return mInnerTopLevel; }
		set
		{
			mInnerTopLevel = value;
			if (mInnerTopLevel != null)
			{
				this.name = mInnerTopLevel.Name;
				this.position = new Rect(100.0f, 100.0f, mInnerTopLevel.ExternalSize.x, mInnerTopLevel.ExternalSize.y);
				mInnerTopLevel.AddOnCloseAction(delegate()
				{
					this.Close();
				});

				this.ShowUtility();
			}
		}
	}

	public void OnGUI()
	{
		if (mInnerTopLevel != null && mManager.IsAlive)
		{
			mInnerTopLevel.Draw(mManager.Target, mManager.Target.GetPosition(this));
		}
		else
		{
			this.Close();
		}
	}
}
