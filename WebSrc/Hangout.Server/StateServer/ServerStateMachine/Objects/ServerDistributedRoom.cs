using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;

namespace Hangout.Server
{
    public class ServerDistributedRoom : ServerDistributedObject, IServerDistributedRoom
    {
        private string mRoomName = string.Empty;
        private RoomId mRoomId = null;
        private RoomType mRoomType;
        private PrivacyLevel mPrivacyLevel = PrivacyLevel.Default;
        private List<Guid> mRoomPopulation;

        public List<Guid> Population
        {
            get { return mRoomPopulation; }
        }
        
        public PrivacyLevel PrivacyLevel
        {
            get { return mPrivacyLevel; }
        }

        public string RoomName
        {
            get { return mRoomName; }
        }

        public RoomId RoomId
        {
            get { return mRoomId; }
        }

        public RoomType RoomType
        {
            get { return mRoomType; }
        }

        public ServerDistributedRoom(ServerObjectRepository serverObjectRepository, AccountId roomOwnerAccountId, string roomName, RoomType roomType, RoomId roomId, PrivacyLevel privacyLevel, DistributedObjectId doId, XmlNode itemIdXml)
            : base(serverObjectRepository, doId)
        {
            mRoomPopulation = new List<Guid>();
            mRoomName = roomName;
            mRoomType = roomType;
            mRoomId = roomId;
            mObjectType = DistributedObjectTypes.Room;
            mPrivacyLevel = privacyLevel;

            mObjectData.Add(mObjectType); //0
            mObjectData.Add("mock path");
            mObjectData.Add(mRoomType);
            mObjectData.Add(itemIdXml.OuterXml);
            mObjectData.Add(mPrivacyLevel);
            mObjectData.Add(mRoomName); //5
            mObjectData.Add(mRoomPopulation.Count);
            mObjectData.Add(mRoomId);
			mObjectData.Add(roomOwnerAccountId);
        }

        public void IncrementPopulation(Guid sessionIdToJoinRoom)
        {
            mRoomPopulation.Add(sessionIdToJoinRoom);
            UpdatePopulationInRoomData();
        }

        public void DecrementPopulation(Guid sessionIdToLeaveRoom)
        {
            mRoomPopulation.Remove(sessionIdToLeaveRoom);
            UpdatePopulationInRoomData();
        }

        private void UpdatePopulationInRoomData()
        {
            mObjectData[6] = mRoomPopulation.Count;
        }

        public bool ContainsUser(Guid sessionId)
        {
            return mRoomPopulation.Contains(sessionId);
        }

        protected override void RegisterMessageActions()
        {
        }

    }
}
