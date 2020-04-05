/**  --------------------------------------------------------  *
 *   ModelStation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client;
using Hangout.Client.Gui;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	/// <summary>
	/// ModelStation is a FashionGameStation that Models can go to
	/// </summary>
	public abstract class ModelStation : FashionGameStation
	{
		private readonly Transform mAvatarSitTransform;
		
		public ModelStation(Pair<Vector3> location, string name, Texture2D image, float time, Vector3 guiOffset, GameObject displayObject)
			: base(location, name, image, time, guiOffset, displayObject)
		{
			GameObject avatarStool = GameObjectUtility.GetNamedChildRecursive("Stool", displayObject);
			if (avatarStool == null)
			{
				throw new Exception("Unable to find Stool object in Station " + displayObject.name);
			}
			mAvatarSitTransform = avatarStool.transform;
		}

		public override bool RequiresWorker
		{
			get { return true; }
		}

		public Vector3 AvatarTargetPosition
		{
			get { return mAvatarSitTransform.position; }
		}

		private FashionModel mModelAtThisStation = null;
		public bool InUse
		{
			get { return mModelAtThisStation != null; }
		}

		private ITask mWaitThenContinue = null;
		public virtual void ModelArrived(FashionModel model)
		{
			if (mModelAtThisStation == null)
			{
				if( this.RequiresWorker )
				{
					Worker.PlayWorkingAnimation();
				}
				PlayActivationSound();

				mModelAtThisStation = model;
				if (mWaitThenContinue != null)
				{
					mWaitThenContinue.Exit();
				}
				mWaitThenContinue = mScheduler.StartCoroutine(WaitThenContinue(model, WaitTime));

				mWaitThenContinue.AddOnExitAction(delegate()
				{
					model.Clickable = true;
					model.WalkToCenter();
					mModelAtThisStation = null;

					this.HideProgressGui();
				});
			}
			else
			{
				model.WalkToCenter();
			}
		}

		protected IEnumerator<IYieldInstruction> WaitThenContinue(FashionModel model, float pauseTime)
		{
			ShowProgressGui();

			model.Clickable = false;
			float inversePauseTime = 1.0f / pauseTime;
			for (float t = 0.0f; t < pauseTime; t += Time.deltaTime)
			{
				SetProgress(inversePauseTime * t);
				yield return new YieldUntilNextFrame();
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			if (mWaitThenContinue != null)
			{
				mWaitThenContinue.Exit();
			}
		}
	}
}
