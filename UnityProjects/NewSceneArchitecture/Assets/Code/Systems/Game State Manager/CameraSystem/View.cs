/*
   Created by Vilas Tewari on 2009-07-26.
   
	A View is a class that positions and orients a transform relative to
	a target Transform
	
	A View can contrain its position to a container
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public abstract class View
	{
		public bool snapToView;
		public float translateLagMultiplier = 1f;	// Camera Movement characteritics
		public float rotateLagMultiplier = 1f;
		public ViewContainer mViewContainer; // What space are we contrained to

		private float mFieldOfView = 0.0f;
		public float FieldOfView
		{
			get { return mFieldOfView; }
			set { mFieldOfView = value; }
		}
		
		protected Transform mTarget; // What transform are we interested in
		protected Vector3 lookAtPoint = Vector3.zero; // What point do want to look at
		
		private ITask mTrackingTask;

		// Optimization
		private Transform mCameraTargetTransform;
		protected Transform CameraTargetTransform
		{
			get { return mCameraTargetTransform; }
		}

		/*
			Properties
		*/
		private Vector3 mPosition;
		public Vector3 Position
		{
			get { return mPosition; }
		}
		
		public Quaternion mRotation;
		public Quaternion Rotation
		{
			get { return mRotation; }
		}
		
		public Transform Target
		{
			get { return mTarget; }
			set { mTarget = value; }
		}
		public Vector3 LookAtPoint
		{
			get{ return lookAtPoint; }
		}
		public bool SnapToView
		{
			get{ return snapToView; }
			set{ snapToView = value; }
		}
		public float TranslateLag
		{
			get{ return translateLagMultiplier; }
			set{ translateLagMultiplier = value; }
		}
		public float RotateLag
		{
			get{ return rotateLagMultiplier; }
			set{ rotateLagMultiplier = value; }
		}
		
		public View(Transform cameraTargetTransform, Transform target, ViewContainer container)
		{
			mCameraTargetTransform = cameraTargetTransform;
			mViewContainer = container;
			mTarget = target;
		}

		// Tracking
		public virtual ITask StartTracking()
		{
			mTrackingTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(TrackTarget());
			return mTrackingTask;
		}

		public void StopTracking()
		{
			if (mTrackingTask != null)
			{
				mTrackingTask.Exit();
			}
		}
		
		// Caculate the point in space that we are looking at
		protected abstract Vector3 CalculateLookAtPoint();
		
		// funtion that will Calculate the view position
		protected abstract Vector3 CalculatePosition();

		// funtion that will Calculate the orientation as required
		protected abstract Quaternion CalculateRotation();
		
		// Coroutine that waits for Update()
		private IEnumerator<IYieldInstruction> TrackTarget()
		{
			while ( true ) 
			{
				lookAtPoint = CalculateLookAtPoint();
				
				if ( mViewContainer != null)
				{
					mPosition = mViewContainer.ConstrainToContainer(CalculatePosition());
				}
				else
				{
					mPosition = CalculatePosition();
				}
				mRotation = CalculateRotation();
				
				mCameraTargetTransform.position = mPosition;
				mCameraTargetTransform.rotation = mRotation;
				yield return new YieldUntilFixedUpdate();
			}
		}
	}
}