/**  --------------------------------------------------------  *
 *   FloatFromPosition.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class FloatFromPosition : IGuiPosition
	{
		private readonly GuiAnchor mAnchor;
		private Vector2 mCurrentPosition; // In screen space
		private readonly Hangout.Shared.Action mOnCompleteCallback;

		public FloatFromPosition(IScheduler scheduler, Camera camera, Vector3 startPosition, GuiAnchor anchor, float floatSpeed, float floatTime, Hangout.Shared.Action onCompleteCallback)
		{
			if (camera == null)
			{
				throw new ArgumentNullException("camera");
			}
			if( scheduler == null )
			{
				throw new ArgumentNullException("scheduler");
			}

			mAnchor = anchor;

			scheduler.StartCoroutine(Animate(startPosition, camera, floatSpeed, floatTime));
			mOnCompleteCallback = onCompleteCallback;
		}

		public FloatFromPosition(IScheduler scheduler, Camera camera, Vector3 startPosition, float floatSpeed, float floatTime, Hangout.Shared.Action onCompleteCallback)
			: this(scheduler, camera, startPosition, null, floatSpeed, floatTime, onCompleteCallback)
		{
		}

		private IEnumerator<IYieldInstruction> Animate(Vector3 startPosition, Camera camera, float speed, float time)
		{
			Vector3 position = camera.WorldToScreenPoint(startPosition);
			mCurrentPosition = new Vector2(position.x, Screen.height - position.y);
			
			for(float t = 0.0f; t < time; t += Time.deltaTime)
			{
				yield return new YieldUntilNextFrame();
				mCurrentPosition.y -= speed * Time.deltaTime;
			}

			yield return new YieldUntilNextFrame();
			if( mOnCompleteCallback != null )
			{
				mOnCompleteCallback();
			}
		}

		public void UpdatePosition(IGuiElement e, Vector2 position)
		{
		}

		public Vector2 GetPosition(IGuiElement e)
		{
			Vector2 result = mCurrentPosition;
			if (mAnchor != null)
			{
				result -= mAnchor.OffsetFromTopLeft(e.ExternalSize);
			}
	
			return result;
		}
	}
}