/**  --------------------------------------------------------  *
 *   AvatarAnimationStateMachine.cs
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
	/// Controls all the animation for an avatar
	public abstract class AvatarAnimationStateMachine : StateMachine, IDisposable
	{
		private readonly UnityEngine.Animation mAnimationComponent;
		protected UnityEngine.Animation AnimationComponent
		{
			get { return mAnimationComponent; }
		}
		public void PlayAnimation(UnityEngine.AnimationClip animationClip)
		{
			if (animationClip == null)
			{
				throw new ArgumentNullException("animationClip");
			}

			if (mAnimationComponent[animationClip.name] == null)
			{
				mAnimationComponent.AddClip(animationClip, animationClip.name);
			}
			if (mAnimationComponent.gameObject.active == true)
			{
				mAnimationComponent[animationClip.name].enabled = true;
				mAnimationComponent[animationClip.name].weight = 1.0f;
				mAnimationComponent.CrossFade(animationClip.name, 0.3f, PlayMode.StopAll);
			}
		}
		
		public void StopPlayingAnimation()
		{
			mAnimationComponent.Stop();
		}
		
		public bool IsAnimating 
		{
			get { return mAnimationComponent.isPlaying; }
		}
		
		public AvatarAnimationStateMachine(AvatarEntity avatarEntity)
		{
			if (avatarEntity == null)
			{
				throw new ArgumentNullException("avatarEntity");
			}
			Animation animationComponent = avatarEntity.UnityGameObject.GetComponent(typeof(Animation)) as Animation;
			if( animationComponent == null )
			{
				throw new ArgumentNullException("animationComponent");
			}
			
			mAnimationComponent = animationComponent;
			mAnimationComponent.playAutomatically = false;
			mAnimationComponent.Stop();
		}

		public virtual void Dispose()
		{
			if (this.CurrentState != null)
			{
				this.CurrentState.Dispose();
			}
		}
	}	
}
