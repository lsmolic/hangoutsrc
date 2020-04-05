/* A Pherg Spot 10/21/09 */

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

using Hangout.Client.Gui;
using UnityEngine;

namespace Hangout.Client
{
	public class GreenScreenRoomTutorialGui : GuiController
	{
		private Window mActiveWindow;

		private const string mShoppingWindowName = "ShoppingPopupWindow";
		private const string mFashionWindowName = "FashionPopupWindow";
		private const string mMovementWindowName = "MovementPopupWindow";
		private const string mDecorateWindowName = "DecoratePopupWindow";
		private const string mMapWindowName = "MapPopupWindow";
		private const string mGetCashWindowName = "GetCashPopupWindow";
		
		public GreenScreenRoomTutorialGui(IGuiManager manager, string guiPath)
		 : base (manager, guiPath)
		{	
			foreach (Window window in AllGuis)
			{
				window.Showing = false;
				Button closeButton = window.SelectSingleElement<Button>("MainFrame/PopupFrame/TitleBarFrame/CloseButton");
				closeButton.AddOnPressedAction(HidePopup);
				window.OnShowing(OnShowingCallback);
			}
		}
		
		private void OnShowingCallback(bool showing)
		{
			if (showing)
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_POPUP_APPEAR_A);
			}
		}
		
		public void OpenShoppingPopup()
		{
			OpenPopup(mShoppingWindowName);
		}
		
		public void OpenFashionGamePopup()
		{
			OpenPopup(mFashionWindowName);
		}
		
		public void OpenMovementPopup()
		{
			OpenPopup(mMovementWindowName);
		}
		
		public void OpenDecoratePopup()
		{
			OpenPopup(mDecorateWindowName);
		}

		public void OpenMapPopup()
		{
			OpenPopup(mMapWindowName);
		}

		public void OpenGetCashPopup()
		{
			OpenPopup(mGetCashWindowName);
		}
		
		private void OpenPopup(string windowName)
		{
			foreach (Window window in AllGuis)
			{
				if (window.Name == windowName)
				{
					mActiveWindow = window;
					mActiveWindow.Showing = true;
					return;
				}
			}
		}
		
		public void HidePopup()
		{
			if (mActiveWindow != null)
			{
				mActiveWindow.Showing = false;
			}
		}
		
	}
}
