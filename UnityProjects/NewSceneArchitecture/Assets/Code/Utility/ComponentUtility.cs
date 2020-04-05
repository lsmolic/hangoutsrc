/**  --------------------------------------------------------  *
 *   ComponentUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public static class ComponentUtility
	{
		/// Creates a new GameObject and Component of type T on it.
		public static T NewGameObject<T>(string gameObjectName) where T : UnityEngine.Component
		{
			GameObject go = new GameObject(gameObjectName);
			T result = go.AddComponent(typeof(T)) as T;
			return result;
		}
	}
}