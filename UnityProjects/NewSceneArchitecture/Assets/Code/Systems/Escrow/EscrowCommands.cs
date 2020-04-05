using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client
{
	public static class EscrowCommands
	{
		public static void RequestEscrow(EscrowType escrowType)
		{
			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestEscrowMessage = new Message();
			requestEscrowMessage.Callback = (int)MessageSubType.ProcessEscrowTransaction;
			List<object> messageData = new List<object>();
			messageData.Add(escrowType);
			requestEscrowMessage.EscrowMessage(messageData);

			clientMessageProcessor.SendMessageToReflector(requestEscrowMessage);
		}
	}
}
