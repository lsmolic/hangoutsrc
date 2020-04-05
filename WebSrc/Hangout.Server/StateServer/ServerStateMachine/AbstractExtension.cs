using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
	/// <summary>
	/// Every extension has to accomplish three different tasks:
	/// <li>Initialize
	/// <li>Handle client requests
	/// <li>Destroy and release its resources
	/// </summary>
	public abstract class AbstractExtension : IServerExtension, IDisposable
	{
		protected readonly ServerStateMachine mServerStateMachine;
		protected Dictionary<MessageSubType, Hangout.Shared.Action<Message, Guid>> mMessageActions = new Dictionary<MessageSubType, Hangout.Shared.Action<Message, Guid>>();

		public AbstractExtension(ServerStateMachine serverStateMachine)
		{
			mServerStateMachine = serverStateMachine;
		}

		/// <summary>
		/// Initialize the extension.  Called when the server loads the extension.
		/// It's good practice to put initialization code here instead of the constructor to make writing
		/// unit tests easier
		/// </summary>
		public virtual void Init()
		{

		}

		/// <summary>
		/// This method should be overridden by the child class.
		/// This method is called when the extension is being stopped by the server: here you should release the resources acquired by the extension (close files, db connections etc...)
		/// </summary>
		public virtual void Dispose()
		{

		}

		/// <summary>
		/// Handles client requests
		/// </summary>
		/// <param name="receivedMessage"></param>
		/// <param name="senderId"></param>
		/// <param name="recipients"></param>
		public virtual void ReceiveRequest(Message receivedMessage, Guid senderId)
		{
			Hangout.Shared.Messages.MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions, senderId);
		}

		/// <summary>
		/// Send message back to channel corresponding to the provided sessionId.  Usually this is the sessionId that sent the original request
		/// </summary>
		/// <param name="message"></param>
		/// <param name="sessionId"></param>
		protected void SendMessageToClient(Message message, Guid sessionId)
		{
			mServerStateMachine.SendMessageToReflector(message, sessionId);
		}

		/// <summary>
		/// Send message back to the admin client.  This is mainly used for debugging, monitoring and reporting
		/// </summary>
		/// <param name="message"></param>
		/// <param name="sessionId"></param>
		protected void SendMessageToAdmin(Message message, Guid sessionId)
		{
			mServerStateMachine.SendMessageToAdminReflector(message, sessionId);
		}

	}
}
