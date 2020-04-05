using UnityEngine;
using System.Collections;

using Hangout.Shared;

namespace Hangout.Client
{
	public delegate void SendMessageCallback( Message message ); 
	public interface IMessageRouter
	{
		void ReceiveMessage( Message message );
		void RegisterSendMessageCallback( SendMessageCallback sendMessageCallback );
	}
}