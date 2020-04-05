/**  --------------------------------------------------------  *
 *   AppEntry.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/09/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Shared;

/*
 * AppEntry is the connection point between our code and the Unity MonoBehavior system.
 */
public abstract class AppEntry : MonoBehaviour 
{
	private IScheduler mScheduler;
	protected IScheduler Scheduler
	{
		get { return mScheduler; }
	}
	
	protected virtual void Awake() 
	{
        UnityEngine.Application.runInBackground = true;
        
        GameObject schedulerGameObject = new GameObject();
		schedulerGameObject.name = "Scheduler";
		mScheduler = schedulerGameObject.AddComponent(typeof(Scheduler)) as Scheduler;
		schedulerGameObject.transform.parent = this.transform;
	}
	
	protected void IgnoreUnusedVariableWarning(object o) {}
}
