using System;
using System.Collections.Generic;
using Hangout.Shared;


namespace Hangout.Client
{
	public class DistributedObjectFactory
	{
		private Guid mLocalSessionId = Guid.Empty;
		private SendMessageCallback mSendMessage;

		public DistributedObjectFactory(Guid localSessionId)
		{
			mLocalSessionId = localSessionId;
		}

		public void RegisterSendMessageCallback(SendMessageCallback sendMessageCallback)
		{
			mSendMessage = sendMessageCallback;
		}

		public IClientDistributedObject CreateDistributedObject(DistributedObjectId distributedObjectId, List<object> messageData)
		{
			DistributedObjectTypes type = CheckType.TryAssignType<DistributedObjectTypes>(messageData[0]);

			IClientDistributedObject clientDistributedObject = null;
			switch (type)
			{
				case DistributedObjectTypes.Avatar:
					Guid localSessionId = CheckType.TryAssignType<Guid>(messageData[2]);
					if (mLocalSessionId == localSessionId)
					{
						clientDistributedObject = BuildLocalAvatarDistributedObject(distributedObjectId, messageData);
					}
					else
					{
						clientDistributedObject = BuildForeignAvatarDistributedObject(distributedObjectId, messageData);
					}
					break;
				case DistributedObjectTypes.Room:
					clientDistributedObject = BuildRoomDistributedObject(distributedObjectId, messageData);
					break;
				case DistributedObjectTypes.MockObject:
					clientDistributedObject = BuildMockDistributedObject(distributedObjectId, messageData);
					break;
				default:
					throw new NotImplementedException(type.ToString());
			}
			return clientDistributedObject;
		}

		private AvatarDistributedObject BuildLocalAvatarDistributedObject(DistributedObjectId id, List<object> messageData)
		{
			AvatarDistributedObject av = new LocalAvatarDistributedObject(mSendMessage, id, messageData);
			GameFacade.Instance.RegisterProxy(new LocalAvatarProxy((LocalAvatarDistributedObject)av));
			return av;
		}

		private AvatarDistributedObject BuildForeignAvatarDistributedObject(DistributedObjectId id, List<object> messageData)
		{
			AvatarDistributedObject av = new ForeignAvatarDistributedObject(mSendMessage, id, messageData);
			return av;
		}

		private IClientDistributedRoom BuildRoomDistributedObject(DistributedObjectId id, List<object> messageData)
		{
			RoomType roomType = CheckType.TryAssignType<RoomType>(messageData[2]);

			IClientDistributedRoom clientDistributedRoom = null;
			switch (roomType)
			{
				case RoomType.GreenScreenRoom:
					clientDistributedRoom = new ClientDistributedGreenScreenRoom(mSendMessage, id, messageData);
					break;
				case RoomType.MiniGameRoom:
					throw new System.Exception("we shouldn't be creating these anymore in this way!! talk to matt!");
			}
			RoomManagerProxy roomManagerProxy = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>();
			roomManagerProxy.UpdateCurrentRoomReference(clientDistributedRoom);
			return clientDistributedRoom;
		}

		private IClientDistributedObject BuildMockDistributedObject(DistributedObjectId id, List<object> messageData)
		{
			return new ClientMockDistributedObject(mSendMessage, id);
		}
	}
}