/*=============================================================
(c) all rights reserved
================================================================*/
using System;
using System.Collections.Generic;
using Hangout.Client.Gui;
using Hangout.Shared;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

namespace Hangout.Client
{
	public class StartupCommand : SimpleCommand
	{
		public override void Execute(INotification notification)
		{
			//Create Proxies.. these should be created before Mediators
			GameProxy gameProxy = new GameProxy();

            // Register logger mediator first so things can use Console.WriteLine
            GameFacade.Instance.RegisterMediator(new LoginMediator());
			GameFacade.Instance.RegisterMediator(new CameraManagerMediator());
			GameFacade.Instance.RegisterMediator(new RoomAPIMediator());
			GameFacade.Instance.RegisterMediator(new AvatarMediator());
			GameFacade.Instance.RegisterMediator(new PopupMediator()); 
			GameFacade.Instance.RegisterMediator(new ErrorHandlerMediator());
			GameFacade.Instance.RegisterMediator(new EscrowMediator());

			// Register Proxies
            GameFacade.Instance.RegisterProxy(new ClientMessageProcessor());
			GameFacade.Instance.RegisterProxy(new LoginProxy());
			GameFacade.Instance.RegisterProxy(gameProxy);
            GameFacade.Instance.RegisterProxy(new InventoryProxy());
			GameFacade.Instance.RegisterProxy(new ClientAssetRepository());

			// Do stuff that depends on mediators and proxies being registered with the facade

			
			// Call initialization methods
			gameProxy.InitializeScene();
			this.SendNotification(GameFacade.APPLICATION_INIT);

			// Start connection to server
			SendNotification(GameFacade.REQUEST_CONNECT);
		}
	}
}
