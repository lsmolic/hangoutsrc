/*=============================================================
(c) all rights reserved
================================================================*/
using UnityEngine;

using PureMVC.Patterns;
using PureMVC.Interfaces;

namespace Hangout.Client
{
	public class SendChatCommand : SimpleCommand, ICommand
	{
		public override void Execute(INotification notification)
		{
            string chatText = (string)((object [])notification.Body)[0]; 

            LocalAvatarProxy localAvatarProxy = GameFacade.Instance.RetrieveProxy<LocalAvatarProxy>();
            localAvatarProxy.LocalAvatarDistributedObject.SendChat(chatText);	
		}       
		
	}
	
}
