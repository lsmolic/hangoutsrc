/*
   Created by Vilas Tewari on 2009-07-26.
*/


using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class FollowView : LookAtView
	{
		public float minFollowDistance = 1.0f;
		
		public float MinFollowDistance
		{
			get{ return minFollowDistance;}
			set{ minFollowDistance = value;}
		}

		public FollowView(Transform cameraTargetTransform, Transform target, ViewContainer container)
			: base(cameraTargetTransform, target, container)
		{
		}
		
		protected override Vector3 CalculatePosition()
		{
			return Vector3.Normalize(CameraTargetTransform.position - lookAtPoint) * minFollowDistance + lookAtPoint;
		}
	}
}
