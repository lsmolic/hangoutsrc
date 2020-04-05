using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client.Gui
{
	public class FriendsGui : GuiController
	{
		private const string mResourcePath = "resources://GUI/Friends/FriendsGui.gui";
		private Window mMainWindow = null;

		private ScrollFrame mFriendListScrollFrame = null;
		private IGuiFrame mFriendListingPrototypeFrame = null;

		public bool Showing
		{
			get { return mMainWindow.Showing; }
			set { mMainWindow.Showing = value; }
		}

		public FriendsGui(IGuiManager guiManager)
			: base(guiManager, mResourcePath)
		{
			foreach (IGuiElement element in this.AllElements)
			{
				if (element.Name == "FriendsGui" && element is Window)
				{
					mMainWindow = (Window)element;

					Button closeButton = mMainWindow.SelectSingleElement<Button>("HeaderFrame/CloseButton");
					closeButton.AddOnPressedAction(
						delegate()
						{
							GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_CLOSE);
							mMainWindow.Showing = false;
						}
					);

					mFriendListScrollFrame = mMainWindow.SelectSingleElement<ScrollFrame>("MainFrame/FriendListScrollFrame");
					mFriendListingPrototypeFrame = mMainWindow.SelectSingleElement<IGuiFrame>("MainFrame/FriendListScrollFrame/FriendListingPrototype");
					mFriendListScrollFrame.RemoveChildWidget(mFriendListingPrototypeFrame);
				}
			}
		}

		public void ListFriends(List<string> receivedFriends)
		{
			foreach (string friendName in receivedFriends)
			{
				IGuiFrame friendListing = (IGuiFrame)mFriendListingPrototypeFrame.Clone();
				//Image friendImage = friendListing.SelectSingleElement<Image>("FriendImage");
				Label friendNameLabel = friendListing.SelectSingleElement <Label>("FriendName");
				friendNameLabel.Text = friendName;

				//Button chatButton = friendListing.SelectSingleElement<Button>("ChatButton");
				//Button gotoButton = friendListing.SelectSingleElement<Button>("GotoButton");

				mFriendListScrollFrame.AddChildWidget(friendListing, new HorizontalAutoLayout());
			}

		}
	}
}
