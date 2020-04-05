using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

using Hangout.Client.FashionGame;

namespace Hangout.Client
{
    public abstract class ClientDistributedMiniGameRoom : ClientDistributedObject, IClientDistributedRoom
    {
        private RoomType mRoomType = RoomType.MiniGameRoom;
		private PrivacyLevel mPrivacyLevel = PrivacyLevel.Private;
		

        public PrivacyLevel PrivacyLevel
        {
			get { return mPrivacyLevel; }
        }

        public string RoomName
        {
            get { return "FashionMinigame"; }
        }

        public RoomId RoomId
        {
            get { throw new NotImplementedException(); }
        }

        public RoomType RoomType
        {
            get { return mRoomType; }
        }

        public ClientDistributedMiniGameRoom(SendMessageCallback sendMessage, DistributedObjectId doId, List<object> messageData)
            : base(sendMessage, doId)
        {
            BuildEntity();
        }

        protected override void RegisterMessageActions()
        {
        }
    }
}
