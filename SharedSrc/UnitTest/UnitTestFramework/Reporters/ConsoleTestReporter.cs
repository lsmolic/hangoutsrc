/**  --------------------------------------------------------  *
 *   ConsoleTestReporter.cs  
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
	public class ConsoleTestReporter : ITestReporter
	{
		private void ReportTestFailed(Test test)
		{
			Write("Failed Test: ");
			Write(test.ClassName);
			Write(":");
			Write(test.FunctionName);
			Write("()");

			Exception e = test.FailedException;
			while(e.InnerException != null)
			{
				e = e.InnerException;
			}

			// Create the failed message if there is one
			if (e.Message != "")
			{
				WriteLine();
				WriteLine(e.Message);
			}

			WriteLine();
			WriteLine(e.ToString());
			WriteLine();
		}


		private void WriteLine(string line)
		{
			Console.WriteLine(line);
			System.Diagnostics.Debugger.Log(0, null, line + "\n");
		}

		private void WriteLine()
		{
			Console.WriteLine();
			System.Diagnostics.Debugger.Log(0, null, "\n");
		}

		private void Write(string text)
		{
			Console.Write(text);
			System.Diagnostics.Debugger.Log(0, null, text);
		}

		public void Report(Test[] passedTests, Test[] failedTests, Test[] longTests, float totalRuntime)
		{
			System.Diagnostics.Debugger.Log(0, null, "\n\n\n");
			if (failedTests.Length > 0)
			{
				WriteLine(failedTests.Length + " of " + (passedTests.Length + failedTests.Length) + " tests failed");
				
				foreach (Test test in failedTests)
				{
					ReportTestFailed(test);
				}
			}
			else
			{
				WriteLine("All " + passedTests.Length + " Tests Passed Successfully");
			}

			WriteLine("Total test time: " + totalRuntime.ToString("f2") + " seconds");

			if (longTests.Length > 0)
			{
				WriteLine("\nLong tests:");
				foreach (Test test in longTests)
				{
					Write("\t");
					Write(test.ClassName);
					Write(":");
					Write(test.FunctionName);
					Write(" took ");
					Write(test.ExecutionTime.ToString("f2"));
					Write(" seconds\n");
				}
			}
			System.Diagnostics.Debugger.Log(0, null, "\n\n\n");
		}
	}
}
