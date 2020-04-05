/**  --------------------------------------------------------  *
 *   NewScriptProcessor.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/12/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.IO;
using System.Text;
using UnityEditor;

public static class NewScriptProcessor 
{
	// Template file keywords
	private const string mAuthorKey = "{$AUTHOR}";
	private const string mDateKey = "{$DATE}";
	private const string mClassNameKey = "{$CLASS}";
	private const string mFileNameKey = "{$FILENAME}";

    public static void Import( FileInfo scriptFile ) 
	{
	
		if( scriptFile.Name != "NewBehaviourScript.cs" ) 
		{
			throw new ArgumentException("Import needs a path to a NewBehaviorScript.cs file", "scriptFilePath");
		}
		string[] scriptLines = File.ReadAllLines(scriptFile.FullName);
		scriptFile.Delete();

		string newScriptFilename = EditorUtility.SaveFilePanel("Enter Script Name", scriptFile.DirectoryName, "NewScript", "cs");
		if( String.IsNullOrEmpty(newScriptFilename) ) 
		{
			return;
		}
		
		FileInfo scriptFileInfo = new FileInfo( newScriptFilename );
		string className = scriptFileInfo.Name.Substring(0, scriptFileInfo.Name.Length - ".cs".Length);
		string author = EditorUserSettings.GetSetting("UserName");
		if( author == String.Empty ) 
		{
			author = "UserNameEmpty";
			EditorUserSettings.SetSetting("UserName", author);
			EditorUserSettings.SaveToDisk();
		}
		string date = DateTime.Now.ToShortDateString();

		StringBuilder processedScript = new StringBuilder();
		foreach( string line in scriptLines ) 
		{
			string newLine;
			newLine = line.Replace(mAuthorKey, author);
			newLine = newLine.Replace(mDateKey, date);
			newLine = newLine.Replace(mClassNameKey, className);
			newLine = newLine.Replace(mFileNameKey, scriptFileInfo.Name);

			processedScript.AppendLine(newLine);
		}

		File.WriteAllText(scriptFileInfo.FullName, processedScript.ToString());

		System.Threading.Thread.Sleep(100); // Give textmate some time to notice if the file's in a project
		System.Diagnostics.Process.Start("open", scriptFileInfo.FullName);
	}
}
