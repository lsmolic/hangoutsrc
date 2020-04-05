/**  --------------------------------------------------------  *
 *   CameraManagerMediator.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/01/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns;
using Hangout.Shared;

namespace Hangout.Client
{
	public class CameraManagerMediator : Mediator, ICameraManager
	{
		private GameObject mMainCamera;
		public GameObject MainCamera
		{
			get { return mMainCamera; }
		}

		public override void OnRemove()
		{
			base.OnRemove();
		}

		public Camera GetMainCamera()
		{
			return mMainCamera.GetComponent(typeof(Camera)) as Camera;
		}

		public ITask StartCoroutine(IEnumerator<IYieldInstruction> task)
		{
			return GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(task);
		}

		public override void OnRegister()
		{
			base.OnRegister();

			mMainCamera = GameObject.FindWithTag("MainCamera");
			// Set viewport to fit inside topbar and navbar.
			mMainCamera.camera.pixelRect = new Rect(0,0,760,500);
			mMainCamera.camera.backgroundColor = Color.black;
			if (mMainCamera == null)
			{
				throw new Exception("Cannot find MainCamera in scene");
			}
		}
	}
}
