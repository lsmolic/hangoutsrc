/**  --------------------------------------------------------  *
 *   DefaultForeignAvatarState.cs
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
	class DefaultForeignAvatarState : DefaultAvatarState
	{
		private ForeignAvatarEntity mForeignAvatarEntity;
		
		private ForeignAvatarAnimationStateMachine mForeignAvatarAnimationStateMachine;
		public DefaultForeignAvatarState(ForeignAvatarEntity avatarEntity)
			: base(avatarEntity)
		{
			mForeignAvatarEntity = avatarEntity;
		}

		public override void EnterState()
		{
			mForeignAvatarAnimationStateMachine = new ForeignAvatarAnimationStateMachine(mForeignAvatarEntity);
			this.AnimationStateMachine = mForeignAvatarAnimationStateMachine;
		}

		public override void ExitState()
		{
			mForeignAvatarAnimationStateMachine.Dispose();
		}

		public void SetCurrentAvatarSpeed(float speed)
		{
			((ForeignAvatarAnimationStateMachine)this.AnimationStateMachine).SetCurrentAvatarSpeed(speed);
		}

		public override string Name
		{
			get { return "DefaultForeignAvatarState"; }
		}

		public void PlayEmote(string emoteName)
		{
			mForeignAvatarAnimationStateMachine.PlayEmote(emoteName);
		}
	}
}
