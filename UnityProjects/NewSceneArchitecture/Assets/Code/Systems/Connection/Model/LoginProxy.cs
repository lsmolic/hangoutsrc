/*=============================================================
(c) all rights reserved
================================================================*/

using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;

namespace Hangout.Client
{
	class LoginProxy:Proxy, IProxy
	{		
		public void ProcessLoginInfo(List<object> loginData)
		{
			Message loginInfoMessage = new Message(MessageType.Connect, MessageSubType.RequestLogin, loginData);
			
			// Send login info up to reflector
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            clientMessageProcessor.SendMessageToReflector(loginInfoMessage);
		}

		
	}
	
}
