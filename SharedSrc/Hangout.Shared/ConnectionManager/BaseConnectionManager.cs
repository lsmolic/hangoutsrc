using System;
using System.Collections.Generic;

namespace Hangout.Shared
{
	public abstract class BaseConnectionManager : IConnectionManager
	{
		protected Dictionary<MessageSubType, Action<Message, Guid>> mMessageActions = new Dictionary<MessageSubType, Action<Message, Guid>>();
		
		public virtual void ReceiveMessage(Message receivedMessage, Guid senderId)

		{
            Messages.MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions, senderId);
		}
	}
}