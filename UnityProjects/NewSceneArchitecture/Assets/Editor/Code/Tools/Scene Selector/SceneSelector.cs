/**  --------------------------------------------------------  *
 *   SceneSelector.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/11/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEditor;
using UnityEngine;


public static class SceneSelector 
{
	private static void LoadScene<T>() where T : AppEntry 
	{
		EditorApplication.NewScene();
		GameObject entryObject = new GameObject("Entry Point");
		entryObject.AddComponent(typeof(T));
	}
	
	[MenuItem("Scene/Hangout %H")]
	public static void LoadHangoutScene() 
	{
		LoadScene<SceneInit>();
	}
	
	[MenuItem("Scene/Prototype/Tweakables")]
	public static void LoadTweakablesPrototype() 
	{
		LoadScene<TweakablePrototype>();
	}
	
	[MenuItem("Scene/Tools/GUI Builder")]
	public static void LoadGuiBuilder() 
	{
		LoadScene<GuiBuilder>();
	}
}
