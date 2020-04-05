using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Hangout.Shared;
using Hangout.Server.StateServer;
using System.ComponentModel;

namespace Hangout.Server
{
    public class RemoteCallToService
    {
		private static CallToServiceCallbackManager mCallToServiceCallbackManager = new CallToServiceCallbackManager();
		public static CallToServiceCallbackManager CallToServiceCallbackManager
		{
			get { return mCallToServiceCallbackManager; }
		}
		
        private IServiceInterface callToService = null;
        private BackgroundWorker mBackgroundWorkerServiceCall = null;

        private TcpChannel channel;
        private bool isConnected = false;

        /// <summary>
        /// Connect to the Remote server via tcp
        /// </summary>
        /// <returns>true if successfull else false</returns>
        public bool RemoteConnect()
        {
            try
            {

                if ((channel != null) && ChannelServices.GetChannel(channel.ChannelName) != null)
                {
                    BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                    BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                    serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                    IDictionary props = new Hashtable();
                    props["port"] = 0;
                    string s = System.Guid.NewGuid().ToString();
                    props["name"] = s;
                    props["typeFilterLevel"] = TypeFilterLevel.Full;
                    channel = new TcpChannel(props, clientProvider, serverProvider);
                    ChannelServices.RegisterChannel(channel, false);

                    isConnected = true;
                }
            }
            catch (Exception ex)
            {
                isConnected = false;
				StateServerAssert.Assert(ex);
            }

            return isConnected;
        }

        /// <summary>
        /// Disconnect the remote server
        /// </summary>
        ~RemoteCallToService()
        {
            RemoteDisconnect();
        }

        /// <summary>
        // Activate the remote interface IServiceInterface 
        /// </summary>
        /// <returns>true if activated else false</returns>
        public virtual bool ActivateTheInterface()
        {
            bool activated = false;

            try
            {
                WellKnownServiceTypeEntry WKSTE = new WellKnownServiceTypeEntry(typeof(IServiceInterface), StateServerConfig.RestServicesUrl, WellKnownObjectMode.SingleCall);

                if (RemotingConfiguration.ApplicationName == null)
                {
                    RemotingConfiguration.ApplicationName = "RestService";
                    RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);
                }

                callToService = (IServiceInterface)Activator.GetObject(typeof(IServiceInterface), WKSTE.ObjectUri);

                if (callToService != null)
                {
                    activated = true;
                }
            }

            catch (System.Exception ex)
            {
                StateServerAssert.Assert(ex);
            }

            return activated;
        }

        /// <summary>
        /// Disconnect the Remote channel
        /// </summary>
        public void RemoteDisconnect()
        {
            isConnected = false;
            if (channel != null)
            {
                ChannelServices.UnregisterChannel(channel);
            }
        }


        /// <summary>
        /// Check if the connection is connected if not restablish the connection
        /// </summary>
        protected void CheckForConnection()
        {
            if (isConnected == false)
            {
                RemoteConnect();
                ActivateTheInterface();
            }
        }

