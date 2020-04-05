using System;

namespace Hangout.Shared
{
	public interface IConnectionManager
	{
		void ReceiveMessage(Message receivedMessage, Guid senderId);
	}
}