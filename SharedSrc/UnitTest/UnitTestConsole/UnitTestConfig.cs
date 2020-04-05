using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Hangout.Server.StateServer
{
    public static class StateServerConfig
    {
        public static readonly string WebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
		public static readonly string RestServicesUrl = ConfigurationSettings.AppSettings["RemotingServicesUrl"];
        public static readonly string PaymentItemsServicesUrl = ConfigurationSettings.AppSettings["PaymentItemsServicesUrl"];

    }
}
