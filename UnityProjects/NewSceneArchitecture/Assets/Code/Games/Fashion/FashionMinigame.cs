/**  --------------------------------------------------------  *
 *   FashionMinigame.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/02/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

using PureMVC.Interfaces;

namespace Hangout.Client.FashionGame
{
	public class FashionMinigame : ClientDistributedMiniGameRoom
	{
		public const int MODEL_LAYER = 31;
		public const int UNSELECTABLE_MODEL_LAYER = 30;
		public const int STATION_LAYER = 29;
		public const int CLOTHING_LAYER = 28;
		public const int GROUND_LAYER = 27;
		public const int IGNORE_LIGHTING = 26;
		public static readonly int IGNORE_RAYCAST = LayerMask.NameToLayer("Ignore Raycast");

		public const string FACEBOOK_FEED_COPY_PATH = "assets://FashionMinigame/FashionMinigameFacebookFeedCopy.xml";

		public const string EARNED_EXPERIENCE_NOTIFICATION = "Woah, Fashion Game Experience!";

		private static FashionGameStateMachine mCurrentStateMachine;
		public FashionMinigame(SendMessageCallback sendMessage, DistributedObjectId doId, List<object> messageData, FashionGameStateMachine stateMachine)
            : base(sendMessage, doId, messageData)
		{
			GameFacade.Instance.SendNotification(GameFacade.ENTER_FASHION_MINIGAME);
			mCurrentStateMachine = stateMachine;
        }

		public override void BuildEntity()
		{
			GameFacade.Instance.RegisterMediator(new FashionCameraMediator());
			GameFacade.Instance.RegisterMediator(new ClothingMediator());

            GameObject mainCamera = GameObject.FindWithTag("MainCamera");
            mainCamera.camera.pixelRect = new Rect(10, 40, 740, 420);

			NavigationBar navBar = GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar;
			navBar.ClosetButton.Disable();
			navBar.FriendButton.Disable();
			navBar.RoomButton.Disable();
			navBar.ShopButton.Disable();
			navBar.EmoteMenuButton.Disable();
			navBar.EntourageButton.Disable();
			navBar.MapButton.Disable();
			navBar.UpdateCurrentLocationLabel("Fashion City");

			TopBar topBar = GameFacade.Instance.RetrieveMediator<ToolbarMediator>().TopBar;
			topBar.Mode = TopBar.TopBarMode.FashionMinigame;
		}

		private void CleanupMediator<T>() where T : IMediator
		{
			if (GameFacade.Instance.HasMediator<T>())
			{
				GameFacade.Instance.RemoveMediator<T>();
			}
		}

		private bool mDisposed = false;
		public override void Dispose()
		{
			if (!mDisposed)
			{
				// Mediators set up in InitializeGameState
				CleanupMediator<FashionGameGui>();
				CleanupMediator<FashionNpcMediator>();
				CleanupMediator<PlayerProgression>();
				CleanupMediator<ClothingMediator>();
				CleanupMediator<FashionGameInput>();
				CleanupMediator<FashionMinigameLoadingMediator>();
				CleanupMediator<FashionGameStateMachine>();

				GameFacade.Instance.RemoveMediator<FashionCameraMediator>();
				GameFacade.Instance.RemoveMediator<ClothingMediator>();

				NavigationBar navBar = GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar;
				navBar.ClosetButton.Enable();
				navBar.FriendButton.Enable();
				navBar.RoomButton.Enable();
				navBar.ShopButton.Enable();
				navBar.EmoteMenuButton.Enable();
				TopBar topBar = GameFacade.Instance.RetrieveMediator<ToolbarMediator>().TopBar;
				topBar.Mode = TopBar.TopBarMode.Hangout;
				mDisposed = true;
			}
		}

		public static void Exit()
		{
			RoomManagerProxy roomManagerProxy = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>();
			roomManagerProxy.JoinLastRoom();
			if( mCurrentStateMachine != null )
			{
				mCurrentStateMachine.CurrentState.ExitState();
				mCurrentStateMachine = null;
			}
		}

		public static void GoToRunway()
		{
			if (GameFacade.Instance.HasMediator<FashionLevel>())
			{
				string levelBackgroundPath = GameFacade.Instance.RetrieveMediator<FashionLevel>().RunwayBackgroundPath;
				string levelName = GameFacade.Instance.RetrieveMediator<FashionLevel>().LocationName;
				string runwayMusicPath = GameFacade.Instance.RetrieveMediator<FashionLevel>().RunwayMusicPath;
				
				List<string> levelInfo = new List<string>();
				levelInfo.Add(levelBackgroundPath);
				levelInfo.Add(levelName);
				levelInfo.Add(runwayMusicPath);
				GameFacade.Instance.SendNotification(GameFacade.ENTER_RUNWAY_SEQUENCE, levelInfo);
			}
		}
	}
}
