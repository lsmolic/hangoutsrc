/**  --------------------------------------------------------  *
 *   GameObjectUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/05/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public static class GameObjectUtility
	{
		/// <summary>
		/// Searches the entire hierarchy under root for the name
		/// </summary>
		public static GameObject GetNamedChildRecursive(string name, GameObject root)
		{
			GameObject result = null;
			foreach (Transform child in root.transform)
			{
				if (child.gameObject.name == name)
				{
					result = child.gameObject;
					break;
				}
				else
				{
					GameObject childResult = GetNamedChildRecursive(name, child.gameObject);
					if (childResult != null)
					{
						result = childResult;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Gets a GameObject that's a direct child of the parent object
		/// </summary>
		public static GameObject GetNamedChild(string name, GameObject parent)
		{
			GameObject result = null;
			foreach (Transform child in parent.transform)
			{
				if (child.gameObject.name == name)
				{
					result = child.gameObject;
					break;
				}
			}
			return result;
		}
	}
}