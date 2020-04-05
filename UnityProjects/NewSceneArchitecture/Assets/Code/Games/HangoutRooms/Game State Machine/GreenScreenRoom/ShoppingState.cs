/*
 * A Pherg Joint.
 * 10/02/09
 */

using UnityEngine;

namespace Hangout.Client
{
	public class ShoppingState : State
	{		
		private AvatarMediator mAvatarMediator;
		public override void EnterState()
		{
			mAvatarMediator = GameFacade.Instance.RetrieveMediator<AvatarMediator>();
			
			GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraStateMachine>().StartShoppingCamera();
			GameFacade.Instance.RetrieveMediator<GreenScreenRoomInputStateMachine>().MovementDisabled();

            InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
            inventoryProxy.BeginShopping();

			Vector3 flattenedForward = GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraMediator>().MainCamera.transform.forward;
			flattenedForward.y = 0.0f;
			flattenedForward.Normalize();
			mAvatarMediator.LocalAvatarEntity.UnityGameObject.transform.rotation = Quaternion.LookRotation((-1.0f * flattenedForward), Vector3.up);
			mAvatarMediator.LocalAvatarEntity.Nametag.Visible = false;
			mAvatarMediator.HideForeignAvatars();
		}

		public override void ExitState()
		{
			mAvatarMediator.LocalAvatarEntity.Nametag.Visible = true;
			mAvatarMediator.ShowForeignAvatars();
			GameFacade.Instance.RetrieveProxy<InventoryProxy>().CloseInventory();
		}
	}
}
