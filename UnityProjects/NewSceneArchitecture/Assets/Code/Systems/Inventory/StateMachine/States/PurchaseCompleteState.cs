/**  --------------------------------------------------------  *
 *   PurchaseCompleteState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class PurchaseCompleteState : StoreGuiState
	{
		private readonly GuiFrame mPurchaseCompleteFrame;
		private readonly Button mOkButton;
		
		public PurchaseCompleteState(Window guiWindow, Hangout.Shared.Action backToStoreSelector )
			: base(guiWindow)
		{			
			mPurchaseCompleteFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/PurchaseCompleteFrame");
			mOkButton = GuiWindow.SelectSingleElement<Button>("MainFrame/PurchaseCompleteFrame/OkButton");
			mOkButton.AddOnPressedAction(delegate()
			{
				backToStoreSelector();
			});
		}

		public override void EnterState()
		{
			mPurchaseCompleteFrame.Showing = true;
		}

		public override void ExitState()
		{
			mPurchaseCompleteFrame.Showing = false;
		}

        public override void HandleSearchResults(XmlDocument xmlResponse)
        {
            throw new NotImplementedException("HandleSearchResults not implemented");
        }

   }
}
