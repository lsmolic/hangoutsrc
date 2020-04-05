using System;
using System.Collections.Generic;
using Hangout.Shared;
using Hangout.Shared.Messages;
using PureMVC.Patterns;

namespace Hangout.Client
{
	public class ErrorHandlerMediator : Mediator
	{
		private Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();

		public ErrorHandlerMediator()
		{
			mMessageActions.Add(MessageSubType.UserLoginGetOrCreateAvatarError, UserLoginGetOrCreateAvatarError);
			mMessageActions.Add(MessageSubType.UserLoginGetOrCreateRoomError, UserLoginGetOrCreateRoomError);
			mMessageActions.Add(MessageSubType.UserLoginCannotGetAccountFromSessionManagerError, UserLoginCannotGetAccountFromSessionManagerError);
		}

		public void ReceiveMessage(Message receivedMessage)
		{
			MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
		}

		private void UserLoginGetOrCreateAvatarError(Message receivedMessage)
		{
			GameFacade.Instance.SendNotification(GameFacade.LOGIN_FAILED);
		}

		private void UserLoginGetOrCreateRoomError(Message receivedMessage)
		{
			GameFacade.Instance.SendNotification(GameFacade.LOGIN_FAILED);
		}

		private void UserLoginCannotGetAccountFromSessionManagerError(Message receivedMessage)
		{
			GameFacade.Instance.SendNotification(GameFacade.LOGIN_FAILED);
		}
	}
}
