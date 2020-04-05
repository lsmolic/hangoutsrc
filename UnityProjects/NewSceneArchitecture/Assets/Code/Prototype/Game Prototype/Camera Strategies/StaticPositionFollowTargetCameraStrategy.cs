/**  --------------------------------------------------------  *
 *   StaticPositionFollowTargetCameraStrategy\.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/01/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public class StaticPositionFollowTargetCameraStrategy : ICameraStrategy 
	{

		private ICameraManager mCameraManager;
		private Camera mCamera;
		private Transform mLookAtTransform;
		private Transform mStaticPositionTransform;
		private ITask mTransitionFixedUpdateTask;
		private ITask mFixedUpdateTask;

		public StaticPositionFollowTargetCameraStrategy( ICameraManager iCameraManager, Camera inCamera, Transform staticPosition, Transform dynamicLookAtTransform ) 
		{
			mCameraManager = iCameraManager;
			mCamera = inCamera;
			mStaticPositionTransform = staticPosition;
			mLookAtTransform = dynamicLookAtTransform;		
		}

		public void Activate() 
		{
			mTransitionFixedUpdateTask = mCameraManager.StartCoroutine( TransitionUpdateTask() );
		}

		private IEnumerator<IYieldInstruction> CameraFixedUpdate() 
		{
			while(true)
			{
				yield return new YieldUntilFixedUpdate();
				mCamera.transform.rotation = Quaternion.LookRotation( mLookAtTransform.transform.position - mCamera.transform.position );
			}
		}

		public IEnumerator<IYieldInstruction> TransitionUpdateTask() 
		{
			float lerpTime = 0.0f;
			while(true) 
			{
				yield return new YieldUntilFixedUpdate();
				Vector3 Distance = mStaticPositionTransform.position - mCamera.transform.position;
				if ( Distance.sqrMagnitude > 0.01f ) 
				{
					mCamera.transform.position = Vector3.Lerp( mCamera.transform.position, mStaticPositionTransform.transform.position, lerpTime );
					lerpTime += Time.fixedDeltaTime;
				}
				else
				{
					mTransitionFixedUpdateTask.Exit();
					mFixedUpdateTask = mCameraManager.StartCoroutine( CameraFixedUpdate() );
				}
			}
		}

		public void Deactivate() 
		{
			mTransitionFixedUpdateTask.Exit();
			mFixedUpdateTask.Exit();
		}	
		
		public Camera GetCamera()
		{
			return mCamera;
		}
	}
	
}
