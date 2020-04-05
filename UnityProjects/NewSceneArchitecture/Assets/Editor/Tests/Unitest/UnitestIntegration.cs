using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
using System.IO;

using Hangout.Shared.UnitTest;
using System.Reflection;

public static class UnitestIntegration
{
	/// Run all the unit tests in this assembly.
	/// Return value is whether all the tests passed or not.
	[MenuItem("Unitest/Run Tests %u")]
	public static bool RunTests()
	{		
		DirectoryInfo outDir = new DirectoryInfo(Application.dataPath + "UnitestOutput");
		bool result = UnitTest.RunTestsInAssemblies
		(
			new HtmlTestReporter
			(
				outDir, 
				HtmlTestReporter.Results.OpenInBrowserWin
			),
			AppDomain.CurrentDomain.GetAssemblies()
		);
		return result;
	}
}
