/**  --------------------------------------------------------  *
 *   LoadLevelState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/03/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;


using Hangout.Shared;

namespace Hangout.Client.FashionGame
{
	public class LoadLevelState : State
	{
		private readonly Action<FashionLevel> mOnLevelLoaded;
		private readonly IScheduler mScheduler;

		public LoadLevelState(Action<FashionLevel> onLevelLoaded)
		{
			if (onLevelLoaded == null)
			{
				throw new ArgumentNullException("onLevelLoaded");
			}
			mOnLevelLoaded = onLevelLoaded;
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
		}

		public override void EnterState()
		{
			PlayerProgression progression = GameFacade.Instance.RetrieveMediator<PlayerProgression>();
			progression.GetNextLevel(delegate(FashionLevel level)
			{
				mScheduler.StartCoroutine(SetupLevel(level));
			});
		}

		private IEnumerator<IYieldInstruction> SetupLevel(FashionLevel level)
		{
			// Wait for the async parts of the level to complete
			yield return new YieldWhile(delegate()
			{
				return !level.IsLoaded;
			});

			// Load the models for this level
			FashionNpcMediator npcFactory = GameFacade.Instance.RetrieveMediator<FashionNpcMediator>();
			npcFactory.BuildNpcsForLevel(level);

			// Yield if there are any new models to download
			yield return new YieldWhile(delegate()
			{
				return npcFactory.Downloading;
			});

			mOnLevelLoaded(level);
		}

		public override void ExitState()
		{
			// Kill the loading screen if it's up
			GameFacade.Instance.SendNotification(GameFacade.REMOVE_FASHION_MINIGAME_LOADING_SCREEN);
		}
	}
}
