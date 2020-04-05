using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client.FashionGame
{
	public class FashionGameStateMachine : StateMachine
	{
		private RunwaySequenceState mRunwaySequenceState;
		
		public FashionGameStateMachine()
		{
			InitializeGameState initialState = new InitializeGameState(BeginLoadLevel);
			
			this.EnterInitialState(initialState);

			GameFacade.Instance.RegisterMediator(new FashionMinigameLoadingMediator());

			mRunwaySequenceState = new RunwaySequenceState(FashionMinigame.Exit);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			GameFacade.Instance.RemoveMediator<FashionMinigameLoadingMediator>();
			this.CurrentState.ExitState();
		}

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			return new string[]
			{ 
				GameFacade.ENTER_RUNWAY_SEQUENCE,
			};
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);

			switch (notification.Name)
			{
				case GameFacade.ENTER_RUNWAY_SEQUENCE:
					if (this.CurrentState != mRunwaySequenceState)
					{
						if (!this.CurrentState.CanGoToState(mRunwaySequenceState.Name))
						{
							this.CurrentState.AddTransition(mRunwaySequenceState);
						}
						List<string> levelInfo = notification.Body as List<string>;
						mRunwaySequenceState.BackgroundUrl = levelInfo[0];
						mRunwaySequenceState.LocationName = levelInfo[1];
						mRunwaySequenceState.MusicPath = levelInfo[2];
						this.TransitionToState(mRunwaySequenceState);
					}
					break;
				default:
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}

		private void BeginLoadLevel()
		{
			LoadLevelState loadLevelState = new LoadLevelState(LevelLoaded);
			
			if (this.CurrentState != null)
			{
				this.CurrentState.AddTransition(loadLevelState);
			}
			this.TransitionToState(loadLevelState);
			mRunwaySequenceState = new RunwaySequenceState(FashionMinigame.Exit);
		}

		private void LevelLoaded(FashionLevel level)
		{
			PlayLevelState playLevelState = new PlayLevelState(level, BeginLoadLevel);
			if (this.CurrentState is LoadLevelState)
			{
				this.CurrentState.AddTransition(playLevelState);
				this.TransitionToState(playLevelState);
			}
			else
			{
				throw new Exception("Cannot enter a PlayLevelState if the GameStateMachine is not in a completed LoadLevelState.");
			}
		}
		
		public void Dispose()
		{
			this.CurrentState.ExitState();
		}
	}
}
