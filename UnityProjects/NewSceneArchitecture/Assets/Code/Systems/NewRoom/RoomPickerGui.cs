using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
    public class RoomPickerGui : GuiController
    {
		private const string mResourcePath = "resources://GUI/Room/RoomPickerGui.gui";
        private MessageSubType mCurrentRoomRequestType = MessageSubType.ClientOwnedRooms;

        private IGuiFrame mRoomListScrollFrame = null;
        private IGuiFrame mRoomListingPrototypeFrame = null;
		private System.Action<RoomType> mSendSwitchingToRoomTypeNotification = null;
		private Label mTitleLabel = null;

        private Window mMainWindow = null;

        public bool Showing
        {
            get { return mMainWindow.Showing; }
            set { mMainWindow.Showing = value; }
        }

		public RoomPickerGui(IGuiManager guiManager, System.Action<RoomType> sendSwitchingToRoomTypeNotification)
            : base(guiManager, mResourcePath)
        {
			mSendSwitchingToRoomTypeNotification = sendSwitchingToRoomTypeNotification;
            foreach( IGuiElement element in this.AllElements)
            {
                if( element.Name == "RoomPickerGui" && element is Window)
                {
                    mMainWindow = (Window)element;
                    mMainWindow.OnShowing(OnShowingCallback);

                    mTitleLabel = mMainWindow.SelectSingleElement<Label>("MainFrame/RoomListingsFrame/TitleBarLabel");
                    //we're going to initially display the client's rooms so the "My Rooms" title should be displayed first
                    mTitleLabel.Text = Translation.ROOM_PICKER_MY_ROOMS;

                    Button closeButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingsFrame/CancelButton");
                    closeButton.AddOnPressedAction(
                        delegate()
                        {
                            mMainWindow.Showing = false;
                        }
                    );

                    //setup the buttons for displaying the various types of rooms
                    Button clientOwnedRoomsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingButtons/ClientOwnedRoomsButton");
					Button friendsRoomsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingButtons/FriendsRoomsButton");
					Button hangoutPublicRoomsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingButtons/HangoutPublicRoomsButton");
					
					clientOwnedRoomsButton.AddOnPressedAction
					(
                        delegate()
                        {
							ClearRoomsWindow();
							UpdateWindowTitleLabel(MessageSubType.ClientOwnedRooms);
                            RoomAPICommands.RequestRoomsFromServer(MessageSubType.ClientOwnedRooms);
                            mCurrentRoomRequestType = MessageSubType.ClientOwnedRooms;
                        }
                    );
					friendsRoomsButton.AddOnPressedAction
					(
                        delegate()
                        {
							ClearRoomsWindow();
							UpdateWindowTitleLabel(MessageSubType.FriendsRooms);
                            RoomAPICommands.RequestRoomsFromServer(MessageSubType.FriendsRooms);
                            mCurrentRoomRequestType = MessageSubType.FriendsRooms;
                        }
                    );
                    hangoutPublicRoomsButton.AddOnPressedAction
					(
                        delegate()
                        {
							ClearRoomsWindow();
							UpdateWindowTitleLabel(MessageSubType.PublicRooms);
                            RoomAPICommands.RequestRoomsFromServer(MessageSubType.PublicRooms);
                            mCurrentRoomRequestType = MessageSubType.PublicRooms;
                        }
                    );

                    //set up the grid view / scroll area where the rooms are listed
					mRoomListScrollFrame = mMainWindow.SelectSingleElement <IGuiFrame>("MainFrame/RoomListingsFrame/RoomListScrollFrame");
					mRoomListingPrototypeFrame = mMainWindow.SelectSingleElement <IGuiFrame>("MainFrame/RoomListingsFrame/RoomListScrollFrame/RoomListingPrototypeFrame");
					mRoomListScrollFrame.RemoveChildWidget(mRoomListingPrototypeFrame);
                }
            }
        }
        
        private void OnShowingCallback(bool showing)
        {
			if (!showing) 
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_CLOSE);
			}
        }

		public void RequestRooms(MessageSubType roomRequestType)
		{
			RoomAPICommands.RequestRoomsFromServer(roomRequestType);
			UpdateWindowTitleLabel(roomRequestType);
			ClearRoomsWindow();
		}

		private void UpdateWindowTitleLabel(MessageSubType MessageSubType)
		{
			// Enable all the tabs, then selectively disable the one that is selected
			Button clientOwnedRoomsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingButtons/ClientOwnedRoomsButton");
			Button friendsRoomsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingButtons/FriendsRoomsButton");
			Button hangoutPublicRoomsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/RoomListingButtons/HangoutPublicRoomsButton");
			clientOwnedRoomsButton.Enabled = true;
			friendsRoomsButton.Enabled = true;
			hangoutPublicRoomsButton.Enabled = true;

			switch(MessageSubType)
			{
				case MessageSubType.ClientOwnedRooms:
					mTitleLabel.Text = Translation.ROOM_PICKER_MY_ROOMS;
					clientOwnedRoomsButton.Enabled = false;
					break;
				case MessageSubType.FriendsRooms:
					mTitleLabel.Text = Translation.ROOM_PICKER_FRIENDS_ROOMS;
					friendsRoomsButton.Enabled = false;
					break;
				case MessageSubType.PublicRooms:
					mTitleLabel.Text = Translation.ROOM_PICKER_PUBLIC_ROOMS;
					hangoutPublicRoomsButton.Enabled = false;
					break;
			}
		}

		private void ClearRoomsWindow()
		{
			mRoomListScrollFrame.ClearChildWidgets();
		}

        /// <summary>
        /// the list of objects should just be the room's data in the same order as the create message
        /// </summary>
        /// <param name="availableRooms"></param>
        public void ListRooms(Dictionary<RoomId, List<object>> availableRooms, RoomId currentRoomId)
        {
            //we need to procedurally populate the scroll frame with the names of the available rooms from the server
            foreach (KeyValuePair<RoomId, List<object>> room in availableRooms)
            {
				RoomType roomtype = CheckType.TryAssignType<RoomType>(room.Value[2]);
				if(roomtype == RoomType.GreenScreenRoom)
				{
					IGuiFrame roomListing = (IGuiFrame)mRoomListingPrototypeFrame.Clone();

					Label roomNameLabel = roomListing.SelectSingleElement<Label>("RoomNameLabel");
					roomNameLabel.Text = CheckType.TryAssignType<string>(room.Value[5]);

					Label privacyLevelLabel = roomListing.SelectSingleElement<Label>("PrivacyLevelLabel");
					privacyLevelLabel.Text = CheckType.TryAssignType<PrivacyLevel>(room.Value[4]).ToString();

					Label populationLevelLabel = roomListing.SelectSingleElement<Label>("PopulationLabel");
					populationLevelLabel.Text = CheckType.TryAssignType<uint>(room.Value[6]).ToString();

					RoomId newRoomId = new RoomId(room.Key);
					Button joinRoomButton = roomListing.SelectSingleElement<Button>("JoinRoomButton");
                    joinRoomButton.Text = Translation.JOIN_ROOM;

					joinRoomButton.AddOnPressedAction
					(
						delegate()
						{
							mSendSwitchingToRoomTypeNotification(roomtype);
							RoomAPICommands.SwitchRoom(newRoomId, mCurrentRoomRequestType);
							mMainWindow.Showing = false;
						}
					);

					mRoomListScrollFrame.AddChildWidget(roomListing, new HorizontalAutoLayout());
				}
            }
        }
      }
}
