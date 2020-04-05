using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client
{
    public static class RoomAPICommands
    {

        public static void DeleteRoom(RoomId roomId)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            Message deleteRoomMessage = new Message();
            deleteRoomMessage.Callback = (int)MessageSubType.Delete;
            List<object> messageData = new List<object>();
            messageData.Add(roomId);
            deleteRoomMessage.RoomMessage(messageData);

            clientMessageProcessor.SendMessageToReflector(deleteRoomMessage);
        }

        public static void SwitchRoom(RoomId newRoomIdToJoin, MessageSubType roomRequestType)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            Message switchRoomMessage = new Message();
            switchRoomMessage.Callback = (int)MessageSubType.SwitchRoom;
            List<object> messageData = new List<object>();
            messageData.Add(newRoomIdToJoin);
            messageData.Add(roomRequestType);
            switchRoomMessage.RoomMessage(messageData);

            clientMessageProcessor.SendMessageToReflector(switchRoomMessage);
        }

        //public static void JoinRoom(RoomId newRoomIdToJoin, MessageSubType roomRequestType)
        //{
        //    ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
        //    Message joinRoomMessage = new Message();
        //    joinRoomMessage.Callback = (int)MessageSubType.JoinRoom;
        //    List<object> messageData = new List<object>();
        //    messageData.Add(newRoomIdToJoin);
        //    messageData.Add(roomRequestType);
        //    joinRoomMessage.RoomMessage(messageData);

        //    clientMessageProcessor.SendMessageToReflector(joinRoomMessage);
        //}

        public static void CreateRoom(string roomName, RoomType roomType)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            Message createRoomMessage = new Message();
            createRoomMessage.Callback = (int)MessageSubType.CreateRoom;
            List<object> messageData = new List<object>();
            messageData.Add(roomName);
            messageData.Add(roomType);
            messageData.Add(PrivacyLevel.Private);
            createRoomMessage.RoomMessage(messageData);
            clientMessageProcessor.SendMessageToReflector(createRoomMessage);
        }

        //this is not used yet, however this iteration i have tasks related to this - matt
        public static void SearchRooms(string searchString, int startIndex, int blockSize)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            Message searchRoomMessage = new Message();
            searchRoomMessage.Callback = (int)MessageSubType.SearchRooms;
            List<object> messageData = new List<object>();
            messageData.Add(searchString);
            messageData.Add(startIndex);
            messageData.Add(blockSize);
            searchRoomMessage.RoomMessage(messageData);

            clientMessageProcessor.SendMessageToReflector(searchRoomMessage);
        }

        public static void RequestRoomsFromServer(MessageSubType roomRequestType)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            Message requestRoomsMessage = new Message();
            requestRoomsMessage.Callback = (int)MessageSubType.RequestRooms;
            List<object> messageData = new List<object>();
            messageData.Add(roomRequestType);
            requestRoomsMessage.RoomMessage(messageData);

            clientMessageProcessor.SendMessageToReflector(requestRoomsMessage);
        }

		public static void LeaveRoom(RoomId roomId)
		{
			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message leaveRoomMessage = new Message();
			leaveRoomMessage.Callback = (int)MessageSubType.LeaveRoom; 
			List<object> messageData = new List<object>();
			messageData.Add(roomId);
			leaveRoomMessage.RoomMessage(messageData);

			clientMessageProcessor.SendMessageToReflector(leaveRoomMessage);
		}
    }
}
