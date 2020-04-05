/* Pherg 10/13/09 */

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Hangout.Client
{
	public class GreenScreenRoomCameraStateMachine : StateMachine
	{		
		private GreenScreenRoomDefaultCameraState mGreenScreenRoomDefaultCameraState;
		private ShoppingCameraState mShoppingCameraState;
		private InactiveState mInactiveState;
		
		public GreenScreenRoomCameraStateMachine(GameObject mainCamera, Transform cameraTargetTransform)
		{
			mGreenScreenRoomDefaultCameraState = new GreenScreenRoomDefaultCameraState(cameraTargetTransform);
			mShoppingCameraState = new ShoppingCameraState(cameraTargetTransform);
			mInactiveState = new InactiveState();

			mInactiveState.AddTransition(mGreenScreenRoomDefaultCameraState);
			mGreenScreenRoomDefaultCameraState.AddTransition(mShoppingCameraState);
			mShoppingCameraState.AddTransition(mGreenScreenRoomDefaultCameraState);

			this.EnterInitialState(mInactiveState);
		}

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			return new string[]
			{ 
				GameFacade.SHOP_BODY_VIEW,
				GameFacade.SHOP_HEAD_VIEW,
			};
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);

			switch (notification.Name)
			{
				case GameFacade.SHOP_BODY_VIEW:
					if (this.CurrentState == mShoppingCameraState)
					{
						mShoppingCameraState.BodyView();
					}
					break;
				case GameFacade.SHOP_HEAD_VIEW:
					if (this.CurrentState == mShoppingCameraState)
					{
						mShoppingCameraState.FaceView();
					}
					break;
				default:
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}
		
		public override void OnRemove()
		{
			this.CurrentState.ExitState();
		}

		public void StartShoppingCamera()
		{
			if (this.CurrentState.CanGoToState(mShoppingCameraState.Name))
			{
				this.TransitionToState(mShoppingCameraState);
			}
		}

		public void StartDefaultInRoomCamera()
		{
			if (this.CurrentState.CanGoToState(mGreenScreenRoomDefaultCameraState.Name))
			{
				this.TransitionToState(mGreenScreenRoomDefaultCameraState);
			}
		}
	}
}
