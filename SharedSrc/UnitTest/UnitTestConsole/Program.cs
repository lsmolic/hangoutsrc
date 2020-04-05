using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Hangout.Shared.UnitTest
{
	class Program
	{
		static int Main(string[] args)
		{

			bool result = false;
			
			try
			{
				if( args.Length == 0 )
				{
					throw new Exception("No arguments provided");
				}
				List<FileInfo> assemblyFiles = new List<FileInfo>();
				for (uint i = 0; i < args.Length; ++i)
				{
					if( args[i] != "-assembly" )
					{
						throw new Exception("Invalid argument " + args[i]);
					}
					else
					{
						++i;
						FileInfo assemblyFile = new FileInfo(args[i]);
						if( !assemblyFile.Exists )
						{
							throw new FileNotFoundException("Unable to find assembly at: " + assemblyFile.FullName);
						}
						assemblyFiles.Add(assemblyFile);
					}
				}

                result = UnitTest.RunTestsInAssemblies
				(
					new MulticastTestReporter
					(
						new ITestReporter[2]
						{
							new ConsoleTestReporter(), 
							new NUnitXmlTestReporter(new FileInfo(Environment.CurrentDirectory + "/TestResult.xml"), "HangoutTestSuite")
						}
					),
					assemblyFiles
				);
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
				string error = ex + "\n\nExample Usage: UnitestConsole.exe -assembly ../AssemblyPath/Assembly.dll -assembly /Another/Assembly.dll\n";
				Debugger.Log(0, null, error);
				Console.Error.WriteLine(error);
			}

			return result ? 0 : 1;
		}
	}
}
