using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared.Messages;
using Hangout.Shared;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
    public class AdminManager : AbstractExtension
    {
        private readonly ILog mLogger;
        private readonly ServerStateMachine mServerStateMachine = null;
        private Hangout.Shared.Action<Message, Guid> mSendMessageToClientCallback = null;
        private Hangout.Shared.Action<Message, List<Guid>> mSendMessageToAdminCallback = null;

		public AdminManager(ServerStateMachine serverStateMachine) : base (serverStateMachine)
		{
		}

        private void ServicesHealthCheck(Guid sessionId)
        {
            System.Action<XmlDocument> servicesHealthCheckCallBack = delegate(XmlDocument xmlResponse)
            {
                Message adminMessage = new Message();
                List<object> data = new List<object>();
                data.Add(xmlResponse.InnerXml);
                adminMessage.AdminDataMessage(data);
                adminMessage.Callback =  (int)MessageSubType.HealthCheck;

                SendMessageToClient(adminMessage, sessionId);
            };

            WebServiceRequest healthCheckService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "HealthCheck", "HealthCheck");
            healthCheckService.GetWebResponseAsync(servicesHealthCheckCallBack);
        }


        public override void ReceiveRequest(Message message, Guid senderId)
        {
            if (message.Callback == (int)MessageSubType.HealthCheck)
            {
                ServicesHealthCheck(senderId);
            }
            else
            {
                string objectRepoString = mServerStateMachine.ServerObjectRepository.ToString();
                //XmlTextWriterFormattedNoDeclaration xmlTextWriter = new XmlTextWriterFormattedNoDeclaration(stringWriter);

                Message adminMessage = new Message();
                List<object> data = new List<object>();
                data.Add(objectRepoString);
                adminMessage.AdminDataMessage(data);
				SendMessageToClient(adminMessage, senderId);
            }
        }
    }
}
