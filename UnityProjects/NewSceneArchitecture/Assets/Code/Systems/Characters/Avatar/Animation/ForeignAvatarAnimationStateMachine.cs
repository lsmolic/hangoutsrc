/**  --------------------------------------------------------  *
 *   LocalAvatarAnimationStateMachine.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	class ForeignAvatarAnimationStateMachine : DefaultAvatarAnimationStateMachine
	{
		public ForeignAvatarAnimationStateMachine(ForeignAvatarEntity avatarEntity)
			: base(avatarEntity)
		{
		}


		private float mCurrentAvatarSpeed;
		public override float CurrentAvatarSpeed
		{
			get
			{
				return mCurrentAvatarSpeed;
			}
		}
		
		public void PlayEmote(string emoteName)
		{
			RigAnimationName animationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), emoteName);
			GameFacade.Instance.RetrieveProxy<AnimationProxy>().GetRigAnimation(animationName, delegate(AnimationClip animationClip)
			{
				AnimationSequence emoteSequence = new AnimationSequence(animationClip.name, true);
				emoteSequence.AddListing(new AnimationSequence.Listing(animationClip, new AnimationLoopCount(1, 1)));
				Emote emote = new Emote(animationName, emoteSequence);
				this.PlayEmote(emote);
			});
		}

		public void SetCurrentAvatarSpeed(float speed)
		{
			mCurrentAvatarSpeed = speed;
		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
