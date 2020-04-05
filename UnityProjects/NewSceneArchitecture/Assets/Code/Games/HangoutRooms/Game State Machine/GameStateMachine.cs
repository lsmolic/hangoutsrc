/**  --------------------------------------------------------  *
 *   GameStateMachine.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/17/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using Hangout.Client.Gui;

using System;
using System.Collections.Generic;

using Hangout.Client.FashionGame;
using Hangout.Shared;

namespace Hangout.Client
{
	public class GameStateMachine : StateMachine
	{		
		private GreenScreenRoomGameState mGreenScreenRoomState;
		private FashionGameState mFashionGameState;
		private InactiveState mInactiveState;
		private GameStateMachineLoadingState mGameStateMachineLoadingState;
		
		public GameStateMachine()
		{		
			mGreenScreenRoomState = new GreenScreenRoomGameState();
			mFashionGameState = new FashionGameState();
			mInactiveState = new InactiveState();
			mGameStateMachineLoadingState = new GameStateMachineLoadingState(HandleInitialEntryToGame);

			mGreenScreenRoomState.AddTransition(mInactiveState);
			mGreenScreenRoomState.AddTransition(mFashionGameState);
			mGreenScreenRoomState.AddTransition(mGreenScreenRoomState);

			mFashionGameState.AddTransition(mInactiveState);
			mFashionGameState.AddTransition(mGreenScreenRoomState);

			mGameStateMachineLoadingState.AddTransition(mGreenScreenRoomState);
			mGameStateMachineLoadingState.AddTransition(mFashionGameState);

			mInactiveState.AddTransition(mFashionGameState);
			mInactiveState.AddTransition(mGreenScreenRoomState);
			mInactiveState.AddTransition(mGameStateMachineLoadingState);
			
			mGameStateMachineLoadingState.AddTransition(mGreenScreenRoomState);
			mGameStateMachineLoadingState.AddTransition(mFashionGameState);
			mGameStateMachineLoadingState.AddTransition(mInactiveState);
			this.EnterInitialState(mGameStateMachineLoadingState);

			if (GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().Loaded)
			{
				mGameStateMachineLoadingState.ClientAssetRepositoryDoneLoading();
				LoadStrcuturesWhichDependOnClientAssetRepository();
			}
		}

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			return new string[]
			{ 
				GameFacade.CLIENT_ASSET_REPOSITORY_LOADED,
				GameFacade.SOUND_PROXY_LOADED,
				GameFacade.SWITCHING_TO_FASHION_MINI_GAME,
				GameFacade.SWITCHING_TO_GREEN_SCREEN_ROOM,
			};
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);
			
			switch(notification.Name)
			{
				case GameFacade.CLIENT_ASSET_REPOSITORY_LOADED:
					if (this.CurrentState == mGameStateMachineLoadingState)
					{
						mGameStateMachineLoadingState.ClientAssetRepositoryDoneLoading();
						LoadStrcuturesWhichDependOnClientAssetRepository();
					}
					else
					{
						Console.LogError("Client asset repository has loaded but the game state machine is not in the Loading State.");
					}
					break;
				case GameFacade.SOUND_PROXY_LOADED:
					if (this.CurrentState == mGameStateMachineLoadingState)
					{
						mGameStateMachineLoadingState.SoundProxyDoneLoading();
					}
					else
					{
						Console.LogError("Sound Proxy has loaded but the game state machine is not in the Loading State.");
					}
					break;
				case GameFacade.SWITCHING_TO_FASHION_MINI_GAME:
					this.TransitionToState(mInactiveState);
					this.TransitionToState(mFashionGameState);
					break;
				case GameFacade.SWITCHING_TO_GREEN_SCREEN_ROOM:
					this.TransitionToState(mInactiveState);
					this.TransitionToState(mGreenScreenRoomState);
					break;
				default:
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}
		
		private void LoadStrcuturesWhichDependOnClientAssetRepository()
		{
			GameFacade.Instance.RegisterProxy(new SoundProxy());
		}
		
        /// <summary>
        /// Handle initial entry into the game.  Put user in a default location based on their if they are a first time user, 
        /// and their entry point into the client from the web
        /// </summary>
        private void HandleInitialEntryToGame()
        {
            UserAccountProxy userAccountProxy = GameFacade.Instance.RetrieveProxy<UserAccountProxy>();
            bool isFirstTimeUser = false;
            Console.WriteLine("User properties " + userAccountProxy.ToString());
            if(!userAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.FirstTimeUser, ref isFirstTimeUser))
            {
                Console.WriteLine("First time user property not set, assume it our first time in");
                isFirstTimeUser = true;
            }

			ConfigManagerClient configManager = GameFacade.Instance.RetrieveProxy<ConfigManagerClient>();
			bool returnUsersStartMinigame = configManager.GetBool("return_users_start_minigame", true);

            if(isFirstTimeUser)
            {
                ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
                switch (connectionProxy.WebEntryPointId)
                {
                    case FunnelGlobals.PUBLIC_LOBBY:
                        RoomManagerProxy roomManager = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>();
                        roomManager.JoinLastRoom();
                        break;
                    case FunnelGlobals.FASHION_MINIGAME:
                        SendNotification(GameFacade.SWITCHING_TO_FASHION_MINI_GAME);
                        break;
                    default:
                        throw new Exception("Unexpected entry point received: " + connectionProxy.WebEntryPointId);
                }
                // Set firstTimeUser property to false
                userAccountProxy.SetAccountProperty<bool>(UserAccountProperties.FirstTimeUser, false);
            }
            else
            {
				if (returnUsersStartMinigame)
				{
					SendNotification(GameFacade.SWITCHING_TO_FASHION_MINI_GAME);
				}
				else
				{
					Console.WriteLine("Not a First time user go to room");
					// Returning user.  Go to default room
					RoomManagerProxy roomManager = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>();
					roomManager.JoinLastRoom();
				}
            }
        }

		public void Deactivate()
		{
			this.TransitionToState(mInactiveState);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			GameFacade.Instance.RemoveProxy(typeof(SoundProxy).Name);
		}
	}
}
