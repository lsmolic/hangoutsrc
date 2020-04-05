/**  --------------------------------------------------------  *
 *   Npc.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Interfaces;

using UnityEngine;

namespace Hangout.Client
{
	/// <summary>
	/// Non-Player Character. Contains the code to set up a character from a bunch of assets.
	/// </summary>
	public abstract class Npc : INpc
    {
		private HeadController mHeadController;
		public HeadController HeadController
		{
			get { return mHeadController; }
		}

		private BodyController mBodyController;
		public BodyController BodyController
		{
			get { return mBodyController; }
		}

		private GameObject mDisplayObject = null;
		public UnityEngine.GameObject DisplayObject
		{
			get { return mDisplayObject; }
		}

		private GameObject mRootObject = null;
		public UnityEngine.GameObject UnityGameObject
		{
			get { return mRootObject; }
		}

		private readonly string mName;
		public string Name
		{
			get { return mName; }
		}

		public abstract bool Active { get; }

		public Npc(string name, GameObject displayObject, HeadController headController, BodyController bodyController)
		{
			// TODO: Hard coded values
			float avatarHeight = 1.75f;
			float avatarCollisionRadius = 0.5f;

			if (displayObject == null)
			{
				throw new ArgumentNullException("displayObject");
			}
			mDisplayObject = displayObject;

			if (bodyController == null)
			{
				throw new ArgumentNullException("bodyController");
			}
			mHeadController = headController;

			if (headController == null)
			{
				throw new ArgumentNullException("headController");
			}
			mBodyController = bodyController;

			mName = name;

			mRootObject = new GameObject("Model");

			CapsuleCollider avatarCollider = mRootObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			avatarCollider.center = avatarHeight * 0.5f * Vector3.up;
			avatarCollider.height = avatarHeight;
			avatarCollider.radius = avatarCollisionRadius;

			mDisplayObject.transform.parent = mRootObject.transform;
			mDisplayObject.transform.localPosition = Vector3.zero;
		}

		public float DistanceTo(Vector3 point)
		{
			return (mRootObject.transform.position - point).magnitude;
		}

		public virtual void Dispose()
		{
			GameObject.Destroy(mDisplayObject);
			GameObject.Destroy(mRootObject);

			mHeadController.Dispose();
			mBodyController.Dispose();
		}
    }
}