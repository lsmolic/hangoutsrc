/* Pherg 11/24/09
 * Does avatar state machine things. . .
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client
{
	public class LocalAvatarStateMachine : StateMachine
	{
		private DefaultLocalAvatarState mDefaultLocalAvatarState;
		
		public LocalAvatarStateMachine(LocalAvatarEntity localAvatarEntity)
		{			
			mDefaultLocalAvatarState = new DefaultLocalAvatarState(localAvatarEntity);

			this.EnterInitialState(mDefaultLocalAvatarState);
		}
		
		public void Dispose()
		{
			this.CurrentState.ExitState();
		}
	}
}
