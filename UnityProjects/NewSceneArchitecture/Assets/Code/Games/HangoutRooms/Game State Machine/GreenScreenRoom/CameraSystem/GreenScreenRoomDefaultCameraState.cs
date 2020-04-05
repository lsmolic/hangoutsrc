using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Hangout.Client
{
	public class GreenScreenRoomDefaultCameraState : State
	{
		private Transform mCameraTargetTransform;
		
		private FixedAngleFollowView mFixedAngleFollowView = null;

		private Vector3 AVATAR_TRANSFORM_OFFSET = new Vector3(0.0f, 1.1f, 0.0f);
		
		private BoxContainer mBoxContainer;

		public GreenScreenRoomDefaultCameraState(Transform cameraTargetTransform)
		{
			mCameraTargetTransform = cameraTargetTransform;
		}

		public override void EnterState()
		{
            //Console.WriteLine("GreenScreenRoomDefaultCameraState.EnterState");
			AvatarEntity localAvatar = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity;

			//TODO: Get rid of these hardcoded values, have them be set up by the room object or the avatar object.
			Vector3 position = new Vector3(0.0f, 2.35f, 6.15f);
			Vector3 scale = new Vector3(2.0f, 0.4f, 0.5f);
			mBoxContainer = new BoxContainer(position, scale);

			mFixedAngleFollowView = new FixedAngleFollowView(mCameraTargetTransform, localAvatar.UnityGameObject.transform, new Vector3(0.0f, 180.0f, 0.0f), mBoxContainer);
			mFixedAngleFollowView.TargetOffset = AVATAR_TRANSFORM_OFFSET;
            mFixedAngleFollowView.minFollowDistance = 1.0f;

			mFixedAngleFollowView.StartTracking();
		}

		public override void ExitState()
		{
            //Console.WriteLine("GreenScreenRoomDefaultCameraState.ExitState");
            mBoxContainer.Dispose();
			mFixedAngleFollowView.StopTracking();
		}
	}
}