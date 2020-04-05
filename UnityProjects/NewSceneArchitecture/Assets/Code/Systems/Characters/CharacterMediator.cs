/*
 * Mortoc wrote this on 9/29/09
 */

using UnityEngine; 

using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using PureMVC.Patterns;

using Hangout.Shared;

namespace Hangout.Client
{
	/// <summary>
	/// Abstract base for NpcMediator and AvatarMediator
	/// </summary>
	public abstract class CharacterMediator : Mediator
	{
		public static void AddAnimationClipToRig(GameObject rig, string pathToAnimation)
		{
			GameObject clipGameObject = GameObject.Instantiate(Resources.Load(pathToAnimation)) as GameObject;
			if (clipGameObject == null)
			{
				throw new Exception("Unable to instantiate game obect from path : " +pathToAnimation);
			}
			
			if (clipGameObject.animation == null)
			{
				throw new Exception("Instantiated game object has no animation component.");
			}
			
			AnimationClip clip = clipGameObject.animation.clip;
			if (clip == null)
			{
				throw new Exception("Unable to get clip from gameobject animation component.");
			}
			
			rig.animation.AddClip(clip, clip.name);

			GameObject.Destroy(clipGameObject);
		}
	}
}
