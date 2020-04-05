/**  --------------------------------------------------------  *
 *   AnimationTesterEditor.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/21/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationTester))]
public class AnimationTesterEditor : Editor
{
	// 2.6 Change
	public override void OnInspectorGUI()
	//public override void OnInspectorGUI()
	{
		GUILayout.Label(target.name);
		Animation targetAnimation = ((Component)target).animation;
		foreach (AnimationState state in targetAnimation)
		{
			if (GUILayout.Button(state.clip.name))
			{
				targetAnimation.Stop();
				targetAnimation.Play(state.clip.name);
			}
		}
	}
}
