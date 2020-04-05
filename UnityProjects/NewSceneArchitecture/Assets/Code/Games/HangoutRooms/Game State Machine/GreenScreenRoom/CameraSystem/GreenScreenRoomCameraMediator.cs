/* Pherg 10/13/09 */

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using PureMVC.Patterns;
using Hangout.Shared;

namespace Hangout.Client
{
	public class GreenScreenRoomCameraMediator : Mediator, ICameraManager
	{
		private Transform mCameraTargetTransform;
		public Transform CameraTargetTransform
		{
			get { return mCameraTargetTransform; }
		}

		private Vector3 mCameraVelocity = Vector3.zero;
		private Vector3 mAngularVelocity = Vector3.zero;

		private float mTranslateSmoothTime = 0.6f;
		private float mRotateSmoothTime = 0.6f;

		private IScheduler mScheduler = null;

		private GameObject mMainCamera;
		public GameObject MainCamera
		{
			get { return mMainCamera; }
		}

		private ITask mCameraUpdateTask;
		
		public GreenScreenRoomCameraMediator()
		{
			mMainCamera = GameObject.FindWithTag("MainCamera");
			if (mMainCamera == null)
			{
				throw new Exception("Cannot find MainCamera in scene");
			}
			// Set viewport to fit inside topbar and navbar.
			mMainCamera.camera.pixelRect = new Rect(10, 40, 740, 420);
			mMainCamera.camera.fieldOfView = 30.0f;
			BuildMainCamera();
		}	
		
		public override void OnRegister()
		{
			base.OnRegister();
			// Start from the target position and rotation, but in a little closer so we lerp out with a smooth entrance effect
			mMainCamera.transform.position = new Vector3(0.0f, 2.0f, 3.0f);
			mMainCamera.transform.rotation = Quaternion.Euler(10.0f, 180.0f, 0.0f);
			mCameraUpdateTask = GameFacade.Instance.RetrieveMediator<GreenScreenRoomCameraMediator>().StartCoroutine(AnimateTo(mCameraTargetTransform));
			GameFacade.Instance.RegisterMediator(new GreenScreenRoomCameraStateMachine(mMainCamera, mCameraTargetTransform));
		}
		
		public override void OnRemove()
		{
			base.OnRemove();
			if (mCameraUpdateTask != null)
			{
				mCameraUpdateTask.Exit();
			}
			GameObject.Destroy(mCameraTargetTransform.gameObject);

			GameFacade.Instance.RemoveMediator(typeof(GreenScreenRoomCameraStateMachine).Name);
		}

		private void BuildMainCamera()
		{
			mMainCamera.camera.fieldOfView = 30.0f;
			//TODO: Remove these hardcoded values, have a transform attached to the room which defines
			//the default camera state for the room.
			mCameraTargetTransform = new GameObject("Camera Target Transform").transform;
			// Give the target the correct starting position so it does not jerk the maincamera around
			mCameraTargetTransform.position = new Vector3(0.0f, 2.0f, 5.9f);
			mCameraTargetTransform.rotation = Quaternion.Euler(10.0f, 180.0f, 0.0f);
		}

		public ITask StartCoroutine(IEnumerator<IYieldInstruction> task)
		{
			if (mScheduler == null)
			{
				mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			}
			return mScheduler.StartCoroutine(task);
		}

		public IEnumerator<IYieldInstruction> AnimateTo(Transform cameraTargetTransform)
		{
			while (true)
			{
				mMainCamera.transform.position = SmoothDampPosition(mMainCamera.transform.position, cameraTargetTransform.position);
				mMainCamera.transform.rotation = SmoothDampRotation(mMainCamera.transform.rotation, cameraTargetTransform.rotation);
				yield return new YieldUntilFixedUpdate();
			}
		}

		/*
			Calculate new Position, Rotation
		*/
		private Vector3 SmoothDampPosition(Vector3 current, Vector3 target)
		{
			Vector3 result = Vector3.zero;
			float cameraVelocityHelperX = mCameraVelocity.x;
			float cameraVelocityHelperY = mCameraVelocity.y;
			float cameraVelocityHelperZ = mCameraVelocity.z;

			result.x = Mathf.SmoothDamp(current.x, target.x, ref cameraVelocityHelperX, mTranslateSmoothTime);
			result.y = Mathf.SmoothDamp(current.y, target.y, ref cameraVelocityHelperY, mTranslateSmoothTime);
			result.z = Mathf.SmoothDamp(current.z, target.z, ref cameraVelocityHelperZ, mTranslateSmoothTime);

			mCameraVelocity.x = cameraVelocityHelperX;
			mCameraVelocity.y = cameraVelocityHelperY;
			mCameraVelocity.z = cameraVelocityHelperZ;

			return result;
		}
		private Quaternion SmoothDampRotation(Quaternion current, Quaternion target)
		{
			Vector3 result = Vector3.zero;
			Vector3 currentEuler = current.eulerAngles;
			Vector3 targetEuler = target.eulerAngles;

			float cameraAngularVelocityX = mAngularVelocity.x;
			float cameraAngularVelocityY = mAngularVelocity.y;
			float cameraAngularVelocityZ = mAngularVelocity.z;

			result.x = Mathf.SmoothDampAngle(currentEuler.x, targetEuler.x, ref cameraAngularVelocityX, mRotateSmoothTime);
			result.y = Mathf.SmoothDampAngle(currentEuler.y, targetEuler.y, ref cameraAngularVelocityY, mRotateSmoothTime);
			result.z = Mathf.SmoothDampAngle(currentEuler.z, targetEuler.z, ref cameraAngularVelocityZ, mRotateSmoothTime);

			mAngularVelocity.x = cameraAngularVelocityX;
			mAngularVelocity.y = cameraAngularVelocityY;
			mAngularVelocity.z = cameraAngularVelocityZ;

			return Quaternion.Euler(result.x, result.y, result.z);
		}
	}
}
