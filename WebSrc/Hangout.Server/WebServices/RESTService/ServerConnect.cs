using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Hangout.Shared;
using System.Configuration;

namespace Hangout.Server.WebServices
{
    public class ServiceConnect
    {
		private ServicesLog mServiceLog = new ServicesLog("ServiceConnect");
        // Server
        public bool Listen()
        {
			string port = ConfigurationManager.AppSettings["RemotingListenPort"];
            //string port = WebConfig.RemotingListenPort;
            bool conneted = false;
            try
            {
                BinaryClientFormatterSinkProvider clientProvider = null;
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                IDictionary props = new Hashtable();
                props["port"] = port;
                props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                TcpChannel channel = new TcpChannel(props, clientProvider, serverProvider);

                ChannelServices.RegisterChannel(channel, false);

                Console.WriteLine("Listening for Remote Service on Port:{0}", props["port"]);
                conneted = true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error Listening for Remote Service on Port:{0} Error:{1}", port, ex.Message);
            }

            return conneted;
        }

        public bool RegisterWellKnownServiceType()
        {
            bool registered = false;
            try
            {
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RESTServiceHandler), "RestService", WellKnownObjectMode.SingleCall);
                Console.WriteLine("ServiceConnect RegisterWellKnownServiceType ServiceHandler");
                registered = true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("ServiceConnect RegisterWellKnownServiceType ServiceHandler Error:{1}", ex.Message);
            }

            return registered;
        }

    }


    public class RESTServiceHandler : MarshalByRefObject, IServiceInterface
    {
		private ServicesLog mServiceLog = new ServicesLog("RESTServiceHandler");

        public string ProcessRestService(RESTCommand restCommand)
        {
			mServiceLog.Log.Debug("ProcessRestService (Remoting ProcessRequest) noun:(" + restCommand.Noun + ") verb:(" + restCommand.Verb +")");

            RESTParser parser = new RESTParser();

            string response = parser.ProcessRestService(restCommand);

            Console.WriteLine("ProcessRestService RESTCommand Message Rcv: {0}", restCommand.Noun);

            return response;
        }
   }

}