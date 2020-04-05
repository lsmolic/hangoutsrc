using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;
using System.Configuration;


namespace Hangout.Server
{
    class Program
    {
        private const int MIN_WORK_THREADS = 5;
        private const int MAX_WORK_THREADS = 250;
        private const int MIN_IOCP_THREADS = 5;
        private const int MAX_IOCP_THREADS = 1000;
        private const int SERVICE_CONNECTION_LIMIT = 100;

        private static ILog mLogger = null;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            mLogger = LogManager.GetLogger("Test");
            RunGlobalSetup();

            mLogger.Debug("Sending Requests Async: " + ConfigurationSettings.AppSettings["SendRequestsAsync"]);

            EscrowServiceTest serviceTest = new EscrowServiceTest();

            ConsoleKey key;
            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Enter)
            {
                switch (key)
                {
                    case ConsoleKey.Enter:
                        break;
                }
            }
        }


        public static void RunGlobalSetup()
        {
            // Must increase max threads before min threads
            if (!ThreadPool.SetMaxThreads(MAX_WORK_THREADS, MAX_IOCP_THREADS))
            {
                throw new Exception("Couldn't set max threads");
            }

            if (!ThreadPool.SetMinThreads(MIN_WORK_THREADS, MIN_IOCP_THREADS))
            {
                throw new Exception("Couldn't set min threads");
            }

            // IAN - Potential bottleneck here, this limits the number of outstanding service request connections
            System.Net.ServicePointManager.DefaultConnectionLimit = SERVICE_CONNECTION_LIMIT;
        }
    }
}
