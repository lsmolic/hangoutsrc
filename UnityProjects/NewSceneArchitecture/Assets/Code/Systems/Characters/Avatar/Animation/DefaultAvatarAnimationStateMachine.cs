/**  --------------------------------------------------------  *
 *   DefaultAvatarAnimationStateMachine.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 11/23/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;
using Hangout.Shared;

using System.Xml;

namespace Hangout.Client
{
	/// Controls all the animation for an avatar
	public abstract class DefaultAvatarAnimationStateMachine : AvatarAnimationStateMachine, IDisposable
	{
		private const float mMinWalkSpeed = 0.05f; /// The speed at which the walk animation starts playing
		private float mWalkAnimationSpeed = 1.125f; /// When the avatar walk animation is playing at 100% speed, how fast is the avatar translating
		private const string PATH_TO_ANIMATION_SPEEDS = "resources://XmlDocuments/WalkCycleSpeeds";
		private XmlDocument mWalkSpeedDocument;

		private AvatarEntity mAvatarEntity;
	
		private ITask mMonitorSpeedTask = null;

		private AnimationSequence mIdleState = null;
		public AnimationSequence IdleState
		{
			get { return mIdleState; }
		}

		private AnimationSequence mWalkState = null;
		public AnimationSequence WalkState
		{
			get { return mWalkState; }
		}

		public abstract float CurrentAvatarSpeed { get; }

		public DefaultAvatarAnimationStateMachine(AvatarEntity avatarEntity)
			: base(avatarEntity)
		{
			mWalkSpeedDocument = XmlUtility.LoadXmlDocument(PATH_TO_ANIMATION_SPEEDS);
			mAvatarEntity = avatarEntity;
			// Build the walk and idle states
			GetWalkAnimationFromAvatar();
			GetIdleAnimationFromAvatar();
			
			avatarEntity.RegisterForWalkAnimationChange(GetWalkAnimationFromAvatar);
			avatarEntity.RegisterForIdleAnimationChange(GetIdleAnimationFromAvatar);
		}
		
		private void SetIdleAnimation(AnimationClip idleClip)
		{
			if (mIdleState != null)
			{
				mIdleState.Dispose();
			}
			mIdleState = new AnimationSequence(idleClip.name);
			mIdleState.AddListing(new AnimationSequence.Listing(idleClip, new AnimationLoopCount()));
			CheckIfReadyToStartAnimation();
		}
		
		private void SetWalkAnimation(AnimationClip walkClip)
		{
			if (mWalkState != null)
			{
				mWalkState.Dispose();
			}
			mWalkState = new AnimationSequence(walkClip.name);
			mWalkState.AddListing(new AnimationSequence.Listing(walkClip, new AnimationLoopCount()));

			foreach (XmlNode walkSpeed in mWalkSpeedDocument.SelectNodes("WalkSpeeds/WalkSpeed"))
			{
				if (walkSpeed.SelectSingleNode("ClipName").InnerText == walkClip.name)
				{
					mWalkAnimationSpeed = float.Parse(walkSpeed.SelectSingleNode("Speed").InnerText);

					mAvatarEntity.MaxWalkSpeed = mWalkAnimationSpeed;

					CheckIfReadyToStartAnimation();
					return;
				}
			}
			mWalkAnimationSpeed = 1.0f;
			mAvatarEntity.MaxWalkSpeed = mWalkAnimationSpeed;
			CheckIfReadyToStartAnimation();
		}
		
		protected void GetWalkAnimationFromAvatar()
		{
			GameFacade.Instance.RetrieveProxy<AnimationProxy>().GetRigAnimation(mAvatarEntity.WalkRigAnimationName, SetWalkAnimation);
		}

		protected void GetIdleAnimationFromAvatar()
		{
			GameFacade.Instance.RetrieveProxy<AnimationProxy>().GetRigAnimation(mAvatarEntity.IdleRigAnimationName, SetIdleAnimation);
		}
		
		private void CheckIfReadyToStartAnimation()
		{
			if (mIdleState != null && mWalkState != null)
			{
				if (mMonitorSpeedTask != null)
				{
				
					mMonitorSpeedTask.Exit();
				}
				StartAnimation();
			}
		}
		
		private void StartAnimation()
		{
			mIdleState.OneShot = false;
			mWalkState.OneShot = false;

			this.EnterInitialState(mIdleState);

			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mMonitorSpeedTask = scheduler.StartCoroutine(MonitorAvatarSpeed());
		}

		private IEnumerator<IYieldInstruction> MonitorAvatarSpeed()
		{
			while (true)
			{
				float currentAvatarSpeed = this.CurrentAvatarSpeed;
				if (currentAvatarSpeed > mMinWalkSpeed)
				{
					if (this.CurrentState.Name != this.WalkState.Name)
					{
						this.SetToWalk(currentAvatarSpeed / mWalkAnimationSpeed);
					}
				}
				else if (this.CurrentState.Name == this.WalkState.Name)
				{
					this.SetToIdle();
				}

				if ((this.CurrentState is AnimationStateBase) && ((AnimationStateBase)this.CurrentState).DoneAnimating)
				{
					this.SetToIdle();
				}

				yield return new YieldUntilNextFrame();
			}
		}

		private void SetToIdle()
		{
			if (this.CurrentState != mIdleState)
			{
				if (!this.CurrentState.CanGoToState(mIdleState.Name))
				{
					this.CurrentState.AddTransition(mIdleState);
				}
				this.TransitionToState(mIdleState);
			}
		}

		private void SetToWalk(float walkSpeed)
		{
			if (this.CurrentState != mWalkState)
			{
				if (!this.CurrentState.CanGoToState(mWalkState.Name))
				{
					this.CurrentState.AddTransition(mWalkState);
				}

				this.TransitionToState(mWalkState);
			}
		}

		protected void PlayEmote(Emote emote)
		{
			if (this.CurrentState == null)
			{
				this.EnterInitialState(emote.Sequence);
				return;
			}
			// Play emote immediately on local client
			else if (!this.CurrentState.CanGoToState(emote.Name.ToString()))
			{
				this.CurrentState.AddTransition(emote.Sequence);
			}
			this.TransitionToState(emote.Sequence);
		}

		public override void Dispose()
		{
			if (mMonitorSpeedTask != null)
			{
				mMonitorSpeedTask.Exit();
			}
			if (this.CurrentState != null)
			{
				this.CurrentState.ExitState();
			}
			base.Dispose();
		}
	}
}
