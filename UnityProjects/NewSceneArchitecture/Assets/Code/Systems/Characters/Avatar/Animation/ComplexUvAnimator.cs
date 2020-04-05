/*
   Created by Vilas Tewari on 2008-12-01.

   A Class that can play UvAnimations and ComplexUvAnimations
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class ComplexUvAnimator
	{
		private AvatarFaceMesh mAvatarFaceMesh;
		private bool mIsPlaying = false;

		public bool IsPlaying
		{
			get { return mIsPlaying || mAvatarFaceMesh.IsPlaying; }
		}

		public ComplexUvAnimator(AvatarFaceMesh avatarFaceMesh)
		{
			mAvatarFaceMesh = avatarFaceMesh;
		}

		public void StopAnimation()
		{
			mIsPlaying = false;
		}

		public IEnumerator<IYieldInstruction> PlayComplexAnimation(ComplexUvAnimation cAnimation)
		{
			mIsPlaying = true;
			foreach (ComplexFrame cFrame in cAnimation.Frames)
			{
				/* Repeat this frame RepeatFrame times */
				for (int x = 0; x < cFrame.RepeatFrame; ++x)
				{
					/* wait till FaceMesh is idle */
					while (mAvatarFaceMesh.IsPlaying)
					{
						yield return new YieldUntilNextFrame();
					}
					/* Start all simpleAnimations in this Complexframe */
					foreach (SimpleUvAnimation uvAnimation in cFrame.UvAnimations)
					{
						PlayUvAnimation(uvAnimation);
					}
					/* wait for a frame before looping. Otherwise FaceMesh may not have been updated to idle yet */
					yield return new YieldUntilNextFrame();
				}
				// When we are done playing a frame, wait for WaitAfterFinished seconds
				yield return new YieldForSeconds(cFrame.WaitAfterFinished);
			}
			mIsPlaying = false;
		}


		public void PlayUvAnimation(SimpleUvAnimation anim)
		{
			/* For each animation target */
			for (int x = 0; x < anim.UvShellTargetCount; ++x)
			{
				mAvatarFaceMesh.Play(anim.GetUvShellTarget(x), anim.GetFrameSequence(x));
			}
		}
	}
}