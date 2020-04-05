using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Policy;
using Hangout.Shared;

using System.Xml;
using Hangout.Server;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace Hangout.Server.WebServices
{
    public class PaymentItemConnect
    {
        /// <summary>
        /// RegisterWellKnownServiceType "PaymentItems"
        /// </summary>
        /// <returns></returns>
        public bool RegisterWellKnownServiceType()
        {
            bool registered = false;
            try
            {
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(PaymentItem), "PaymentItems", WellKnownObjectMode.SingleCall);
                Console.WriteLine("PaymentItemConnect RegisterWellKnownServiceType PaymentItemHandler");
                registered = true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("PaymentItemConnect RegisterWellKnownServiceType Error:{1}", ex.Message);
            }

            return registered;
        }
    }
}