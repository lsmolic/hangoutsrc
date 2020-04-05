/*
	Created by Vilas Tewari on 2009-07-20.
   
	NameTag3D is an Object that creates a Text3D object and a Billboard background
	and can animate them to be visible or hidden
	
	The NameTag3D always looks at the camera
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public class NameTag3D : IDisposable
	{
		private readonly IScheduler mScheduler;

		private Vector3 mFollowOffset;
		private readonly Transform mFollowTransform;

		private float mScale = 0.06f;
		private bool mVisible = false;

		private Billboard mBackground;
		private Text3D mName;

		public bool Visible
		{
			get { return mVisible;  }
			set
			{
				mVisible = value;
				mName.Visible = value;
				mBackground.Visible = value;
			}
		}
		public string DisplayString
		{
			set { mName.DisplayString = value; }
			get { return mName.DisplayString; }
		}
		public Vector3 Offset
		{
			set { mFollowOffset = value; }
			get { return mFollowOffset; }
		}
		public float Scale
		{
			set { mScale = value; }
			get { return mScale; }
		}

		public void SetGameObjectsParents(GameObject parent)
		{
			mBackground.Transform.parent = parent.transform;
			mName.Transform.parent = parent.transform;
		}

		public NameTag3D(string displayString, Color displayColor, Font displayFont, Transform followTransform, IScheduler scheduler)
		{
			if( displayFont == null )
			{
				throw new ArgumentNullException("displayFont");
			}
			if (followTransform == null)
			{
				throw new ArgumentNullException("followTransform");
			}
			if (scheduler == null)
			{
				throw new ArgumentNullException("scheduler");
			}

			mScheduler = scheduler;

			mFollowTransform = followTransform;

			//	Create a Text3D Object for the name
			mName = new Text3D(displayFont);
			mName.Visible = false;
			mName.Scale = new Vector3(mScale, mScale, mScale);
			mName.Color = Color.white;
			DisplayString = displayString;


			//	Create a Billboard Object for background
			mBackground = new Billboard();
			mBackground.Visible = false;
			mBackground.Color = displayColor;
			mBackground.SetTexture((Texture2D)Resources.Load("GUI/TopLevel/frame_white_5px_slice"));
			mUpdateTagPositionTask = mScheduler.StartCoroutine(UpdateTagPosition());
		}

		private ITask mUpdateTagPositionTask = null;

		public IEnumerator<IYieldInstruction> UpdateTagPosition()
		{
			while (true)
			{
				Vector3 newCenter = mFollowTransform.position + mFollowOffset;
				Bounds textBounds = mName.Bounds;

				// Set Scale
				mBackground.XPartScale(Vector3.Scale((mName.Scale + textBounds.extents * 0.9f), new Vector3(1f, 0.7f, 1f)));
				// mBackground.Scale = (mName.Scale + textBounds.extents);

				// Set Position
				mName.Center = newCenter;
				Transform bBoard = mBackground.Transform;
				bBoard.position = newCenter + (newCenter - Camera.main.transform.position).normalized * 0.05f;

				// Set Rotation for background
				Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, Camera.main.transform.position - mName.Center);
				Vector3 eulerRotation = newRotation.eulerAngles;
				eulerRotation.z = 0.0f;
				bBoard.rotation = Quaternion.Euler(eulerRotation);

				// Set Rotation for Name
				Quaternion nameRotation = Quaternion.FromToRotation(Vector3.forward, mName.Center - Camera.main.transform.position);
				eulerRotation = nameRotation.eulerAngles;
				eulerRotation.z = 0;
				mName.Rotation = Quaternion.Euler(eulerRotation);

				yield return new YieldUntilNextFrame();
			}
		}

		public void Dispose()
		{
			mUpdateTagPositionTask.Exit();

			mName.Dispose();
			mName = null;

			mBackground.Dispose();
			mBackground = null;
		}
	}
}