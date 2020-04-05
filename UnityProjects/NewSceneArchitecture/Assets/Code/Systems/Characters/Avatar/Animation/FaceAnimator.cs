using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Hangout.Shared;
using System;

namespace Hangout.Client
{
	public class FaceAnimator : IDisposable
	{
		public ComplexUvAnimator mComplexUvAnimator;
		public AvatarFaceMesh mAvatarFaceMesh;
		public float mAnimationInterval = 1.5f;
		
		private ITask mAnimate = null;
		
		private IScheduler mScheduler;

		private FaceAnimation mFaceAnimation;
		
		public bool IsPlaying
		{
			get { return mComplexUvAnimator.IsPlaying; }
		}
		public FaceAnimation FaceAnimation
		{
			get { return mFaceAnimation; }
			set
			{
				mFaceAnimation = value;
				PlayAnimation(mFaceAnimation.InitialAnimation);
				if (mAnimate == null)
				{
					mAnimate = mScheduler.StartCoroutine(Animate());
				}
			}
		}
		
		// Use this for initialization
		public FaceAnimator(AvatarFaceMesh avatarFaceMesh)
		{		
			mAvatarFaceMesh = avatarFaceMesh;
			mComplexUvAnimator = new ComplexUvAnimator(avatarFaceMesh);
			
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
		}

		public void Dispose()
		{
			mAvatarFaceMesh.Dispose();
		}

		public IEnumerator<IYieldInstruction> Animate()
		{
			yield return new YieldUntilNextFrame();
			while( true )
			{
				if( !IsPlaying )
				{
					// Wait for interval after the animator is done animating
					yield return new YieldForSeconds( mAnimationInterval );
					
					// Play a mood animation. It could be a ComplexUvAnimation or a UvAnimation
					if( !IsPlaying )
					{
						PlayAnimation(mFaceAnimation.RandomAnimation);
					}
				}
				else
					yield return new YieldUntilNextFrame();
			}
		}

		private void PlayAnimation(UvAnimation animation)
		{
			if (animation is SimpleUvAnimation)
			{
				mComplexUvAnimator.StopAnimation();
				SimpleUvAnimation simpleUvAnimation = (SimpleUvAnimation)animation;
				mComplexUvAnimator.PlayUvAnimation(simpleUvAnimation);
			}
			else if (animation is ComplexUvAnimation)
			{
				mComplexUvAnimator.StopAnimation();
				ComplexUvAnimation complexUvAnimation = (ComplexUvAnimation) animation;
				mScheduler.StartCoroutine(mComplexUvAnimator.PlayComplexAnimation(complexUvAnimation));
			}
			else
			{
				Console.LogError("UvAnimation being passed into PlayAnimation is neither a SimpleUvAnimation or ComplexUvAnimation type");
			}
 		}
	}
}