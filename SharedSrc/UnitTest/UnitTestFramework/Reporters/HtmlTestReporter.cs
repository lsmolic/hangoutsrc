/**  --------------------------------------------------------  *
 *   HtmlTestReporter.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Hangout.Shared.UnitTest
{
	public class HtmlTestReporter : ITestReporter
	{
		public enum Results
		{
			OpenInBrowserOSX,
			OpenInBrowserWin,
			DoNotOpen
		}
		private readonly DirectoryInfo mHtmlRootDirectory;
		private readonly Results mHandleResults;
		public HtmlTestReporter(DirectoryInfo htmlRoot, Results handleResults)
		{
			if( htmlRoot == null )
			{
				throw new ArgumentNullException("htmlRoot");
			}
			mHtmlRootDirectory = htmlRoot;
			mHandleResults = handleResults;
		}

		private string BuildFailedTestHtml(Test test)
		{
			StringBuilder result = new StringBuilder();
			result.Append("<font size=\"2\">Failed Test: ");
			result.Append(test.ClassName);
			result.Append(":");
			result.Append(test.FunctionName);
			result.Append("()");

			Exception e = test.FailedException;
			while(e.InnerException != null)
			{
				e = e.InnerException;
			}

			// Create the failed message if there is one
			if (e.Message != "")
			{
				result.Append("<br>  ");
			}
			result.Append(e.Message);

			// Create a MoreInfo file
			string moreInfoFilename = mHtmlRootDirectory.FullName + "UnitTestResults." + test.ClassName + "." + test.FunctionName + ".html";

			result.Append("<br><a href=\"file://");
			result.Append(moreInfoFilename);
			result.Append("\">Stack Trace</a>");

			if (File.Exists(moreInfoFilename))
			{
				File.Delete(moreInfoFilename);
			}

			StringBuilder moreInfoFileContents = new StringBuilder();
			moreInfoFileContents.Append("<html><p><font size=\"2\">");
			moreInfoFileContents.Append(FormatToHTML(FormatLinks(e.ToString())));
			moreInfoFileContents.Append("<br><a href=\"file://");
			moreInfoFileContents.Append(mHtmlRootDirectory.FullName + "UnitTestResults.html");
			moreInfoFileContents.Append("\"><br>Back</a></p></html>");

			using (StreamWriter sw = new StreamWriter(moreInfoFilename))
			{
				sw.Write(moreInfoFileContents.ToString());
			}

			result.Append("</font><br><hr><br>");

			return result.ToString();
		}
		public void Report(Test[] passedTests, Test[] failedTests, Test[] longTests, float totalRuntime)
		{
			if (!mHtmlRootDirectory.Exists)
			{
				mHtmlRootDirectory.Create();
			}

			string title = "";

			StringBuilder testResults = new StringBuilder();
			StringBuilder html = new StringBuilder();
			if (failedTests.Length > 0)
			{
				title = failedTests.Length + " of " + (passedTests.Length + failedTests.Length) + " tests failed:";
				testResults.Append("<h4><font color=red>");

				foreach (Test test in failedTests)
				{
					testResults.Append(BuildFailedTestHtml(test));
				}

				testResults.Append("</font></h4>");
			}
			else
			{
				title = "All " + passedTests.Length + " Tests Passed Successfully";
				testResults.Append("<h4><font color=green>");
				testResults.Append(title);
				testResults.Append("</font></h4>");
			}


			html.Append("<html><title>");
			html.Append(title);
			html.Append("</title><p><font size=\"2\">");
			html.Append(testResults.ToString());
			html.Append("Total test time: " + totalRuntime.ToString("f2") + " seconds");


			if (longTests.Length > 0)
			{
				html.Append("<br>Long tests:<br>");
				foreach (Test test in longTests)
				{
					html.Append("<t>");
					html.Append(test.ClassName);
					html.Append(":");
					html.Append(test.FunctionName);
					html.Append(" took ");
					html.Append(test.ExecutionTime.ToString("f2"));
					html.Append(" seconds<br>");
				}
			}
			html.Append("</font>");
			html.Append("</p></html>");


			string filename = mHtmlRootDirectory.FullName + "UnitTestResults.html";

			using (StreamWriter sw = new StreamWriter(filename))
			{
				sw.Write(html.ToString());
				sw.Flush();
			}

			if (mHandleResults == Results.OpenInBrowserOSX)
			{
				System.Diagnostics.Process.Start("open", mHtmlRootDirectory.FullName + "UnitTestResults.html");
			}
			else if(mHandleResults == Results.OpenInBrowserWin)
			{
				System.Diagnostics.Process.Start("explorer", mHtmlRootDirectory.FullName + "UnitTestResults.html");
			}
		}


		private static string FormatLinks(string s)
		{
			// This regex parses the example string:
			// 		at Assert.Fail (System.String FailMessage) [0x00135] in /Users/mortoc/HangoutIndustries/HangoutDev/Assets/Plugins/Assert.cs:69 
			// into:
			// 		group1:/Users/mortoc/HangoutIndustries/HangoutDev/Assets/Plugins/Assert.cs
			//		group2:69
			Regex rx = new Regex(@"in\s(?<1>\S+)\:(?<2>\d+)\s*$");

			string[] lines = s.Split('\n');
			StringBuilder result = new StringBuilder("");

			foreach (string line in lines)
			{
				Match m = rx.Match(line);
				if (m.Success)
				{
					result.Append("<p>");
					result.Append(line);
					result.Append("</p><br>");
				}
				else
				{
					result.Append(line);
				}
			}
			return result.ToString();
		}

		private static string FormatToHTML(string s)
		{
			Regex newline = new Regex("(\n|\r|\f)");
			return newline.Replace(s, "<br>");
		}
	}
}
