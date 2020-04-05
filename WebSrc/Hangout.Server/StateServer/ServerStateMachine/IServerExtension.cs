using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
	public interface IServerExtension
	{
		/// <summary>
		/// This method should be overridden by the child class.
		/// Here you should run your extension initialization code. This method is executed once, when the extension is started.
		/// </summary>
		void Init();

		/// <summary>
		/// Handles client requests
		/// </summary>
		/// <param name="receivedMessage"></param>
		/// <param name="senderId"></param>
		/// <param name="recipients"></param>
		void ReceiveRequest(Message receivedMessage, Guid senderId);

	}
}
