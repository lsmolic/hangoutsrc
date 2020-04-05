using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Hangout.Shared.UnitTest;

namespace Hangout.Client
{
    public class RunSystemUnitTests
    {

        public int RunTests(string testResultDir)
        {
            bool result = false;

            try
            {
                if (String.IsNullOrEmpty(testResultDir))
                {
                    testResultDir = Environment.CurrentDirectory ;
                }

                ITestReporter[] iTestReporter = new ITestReporter[2]
                {
		            new ConsoleTestReporter(), 
			        new NUnitXmlTestReporter(new FileInfo(testResultDir + "/TestResult.xml"), "HangoutGoNoTestSuite")
		        };

                MulticastTestReporter multicastTestReporter = new MulticastTestReporter(iTestReporter);

                result = UnitTest.RunTestsInThisAppDomain(multicastTestReporter);
            }

            catch (ReflectionTypeLoadException ex)
            {
                string message = "Unable to load assembly\n";
                foreach (Exception e in ex.LoaderExceptions)
                {
                    message += "Type Loader Exception:\n";
                    message += e;
                    message += "\n";
                }
                Debugger.Log(0, null, ex.ToString());
                Console.Error.WriteLine(ex.ToString());
                Debugger.Log(0, null, message);
                Console.Error.WriteLine(message);
            }
            catch (System.Exception ex)
            {
                Debugger.Log(0, null, "\n\n");
                string error = ex.Message + "\n\nExample Usage: UnitestConsole.exe -assembly ../AssemblyPath/Assembly.dll -assembly /Another/Assembly.dll\n";
                Debugger.Log(0, null, error);
                Console.Error.WriteLine(error);
            }

            return result ? 0 : 1;
        }
    }
}