        /// <summary>
        /// Invoke Services async
        /// </summary>
        /// <param name="serviceCommand">The Service Command</param>
        /// <param name="callback">Method to callback to handle the sendtoclient message</param>
        public void InvokeServiceAsync(ServiceCommand serviceCommand, Action<string> callback)
        {
            ServiceCommandAsync serviceCommandAsync = new ServiceCommandAsync();
            serviceCommandAsync.ServiceCommandData = serviceCommand;
            serviceCommandAsync.Callback = callback;

            mBackgroundWorkerServiceCall = new BackgroundWorker();
            mBackgroundWorkerServiceCall.WorkerSupportsCancellation = false;
            mBackgroundWorkerServiceCall.DoWork += new DoWorkEventHandler(bwMainDoWork);
            mBackgroundWorkerServiceCall.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwMainRunWorkerCompleted);
            mBackgroundWorkerServiceCall.RunWorkerAsync(serviceCommandAsync);
        }

        /// <summary>
        /// Invoke Service Blocking
        /// </summary>
        /// <param name="serviceCommand">The Service Command</param>
        /// <returns>The xml string response</returns>
        public string InvokeServiceBlocking(ServiceCommand serviceCommand)
        {
            string response = "";

            response = InvokeServiceCommand((ServiceCommand)serviceCommand);

            return response;
        }


        /// <summary>
        /// Background worker thread that does the service call
        /// </summary>
        /// <param name="sender">sender not used</param>
        /// <param name="e">ServiceCommandAsync arguments</param>
        private void bwMainDoWork(object sender, DoWorkEventArgs e)
		{
            ServiceCommandAsync serviceCommandAsync = (ServiceCommandAsync)e.Argument;
            serviceCommandAsync.Result = InvokeServiceCommand(serviceCommandAsync.ServiceCommandData);
            e.Result = serviceCommandAsync;
        }


        /// <summary>
        /// Inovke the Service command can be called from async or blocking functions
        /// </summary>
        /// <param name="serviceCommand">ServiceCommand to pass in the services call</param>
        /// <returns>The service response string xml data or error response</returns>
        private string InvokeServiceCommand(ServiceCommand serviceCommand)
        {
            string response = "";

            try
            {
                CheckForConnection();
                response = InvokeServiceCommandHandler(serviceCommand);
            }

            catch (System.Net.Sockets.SocketException ex)
            {
                isConnected = false;
                response = String.Format("<Error><Message>{0}</Message></Error>", ex);
            }

            catch (Exception ex)
            {
                response = String.Format("<Error><Message>{0}</Message></Error>", ex);
            }

            return response;
        }


        /// <summary>
        /// Virtual to call the ProcessRestService
        /// </summary>
        /// <param name="serviceCommand">Service command to execute</param>
        /// <returns></returns>
        protected virtual string InvokeServiceCommandHandler(ServiceCommand serviceCommand)
        {
           return (callToService.ProcessRestService((RESTCommand)serviceCommand));
        }


        /// <summary>
        /// Background worker completed
        /// </summary>
        /// <param name="sender">sender not used</param>
        /// <param name="e">ServiceCommandAsync parameters containing the results and information required for the callback</param>
		private void bwMainRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            string response = "";
            Action<string> callback = null;
            string type = "";

            if (e.Cancelled) 
            {
                response = "Cancelled";
            }
            else if (e.Error != null)
            {
                ServiceCommandAsync serviceCommandAsyncResult = (ServiceCommandAsync)e.Result;
                response = String.Format("<Error><Message>{0}</Message></Error>", e.Error.Message);
            }
            else
            {
                ServiceCommandAsync serviceCommandAsyncResult = (ServiceCommandAsync)e.Result;
                response = serviceCommandAsyncResult.Result;
                callback = serviceCommandAsyncResult.Callback;
                type = serviceCommandAsyncResult.ServiceCommandData.Noun;
            }

            InvokeServiceReturn(response, type, callback);
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="recipients"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// 
        /// <summary>
        /// The Service return, used for async RESTService remote method calls
        /// </summary>
        /// <param name="response">The Service response message</param>
        /// <param name="type">The message type(noun of the message) either PaymentItemsService or other</param>
        /// <param name="callback">ServerMessageProcessor pointer used to call SendMessageToReflector</param>
        protected virtual void InvokeServiceReturn(string response, string type, System.Action<string> callback)
        {
			mCallToServiceCallbackManager.Enqueue(callback, response);
        }
    }

    public class ServiceCommands
    {
        /// <summary>
        /// Async call to Get the Room Catalog Id By RoomId
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="callToService"></param>
        /// <param name="recipients"></param>
        /// <param name="callback"></param>
        public void GetRoomCatalogIdByRoomId(string roomId, RemoteCallToService callToService, System.Action<string> callback)
        {
            RESTCommand restCommand = new RESTCommand();
            restCommand.Noun = "Rooms";
            restCommand.Verb = "GetRoomCatalogIdByRoomId";
            restCommand.Parameters.Add("roomId", roomId);

            callToService.InvokeServiceAsync(restCommand, callback);
        }

        /// <summary>
        /// Blocking call to Get the Room Catalog Id By RoomId
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="callToService"></param>
        /// <returns></returns>
        public string GetRoomCatalogIdByRoomIdBlocking(string roomId, RemoteCallToService callToService)
        {
            RESTCommand restCommand = new RESTCommand();
            restCommand.Noun = "Rooms";
            restCommand.Verb = "GetRoomCatalogIdByRoomId";
            restCommand.Parameters.Add("roomId", roomId);

            return (callToService.InvokeServiceBlocking(restCommand));
        }
        /// <summary>
        /// Async call to the PaymentItems service calling through the RESTService
        /// </summary>
        /// <param name="paymentCommand">PaymmentITems command</param>
        /// <param name="callToService">The RESTService handler call</param>
        /// <param name="recipients">The client receipants of the returned message</param>
        /// <param name="callback">Callback action called on service response</param>
        public void PaymentItemCommand(PaymentCommand paymentCommand, RemoteCallToService callToService, System.Action<string> callback)
        {
            RESTCommand restCommand = new RESTCommand();
            restCommand.Noun = "PaymentItemsService";
            restCommand.Verb = "ProcessPaymentItem";

            restCommand.Parameters = paymentCommand.Parameters;

            restCommand.Parameters.Add("noun", paymentCommand.Noun);
            restCommand.Parameters.Add("verb", paymentCommand.Verb);

            callToService.InvokeServiceAsync(restCommand, callback);
        }
    } 
}