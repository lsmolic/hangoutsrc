/**  --------------------------------------------------------  *
 *   PlayLevelState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/03/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client;

using Hangout.Shared;
using Hangout.Shared.FashionGame;
using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class PlayLevelState : State
	{
		private readonly IScheduler mScheduler;
		private readonly FashionLevel mLevel;
		
		private ITask mPlayLevelTask;
		
		private Hangout.Shared.Action mOnLevelComplete;

		public PlayLevelState(FashionLevel level, Hangout.Shared.Action onLevelComplete)
		{
			if( level == null ) 
			{
				throw new ArgumentNullException("level");
			}
			mLevel = level;

			GameFacade.Instance.RegisterMediator(mLevel);

			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			
			mOnLevelComplete = onLevelComplete;
		}

		public override void EnterState()
		{
			mPlayLevelTask = mScheduler.StartCoroutine(PlayLevel());
		}

		private IEnumerator<IYieldInstruction> PlayLevel()
		{
			FashionNpcMediator npcFactory = GameFacade.Instance.RetrieveMediator<FashionNpcMediator>();
			mLevel.StartLevel(!npcFactory.HasModelsForLevel(mLevel));

			yield return new YieldWhile(delegate()
			{
				return !mLevel.IsComplete;
			});
			mOnLevelComplete();
		}

		public override void ExitState()
		{
			GameFacade.Instance.RemoveMediator<FashionLevel>();
			if (mPlayLevelTask != null)
			{
				mPlayLevelTask.Exit();
			}
		}
	}
}
