/**  --------------------------------------------------------  *
 *   RunMockery.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/25/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hangout.Client;
using UnityEditor;
using UnityEngine;

public static class RunMockery
{
	[MenuItem("Hangout.net/Mockery/Unity DLLs")]
	public static void MockUnityDlls()
	{
		List<Assembly> assemblies = new List<Assembly>();
		foreach (Assembly assy in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assy.FullName.Contains("Unity"))
			{
				assemblies.Add(assy);
			}
		}

		StringBuilder sb = new StringBuilder();
		Mockery unityMockery = new Mockery(
			delegate(string[] output)
			{
				foreach (string s in output)
				{
					sb.AppendLine(s);
				}
			},
			assemblies
		);

		unityMockery.SaveMockery(Application.dataPath + "/../Mockery");
		Debug.Log(sb);

		System.Diagnostics.Process.Start("open", Application.dataPath + "/../Mockery");
	}
}
