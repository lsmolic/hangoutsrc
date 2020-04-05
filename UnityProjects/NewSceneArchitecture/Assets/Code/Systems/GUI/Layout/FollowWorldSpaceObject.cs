/**  --------------------------------------------------------  *
 *   FollowWorldSpaceObject.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class FollowWorldSpaceObject : IGuiPosition
	{
		private Transform mTransform;
		private Vector2 mOffset;
		private Vector3 mWorldSpaceOffset;
		private Camera mCamera;
		private GuiAnchor mAnchor;
		private bool mDisposed = false;

		public FollowWorldSpaceObject(Camera camera, Transform worldSpaceObject, GuiAnchor widgetAnchor, Vector2 offset, Vector3 worldSpaceOffset)
		{
			if (camera == null)
			{
				throw new ArgumentNullException("camera");
			}
			if (worldSpaceObject == null)
			{
				throw new ArgumentNullException("worldSpaceObject");
			}
			mTransform = worldSpaceObject;
			mOffset = offset;
			mCamera = camera;
			mAnchor = widgetAnchor;
			mWorldSpaceOffset = worldSpaceOffset;
		}

		public FollowWorldSpaceObject(Camera camera, Transform worldSpaceObject)
			: this(camera, worldSpaceObject, null, Vector2.zero, Vector3.zero) { }


		public void UpdatePosition(IGuiElement e, Vector2 position)
		{
		}

		public void Dispose()
		{
			mDisposed = true;
		}

		public Vector2 GetPosition(IGuiElement e)
		{
			Vector2 result = Vector2.zero;
			if (!mDisposed)
			{
				Vector3 position = mCamera.WorldToScreenPoint(mTransform.position + mWorldSpaceOffset);
				result = new Vector2(position.x + mOffset.x, (Screen.height - position.y) + mOffset.y);
				if (mAnchor != null)
				{
					result -= mAnchor.OffsetFromTopLeft(e.ExternalSize);
				}
			}
			return result;
		}
	}
}