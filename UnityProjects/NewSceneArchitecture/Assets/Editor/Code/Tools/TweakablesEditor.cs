/**  --------------------------------------------------------  *
 *   TweakablesEditor.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.IO;
using System.Xml;
using Hangout.Client;
using UnityEditor;
using UnityEngine;

public class TweakablesEditor : EditorWindow 
{
	[MenuItem("Hangout.net/Tweakable Editor")]
	public static void OpenTweakablesEditor() 
	{
		(EditorWindow.GetWindow(typeof(TweakablesEditor))).Show(true);
	}
	
	private XmlDocument mTargetDoc;
	private FileInfo mInspectedFile = null;
	
	private void OnSelectionChange() 
	{
		Repaint();
	}

	private void OnGUI() 
	{
		foreach( UnityEngine.Object obj in Selection.GetFiltered(typeof(TextAsset), SelectionMode.Unfiltered) ) 
		{
			if( obj is TextAsset ) 
			{
				mInspectedFile = new FileInfo( Application.dataPath + AssetDatabase.GetAssetPath(obj).Substring("Assets".Length) );
				try 
				{
					mTargetDoc = new XmlDocument(); 
					mTargetDoc.PreserveWhitespace = true;
					mTargetDoc.Load(mInspectedFile.FullName);
					break;
				} 
				catch (XmlException) 
				{
					// The file isn't XML, we can ignore it
					GUILayout.Label("Selection is not an XML file.");
					return;
				}
			}
		}
		
		GUILayout.Space(4.0f);
		
		if( mTargetDoc == null ) 
		{
			GUILayout.Label("Selection is not a XML file.");
			return;
		}
		
		if( GUILayout.Button(mInspectedFile.Name) )
		{
			if( Application.platform == RuntimePlatform.WindowsEditor )
			{
				System.Diagnostics.Process.Start("start", mInspectedFile.FullName);
			}
			else
			{
				System.Diagnostics.Process.Start("open", mInspectedFile.FullName);
			}
		}
		
		// Select all the nodes in the file
		bool updateFile = false;
		foreach( XmlNode node in mTargetDoc.SelectNodes("//Tweakable") ) 
		{
			GUILayout.Space(12.0f);
			if( node.Attributes["type"] != null ) 
			{
				bool nodeValueChanged = false;
				
				switch( node.Attributes["type"].InnerText ) 
				{
					case "FloatSlider":
						nodeValueChanged = DrawFloatSlider(node);
						break;
					case "Text":
						nodeValueChanged = DrawTextbox(node);
						break;
					case "Color":
						nodeValueChanged = DrawColorField(node);
						break;
				}
				
				if( nodeValueChanged ) 
				{
					updateFile = true;
				}
			} 
			else 
			{
				GUILayout.Label("Error: Node (" + node.Name + ") is missing a type attribute. (Ex. type=\"FloatSlider\")");
			}
		}
		
		if( updateFile ) 
		{
			mTargetDoc.Save(mInspectedFile.FullName);
		}
	}
	
	
	private static bool DrawColorField(XmlNode node) 
	{
		if( node.Attributes["name"] == null ) 
		{
			GUILayout.Label("Error: There is no name associated with Text");
			return false;
		}
		
		string name = node.Attributes["name"].InnerText;
		Color value = Color.black;
		
		if( node.Attributes["value"] == null ) 
		{
			GUILayout.Label("Error: There is no value associated with Text (" + name + ")");
			return false;
		}
		
		try 
		{
			value = ColorUtility.HexToColor(node.Attributes["value"].InnerText);
		} 
		catch( FormatException ) 
		{
			GUILayout.Label("Error: Cannot convert hex value (" + node.Attributes["value"].InnerText + ") to a color.");
			return false;
		}
		
		EditorGUILayout.PrefixLabel(name);
		Color newValue = EditorGUILayout.ColorField(value);
		if( newValue.r != value.r ||
			newValue.g != value.g ||
			newValue.b != value.b ||
			newValue.a != value.a ) 
		{
			node.Attributes["value"].InnerText = ColorUtility.ColorToHex(newValue);
			return true;
		}
		
		return false;
	}
	
	private static bool DrawTextbox(XmlNode node) 
	{
		if( node.Attributes["name"] == null ) 
		{
			GUILayout.Label("Error: There is no name associated with Text");
			return false;
		}
		
		string name = node.Attributes["name"].InnerText;
		string value = "";
		
		if( node.Attributes["value"] == null ) 
		{
			GUILayout.Label("Error: There is no value associated with Text (" + name + ")");
			return false;
		}
		
		value = node.Attributes["value"].InnerText;
		EditorGUILayout.PrefixLabel(name);
		string newValue = EditorGUILayout.TextField(value);
		if( newValue != value ) 
		{
			node.Attributes["value"].InnerText = newValue.ToString();
			return true;
		}
		
		return false;
	}
	
	
	private static bool DrawFloatSlider(XmlNode node) 
	{
		if( node.Attributes["name"] == null ) 
		{
			GUILayout.Label("Error: There is no name associated with FloatSlider");
			return false;
		}
		
		string name = node.Attributes["name"].InnerText;
		float rangeLow = 0.0f;
		float rangeHigh = 1.0f;
		float value = 0.0f;
		
		if( node.Attributes["value"] == null ) 
		{
			GUILayout.Label("Error: There is no value associated with FloatSlider (" + name + ")");
			return false;
		}
		
		try 
		{
			value = float.Parse(node.Attributes["value"].InnerText);
		} 
		catch( FormatException ) 
		{
			GUILayout.Label("Error: Unable to parse value data associated with FloatSlider (" + name + "), " + node.Attributes["value"].InnerText + " is not a recognized numerical value.");
			return false;
		}
		
		if( node.Attributes["range"] != null ) 
		{
			string[] lowHigh = node.Attributes["range"].InnerText.Split(' ');
			if( lowHigh.Length != 2 ) 
			{
				GUILayout.Label("Error: Range data on FloatSlider (" + name + ") is in an invalid format, should be in the form 'range=\"0 1\".");
				return false;
			}
			
			try 
			{
				rangeLow = float.Parse(lowHigh[0]);
				rangeHigh = float.Parse(lowHigh[1]);
			} 
			catch (FormatException) 
			{
				GUILayout.Label("Error: Range data on FloatSlider (" + name + ") must contain only 2 valid numbers seperated by a space.");
				return false;
			}
		}
			
		GUILayout.Label(name);
		float newValue = EditorGUILayout.Slider(value, rangeLow, rangeHigh);
		if( newValue != value ) 
		{
			node.Attributes["value"].InnerText = newValue.ToString();
			return true;
		}
		
		return false;
	}
}
