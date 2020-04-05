using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
	[Serializable]
	public class Message : IMessage
	{
        [NonSerialized]
        private static int mNextMsgId = 1;

        public int mId = 0;
        public int Id
        {
            get { return mId; }
            set { mId = value; }
        }

		public TimeSpan TimeSinceMessageWasCreated
		{
			get { return DateTime.Now - mMessageCreatedTimer; }
		}

		private bool mStoreInRam = false;
		private bool mBroadcast = false;
		private DistributedObjectId mDistributedObjectId = null;
		private int mCallback = -1;
		private List<object> mData = new List<object>();
		private MessageType mMessageType = new MessageType();
		private DateTime mMessageCreatedTimer = DateTime.Now;

		#region Message Members

		public Message() 
        {
            mId = mNextMsgId++;
        }

        public Message(MessageType type, List<object> data):this()
        {
            mMessageType = type;
            mData = data;
        }

        public Message(MessageType type, int callback, List<object> data):this()
        {
            mMessageType = type;
            mCallback = callback;
            mData = data;
        }

		public Message(MessageType type, MessageSubType messageSubType, List<object> data)
			: this()
		{
			mMessageType = type;
			mCallback = (int)messageSubType;
			mData = data;
		}

		public Message(SerializationInfo info, StreamingContext ctxt):this()
		{
			mBroadcast = info.GetBoolean("broadcast");
			mStoreInRam = info.GetBoolean("storeInRam");
            mDistributedObjectId = (DistributedObjectId)info.GetValue("objectId", typeof(DistributedObjectId));//info.GetUInt32("objectId");
			mCallback = info.GetInt32("callback");
			mData = (List<object>)info.GetValue("data", typeof(List<object>));
			mMessageType = (MessageType)info.GetValue("messageType", typeof(MessageType));
            mId = info.GetInt32("id");
		}

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Message ["+mId+"]:");

            result.Append("\tMessage Type:\t");
            result.Append(mMessageType.ToString());
            result.Append("\n");
            /*
            result.Append("\tStore In Ram:\t");
            result.Append(mStoreInRam);
            result.Append("\n");

            result.Append("\tBroadcast:\t");
            result.Append(mBroadcast);
            result.Append("\n");

            result.Append("\tObject ID:\t");
            result.Append(mDistributedObjectId);
            result.Append("\n");
            */
            result.Append("\tCallback:\t");
            result.Append(mCallback);
            result.Append("\n");

            result.Append("\tData:\n");
            foreach (object o in mData)
            {
                result.Append("\t\t");
                result.Append(o.ToString());
                result.Append("\n");
            }

            return result.ToString();
        }

        public void CreateObjectMessage(bool broadcastMessage, bool storeInRam, DistributedObjectId distributedObjectId, List<object> data)
		{
			mBroadcast = broadcastMessage;
			mStoreInRam = storeInRam;
			mDistributedObjectId = distributedObjectId;
			mData = data;
			mMessageType = MessageType.Create;
		}

		public void DeleteObjectMessage(bool broadcastMessage, DistributedObjectId distributedObjectId, List<object> data)
		{
			mBroadcast = broadcastMessage;
            mDistributedObjectId = distributedObjectId;
			mData = data;
			mMessageType = MessageType.Delete;
		}

		//The first argument in List<object>data will contain all the serializable data we want
		//Eg: Data = new List<object> ( float x, float y, float z, float x, float y, float z, float w );
        public void UpdateObjectMessage(bool broadcastMessage, bool storeInRam, DistributedObjectId distributedObjectId, int callback, List<object> data)
		{
			mBroadcast = broadcastMessage;
			mStoreInRam = storeInRam;
            mDistributedObjectId = distributedObjectId;
			mCallback = callback;
			mData = data;
			mMessageType = MessageType.Update;
        }

        public void LoadingMessage(bool broadcastMessage, bool storeInRam, List<object> data)
        {
            mBroadcast = broadcastMessage;
            mStoreInRam = storeInRam;
            mMessageType = MessageType.Loading;
            mData = data;
        }


		public void DisconnectMessage()
		{
			mMessageType = MessageType.Connect;
			mCallback = (int)MessageSubType.Disconnect;
		}

        public void RoomMessage(List<object> data)
        {
            mMessageType = MessageType.Room;
            mData = data;
        }


        public void FriendsMessage(List<object> data)
        {
            mMessageType = MessageType.Friends;
            mData = data;
        }

        public void PaymentItemsMessage(List<object> data)
        {
            mMessageType = MessageType.PaymentItems;
            mData = data;
        }

        public void AdminDataMessage(List<object> data)
        {
            mMessageType = MessageType.Admin;
            mData = data;
        }

		public void ErrorMessage(List<object> data)
		{
			mMessageType = MessageType.Error;
			mData = data;
        }

		public void FashionGameLoadingInfoMessage(List<object> data)
		{
			mCallback = (int)MessageSubType.RequestLoadingInfo;
			mMessageType = MessageType.FashionMinigame;
			mData = data;
        }

		public void FashionGameModelAssetMessage(List<object> data)
		{
			mCallback = (int)MessageSubType.RequestNpcAssets;
			mMessageType = MessageType.FashionMinigame;
			mData = data;
        }

		public void FashionGameGetData(List<object> data)
		{
			mCallback = (int)MessageSubType.GetPlayerData;
			mMessageType = MessageType.FashionMinigame;
			mData = data;
		}

		public void FashionGameSetData(List<object> data)
		{
			mCallback = (int)MessageSubType.SetPlayerData;
			mMessageType = MessageType.FashionMinigame;
			mData = data;
        }

        public void FashionGameLevelComplete(List<object> data)
        {
			mCallback = (int)MessageSubType.LevelComplete;
            mMessageType = MessageType.FashionMinigame;
            mData = data;
        }

		public void FashionGameFriendsToHire(List<object> data)
		{
			mCallback = (int)MessageSubType.GetFriendsToHire;
			mMessageType = MessageType.FashionMinigame;
			mData = data;
        }

		public void FashionGameHireFriend(List<object> data)
		{
			mCallback = (int)MessageSubType.HireFriend;
			mMessageType = MessageType.FashionMinigame;
			mData = data;
        }		

		public void AccountMessage(List<object> data)
		{
			mMessageType = MessageType.Account;
			mData = data;
        }

		public void EscrowMessage(List<object> data)
		{
			mMessageType = MessageType.Escrow;
			mData = data;
		}

		#endregion

		#region IMessage Members

		public bool Broadcast
		{
			get { return mBroadcast; }
		}

		public bool StoreInRam
		{
			get { return mStoreInRam; }
		}

		public DistributedObjectId DistributedObjectId
		{
			get { return mDistributedObjectId; }
		}

		public int Callback
		{
			get { return mCallback; }
			set { mCallback = value; }
		}

		public List<object> Data
		{
			get { return mData; }
		}
		public MessageType MessageType
		{
			get { return mMessageType; }
		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("broadcast", mBroadcast);
			info.AddValue("storeInRam", mStoreInRam);
			info.AddValue("objectId", mDistributedObjectId);
			info.AddValue("callback", mCallback);
			info.AddValue("data", mData);
			info.AddValue("messageType", mMessageType);
            info.AddValue("id", mId);
		}

		#endregion
	}
}
