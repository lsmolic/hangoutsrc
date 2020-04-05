
using UnityEngine;

namespace Hangout.Client
{
	public class ShoppingCameraState : State
	{
		private StaticCameraStrategy mStaticCameraStrategy;

		private Transform mCameraTargetTransform;
		
		//Shopping view variables.
		private float mBodyViewDistance = 3.75f;
		private float mFaceViewDistance = 1.69f;
		private float mBodyViewHeight = 0.9f;
		private float mFaceViewHeight = 1.5f;
		private float mBodyViewRightAdjust = 1.12f;
		private float mFaceViewRightAdjust = 0.64f;
		
		public ShoppingCameraState(Transform cameraTargetTransform)
		{
			mCameraTargetTransform = cameraTargetTransform;
		}
		
		public override void EnterState()
		{

			mStaticCameraStrategy = new StaticCameraStrategy(mCameraTargetTransform);
			mStaticCameraStrategy.Activate();

			BodyView();
		}

		public void BodyView()
		{
			Transform avatarTransform = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity.UnityGameObject.transform;
			
			//Project the forward in the XZ plane.
			Vector3 flattenedForward = mCameraTargetTransform.forward;
			flattenedForward.y = 0.0f;
			flattenedForward.Normalize();
			
			//Quaternion in the XZ.
			Quaternion xzRotation = Quaternion.LookRotation(flattenedForward, Vector3.up);

			Vector3 avatarTargetPosition = avatarTransform.position;
			avatarTargetPosition.y += mBodyViewHeight;
			
			//Target position in front of avatar.
			Vector3 targetPosition = avatarTargetPosition - (flattenedForward * mBodyViewDistance);
			
			//Adjust to the right so the Avatar isn't covered by gui.
			targetPosition += mCameraTargetTransform.right * mBodyViewRightAdjust;

			mStaticCameraStrategy.MoveCamera(targetPosition, xzRotation);
		}

		public void FaceView()
		{
			Transform avatarTransform = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity.UnityGameObject.transform;

			//Project the forward in the XZ plane.
			Vector3 flattenedForward = mCameraTargetTransform.forward;
			flattenedForward.y = 0.0f;
			flattenedForward.Normalize();

			//Quaternion in the XZ.
			Quaternion xzRotation = Quaternion.LookRotation(flattenedForward, Vector3.up);

			Vector3 avatarTargetPosition = avatarTransform.position;
			avatarTargetPosition.y += mFaceViewHeight;

			//Target position in front of avatar.
			Vector3 targetPosition = avatarTargetPosition - (flattenedForward * mFaceViewDistance);

			//Adjust to the right so the Avatar isn't covered by gui.
			targetPosition += mCameraTargetTransform.right * mFaceViewRightAdjust;

			mStaticCameraStrategy.MoveCamera(targetPosition, xzRotation);
		}

		public override void ExitState()
		{
			mStaticCameraStrategy.Deactivate();
		}
	}
}
