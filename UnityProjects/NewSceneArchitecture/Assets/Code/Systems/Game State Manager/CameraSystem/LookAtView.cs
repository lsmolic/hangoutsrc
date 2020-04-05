using UnityEngine;
using System.Collections;

using Hangout.Shared;

namespace Hangout.Client
{
	public class LookAtView : View
	{
		public Vector3 targetOffset = Vector3.zero;
		public bool offsetRelativeToTarget = false;

		private Vector3 mStartPosition;
		public Vector3 StartPosition
		{
			get { return mStartPosition; }
		}
		
		public Vector3 TargetOffset
		{
			get { return targetOffset; }
			set { targetOffset = value; }
		}
		
		public LookAtView(Transform cameraTargetTransform, Transform target, ViewContainer container)
			: base (cameraTargetTransform, target, container)
		{
			mStartPosition = cameraTargetTransform.position;
		}

		public LookAtView(Transform cameraTargetTransform, Transform target, Vector3 position, ViewContainer container)
			: base(cameraTargetTransform, target, container)
		{
			mStartPosition = position;
		}
		
		protected override Vector3 CalculatePosition()
		{
			return CameraTargetTransform.position;
		}
		
		protected override Quaternion CalculateRotation()
		{
			return Quaternion.LookRotation(lookAtPoint - this.Position, Vector3.up);
		}
		
		// Override this function to modify / bias the View's lookAtPoint
		protected override Vector3 CalculateLookAtPoint()
		{
			if ( offsetRelativeToTarget )
			{
				return mTarget.position + mTarget.TransformDirection(targetOffset);
			}
			else
			{
				return mTarget.position + targetOffset;
			}
		}

		public override ITask StartTracking()
		{
			CameraTargetTransform.position = mStartPosition;
			return base.StartTracking();
		}
	}
}
