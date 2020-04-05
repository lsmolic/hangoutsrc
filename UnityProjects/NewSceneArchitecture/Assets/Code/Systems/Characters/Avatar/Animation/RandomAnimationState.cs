/**  --------------------------------------------------------  *
 *   RandomAnimationState.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public class RandomAnimationState : State
	{
		private readonly System.Random mRand = new System.Random((int)DateTime.Now.Ticks + 1);

		private readonly string mName;
		private readonly List<AnimationClip> mAnimations = new List<AnimationClip>();
		private readonly UnityEngine.Animation mAvatarAnimationComponent;
		private readonly IScheduler mScheduler;
		private ITask mAnimationTask = null;
		private ITask mUpdatePlaybackTask = null;
		private float mPlaybackSpeed = 1.0f;

		public float PlaybackSpeed
		{
			get { return mPlaybackSpeed; }
			set { mPlaybackSpeed = value; }
		}
		
		public RandomAnimationState(string stateName, UnityEngine.Animation avatarAnimationComponent, IScheduler scheduler)
		{
			if( avatarAnimationComponent == null )
			{
				throw new ArgumentNullException("avatarAnimationComponent");
			}
			mAvatarAnimationComponent = avatarAnimationComponent;
			
			if( stateName == null )
			{
				throw new ArgumentNullException("stateName");
			}
			mName = stateName;
			
			
			if( scheduler == null )
			{
				throw new ArgumentNullException("scheduler");
			}
			mScheduler = scheduler;
			
		}
		
		private IEnumerator<IYieldInstruction> PlayAnimations()
		{			
			mAvatarAnimationComponent.Stop();
			while(true)
			{
				if( !mAvatarAnimationComponent.isPlaying )
				{
					AnimationClip clip = mAnimations[mRand.Next(0, mAnimations.Count - 1)];
					mAvatarAnimationComponent.Play(clip.name);
				}
				yield return new YieldUntilNextFrame();
			}
		}
		
		
		private IEnumerator<IYieldInstruction> UpdatePlaybackSpeed()
		{			
			while(true)
			{
				foreach( AnimationState state in mAvatarAnimationComponent )
				{
					state.speed = mPlaybackSpeed;
				}
				
				yield return new YieldUntilFixedUpdate();
			}
		}
		
		public void AddAnimation(AnimationClip clip) 
		{
			mAnimations.Add(clip);
		}

		public override string Name
		{
			get { return mName; }
		}
		
		public override void EnterState()
		{
			if( mAnimations.Count > 0 )
			{
				mAnimationTask = mScheduler.StartCoroutine(PlayAnimations());
				mUpdatePlaybackTask = mScheduler.StartCoroutine(UpdatePlaybackSpeed());
			}
			else
			{
				throw new Exception("Cannot enter idle animation state without any idle animations.");
			}
		}
		
		public override void ExitState()
		{
			mAnimationTask.Exit();
			mUpdatePlaybackTask.Exit();
		}
		
		public override void Dispose()
		{
			if (mAnimationTask != null)
			{
				mAnimationTask.Exit();	
			}
			if ( mUpdatePlaybackTask != null )
			{
				mUpdatePlaybackTask.Exit();
			}
		}   
	}	
}
