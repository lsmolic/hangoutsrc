/**  --------------------------------------------------------  *
 *   ITestReporter.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared.UnitTest
{
	public interface ITestReporter
	{
		void Report(Test[] passedTests, Test[] failedTests, Test[] longTests, float totalTime);
	}
}
