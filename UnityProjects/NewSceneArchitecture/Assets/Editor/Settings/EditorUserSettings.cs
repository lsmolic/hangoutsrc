/**  --------------------------------------------------------  *
 *   EditorUserSettings.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Xml;
using System.IO;

public static class EditorUserSettings {
	public static void SaveToDisk() {
		// If mSettingsFileDoc is null, it means no settings have been changed
		if( mSettingsFileDoc != null ) {
			mSettingsFileDoc.Save(SettingsFileAbsolutePath);
		}
	}
	
	public static void CancelPendingChanges() {
		mSettingsFileDoc = null;
	}
	
	/// The settings file is in relation to 'UnityEngine.Application.dataPath'
	private const string mSettingsFile = "/../UserSettings.xml";

	public static string SettingsFile {
		get { return (new FileInfo(Application.dataPath + mSettingsFile)).FullName; }
	}
	
	private static XmlDocument mSettingsFileDoc = null;
	
	private static string SettingsFileAbsolutePath {
		get { return Application.dataPath + mSettingsFile; }
	}
	
	
	/// Gets a value from the settings file
	/// Returns empty string if setting isn't found
	/// Doesn't throw
	public static string GetSetting(string name) {
		XmlElement root = GetSettingsElement();
		XmlElement settingElement = (XmlElement)root.SelectSingleNode(name);
		
		string result = String.Empty;
		if( settingElement != null ) {
			result = settingElement.InnerText;
		}
		return result;
	}
	
	
	/// Sets a named element in the settings file
	/// Creates the element in the XML if it doesn't exist yet
	/// Doesn't throw
	public static void SetSetting(string name, string setting) {
		XmlElement root = GetSettingsElement();
		XmlElement settingElement = (XmlElement)root.SelectSingleNode(name);

		if( settingElement == null ) {
			settingElement = mSettingsFileDoc.CreateElement(name);
			root.AppendChild(settingElement);
		}
		settingElement.InnerText = setting;
	}
	
	
	/// Initializes mSettingsFileDoc and the "/settings" element if necessary and returns the settings element
	/// Doesn't throw
	private static XmlElement GetSettingsElement(){
		XmlElement settingsElement = null;
		if( mSettingsFileDoc == null ) {
			mSettingsFileDoc = new XmlDocument();
			
			if( File.Exists(SettingsFileAbsolutePath) ) {
				mSettingsFileDoc.Load(SettingsFileAbsolutePath);
			}
		}
		
		settingsElement = (XmlElement)mSettingsFileDoc.SelectSingleNode("/settings");
		if( settingsElement == null ) {
			settingsElement = mSettingsFileDoc.CreateElement("settings");
			mSettingsFileDoc.AppendChild(settingsElement);
		}
		
		return settingsElement;
	}
}



