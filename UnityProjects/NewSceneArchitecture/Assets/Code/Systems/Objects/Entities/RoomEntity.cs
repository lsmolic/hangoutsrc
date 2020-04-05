/**  --------------------------------------------------------  *
 *   RoomEntity.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class RoomEntity : IEntity
	{
		private GameObject mGameObject;
		
		public RoomEntity ( GameObject gameObject )
		{
			mGameObject =  gameObject;
		}
		
		public GameObject UnityGameObject
		{
			get { return mGameObject; }
		}

		public void Dispose()
		{
			GameObject.Destroy(mGameObject);
			mGameObject = null;
		}
	}	
}
