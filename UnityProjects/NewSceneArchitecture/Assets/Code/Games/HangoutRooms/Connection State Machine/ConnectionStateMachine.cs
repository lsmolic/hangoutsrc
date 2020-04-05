/**  --------------------------------------------------------  *
 *   ConnectionStateMachine.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using Hangout.Client.Gui;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public class ConnectionStateMachine : StateMachine
	{
		private readonly LoginState mLoginState = new LoginState();
		private readonly DisconnectedState mDisconnectedState = new DisconnectedState();
		private readonly ConnectedState mConnectedState = new ConnectedState();

		public ConnectionStateMachine()
		{
			mDisconnectedState.AddTransition(mLoginState);
			mDisconnectedState.AddTransition(mDisconnectedState);
			mLoginState.AddTransition(mConnectedState);
			mLoginState.AddTransition(mDisconnectedState);
			mConnectedState.AddTransition(mDisconnectedState);

			this.EnterInitialState(mDisconnectedState);
		}

        public override System.Collections.Generic.IList<string> ListNotificationInterests()
        {
            IList<string> interestList = new List<string>();
            interestList.Add(GameFacade.CONNECTED);
            interestList.Add(GameFacade.LOGIN_REQUEST);
            interestList.Add(GameFacade.LOGIN_SUCCESS);
            interestList.Add(GameFacade.LOGIN_FAILED);
			interestList.Add(GameFacade.DISCONNECTED);
            return interestList;
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            base.HandleNotification(notification);
            switch (notification.Name)
            {
                case GameFacade.CONNECTED:
					TransitionToState(mLoginState);
                    break;
                case GameFacade.LOGIN_REQUEST:
                    // TODO: add a "Connecting" state
                    break;
                case GameFacade.LOGIN_SUCCESS:
					if (this.CurrentState.GetType() != mConnectedState.GetType())
					{
						UserProperties userProperties = (UserProperties)notification.Body;
						GameFacade.Instance.RegisterProxy(new UserAccountProxy(userProperties));
						TransitionToState(mConnectedState);
					}
                    break;
				case GameFacade.DISCONNECTED:
					TransitionToState(mDisconnectedState);
					break;
                case GameFacade.LOGIN_FAILED:
                    // TODO: don't disconnect, but go back to login retry
                    TransitionToState(mDisconnectedState);
                    break;
            }
        }

		public override void OnRemove()
		{
			base.OnRemove();
			GameFacade.Instance.RemoveProxy(typeof(UserAccountProxy).Name);
		}

		//public override IState TransitionToState(string state)
		//{
		//    Debug.Log("Connection Statemachine: " + state);
		//    return base.TransitionToState(state);
		//}
	}
}
