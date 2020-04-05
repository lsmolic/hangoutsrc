using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Server
{
    public class ServerObjectRepository : ObjectRepository, IServerExtension
    {
		private readonly ServerStateMachine mServerStateMachine;
        //   Maintaining object zone information
        //   Maintaining a class dictionary of name->class for object creation

        protected readonly Dictionary<Guid, List<DistributedObjectId>> mSessionIdsToDistributedObjectIds = new Dictionary<Guid, List<DistributedObjectId>>();

        // Dictionary that maintains zone of each object [doId -> zone]
        protected readonly Dictionary<DistributedObjectId, ZoneId> mDistributedObjectIdToObjectZones = new Dictionary<DistributedObjectId, ZoneId>();

        // Maps every known zone to the list of objects in that zone
        // Note: zone 0 is the null zone, so there will never be a list of objects there.
        protected readonly Dictionary<ZoneId, List<IServerDistributedObject>> mZoneDict = new Dictionary<ZoneId, List<IServerDistributedObject>>();

        public ServerObjectRepository(ServerStateMachine serverStateMachine)
        {
			mServerStateMachine = serverStateMachine;
        }

		public void Init()
		{

		}

		public void Dispose()
		{

		}

        // This object is changing zones, so record the new zone
        public void SetObjectZone(IServerDistributedObject obj, ZoneId zone)
        {
            // Record this object as moving into this new zone.
            // Cleanup any references to the old zone it was in.

            DistributedObjectId doId = obj.DistributedObjectId;

            // First grab the old zone		
            ZoneId oldZone = null;
            mDistributedObjectIdToObjectZones.TryGetValue(doId, out oldZone);

            // Now store the new zone
            mDistributedObjectIdToObjectZones[doId] = zone;

            // If it had a zone before, remove it from the ZoneDict's list
            if (oldZone != null)
            {
                // Remove obj from the old zone list
                List<IServerDistributedObject> objList = null;
                if (mZoneDict.TryGetValue(oldZone, out objList))
                {
                    objList.Remove(obj);
                    // We don't remove an empty list here because the assumption
                    // is that it will be used again in the near future...
                }
            }

            // Now add this object to the ZoneDict's list in the new zone
            if (zone != null)
            {
                // Create an object list for this zone if it is not there already...
                List<IServerDistributedObject> objList = null;
                if (!mZoneDict.TryGetValue(zone, out objList))
                {
                    // New zone being used, create a new list
                    objList = new List<IServerDistributedObject>();
                    mZoneDict.Add(zone, objList);
                }
                objList.Add(obj);
            }
        }

        // Returns the zone that this object is currently in.
        public ZoneId GetZone(DistributedObjectId doId)
        {
            ZoneId zoneId = null;
            mDistributedObjectIdToObjectZones.TryGetValue(doId, out zoneId);
            return zoneId;
        }

        // Returns the zone that this object is currently in.
        public ZoneId GetZone(IServerDistributedObject obj)
        {
            ZoneId zoneId = null;
            mDistributedObjectIdToObjectZones.TryGetValue(obj.DistributedObjectId, out zoneId);
            return zoneId;
        }

        // Returns the list of all objects in this zone.
        public List<IServerDistributedObject> GetObjectsInZone(ZoneId zone)
        {
            List<IServerDistributedObject> objList = null;
            if (!mZoneDict.TryGetValue(zone, out objList))
            {
                objList = new List<IServerDistributedObject>();
            }
            return objList;
        }

		public void ReceiveRequest(Message receivedMessage, Guid senderId)
        {
            switch (receivedMessage.MessageType)
            {
                case MessageType.Update:
					if (receivedMessage.DistributedObjectId == null)
					{
						StateServerAssert.Assert(new System.Exception("receivedMessage.DistributedObjectId is null"));
					}
					else
					{
						IServerDistributedObject serverDistributedObject = (IServerDistributedObject)this.GetObject(receivedMessage.DistributedObjectId);
						if (serverDistributedObject == null)
						{
							Console.WriteLine("WARNING: ReceiveMessage for object not on server: %i", receivedMessage.DistributedObjectId);
						}
						else
						{
							serverDistributedObject.ProcessMessage(receivedMessage);
						}
					}
                    break;
            }
        }

        public void BroadcastDistributedObjectUpdateMessageToZone(Message message, DistributedObjectId serverDistributedObjectId)
        {
            ZoneId zoneId = this.GetZone(serverDistributedObjectId);
            List<Guid> sessionIdsToSendMessage = mServerStateMachine.SessionManager.GetSessionIdsInterestedInZoneId(zoneId);
            mServerStateMachine.SendMessageToReflector(message, sessionIdsToSendMessage);
        }

        public void SendDistributedObjectStateUpdate(IServerDistributedObject distributedObject)
        {
            Message distributedObjectStateUpdateMessage = null;
            if (distributedObject.DistributedObjectStateUpdate(out distributedObjectStateUpdateMessage))
            {
                BroadcastDistributedObjectUpdateMessageToZone(distributedObjectStateUpdateMessage, distributedObject.DistributedObjectId);
            }
        }

        // Add this DistributedObject to the repository.
        public void AddObjectToSessionId(Guid sessionId, IServerDistributedObject obj)
        {

            if (!mSessionIdsToDistributedObjectIds.ContainsKey(sessionId))
            {
                mSessionIdsToDistributedObjectIds[sessionId] = new List<DistributedObjectId>();
            }
            DistributedObjectId doId = obj.DistributedObjectId;

            mSessionIdsToDistributedObjectIds[sessionId].Add(doId);
            // Store in the dictionary of objects
            mObjectIds[doId] = obj;
        }

        //cleans up all distributed objects associated with this session id
        public void RemoveSessionId(Guid sessionId)
        {
            List<DistributedObjectId> distributedObjectIdsAssociatedWithThisSessionId = null;
            if (mSessionIdsToDistributedObjectIds.TryGetValue(sessionId, out distributedObjectIdsAssociatedWithThisSessionId))
            {
                List<IServerDistributedObject> distributedObjectsToRemove = new List<IServerDistributedObject>();
                foreach (DistributedObjectId distributedObjectIdToRemove in distributedObjectIdsAssociatedWithThisSessionId)
                {
                    distributedObjectsToRemove.Add((IServerDistributedObject)mObjectIds[distributedObjectIdToRemove]);
                }

                foreach (IServerDistributedObject distributedObject in distributedObjectsToRemove)
                {
                    RemoveObject(distributedObject);
                }
                mSessionIdsToDistributedObjectIds.Remove(sessionId);
            }
        }

        public List<DistributedObjectId> GetDistributedObjectIdsOwnedBySessionId(Guid sessionId)
        {
            List<DistributedObjectId> distributedObjectIds = new List<DistributedObjectId>();
            if (mSessionIdsToDistributedObjectIds.ContainsKey(sessionId))
            {
                distributedObjectIds = mSessionIdsToDistributedObjectIds[sessionId];
            }
            return distributedObjectIds;
        }

                // Remove this DistributedObject from the repository.
        public override void RemoveObject(IDistributedObject obj)
        {
            if (obj.GetType().IsAssignableFrom(typeof(IServerDistributedObject)))
            {
                // Upcast to server version so we get the extra cleanup
                this.RemoveObject((IServerDistributedObject)obj);
            } else {
                // Just call the base class version
                base.RemoveObject(obj);
            }
        }

        public void RemoveObject(IServerDistributedObject obj)
        {

            base.RemoveObject(obj);

            DistributedObjectId doId = obj.DistributedObjectId;

            // Remove from object zones dict
            ZoneId zone = null;
            mDistributedObjectIdToObjectZones.TryGetValue(doId, out zone);
            mDistributedObjectIdToObjectZones.Remove(doId);			

            // Remove from zone dict
            List<IServerDistributedObject> objList = null;
            if (zone != null && mZoneDict.TryGetValue(zone, out objList))
            {
                objList.Remove(obj);
            }

            //remove from mSessionIdsToDistributedObjectIds
            foreach (KeyValuePair<Guid, List<DistributedObjectId>> sessionIdToDistributedObjectIds in mSessionIdsToDistributedObjectIds)
            {
                sessionIdToDistributedObjectIds.Value.Remove(doId);
            }
        }

        public override string ToString()
        {
            string result = "<Zones>";
            foreach (ZoneId key in mZoneDict.Keys)
            {
                result += String.Format("<Zone id = \'{0}\'>", key.Value);
                List<IServerDistributedObject> distObjList = mZoneDict[key];
                foreach (IServerDistributedObject distObj in distObjList)
                {
                    string typeName = distObj.GetType().Name;
                    result += String.Format("<{0} id = \'{1}\'></{0}>", typeName, distObj.DistributedObjectId.Value);
                }
                result += "</Zone>";
            }
            result += "</Zones>";
            return result;
        }
    }
}