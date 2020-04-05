using System;
using System.Collections.Generic;
using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.Messages;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace Hangout.Client
{
    public class RoomAPIMediator : Mediator
    {
        private RoomId mCurrentRoomId = null;
        private RoomAPIGui mRoomAPIGui = null;
        private RoomPickerGui mRoomPickerGui = null;
        private MapGui mMapGui = null;
        private Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();

        public RoomAPIMediator()
        {
            mMessageActions.Add(MessageSubType.ReceiveRooms, ReceiveRooms);
        }

        public void Init()
        {
            IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
            mRoomAPIGui = new RoomAPIGui(guiManager);
            mRoomAPIGui.Showing = false;
            mRoomPickerGui = new RoomPickerGui(guiManager, SendSwitchingToRoomTypeNotification);
            mRoomPickerGui.Showing = false;
            mMapGui = new MapGui(guiManager);
            mMapGui.Showing = false;
        }

    	public override IList<string> ListNotificationInterests()
		{
			List<string> interestList = new List<string>();

			interestList.Add(GameFacade.SHOW_SERVER_ROOM_API);
            interestList.Add(GameFacade.SHOW_ROOM_PICKER_GUI);
			interestList.Add(GameFacade.TOGGLE_ROOM_PICKER_GUI);
            interestList.Add(GameFacade.ROOM_LOADING_STARTED);
            interestList.Add(GameFacade.MAP_BUTTON_CLICKED);
			interestList.Add(GameFacade.CLOSE_ALL_WINDOWS);

			return interestList;
		}

		public override void HandleNotification(INotification notification)
		{
            if (mRoomPickerGui == null)
            {
                Init();
            }
            switch (notification.Name)
			{
				case GameFacade.CLOSE_ALL_WINDOWS:
					mRoomPickerGui.Showing = false;
					mMapGui.Showing = false;
					break;
				case GameFacade.TOGGLE_ROOM_PICKER_GUI:
					mRoomPickerGui.Showing = !mRoomPickerGui.Showing;
					if(mRoomPickerGui.Showing)
					{
						MessageSubType roomRequestTypeToggleRoomPickerGui = (MessageSubType)notification.Body;
						mRoomPickerGui.RequestRooms(roomRequestTypeToggleRoomPickerGui);
					}
					break;
				case GameFacade.SHOW_ROOM_PICKER_GUI:
					MessageSubType MessageSubTypehowRoomPickerGui = (MessageSubType)notification.Body;
                    mRoomPickerGui.Showing = true;
					mRoomPickerGui.RequestRooms(MessageSubTypehowRoomPickerGui);
					break;
                case GameFacade.SHOW_SERVER_ROOM_API:
                    RoomAPICommands.RequestRoomsFromServer(MessageSubType.ClientOwnedRooms);
                    mRoomAPIGui.Showing = true;
                    break;
                case GameFacade.ROOM_LOADING_STARTED:
                    mCurrentRoomId = (RoomId)notification.Body;
                    mRoomAPIGui.UpdateCurrentRoomId(mCurrentRoomId);
                    break;
                case GameFacade.MAP_BUTTON_CLICKED:
                    mMapGui.Showing = !mMapGui.Showing;
                    break;
			}
		}


        public void ReceiveMessage(Message receivedMessage)
        {
            MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
        }

        private void ReceiveRooms(Message receivedMessage)
        {
            Dictionary<RoomId, List<object>> receivedRooms = GetRoomsFromMessageData(receivedMessage.Data);
            if (mRoomPickerGui == null)
            {
                Init();
            }

            if (mRoomAPIGui.Showing)
            {
                mRoomAPIGui.ListRooms(receivedRooms);
            }
            if (mRoomPickerGui.Showing)
            {
                mRoomPickerGui.ListRooms(receivedRooms, mCurrentRoomId);
            }
        }

        private Dictionary<RoomId, List<object>> GetRoomsFromMessageData(List<object> messageDataObjects)
        {
            Dictionary<RoomId, List<object>> rooms = new Dictionary<RoomId, List<object>>();
            foreach (object messageData in messageDataObjects)
            {
                KeyValuePair<RoomId, List<object>> roomIdToRoomData = new KeyValuePair<RoomId, List<object>>();
                try
                {
                    roomIdToRoomData = (KeyValuePair<RoomId, List<object>>)messageData;
                }
                catch (System.Exception)
                {
                    throw new Exception("Error casting from an object to a KeyValuePair<RoomId, string>.");
                }
                rooms.Add(roomIdToRoomData.Key, roomIdToRoomData.Value);
            }
            return rooms;
        }

		private void SendSwitchingToRoomTypeNotification(RoomType roomType)
		{
			if (roomType == RoomType.GreenScreenRoom)
			{
				GameFacade.Instance.SendNotification(GameFacade.SWITCHING_TO_GREEN_SCREEN_ROOM);
			}
			else if (roomType == RoomType.MiniGameRoom)
			{
				//GameFacade.Instance.SendNotification(GameFacade.SWITCHING_TO_FASHION_FRENZY_MINI_GAME);
				throw new NotImplementedException();
			}
			else
			{
				throw new Exception("Switching to a room type not handled by this Send Notification function, room type: " + roomType);
			}
		}
    }

}
