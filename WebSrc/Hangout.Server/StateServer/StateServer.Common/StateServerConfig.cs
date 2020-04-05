using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Threading;
using log4net;
using Hangout.Server;

namespace Hangout.Server.StateServer
{
    public static class StateServerConfig
    {
        public static readonly string ServerIp = ConfigurationSettings.AppSettings["IpAddress"];
        public static readonly string ServerPort = ConfigurationSettings.AppSettings["Port"];
        public static readonly string IsConnectionAllowed = ConfigurationSettings.AppSettings["IsConnectionAllowed"];
        public static readonly string WebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
		public static readonly string RestServicesUrl = ConfigurationSettings.AppSettings["RemotingServicesUrl"];
        public static readonly string PaymentItemsServicesUrl = ConfigurationSettings.AppSettings["PaymentItemsServicesUrl"];
        public static readonly string PaymentItemsInitialCoin = ConfigurationSettings.AppSettings["PaymentItemsInitialCoin"];
        public static readonly string PaymentItemsInitialCash = ConfigurationSettings.AppSettings["PaymentItemsInitialCash"];

        private const int MIN_WORK_THREADS = 5;
        private const int MAX_WORK_THREADS = 250;
        private const int MIN_IOCP_THREADS = 5;
        private const int MAX_IOCP_THREADS = 1000;
        private const int SERVICE_CONNECTION_LIMIT = 100;

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
