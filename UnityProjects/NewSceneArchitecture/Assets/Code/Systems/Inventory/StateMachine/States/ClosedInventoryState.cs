/**  --------------------------------------------------------  *
 *   ClosedInventoryState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/19/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class ClosedInventoryState : StoreGuiState
	{
		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public ClosedInventoryState(Window guiWindow)
			: base(guiWindow)
		{

		}
		
		public override void EnterState()
		{
			GuiWindow.Showing = false;
			GameFacade.Instance.SendNotification(GameFacade.SHOP_CLOSED);
		}
		
		public override void ExitState()
		{
			GuiWindow.Showing = true;
			GameFacade.Instance.SendNotification(GameFacade.SHOP_OPENED);
		}

        public override void HandleSearchResults(XmlDocument xmlResponse)
        {
            //We just ignore search results passed into here since the gui is closed.
        }
    
    }
}
