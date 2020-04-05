/**  --------------------------------------------------------  *
 *   TweakablesPrototype.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Shared;

public class TweakablePrototype : AppEntry 
{
	[Tweakable("Float Test")]
	private float mTweakableFloat = 0.0f;
	
	[Tweakable("Color Test")]
	private Color mTweakableColor = Color.black;
		
	private TweakablesHandler mTweakablesHandler;
	
	protected override void Awake() 
	{
		base.Awake();
		
		mTweakablesHandler = new TweakablesHandler("resources://Tweaks/TweakTest.tweaks", this);
		IgnoreUnusedVariableWarning( mTweakablesHandler );
		
		GameObject ball = GameObject.CreatePrimitive( PrimitiveType.Sphere );
		ball.transform.position = Vector3.zero;
		
		GameObject cam = new GameObject("Cam");
		cam.AddComponent(typeof(Camera));
		cam.transform.position = new Vector3(100.0f, 40.0f, 10.0f);
		cam.transform.LookAt(Vector3.zero);
		
		GameObject light = new GameObject("Light");
		(light.AddComponent(typeof(Light)) as Light).range = 200.0f;
		light.transform.position = cam.transform.position + (UnityEngine.Random.onUnitSphere * 10.0f);
		
		this.Scheduler.StartCoroutine( KeepBallUpdatedWithTweakValues(ball) );
	}

	
	private IEnumerator<IYieldInstruction> KeepBallUpdatedWithTweakValues(GameObject ball)
	{
		while(true)
		{
			ball.renderer.material.color = mTweakableColor;
			ball.transform.localScale = Vector3.one * mTweakableFloat;
			
			yield return new YieldUntilNextFrame();
		}
	}
}

