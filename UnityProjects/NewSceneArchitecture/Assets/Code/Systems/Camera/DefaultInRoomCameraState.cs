using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Hangout.Client
{
	public class DefaultInRoomCameraState : State
	{	


		public DefaultInRoomCameraState(Transform cameraTransform)
		{	
			//mCameraTargetTransform = cameraTransform;			
		}
		
		public override void EnterState()
		{

		}
		
		public override void ExitState()
		{
			//Debug.Log("DefaultInroomCameraState: ExitState");
			//mFixedAngleFollowView.StopTracking();
		}
	}
}
