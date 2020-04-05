using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Client
{
	public class MovementTutorialState : TutorialState
	{
		private List<IReceipt> mInputReceipts = new List<IReceipt>();
		
		private IInputManager mInputManager;
		
		private ITask mTutorialCompleteTask = null;
	
		private float mWaitForTutorialComplete = 2.0f;
		
		public MovementTutorialState(GreenScreenRoomTutorialGui tutorialGui, Action<TutorialState> tutorialCompletedCallback)
			: base(tutorialGui, tutorialCompletedCallback)
		{
		}
		
		public override void EnterState()
		{
			GreenScreenRoomTutorialGui.OpenMovementPopup();
			mTutorialCompleteTask = null;
			mInputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();

			mInputReceipts.Add(mInputManager.RegisterForButtonDown(KeyCode.Mouse0, CheckIfMouseComplete));
			mInputReceipts.Add(mInputManager.RegisterForButtonDown(KeyCode.UpArrow, TutorialCompleteWait));
			mInputReceipts.Add(mInputManager.RegisterForButtonDown(KeyCode.DownArrow, TutorialCompleteWait));
			mInputReceipts.Add(mInputManager.RegisterForButtonDown(KeyCode.LeftArrow, TutorialCompleteWait));
			mInputReceipts.Add(mInputManager.RegisterForButtonDown(KeyCode.RightArrow, TutorialCompleteWait));
		}
		
		private void CheckIfMouseComplete()
		{
			IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			
			Vector3 mousePosition = mInputManager.MousePosition;
			
			if(guiManager.OccludingTopLevels(mousePosition).Count == 0)
			{
				//ray cast down to where the user clicked and find what was clicked on...
				// Construct a ray from the current mouse coordinates
				Ray rayCastFromCameraIntoWorld = Camera.main.ScreenPointToRay(mousePosition);
				RaycastHit rayCastHit = new RaycastHit();

				if (Physics.Raycast(rayCastFromCameraIntoWorld, out rayCastHit, Mathf.Infinity, 1 << GameFacade.GROUND_LAYER))
				{
					TutorialCompleteWait();
				}
			}
		}
		
		private void TutorialCompleteWait()
		{
			if (mTutorialCompleteTask == null)
			{
				mTutorialCompleteTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitThenComplete());
			}
		}
		
		private IEnumerator<IYieldInstruction> WaitThenComplete()
		{
			yield return new YieldForSeconds(mWaitForTutorialComplete);
			this.TutorialComplete();
		}
		
		public override void ExitState()
		{
			foreach (IReceipt receipt in mInputReceipts)
			{
				receipt.Exit();
			}
			mInputReceipts.Clear();
			base.ExitState();
		}

		public override void TutorialComplete()
		{
			this.UserAccountProxy.SetAccountProperty<bool>(UserAccountProperties.HasCompleteMoveTutorial, true);
			base.TutorialComplete();
		}
	}
}
