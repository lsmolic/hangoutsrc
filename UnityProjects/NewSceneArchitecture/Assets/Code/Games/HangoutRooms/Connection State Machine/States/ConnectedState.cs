/**  --------------------------------------------------------  *
 *   Connected.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class ConnectedState : State
	{
		private GameStateMachine mGameStateMachine;

		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public override void EnterState()
		{
			// Register Mediators
			mGameStateMachine = new GameStateMachine();
			GameFacade.Instance.RegisterMediator(mGameStateMachine);
			GameFacade.Instance.RegisterMediator(new ChatMediator());
			GameFacade.Instance.RegisterMediator(new FriendsMediator());
			GameFacade.Instance.RegisterMediator(new FacebookFeedMediator());

			ToolbarMediator toolbarMediator = new ToolbarMediator();
			GameFacade.Instance.RegisterMediator(toolbarMediator);

			// Register Proxies
			GameFacade.Instance.RegisterProxy(new RoomManagerProxy());

			GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().Init();
        }

		public override void ExitState()
		{
			//kill all the distributed objects
			GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>().DeleteClientCache();
			
			GameFacade.Instance.RemoveMediator(typeof(SoundMediator).Name);
			// Unregister Mediators
			GameFacade.Instance.RemoveMediator(typeof(ToolbarMediator).Name);
			GameFacade.Instance.RemoveMediator(typeof(ChatMediator).Name);
			// Unregister GSM 
			// NOTE: this must be done before unregistering the RoomManagerProxy because there are things in here that try and access it
			mGameStateMachine.Deactivate();
			GameFacade.Instance.RemoveMediator(typeof(GameStateMachine).Name);

			GameFacade.Instance.RemoveMediator(typeof(FacebookFeedMediator).Name);


			// Unregister Proxies
			GameFacade.Instance.RemoveProxy(typeof(RoomManagerProxy).Name);
       }
	}	
}
