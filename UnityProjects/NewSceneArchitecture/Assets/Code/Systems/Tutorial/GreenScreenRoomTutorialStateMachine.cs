/* Pherg 10/21/09 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;
using Hangout.Client.Gui;
using UnityEngine;

namespace Hangout.Client
{
	public class GreenScreenRoomTutorialStateMachine : StateMachine
	{
		private const string mTutorialGuiPath = "resources://GUI/TutorialGui/TutorialPopup.gui";
		private InactiveState mInactiveState;
		private GoShoppingTutorialState mGoShoppingTutorialState;
		private MovementTutorialState mMovementTutorialState;
		private MapTutorialState mMapTutorialState;
		private GetCashTutorialState mGetCashTutorialState;
		private DecorateTutorialState mDecorateTutorialState;
		
		private IScheduler mScheduler;
		
		private List<TutorialState> mTutorialsToShow = new List<TutorialState>();
		
		private GreenScreenRoomTutorialGui mTutorialGui;
		
		private UserAccountProxy mUserAccountProxy;
		
		private ITask mWaitThenTransitionTask = null;
		
		private float mSecondsToWaitBetweenPopups = 4.0f;
		
		public GreenScreenRoomTutorialStateMachine()
		{
			IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			
			mUserAccountProxy = GameFacade.Instance.RetrieveProxy<UserAccountProxy>();
			
			mTutorialGui = new GreenScreenRoomTutorialGui(guiManager, mTutorialGuiPath);
			
			mInactiveState = new InactiveState();

			mMovementTutorialState = new MovementTutorialState(mTutorialGui, NextTutorial);
			mGoShoppingTutorialState = new GoShoppingTutorialState(mTutorialGui, RemoveTutorialFromList);
			mMapTutorialState = new MapTutorialState(mTutorialGui, EmptyCallback);
			mGetCashTutorialState = new GetCashTutorialState(mTutorialGui, RemoveTutorialFromList);
			mDecorateTutorialState = new DecorateTutorialState(mTutorialGui, RemoveTutorialFromList);
			
			mInactiveState.AddTransition(mGoShoppingTutorialState);
			mGoShoppingTutorialState.AddTransition(mInactiveState);

			mInactiveState.AddTransition(mMovementTutorialState);
			mMovementTutorialState.AddTransition(mInactiveState);

			mInactiveState.AddTransition(mMapTutorialState);
			mMapTutorialState.AddTransition(mInactiveState);

			mInactiveState.AddTransition(mGetCashTutorialState);
			mGetCashTutorialState.AddTransition(mInactiveState);

			mInactiveState.AddTransition(mDecorateTutorialState);
			mDecorateTutorialState.AddTransition(mInactiveState);

			this.EnterInitialState(mInactiveState);
			
			SetUpGreenScreenRoomTutorials();
			
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
		}

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			return new string[]
			{
				GameFacade.SHOP_OPENED,
				GameFacade.ENTERED_GREEN_SCREEN_ROOM_DEFAULT_STATE,
				GameFacade.MAP_GUI_CLOSED,
				GameFacade.MAP_GUI_OPENED,
				GameFacade.GET_CASH_GUI_OPENED,
				GameFacade.GET_CASH_GUI_CLOSED,
			};
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);

			switch (notification.Name)
			{
				case GameFacade.SHOP_OPENED:
					if (mTutorialsToShow.Contains(mMapTutorialState) || mTutorialsToShow.Contains(mDecorateTutorialState))
					{
						if (this.CurrentState == mGoShoppingTutorialState)
						{
							mGoShoppingTutorialState.TutorialComplete();
						}
						else if (this.CurrentState == mDecorateTutorialState)
						{
							mDecorateTutorialState.TutorialComplete();
						}
						// TODO: PUt in the code to differentiate between your own room so we can tell which one of these
						// to remove.
						mTutorialsToShow.Remove(mGoShoppingTutorialState);
						mTutorialsToShow.Remove(mDecorateTutorialState);
					}
					break;
				case GameFacade.ENTERED_GREEN_SCREEN_ROOM_DEFAULT_STATE:
					SetTutorialToUse();
					break;
				case GameFacade.MAP_GUI_OPENED:
					if (mTutorialsToShow.Contains(mMapTutorialState))
					{
						if (this.CurrentState == mMapTutorialState)
						{
							mMapTutorialState.TutorialComplete();
						}
						mTutorialsToShow.Remove(mMapTutorialState);
					}
					break;
				case GameFacade.MAP_GUI_CLOSED:
					if (this.CurrentState == mMapTutorialState)
					{
						NextTutorial(mMapTutorialState);
					}
					break;
				case GameFacade.GET_CASH_GUI_OPENED:
					if (mTutorialsToShow.Contains(mGetCashTutorialState))
					{
						if (this.CurrentState == mGetCashTutorialState)
						{
							mGetCashTutorialState.TutorialComplete();
						}
						mTutorialsToShow.Remove(mGetCashTutorialState);
					}
					break;
				case GameFacade.GET_CASH_GUI_CLOSED:
					if (this.CurrentState == mMapTutorialState)
					{
						SetTutorialToUse();
					}
					break;
				default:
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}
		
		private void EmptyCallback(TutorialState tutorialState)
		{
			return;
		}
		
		private void NextTutorial(TutorialState tutorialState)
		{
			RemoveTutorialFromList(tutorialState);
			SetTutorialToUse();
		}

		private void RemoveTutorialFromList(TutorialState stateToRemove)
		{
			mTutorialsToShow.Remove(stateToRemove);
			if (this.CurrentState == stateToRemove)
			{
				this.TransitionToState(mInactiveState);
			}
		}
		
		private void SetTutorialToUse()
		{
			if (mTutorialsToShow.Count > 0 && mTutorialsToShow[0] != this.CurrentState)
			{
				mWaitThenTransitionTask = mScheduler.StartCoroutine(WaitThenTransition());
			}
		}
		
		private IEnumerator<IYieldInstruction> WaitThenTransition()
		{
			yield return new YieldForSeconds(mSecondsToWaitBetweenPopups);
			if (mTutorialsToShow.Count > 0)
			{
				this.TransitionToState(mTutorialsToShow[0]);
			}
			mWaitThenTransitionTask = null;
		}

		private void SetUpGreenScreenRoomTutorials()
		{
			bool completedTutorial = false;
			mUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasCompleteMoveTutorial, ref completedTutorial);
			if (!completedTutorial)
			{
				mTutorialsToShow.Add(mMovementTutorialState);
			}

			mUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasCompletedShoppingTutorial, ref completedTutorial);
			if (!completedTutorial)
			{
				mTutorialsToShow.Add(mGoShoppingTutorialState);
			}

			mUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasCompletedOpenMapTutorial, ref completedTutorial);
			if (!completedTutorial)
			{
				mTutorialsToShow.Add(mMapTutorialState);
			}

			mUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasCompletedDecorateTutorial, ref completedTutorial);
			completedTutorial = false;
			bool isRoomowner = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>().IsRoomOwner();
			if (!completedTutorial && isRoomowner)
			{
				mTutorialsToShow.Add(mDecorateTutorialState);
			}

            //mUserAccountProxy.TryGetAccountProperty<bool>(UserAccountProperties.HasCompletedGetCashTutorial, ref completedTutorial);
            //if (!completedTutorial)
            //{
            //    mTutorialsToShow.Add(mGetCashTutorialState);
            //}
		}

		private void SetUpFashionTutorials()
		{
			if (mWaitThenTransitionTask != null)
			{
				mWaitThenTransitionTask.Exit();
			}
			mTutorialGui.HidePopup();
			mTutorialsToShow.Clear();
		}
		
		public override void OnRemove()
		{
			if (mWaitThenTransitionTask != null)
			{
				mWaitThenTransitionTask.Exit();
			}
			mTutorialGui.HidePopup();
			mTutorialsToShow.Clear();
		}
	}
}
