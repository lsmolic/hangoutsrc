/**  --------------------------------------------------------  *
 *   ShowChatCommand.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 12/01/2009
 *	 
 *   Listen for emoticon events for all avatars and draw chat 
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
	public class ShowEmoticonCommand : SimpleCommand
	{
		public override void Execute(INotification notification)
		{
            AvatarDistributedObject avatar = (AvatarDistributedObject)((object[])notification.Body)[0];
            String emoticonPath = (String)((object[])notification.Body)[1];
            IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
            CameraManagerMediator cameraManager = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>();
            ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			
			clientAssetRepository.LoadAssetFromPath<ImageAsset>(emoticonPath, delegate(ImageAsset imageAsset)
			{
				// Show the chat bubble above the avatars head
				avatar.ShowChat(imageAsset.Texture2D, cameraManager.GetMainCamera(), guiManager);
				
				// TODO: Determine whether or not we want emoticons in the chat log.  Will be a good bit of work
				// to get images as well as text in the chat log.
				// Put the message in the chat log
				//ChatMediator chatMediator = GameFacade.Instance.RetrieveMediator<ChatMediator>();
				//chatMediator.AddChatText(chatText);

				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_POPUP_APPEAR_B);
			});				
		}
	}
}
