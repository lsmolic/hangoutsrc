using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Hangout.Shared;
using Hangout.Shared.Messages;

namespace Hangout.Client
{
    public class RoomManagerProxy : Proxy
    {
        // Hardcode the default room for now.  This is my room.  Change this to 2 after Lucas has modified the GetRooms service to 
        // also look at SystemRooms
        private RoomId mDefaultPublicRoomId = new RoomId(2);

        private DistributedObjectId mCurrentRoomDistributedObjectId;
        private IClientDistributedRoom mCurrentRoom = null;
        private Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();

        public RoomManagerProxy()
        {
            mMessageActions.Add(MessageSubType.LoadNewRoom, BeginRoomLoading);
        }

        public DistributedObjectId CurrentRoomDistributedObjectId
        {
            get { return mCurrentRoomDistributedObjectId; }
        }

        public IClientDistributedRoom CurrentRoom
        {
            get { return mCurrentRoom; }
        }

        public void ReceiveMessage(Message receivedMessage)
        {
            MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
        }

        private void BeginRoomLoading(Message receivedMessage)
        {
            RoomId currentRoomId = CheckType.TryAssignType<RoomId>(receivedMessage.Data[0]);
            mCurrentRoomDistributedObjectId = CheckType.TryAssignType<DistributedObjectId>(receivedMessage.Data[1]);

            GameFacade.Instance.SendNotification(GameFacade.ROOM_LOADING_STARTED, currentRoomId);
        }

        public void UpdateCurrentRoomReference(IClientDistributedRoom room)
        {
            mCurrentRoom = room;
			UserAccountProxy userAccountProxy = GameFacade.Instance.RetrieveProxy<UserAccountProxy>();
			userAccountProxy.SetAccountProperty<RoomId>(UserAccountProperties.LastRoomId, mCurrentRoom.RoomId);
        }

		public void RoomComplete()
		{
			if( mCurrentRoom == null )
			{
				throw new Exception("Cannot set room complete if the current room reference was never set");
			}
            GameFacade.Instance.RetrieveMediator<ToolbarMediator>().SetCurrentLocation(mCurrentRoom.RoomName);

		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomBackgroundItemId"></param>
        public void UpdateCurrentRoomBackground(ItemId roomBackgroundItemId)
        {
            if (mCurrentRoom != null)
            {
                if (mCurrentRoom.RoomType == RoomType.GreenScreenRoom)
                {
                    ClientDistributedGreenScreenRoom greenScreenRoom = mCurrentRoom as ClientDistributedGreenScreenRoom;
                    // Only allow this change if the room is owned by the local user
                    if (greenScreenRoom.IsLocalClientOwnedRoom)
                    {
                        greenScreenRoom.UpdateBackgroundImage(roomBackgroundItemId);
                    }
                }
            }
            else
            {
                throw new System.Exception("Error: the current room is null");
            }
     
        }

        /// <summary>
        /// Join last room user was in.  If user hasn't ever been in a room, put him in a default public room
        /// </summary>
        public void JoinLastRoom()
        {
            UserAccountProxy userAccountProxy = GameFacade.Instance.RetrieveProxy<UserAccountProxy>();

			RoomId lastVisitedRoom = null;

            // Try and get last room from user properties, if it doesn't exist, use the default hardcoded room
            if(!userAccountProxy.TryGetAccountProperty<RoomId>(UserAccountProperties.LastRoomId, ref lastVisitedRoom))
			{
				lastVisitedRoom = mDefaultPublicRoomId;
			}

            Console.WriteLine("Going to last visited room: " + lastVisitedRoom.ToString());
            RoomAPICommands.SwitchRoom(lastVisitedRoom, MessageSubType.ClientOwnedRooms);
            GameFacade.Instance.SendNotification(GameFacade.SWITCHING_TO_GREEN_SCREEN_ROOM);
        }
        
        public bool IsRoomOwner()
        {
			ClientDistributedGreenScreenRoom room = CurrentRoom as ClientDistributedGreenScreenRoom;
			if (room != null && room.IsLocalClientOwnedRoom)
			{
				return true;
			}
			else
			{
				return false;
			}
        }
    }
}
