/**  --------------------------------------------------------  *
 *   FashionGameState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using Hangout.Shared;
using System.Collections.Generic;
using PureMVC.Interfaces;

namespace Hangout.Client.FashionGame
{
	public class FashionGameState : State
	{
		private FashionMinigame mFashionMinigame = null;
		private FashionGameStateMachine mGameStateMachine = null;
		private DateTime mStartGame;

		public override void EnterState()
		{
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.EVENT_MINIGAME_ENTERED);

			mStartGame = DateTime.UtcNow;
			mGameStateMachine = new FashionGameStateMachine();
			mFashionMinigame = new FashionMinigame(null, new DistributedObjectId(0), new List<object>(), mGameStateMachine);
			GameFacade.Instance.RegisterMediator(mGameStateMachine);
		}

		public override void ExitState()
		{
			mGameStateMachine.CurrentState.ExitState();
			mFashionMinigame.Dispose();

			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.EVENT_MINIGAME_EXITED,
				LogGlobals.PLAY_SESSION_TIME, (DateTime.UtcNow - mStartGame).ToString());
		}
	}
}
