/* Pherg 10/13/09 */

using System;

namespace Hangout.Client
{
	public class GreenScreenRoomStateMachine : StateMachine
	{		
		private ShoppingState mShoppingState;
		private GreenScreenRoomDefaultState mGreenScreenRoomDefaultState;
		private InactiveState mInactiveState;

		public override void OnRegister()
		{
			base.OnRegister();

			GameFacade.Instance.RegisterMediator(new GreenScreenRoomTutorialStateMachine());
			GameFacade.Instance.RegisterMediator(new GreenScreenRoomCameraMediator());
			GameFacade.Instance.RegisterMediator(new GreenScreenRoomInputStateMachine());
			GameFacade.Instance.RegisterMediator(new SoundMediator());

			GameFacade.Instance.SendNotification(GameFacade.GREEN_SCREEN_ROOM_LOADED);
			this.TransitionToState(mGreenScreenRoomDefaultState);
		}

		public override void OnRemove()
		{
			GameFacade.Instance.RemoveMediator(typeof(GreenScreenRoomInputStateMachine).Name);
			GameFacade.Instance.RemoveMediator(typeof(GreenScreenRoomCameraMediator).Name);
			GameFacade.Instance.RemoveMediator(typeof(GreenScreenRoomTutorialStateMachine).Name);
			GameFacade.Instance.RemoveMediator(typeof(SoundMediator).Name);
			this.TransitionToState(mInactiveState);
			base.OnRemove();
		}

		public GreenScreenRoomStateMachine()
		{
			mShoppingState = new ShoppingState();
			mGreenScreenRoomDefaultState = new GreenScreenRoomDefaultState();
			mInactiveState = new InactiveState();
			
			mShoppingState.AddTransition(mGreenScreenRoomDefaultState);
			mShoppingState.AddTransition(mInactiveState);
			
			mGreenScreenRoomDefaultState.AddTransition(mShoppingState);
			mGreenScreenRoomDefaultState.AddTransition(mInactiveState);

			mInactiveState.AddTransition(mShoppingState);
			mInactiveState.AddTransition(mGreenScreenRoomDefaultState);

			this.EnterInitialState(mInactiveState);
		}

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			return new string[]
			{ 
				GameFacade.SHOP_BUTTON_CLICKED,
				GameFacade.SHOP_OPENED,
				GameFacade.SHOP_CLOSED,
				GameFacade.CLOSET_BUTTON_CLICKED,
				GameFacade.CLOSE_ALL_WINDOWS,
			};
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);

			switch (notification.Name)
			{
				case GameFacade.CLOSE_ALL_WINDOWS:
					GameFacade.Instance.RetrieveProxy<InventoryProxy>().CloseInventory();
					break;
				case GameFacade.SHOP_BUTTON_CLICKED:
					if (this.CurrentState != mShoppingState)
					{
						this.TransitionToState(mShoppingState);
					}
					else
					{
						this.TransitionToState(mGreenScreenRoomDefaultState);
					}
					break;					
				case GameFacade.SHOP_OPENED:
					if (this.CurrentState != mShoppingState)
					{
						this.TransitionToState(mShoppingState);
					}
					break;
				case GameFacade.SHOP_CLOSED:
					this.TransitionToState(mGreenScreenRoomDefaultState);
					break;
				case GameFacade.CLOSET_BUTTON_CLICKED:
					if (this.CurrentState != mShoppingState)
					{
						this.TransitionToState(mShoppingState);
						GameFacade.Instance.RetrieveProxy<InventoryProxy>().OpenPlayerInventory();
					}
					else
					{
                        GameFacade.Instance.RetrieveProxy<InventoryProxy>().TogglePlayerInventory();
					}
					break;
				default:
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}
		
		public void Deactivate()
		{
			this.TransitionToState(mInactiveState);
		}
	}
}
