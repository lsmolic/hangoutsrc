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
	public class DefaultLocalAvatarState : DefaultAvatarState
	{	
	
		private LocalAvatarEntity mLocalAvatarEntity;
		public DefaultLocalAvatarState(LocalAvatarEntity avatarEntity)
			: base(avatarEntity)
		{
			mLocalAvatarEntity = avatarEntity;
		}
		
		public override void EnterState()
		{
			this.AnimationStateMachine = new LocalAvatarAnimationStateMachine(mLocalAvatarEntity);
             // Register with facade for later use
            GameFacade.Instance.RegisterMediator(this.AnimationStateMachine);
        }

		public override void ExitState()
		{
			this.AnimationStateMachine.Dispose();
			GameFacade.Instance.RemoveMediator(typeof(LocalAvatarAnimationStateMachine).Name);
			base.ExitState();
		}
		
		public override string Name
		{
			get { return "DefaultLocalAvatarState"; }
		}
	}
}
