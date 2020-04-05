using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class FixedAngleFollowView : FollowView
	{
		public bool relativeToTarget = false;
		public Vector3 mAngle = Vector3.zero;
		
		/*
			Properties
		*/
		public bool RelativeToTarget
		{
			get { return relativeToTarget; }
			set { relativeToTarget = value; }
		}
		public Vector3 Angle
		{
			get { return mAngle; }
			set { mAngle = value; }
		}
		
		public FixedAngleFollowView(Transform cameraTargetTransform, Transform target, Vector3 angle, ViewContainer container)
			: base(cameraTargetTransform, target, container)
		 {
			mAngle = angle;
		 }
		
		// Override this function to modify / bias the View's position
		protected override Vector3 CalculatePosition()
		{
			Vector3 directionVector = Quaternion.Euler(mAngle) * Vector3.forward;
			directionVector = directionVector * minFollowDistance;
			
			if ( relativeToTarget )
			{
				return base.CalculatePosition() - (Target.rotation * directionVector);
			}
			else
			{
				return base.CalculatePosition() - directionVector;
			}
		}
	}
}