using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Server;


namespace ServerToServicesTest
{
	class Program
	{
		static void Main(string[] args)
		{
			RunAllTests runAllTests = new RunAllTests();
			runAllTests.RunTests();

			int numberOfServicesCalled = WebServiceRequest.kNumberOfServicesCalled;
			int numberOfServicesCompleted = WebServiceRequest.kNumberOfServicesCompleted;

			Console.WriteLine("starting to run...");
			while (true)
			{
				if (Console.KeyAvailable && Console.ReadKey(false).Key == ConsoleKey.Enter)
				{
					break;
				}

				if (WebServiceRequest.kNumberOfServicesCalled > numberOfServicesCalled)
				{
					numberOfServicesCalled = WebServiceRequest.kNumberOfServicesCalled;
					Console.WriteLine("running " + WebServiceRequest.kNumberOfServicesCalled + " services");
				}

				if (WebServiceRequest.kNumberOfServicesCompleted > numberOfServicesCompleted)
				{
					numberOfServicesCompleted = WebServiceRequest.kNumberOfServicesCompleted;
					Console.WriteLine("completed " + WebServiceRequest.kNumberOfServicesCalled + " services");
				}

				if (WebServiceRequest.kNumberOfServicesCalled == WebServiceRequest.kNumberOfServicesCompleted)
				{
					break;
				}
			}

			Console.WriteLine("DONE: " + WebServiceRequest.kNumberOfServicesCalled + " ran services");
			Console.WriteLine("DONE: " + WebServiceRequest.kNumberOfServicesCompleted + " completed services");

			Console.WriteLine("hit enter to quit!");
			while (true)
			{
				if (Console.ReadKey(false).Key == ConsoleKey.Enter)
				{
					break;
				}
			}

		}
	}
}
