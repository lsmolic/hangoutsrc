/**  --------------------------------------------------------  *
 *   StationWorker.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using Hangout.Shared;
using UnityEngine;
using Hangout.Client.Gui;

namespace Hangout.Client.FashionGame
{
	public class StationWorker : Npc
	{
		private const string NAMETAG_GUI_PATH = "resources://GUI/Minigames/Fashion/NpcNametag.gui";

		private readonly GuiController mNametag;
		private FashionGameStation mAtStation = null;
		private AnimationClip mWorkingAnimation = null;
		private AnimationClip mIdleAnimation = null; 
		private ITask mPlayIdle = null;

		public void PutAtStation(FashionGameStation station)
		{
			mNametag.MainGui.Showing = true;
			mAtStation = station;
			this.UnityGameObject.SetActiveRecursively(true);

			if( mPlayIdle != null )
			{
				mPlayIdle.Exit();
			}

			mPlayIdle = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(PlayIdle());
		}

		public void PlayWorkingAnimation()
		{
			this.DisplayObject.animation.Stop();
			this.DisplayObject.animation.Play(mWorkingAnimation.name);
		}

		public void SetWorkingAnimation(AnimationClip clip)
		{
			mWorkingAnimation = clip;
			this.DisplayObject.animation.AddClip(mWorkingAnimation, mWorkingAnimation.name);
		}

		public void SetIdleAnimation(AnimationClip clip)
		{
			mIdleAnimation = clip;
			this.DisplayObject.animation.AddClip(mIdleAnimation, mIdleAnimation.name);
		}

		public void EndOfLevel()
		{
			mAtStation = null;
			mNametag.MainGui.Showing = false;
			this.UnityGameObject.SetActiveRecursively(false);
		}

		public override bool Active
		{
			get { return mAtStation != null; }
		}

		public StationWorker(string name, GameObject displayObject, HeadController headController, BodyController bodyController)
			: base(name, displayObject, headController, bodyController)
		{
			mNametag = new GuiController(GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>(), NAMETAG_GUI_PATH);
			mNametag.Manager.SetTopLevelPosition
			(
				mNametag.MainGui, 
				new FollowWorldSpaceObject
				(
					GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera,
					displayObject.transform,
					GuiAnchor.CenterCenter,
					Vector2.zero,
					Vector3.up * FashionModel.APPROX_AVATAR_HEIGHT * 1.1f // TODO: Hard coded value
				)
			);
			ITextGuiElement nameElement = mNametag.MainGui.SelectSingleElement<ITextGuiElement>("**/NameLabel");
			nameElement.Text = String.Format(nameElement.Text, name);
			mNametag.MainGui.Showing = false;
		}

		/// <summary>
		/// Always running... sets the animation to idle if there's no animations playing
		/// </summary>
		private IEnumerator<IYieldInstruction> PlayIdle()
		{
			yield return new YieldWhile(delegate()
			{
				bool displayObjectSetup = this.DisplayObject != null && this.DisplayObject.animation != null;
				bool animationsSetup = mIdleAnimation != null;
				return !(displayObjectSetup || animationsSetup);
			});

			DisplayObject.animation.playAutomatically = false;
			DisplayObject.animation.Stop();

			// Jitter the animations' time so that the station workers aren't in unison
			this.DisplayObject.animation[mIdleAnimation.name].normalizedTime = UnityEngine.Random.value;
			this.DisplayObject.animation[mIdleAnimation.name].speed = Mathf.Lerp(0.85f, 1.15f, UnityEngine.Random.value);
			while(true)
			{
				this.DisplayObject.animation.Play(mIdleAnimation.name);
				
				yield return new YieldWhile(delegate()
				{
					return this.DisplayObject.animation.isPlaying;
				});
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			mNametag.Dispose();
			
			if (mPlayIdle != null)
			{
				mPlayIdle.Exit();
			}
		}
	}
}
