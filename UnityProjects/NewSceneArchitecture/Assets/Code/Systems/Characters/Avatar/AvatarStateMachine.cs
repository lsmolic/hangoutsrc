/**  --------------------------------------------------------  *
 *   AvatarStateMachine.cs
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
	public class AvatarStateMachine : StateMachine, IDisposable
	{
		public AvatarStateMachine()
		{
			this.EnterInitialState(new LoadingState());
		}
		
		public void Dispose()
		{
			CurrentState.ExitState();
			CurrentState.Dispose();
		}
	}	
}
