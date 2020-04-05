using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
    public class RoomAPIGui : GuiController
    {

        private RoomId mCurrentRoomId = null;
        private Dictionary<RoomId, List<object>> mAvailableRooms = new Dictionary<RoomId, List<object>>();
        private const string mResourcePath = "resources://GUI/Room/RoomGui.gui";

        private Window mMainWindow = null;

        //"my room" gui widgets
        private ScrollFrame mMyRoomListScrollFrame = null;
        private Button mCreateGreenScreenRoomButton = null;
        private Button mCreateMiniGameRoomButton = null;
        private Textbox mCreateRoomTextbox = null;
        private IGuiFrame mMyRoomListingFramePrototype = null;

        public bool Showing
        {
            get { return mMainWindow.Showing; }
            set { mMainWindow.Showing = value; }
        }

        public RoomId CurrentRoomId
        {
            get { return mCurrentRoomId; }
        }

        public void UpdateCurrentRoomId(RoomId currentRoomId)
        {
            mCurrentRoomId = currentRoomId;
        }

        public RoomAPIGui(IGuiManager guiManager)
            : base(guiManager, mResourcePath)
        {
            foreach( IGuiElement element in this.AllElements)
            {
                if( element.Name == "RoomAPIGui" && element is Window)
                {
                    mMainWindow = (Window)element;

                    Button closeButton = mMainWindow.SelectSingleElement<Button>("HeaderFrame/CloseButton");
                    closeButton.AddOnPressedAction(
                        delegate()
                        {
                            mMainWindow.Showing = false;
                        }
                    );

                    mMyRoomListScrollFrame = mMainWindow.SelectSingleElement<ScrollFrame>("MainFrame/RoomListFrame");
                    mCreateRoomTextbox = mMainWindow.SelectSingleElement<Textbox>("MainFrame/NewRoomName");
                    mCreateGreenScreenRoomButton = mMainWindow.SelectSingleElement<Button>("MainFrame/CreateGreenScreenRoom");
                    mCreateGreenScreenRoomButton.AddOnPressedAction(
                        delegate()
                        {
                            CreateRoom(mCreateRoomTextbox.Text, RoomType.GreenScreenRoom);
                        }
                    );
                    mCreateMiniGameRoomButton = mMainWindow.SelectSingleElement<Button>("MainFrame/CreateMiniGameRoom");
                    mCreateMiniGameRoomButton.AddOnPressedAction(
                        delegate()
                        {
                            CreateRoom(mCreateRoomTextbox.Text, RoomType.MiniGameRoom);
                        }
                    );
                    mMyRoomListingFramePrototype = mMainWindow.SelectSingleElement<IGuiFrame>("MainFrame/RoomListFrame/RoomListingPrototype");

                    mMyRoomListScrollFrame.RemoveChildWidget(mMyRoomListingFramePrototype);

                    Button nextRoomButton = mMainWindow.SelectSingleElement<Button>("MainFrame/NextRoomButton");
                    nextRoomButton.AddOnPressedAction( GetNextRoom );
                    Button previousRoomButton = mMainWindow.SelectSingleElement<Button>("MainFrame/PreviousRoomButton");
                    previousRoomButton.AddOnPressedAction(GetPreviousRoom);
                }
            }
        }

        private void GetNextRoom()
        {
            if (mAvailableRooms.Count > 0)
            {
                List<RoomId> availableRooms = new List<RoomId>(mAvailableRooms.Keys);
                int nextRoomIndex = availableRooms.IndexOf(mCurrentRoomId) + 1;
                if (nextRoomIndex >= availableRooms.Count)
                {
                    nextRoomIndex = 0;
                }
                JoinRoom(availableRooms[nextRoomIndex]);
            }
        }

        private void GetPreviousRoom()
        {
            if (mAvailableRooms.Count > 0)
            {
                List<RoomId> availableRooms = new List<RoomId>(mAvailableRooms.Keys);
                int previousRoomIndex = availableRooms.IndexOf(mCurrentRoomId) - 1;
                if (previousRoomIndex <= -1)
                {
                    previousRoomIndex = availableRooms.Count - 1;
                }
                JoinRoom(availableRooms[previousRoomIndex]);
            }
        }

        public void ListRooms(Dictionary<RoomId, List<object>> availableRooms)
        {
            mAvailableRooms = availableRooms;
            mMyRoomListScrollFrame.ClearChildWidgets();

            //we need to procedurally populate the scroll frame with the names of the available rooms from the server
            foreach (KeyValuePair<RoomId, List<object>> room in availableRooms)
            {
                IGuiFrame roomListing = (IGuiFrame)mMyRoomListingFramePrototype.Clone();

                RoomId newRoomId = new RoomId(room.Key);

                Button deleteRoomButton = roomListing.SelectSingleElement<Button>("DeleteRoomButton");
                deleteRoomButton.AddOnPressedAction(
                    delegate()
                    {
                        DeleteRoom(newRoomId);
                    }
                );

                Button joinRoomButton = roomListing.SelectSingleElement<Button>("JoinRoomButton");
                joinRoomButton.AddOnPressedAction(
                    delegate()
                    {
                        JoinRoom(newRoomId);
                    }
                );

                Label roomNameLabel = roomListing.SelectSingleElement<Label>("RoomName");
                roomNameLabel.Text = room.Value + " - id: " + room.Key.ToString();

                mMyRoomListScrollFrame.AddChildWidget(roomListing, new HorizontalAutoLayout());
            }
        }

        private void DeleteRoom(RoomId roomIdToDelete)
        {
            RoomAPICommands.DeleteRoom(roomIdToDelete);
        }

        private void JoinRoom(RoomId roomIdToJoin)
        {
            RoomAPICommands.SwitchRoom(roomIdToJoin, MessageSubType.ClientOwnedRooms);
        }

        private void CreateRoom(string roomName, RoomType roomType)
        {
            RoomAPICommands.CreateRoom(roomName, roomType);
        }

    }
}
