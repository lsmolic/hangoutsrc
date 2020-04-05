/**  --------------------------------------------------------  *
 *   CollisionUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public static class CollisionUtility
	{
		public static IReceipt ListenForCollisions(Rigidbody rigidbody, Action<UnityEngine.Collision> onCollide)
		{
			CollisionUtilityMonoBehaviour cumb = (CollisionUtilityMonoBehaviour)rigidbody.gameObject.AddComponent(typeof(CollisionUtilityMonoBehaviour));
			cumb.OnCollide = onCollide;
			
			return new Receipt(delegate()
			{
				Component.Destroy(rigidbody.gameObject.GetComponent(typeof(CollisionUtilityMonoBehaviour)));
			});
		}
	}
}