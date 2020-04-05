using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Hangout.Client
{
	public class SmoothDampTransform 
	{
		private Transform dampTransform;
		private float mTranslateSmoothTime;
		public float TranslateSmoothTime
		{
			set { mTranslateSmoothTime = value; }
		}
		private float mRotateSmoothTime;
		public float RotateSmoothTime
		{
			 set { mRotateSmoothTime = value; }
		}
		
		private Vector3 mVelocity = Vector3.zero;
		private Vector3 mAngularVelocity = Vector3.zero;
		
		public SmoothDampTransform( Transform subject, float tTime, float rTime )
		{
			dampTransform = subject;
			mTranslateSmoothTime = tTime;
			mRotateSmoothTime = rTime;
		}
		
		/*
			Smoothly animate the transform to target Position and Rotation
		*/
		public void AnimateTo( Vector3 targetPosition, Quaternion targetRotation )
		{
			dampTransform.position = SmoothDampPosition( dampTransform.position, targetPosition );
			dampTransform.rotation = SmoothDampRotation( dampTransform.rotation, targetRotation );
		}
		
		/*
			Snap the transform to the target Position and Rotation
		*/
		public void SnapTo( Vector3 targetPosition, Quaternion targetRotation )
		{
			dampTransform.position = targetPosition;
			dampTransform.rotation = targetRotation;
			mVelocity = Vector3.zero;
			mAngularVelocity = Vector3.zero;
		}
		
		public void SetAnimationProperties( float transSmoothTime, float rotSmoothTime )
		{
			mTranslateSmoothTime = transSmoothTime;
			mRotateSmoothTime = rotSmoothTime;
		}
		
		/*
			Calculate new Position, Rotation
		*/
		private Vector3 SmoothDampPosition( Vector3 current, Vector3 target )
		{
			Vector3 result = Vector3.zero;
			float cameraVelocityHelperX = mVelocity.x;
			float cameraVelocityHelperY = mVelocity.y;
			float cameraVelocityHelperZ = mVelocity.z;

			result.x = Mathf.SmoothDamp(current.x, target.x, ref cameraVelocityHelperX, mTranslateSmoothTime);
			result.y = Mathf.SmoothDamp(current.y, target.y, ref cameraVelocityHelperY, mTranslateSmoothTime);
			result.z = Mathf.SmoothDamp(current.z, target.z, ref cameraVelocityHelperZ, mTranslateSmoothTime);

			mVelocity.x = cameraVelocityHelperX;
			mVelocity.y = cameraVelocityHelperY;
			mVelocity.z = cameraVelocityHelperZ;

			return result;
		}
		private Quaternion SmoothDampRotation( Quaternion current, Quaternion target )
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