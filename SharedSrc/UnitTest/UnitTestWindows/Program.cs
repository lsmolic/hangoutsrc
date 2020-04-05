﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Hangout.Shared.UnitTest
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application
		/// </summary>
		[STAThread]
		static void Main()
		{
			DirectoryInfo di = new DirectoryInfo(System.IO.Path.GetTempPath());
			UnitTest.RunTestsInThisAppDomain(new HtmlTestReporter(di, HtmlTestReporter.Results.DoNotOpen));

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.Run(new UnitestMainWindow("file://" + di.FullName + "UnitTestResults.html"));
		}
	}
}
