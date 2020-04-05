using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Client.FashionGame;
using UnityEngine;

namespace Hangout.Client
{
	public class MapGui : GuiController
	{
		private const string mResourcePath = "resources://GUI/Room/MapGui.gui";

		private Window mMainWindow = null;

		//private RoomId mCurrentRoomId = null;

		public bool Showing
		{
			get { return mMainWindow.Showing; }
			set { mMainWindow.Showing = value; }
		}

		public MapGui(IGuiManager guiManager) :
			base(guiManager, mResourcePath)
		{
			foreach (IGuiElement element in this.AllElements)
			{
				if (element.Name == "MapGui" && element is Window)
				{
					mMainWindow = (Window)element;
					mMainWindow.OnShowing(OnShowingCallback);

					Button closeButton = mMainWindow.SelectSingleElement<Button>("HeaderFrame/CloseButton");
					closeButton.AddOnPressedAction(
						delegate()
						{
							mMainWindow.Showing = false;
							EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.EVENT_WINDOW_CLOSED);
						}
					);

					Button myHangoutButton = mMainWindow.SelectSingleElement<Button>("MainFrame/MyHangout");
					myHangoutButton.AddOnPressedAction(delegate()
					{
						GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.ToggleWindow(GameFacade.SHOW_ROOM_PICKER_GUI, MessageSubType.ClientOwnedRooms);
						//GameFacade.Instance.SendNotification(GameFacade.SHOW_ROOM_PICKER_GUI, MessageSubType.ClientOwnedRooms);
						this.Showing = false;
						EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.MY_HANGOUT_CLICKED);
					});

					Button friendHangoutsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/FriendHangouts");
					friendHangoutsButton.AddOnPressedAction(delegate()
					{
						GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.ToggleWindow(GameFacade.SHOW_ROOM_PICKER_GUI, MessageSubType.FriendsRooms);
						this.Showing = false;
						EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.FRIEND_HANGOUT_CLICKED);
					});

					Button publicHangoutsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/PublicHangouts");
					publicHangoutsButton.AddOnPressedAction(delegate()
					{
						GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.ToggleWindow(GameFacade.SHOW_ROOM_PICKER_GUI, MessageSubType.PublicRooms);
						this.Showing = false;
						EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.PUBLIC_HANGOUT_CLICKED);
					});

					Button shopButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Shop");
					shopButton.AddOnPressedAction(delegate()
					{
						this.Showing = false;
						GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.ToggleWindow(GameFacade.SHOP_BUTTON_CLICKED);
						EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.SHOP_CLICKED);
					});

					Button fashionFrenzyButton = mMainWindow.SelectSingleElement<Button>("MainFrame/FashionFrenzy");
					fashionFrenzyButton.AddOnPressedAction(delegate()
					{
						this.Showing = false;
						GameFacade.Instance.RetrieveMediator<ToolbarMediator>().NavigationBar.CloseAllWindows();
						GameFacade.Instance.SendNotification(GameFacade.SWITCHING_TO_FASHION_MINI_GAME);
						EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.FASHION_CITY_CLICKED);
					});

				}
			}

		}

		private void OnShowingCallback(bool showing)
		{
			if (showing)
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_MAP_OPEN);
				GameFacade.Instance.SendNotification(GameFacade.MAP_GUI_OPENED);
				EventLogger.Log(LogGlobals.CATEGORY_MAP, LogGlobals.EVENT_WINDOW_OPENED);
			}
			else
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_CLOSE);
				GameFacade.Instance.SendNotification(GameFacade.MAP_GUI_CLOSED);
				// Dont log for close window here, since window may already be closed.  Log on close button press
			}
		}
	}
}
