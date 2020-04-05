/**  --------------------------------------------------------  *
 *   Unitest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/08/2007
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;


namespace Hangout.Shared.UnitTest
{
	/// <summary>
	/// Unit testing framework for Unity
	/// </summary>
	public static class UnitTest
	{
		private const float mLongExecutionTimeThreshold = 0.1f; /// Time (in seconds) before a test is allowed to run before it gets reported as 'slow'

		public static bool HasTestFixture(System.Type t)
		{
			foreach (Attribute attrib in Attribute.GetCustomAttributes(t))
			{
				if (attrib is TestFixture)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Run the tests on a given System.Type that is tagged with [TestFixture]
		/// </summary>
		/// <param name="fixtureType">The System.Type that will be iterated over for [Test] methods</param>
		/// <param name="passedTests">Passed tests are appended to this list</param>
		/// <param name="failedTests">Failed tests are appended to this list</param>
		/// <param name="longTests">Tests that took longer than mLongExecutionTimeThreshold seconds to complete (pass or fail) are appended to this list</param>
		/// <returns>Time to run tests in the fixture</returns>
		private static float RunTestsInFixture(System.Type fixtureType, List<Test> passedTests, List<Test> failedTests, List<Test> longTests)
		{
			float runtime = 0.0f;

			// Scan for [Test] methods in the fixture
			foreach (MethodInfo testMethodInfo in fixtureType.GetMethods())
			{
				// Each of the attributes on this method
				foreach (Attribute attrib in Attribute.GetCustomAttributes(testMethodInfo))
				{

					// If this attribute is a Test, run the method
					if (attrib is Test)
					{
						Test test = (Test)attrib;
						test.ClassName = fixtureType.Name;
						test.FunctionName = testMethodInfo.Name;

						System.Object[] args = new System.Object[0];	// Test is run without arguments
						System.Object fixture = null;
						try
						{
							List<System.Type> interfaces = new List<System.Type>(fixtureType.GetInterfaces());
							if (interfaces.Contains(typeof(IDisposable)))
							{
								using (IDisposable dFixture = (IDisposable)Activator.CreateInstance(fixtureType))
								{
									test.Start();
									testMethodInfo.Invoke(dFixture, args);	// If nothing throws; Test Passed
								}
							}
							else
							{
								// Instantiate the fixture
								fixture = Activator.CreateInstance(fixtureType);
								test.Start();
								testMethodInfo.Invoke(fixture, args);	// If nothing throws; Test Passed
							}
							passedTests.Add(test);
						}
						catch (System.Exception e)
						{
							test.FailedException = e;
							failedTests.Add(test);
						}
						finally
						{
							// Always end the test
							test.End();

							// Kill the fixture
							fixture = null;

							// Report if the test took too long to complete
							if (test.ExecutionTime > mLongExecutionTimeThreshold)
							{
								longTests.Add(test);
							}

							runtime += test.ExecutionTime;

							// Force Garbage Collection to reduce side effects
							//  between different test calls.
							System.GC.Collect();
							System.GC.WaitForPendingFinalizers();
						}
					}
				}
			}
			return runtime;
		}

		/// <summary>
		/// Run all the tests in this assembly and send the results to the given ITestReporter
		/// </summary>
		/// <param name="testReporter">ITestReporter to report the test results to</param>
		/// <returns></returns>
		public static bool RunTestsInThisAppDomain(ITestReporter testReporter)
		{
			List<System.Type> fixtures = new List<System.Type>();

			foreach( Assembly assy in AppDomain.CurrentDomain.GetAssemblies() )
			{
				// don't scan through the assemblies from the GAC, they don't have [TestFixture]s
				if( !assy.GlobalAssemblyCache )
				{
					fixtures.AddRange(GetFixturesFromAssembly(assy));
				}
			}

			return RunTestFixtures(testReporter, fixtures);
		}

		public static List<System.Type> GetFixturesFromAssembly(Assembly assy)
		{
			List<System.Type> result = new List<Type>();
			foreach (System.Type type in assy.GetTypes())
			{
				if (HasTestFixture(type))
				{
					result.Add(type);
				}
			}
			return result;
		}

		public static bool RunTestsInAssembly(ITestReporter testReporter, Assembly assy)
		{
			return RunTestFixtures(testReporter, GetFixturesFromAssembly(assy));
		}

		public static bool RunTestsInAssemblies(ITestReporter testReporter, IEnumerable<Assembly> assemblies)
		{
			List<System.Type> fixtureTypes = new List<System.Type>();
			foreach( Assembly assy in assemblies )
			{
				fixtureTypes.AddRange(GetFixturesFromAssembly(assy));
			}

			return RunTestFixtures(testReporter, fixtureTypes);
		}


		public static bool RunTestsInAssemblies(ITestReporter testReporter, IEnumerable<FileInfo> assemblyPaths)
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach (FileInfo assyPath in assemblyPaths)
			{
                Assembly a = Assembly.LoadFrom(assyPath.FullName);
				assemblies.Add(a);
			}

			return RunTestsInAssemblies(testReporter, assemblies);
		}

		public static bool RunTestsInAssembly(ITestReporter testReporter, FileInfo assemblyPath)
		{
			Assembly assy = Assembly.LoadFrom(assemblyPath.FullName);
			return RunTestsInAssembly(testReporter, assy);
		}

		public static bool RunTestFixtures(ITestReporter testReporter, List<System.Type> fixtures)
		{
			if( testReporter == null )
			{
				throw new ArgumentNullException("testReporter");
			}

			float totalRuntime = 0.0f;

			List<Test> passedTests = new List<Test>();
			List<Test> failedTests = new List<Test>();
			List<Test> longTests = new List<Test>();

			// Run all the tests and gather the results
			foreach (System.Type type in fixtures)
			{
				totalRuntime += RunTestsInFixture(type, passedTests, failedTests, longTests);
			}
			
			testReporter.Report(passedTests.ToArray(), failedTests.ToArray(), longTests.ToArray(), totalRuntime);

			return failedTests.Count == 0;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class TestFixture : System.Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class Test : System.Attribute
	{
		private DateTime mStartTime;
		private DateTime mEndTime;

		public void Start()
		{
			mStartTime = DateTime.Now;
		}
		public void End()
		{
			mEndTime = DateTime.Now;
		}
		public float ExecutionTime
		{
			get { return ((float)(mEndTime - mStartTime).Ticks) / 10000000.0f; }
		}

		private Exception mFailedException = null;
		public Exception FailedException
		{
			get { return mFailedException; }
			set { mFailedException = value; }
		}

		private string mClassName = null;
		public string ClassName
		{
			get { return mClassName; }
			set { mClassName = value; }
		}

		private string mFunctionName = null;
		public string FunctionName
		{
			get { return mFunctionName; }
			set { mFunctionName = value;  }
		}

		public string Name
		{
			get { return mClassName + "." + mFunctionName; }
		}
	}
}