using System;
using Hangout.Shared;
using System.Collections.Generic;
using Hangout.Shared.Messages;

namespace Hangout.Client
{
    public class ClientConnectionManager
    {
		private ConnectionStateMachine mConnectionStateMachine = null;
		
		//private string mUsername = String.Empty;
		//private string mPassword = String.Empty;

		private Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();

		public ClientConnectionManager()
		{
	        mConnectionStateMachine = new ConnectionStateMachine();

			GameFacade.Instance.RegisterMediator(mConnectionStateMachine);

			mMessageActions.Add(MessageSubType.InvalidLogin, InvalidLogin);
			mMessageActions.Add(MessageSubType.SuccessfulLogin, SuccessfulLogin);
		}
		
		public void ReceiveMessage(Message incomingMessage)
		{
			MessageUtil.ReceiveMessage<MessageSubType>(incomingMessage, mMessageActions);
		}
		
        /// <summary>
        /// Server denied login, notify observers
        /// TODO:  Add "try again?" logic
        /// </summary>
        /// <param name="receivedMessage"></param>
		private void InvalidLogin(Message receivedMessage)
		{
            GameFacade.Instance.SendNotification(GameFacade.LOGIN_FAILED);
		}
		
        /// <summary>
        /// Login to server was successful, notify observers
        /// </summary>
        /// <param name="receivedMessage"></param>
		private void SuccessfulLogin(Message receivedMessage)
		{
            GameFacade.Instance.SendNotification(GameFacade.LOGIN_SUCCESS, (UserProperties)receivedMessage.Data[1]);

            // Now that we're logged in, get the payment id, and let the inventoryProxy cache it for later use by the payment pages
            GameFacade.Instance.RetrieveProxy<InventoryProxy>().GetPaymentId();
		}

    }
}
