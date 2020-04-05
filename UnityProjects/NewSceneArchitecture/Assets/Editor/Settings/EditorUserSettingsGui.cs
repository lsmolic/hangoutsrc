/**  --------------------------------------------------------  *
 *   EditorSettingsGui.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using UnityEditor;
using System;

public class EditorUserSettingsGui : EditorWindow {
	[MenuItem("Edit/User Settings...")]
	public static void EditorUserSettingsWindowSelected() {
        EditorUserSettingsGui window = new EditorUserSettingsGui();
        window.position = new Rect(10, 10, 250, 32);
		window.ShowUtility();
    }
	
	private void OnGUI() {
		DrawSettingGui("User Name", "UserName");
		GUILayout.FlexibleSpace();
		DrawSaveCancelButtons();
	}
	
	private void DrawSaveCancelButtons() {
		GUILayout.BeginHorizontal();
		try{
			GUILayout.FlexibleSpace();
			if( GUILayout.Button("Save", GUILayout.Width(64.0f)) ) {
				EditorUserSettings.SaveToDisk();
				this.Close();
			}
			GUILayout.Space(4.0f);
			if( GUILayout.Button("Cancel", GUILayout.Width(64.0f)) ) {
				EditorUserSettings.CancelPendingChanges();
				this.Close();
			}
		} finally {
			GUILayout.EndHorizontal();
		}
	}
	
	private void DrawSettingGui(string displayName, string settingName){
		GUILayout.BeginHorizontal();
		try {
			GUILayout.Label(displayName + ":");
			GUILayout.FlexibleSpace();
			
			string settingValue = EditorUserSettings.GetSetting(settingName);
			string guiResult = GUILayout.TextField(settingValue, GUILayout.Width(128.0f));
			if( guiResult != settingValue ) {
				EditorUserSettings.SetSetting(settingName, guiResult);
			}
		} finally {
			GUILayout.EndHorizontal();
		}
	}
}
