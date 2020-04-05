/**  --------------------------------------------------------  *
 *   DefaultLocalAvatarState.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	// Avatar chillin around the room in default gameplay mode
	public class DefaultAvatarState : State
	{

		private AvatarAnimationStateMachine mAnimationStateMachine = null;
		protected Hangout.Client.AvatarAnimationStateMachine AnimationStateMachine
		{
			get { return mAnimationStateMachine; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				mAnimationStateMachine = value;
			}
		}
		
        private readonly UnityEngine.Animation mAvatarAnimationComponent;
		protected UnityEngine.Animation AvatarAnimationComponent
		{
			get { return mAvatarAnimationComponent; }
		}
		
		public DefaultAvatarState(AvatarEntity avatarEntity)
		{
			if (avatarEntity == null)
			{
				throw new ArgumentNullException("avatarEntity");
			}

			mAvatarAnimationComponent = avatarEntity.UnityGameObject.GetComponent(typeof(Animation)) as Animation;
			if (mAvatarAnimationComponent ==null)
			{
				throw new Exception("Could not find animation component on the Avatar entity.");
			}
		}
		
		public override void EnterState()
		{
		}
		
		public override void ExitState()
		{
		}
		
		public override string Name
		{
			get { return "DefaultAvatarState"; }
		}
	}
}
