/**  --------------------------------------------------------  *
 *   FashionCameraMediator.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client;

namespace Hangout.Client.FashionGame
{
	public class FashionCameraMediator : Mediator
	{
		private Camera mCamera;
		public Camera Camera
		{
			get { return mCamera; }
		}

		public void SetupCamera(Vector3 position, Quaternion rotation, float fov)
		{
			mCamera.transform.position = position;
			mCamera.transform.rotation = rotation;
			mCamera.fov = fov;
		}

		public override void OnRegister()
		{
			base.OnRegister();

			//mCamera = ComponentUtility.NewGameObject<Camera>("Game Camera");
			// Bug Workaround for the perspective bug... start with the camera in the scene
			GameObject camObj = GameObject.FindWithTag("MainCamera");
			if (camObj == null)
			{
				throw new Exception("Cannot find the MainCamera");
			}

			mCamera = (Camera)camObj.GetComponent(typeof(Camera));
		}
	}
}
