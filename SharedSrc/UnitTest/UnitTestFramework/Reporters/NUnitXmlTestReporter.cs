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
using System.Xml;
using System.IO;

namespace Hangout.Shared.UnitTest
{
	/// <summary>
	/// NUnitXmlTestReporter will emulate the output of an NUnit test run. Useful for integration.
	/// </summary>
	public class NUnitXmlTestReporter : ITestReporter
	{
		private readonly string mSuiteName;
		private readonly FileInfo mOutFile;

		public NUnitXmlTestReporter(FileInfo outFile, string suiteName)
		{
			mSuiteName = suiteName;
			mOutFile = outFile;
		}

		private static void AddAttribute(XmlNode node, string attributeName, string attributeText)
		{
			XmlAttribute newAttribute = node.OwnerDocument.CreateAttribute(attributeName);
			newAttribute.InnerText = attributeText;
			node.Attributes.Append(newAttribute);
		}

		public void Report(Test[] passedTests, Test[] failedTests, Test[] longTests, float totalRuntime)
		{
			XmlDocument doc = new XmlDocument();
			
			XmlNode testResultsNode  = doc.CreateElement("test-results");
			doc.AppendChild(testResultsNode);
			
			XmlNode testSuiteNode = doc.CreateElement("test-suite");
			AddAttribute(testSuiteNode, "executed", "True");
			AddAttribute(testSuiteNode, "success", failedTests.Length == 0 ? "True" : "False");
			AddAttribute(testSuiteNode, "time", totalRuntime.ToString("f3"));
			AddAttribute(testSuiteNode, "asserts", failedTests.Length.ToString());
			testResultsNode.AppendChild(testSuiteNode);

			XmlNode resultsNode = doc.CreateElement("results");
			testSuiteNode.AppendChild(resultsNode);

			foreach (Test passedTest in passedTests)
			{
				XmlNode passedTestNode = doc.CreateElement("test-case");
				AddAttribute(passedTestNode, "name", passedTest.Name);
				AddAttribute(passedTestNode, "executed", "True");
				AddAttribute(passedTestNode, "success", "True");
				AddAttribute(passedTestNode, "time", passedTest.ExecutionTime.ToString("f3"));
				resultsNode.AppendChild(passedTestNode);
			}

			foreach (Test failedTest in failedTests)
			{
				XmlNode failedTestNode = doc.CreateElement("test-case");

				AddAttribute(failedTestNode, "name", failedTest.Name);
				AddAttribute(failedTestNode, "executed", "True");
				AddAttribute(failedTestNode, "success", "False");
				AddAttribute(failedTestNode, "time", failedTest.ExecutionTime.ToString("f3"));

				XmlNode failureNode = doc.CreateElement("failure");
				
				XmlNode messageNode = doc.CreateElement("message");
				messageNode.InnerText = failedTest.FailedException.Message;
				failureNode.AppendChild(messageNode);

				XmlNode stackTraceNode = doc.CreateElement("stack-trace");
				stackTraceNode.InnerText = failedTest.FailedException.StackTrace;
				failureNode.AppendChild(stackTraceNode);

				failedTestNode.AppendChild(failureNode);

				resultsNode.AppendChild(failedTestNode);
			}

			doc.Save(mOutFile.FullName);
		}
	}
}
