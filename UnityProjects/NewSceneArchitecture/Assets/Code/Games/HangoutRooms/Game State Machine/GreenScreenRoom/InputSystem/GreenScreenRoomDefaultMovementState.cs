using System.Collections.Generic;
using Hangout.Client.Gui;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
	public class GreenScreenRoomDefaultMovementState : State
	{
		private Motor mMotor;

		private List<IReceipt> mInputReceipts = new List<IReceipt>();
		
		public override void EnterState()
		{
			SetupMovementInput();
		}

		public override void ExitState()
		{
			StopMovementInput();
		}

		private void SetupMovementInput()
		{
			InputManagerMediator inputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();
			LocalAvatarEntity localAvatar = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity;
			mMotor = new Motor(localAvatar);

			mInputReceipts.Add(inputManager.RegisterForButtonDown(KeyCode.Mouse0, ClickToWalk));
			mInputReceipts.Add(inputManager.RegisterForButtonDown(KeyCode.LeftArrow, MoveLeft));
			mInputReceipts.Add(inputManager.RegisterForButtonUp(KeyCode.LeftArrow, StopLeft));
			mInputReceipts.Add(inputManager.RegisterForButtonDown(KeyCode.RightArrow, MoveRight));
			mInputReceipts.Add(inputManager.RegisterForButtonUp(KeyCode.RightArrow, StopRight));
			mInputReceipts.Add(inputManager.RegisterForButtonDown(KeyCode.UpArrow, MoveUp));
			mInputReceipts.Add(inputManager.RegisterForButtonUp(KeyCode.UpArrow, StopUp));
			mInputReceipts.Add(inputManager.RegisterForButtonDown(KeyCode.DownArrow, MoveDown));
			mInputReceipts.Add(inputManager.RegisterForButtonUp(KeyCode.DownArrow, StopDown));
		}

		private void MoveLeft()
		{
			Transform cameraTransform = GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraMediator>().MainCamera.transform;
			mMotor.WalkLeftRelationalToTransform(cameraTransform);
		}

		private void MoveRight()
		{
			Transform cameraTransform = GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraMediator>().MainCamera.transform;
			mMotor.WalkRightRelationalToTransform(cameraTransform);
		}

		private void MoveUp()
		{
			Transform cameraTransform = GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraMediator>().MainCamera.transform;
			mMotor.WalkForwardRelationalToTransform(cameraTransform);
		}

		private void MoveDown()
		{
			Transform cameraTransform = GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraMediator>().MainCamera.transform;
			mMotor.WalkBackwardRelationalToTransform(cameraTransform);
		}
		
		private void StopLeft()
		{
			mMotor.StopMovementLeft();
		}

		private void StopRight()
		{
			mMotor.StopMovementRight();
		}

		private void StopUp()
		{
			mMotor.StopMovementForward();
		}

		private void StopDown()
		{
			mMotor.StopMovementBackward();
		}
		
		private void StopMovementInput()
		{
			List<IReceipt> receiptCopy = new List<IReceipt>(mInputReceipts);
			foreach (IReceipt receipt in receiptCopy)
			{
				receipt.Exit();
				mInputReceipts.Remove(receipt);
			}
			
			mMotor.Dispose();
		}

		private void ClickToWalk()
		{
			if (mMotor != null)
			{

				InputManagerMediator inputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();
				IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
				
				Vector3 mousePosition = inputManager.MousePosition;
				if(guiManager.OccludingTopLevels(mousePosition).Count == 0)
				{
					//ray cast down to where the user clicked and find what was clicked on...
					// Construct a ray from the current mouse coordinates
					Ray rayCastFromCameraIntoWorld = Camera.main.ScreenPointToRay(mousePosition);
					RaycastHit rayCastHit = new RaycastHit();
					
					if (Physics.Raycast(rayCastFromCameraIntoWorld, out rayCastHit, Mathf.Infinity, 1 << GameFacade.GROUND_LAYER))
					{
						if (Vector3.Dot(rayCastHit.normal, Vector3.up) > 0.95)
						{
							mMotor.AnimateCharacterToPoint(rayCastHit.point);
						}
					}
				}
			}
		}
	}
}
