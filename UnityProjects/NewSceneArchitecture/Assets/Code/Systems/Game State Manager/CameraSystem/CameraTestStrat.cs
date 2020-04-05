/**  --------------------------------------------------------  *
 *   CameraTestStrat.cs  
 *
 *   Author: , Hangout Industries
 *   Date: 06/29/2009
 *	 
 *   --------------------------------------------------------  *
 */


//Deprecated
/*
using UnityEngine;
using System;

public delegate void CameraActivatedCallback();
public delegate void CameraDeactivatedCallback();

public class CameraTestStrat : CameraStrategy {
	private ICameraManager mICameraManager;
	private Vector3 mDestinationPoint = new Vector3(0.0f, 10.0f, -200.0f);
	private ITask mTransitionTask;
	private bool m_strategyEnabled = false;
	private Camera mCamera;
	private float mSpeed = 5.0f;
	private CameraActivatedCallback mCameraActivatedCallback;
	private CameraDeactivatedCallback mCameraDeactivatedCallback;
	
	
	public CameraTestStrat() {}
	
	public override void Activate( ICameraManager iCameraManager, Camera camera ) {
		mICameraManager = iCameraManager;
		if ( iCameraManager == null ) {
			Console.LogError ("Camera manager passed into Default Camera constructor is null.");
		}
		mCamera = iCameraManager.GetCamera();
		m_strategyEnabled = true;
		RegisterTransitionTask();
	}
	
	public override void Deactivate() {
		if ( mCameraDeactivatedCallback != null ) {
			mCameraDeactivatedCallback();
		}
		UnregisterTransitionTask();
	}
	
	public override void TransitionUpdateTask() {
		Vector3 distance = mCamera.transform.position - mDestinationPoint;
		if ( distance.sqrMagnitude > 0.01f ) {
			mCamera.transform.position = Vector3.Lerp( mCamera.transform.position, mDestinationPoint, Time.deltaTime * mSpeed);
		} else {
			UnregisterTransitionTask();
			mICameraManager.PopStrategy();
		}
	}
	
	private void RegisterTransitionTask() {
		mTransitionTask = mICameraManager.AddFixedUpdateTask( TransitionUpdateTask );
	}
	
	public override void UnregisterTransitionTask() {
		mICameraManager.RemoveFixedUpdateTask( mTransitionTask );
	}
	
}
*/