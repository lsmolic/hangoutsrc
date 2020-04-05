/**  --------------------------------------------------------  *
 *   MulticastTestReporter.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/27/2009
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
	public class MulticastTestReporter : ITestReporter
	{
		private readonly IEnumerable<ITestReporter> mReporters;
		public MulticastTestReporter(IEnumerable<ITestReporter> reporters)
		{
			if (reporters == null)
			{
				throw new ArgumentNullException("reporters");
			}
			mReporters = reporters;
		}

		public void Report(Test[] passedTests, Test[] failedTests, Test[] longTests, float totalRuntime)
		{
			foreach (ITestReporter reporter in mReporters)
			{
				reporter.Report(passedTests, failedTests, longTests, totalRuntime);
			}
		}
	}
}
