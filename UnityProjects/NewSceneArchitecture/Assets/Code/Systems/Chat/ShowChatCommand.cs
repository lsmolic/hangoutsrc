/**  --------------------------------------------------------  *
 *   ShowChatCommand.cs
 *
 *   Author: Samir, Hangout Industries
 *   Date: 07/24/2009
 *	 
 *   Listen for all chat events for all avatars and draw chat 
 *   bubble above avatars' heads
 *   --------------------------------------------------------  *
 */
using UnityEngine;

using System;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Hangout.Client.Gui;

namespace Hangout.Client
{	
	public class ShowChatCommand : SimpleCommand
	{
		public override void Execute(INotification notification)
		{
            AvatarDistributedObject avatar = (AvatarDistributedObject)((object[])notification.Body)[0];
            String chatText = (String)((object[])notification.Body)[1];
            IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
            CameraManagerMediator cameraManager = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>();
			ChatMediator chatMediator = GameFacade.Instance.RetrieveMediator<ChatMediator>();
           
            // Show the chat bubble above the avatars head
			avatar.ShowChat(chatText, cameraManager.GetMainCamera(), guiManager);
			// Put the message in the chat log
			chatMediator.AddChatText(chatText);

			GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_POPUP_APPEAR_B);
				
		}
	}
}
	