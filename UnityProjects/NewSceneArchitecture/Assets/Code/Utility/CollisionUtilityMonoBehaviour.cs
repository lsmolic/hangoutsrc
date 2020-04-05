/**  --------------------------------------------------------  *
 *   CollisionUtilityMonoBehaviour.cs  
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

/// <summary>
/// Created by CollisionUtility
/// </summary>

using Hangout.Client;

public class CollisionUtilityMonoBehaviour : MonoBehaviour
{
	private Action<Collision> mOnCollide;
	public Action<Collision> OnCollide
	{
		get { return mOnCollide; }
		set { mOnCollide = value; }
	}
	
	public void OnCollisionEnter(Collision collision)
	{
		try 
		{
			if (mOnCollide != null)
			{
				mOnCollide(collision);
			}
		}
		catch (System.Exception ex)
		{
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log(ex.ToString());
		}
	}
}
