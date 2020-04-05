using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Hangout.Client
{
	public class StaticCameraStrategy : ICameraStrategy
	{		
		private Transform mCameraTargetTransform;
		
		public StaticCameraStrategy(Transform cameraTargetTransform)
		{			
			mCameraTargetTransform = cameraTargetTransform;
		}

		public void Activate()
		{
		}
		
		public void MoveCamera(Vector3 position, Quaternion lookAtRotation)
		{
			mCameraTargetTransform.position = position;
			mCameraTargetTransform.rotation = lookAtRotation;
		}
		
		public void Deactivate()
		{
		}
	}
}
