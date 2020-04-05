using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public class ServerEngine
    {
        protected static ILog mLogger = LogManager.GetLogger("ServerEngine");

        SessionManager mSessionManager = null;
        ServerObjectRepository mServerObjectRepository = null;
        Hangout.Shared.Action<Message, List<Guid>> mSendMessageToReflectorCallback = null;


        public ServerEngine(SessionManager sessionManager, ServerObjectRepository serverObjectRepository, Hangout.Shared.Action<Message, List<Guid>> sendMessageToReflectorCallback)
        {
            mSessionManager = sessionManager;
            mServerObjectRepository = serverObjectRepository;
            mSendMessageToReflectorCallback = sendMessageToReflectorCallback;
        }
        
        //broadcasts the message to all session ids listening to that zone
        public ZoneId RemoveObjectFromZone(IServerDistributedObject obj)
        {
            ZoneId zoneId = mServerObjectRepository.GetZone(obj);

            if (zoneId == null)
            {
                mLogger.Warn("object " + obj.DistributedObjectId + " was already in null zone");
                return null;
            }

            // Sessions interested in the zone this object is being deleted from
            List<Guid> sessionsInterestedInZone = mSessionManager.GetSessionIdsInterestedInZoneId(zoneId);
            SendDestroyObjectMessage(obj, sessionsInterestedInZone);
            mServerObjectRepository.SetObjectZone(obj, null);
            return zoneId;
        }

        public void RemoveObjectFromServer(IServerDistributedObject obj)
        {
            RemoveObjectFromZone(obj);
            mServerObjectRepository.RemoveObject(obj);
        }

        public void ProcessZoneChange(IServerDistributedObject obj, ZoneId zoneId) 
		{
            if (zoneId == null)
            {
                mLogger.Error("ZoneId is null!!");
                return;
            }

			//Console.WriteLine("ProcessZoneChange objId = " + obj.DistributedObjectId + " newZoneId = " + zoneId);

            ZoneId oldZoneId = mServerObjectRepository.GetZone(obj);

			//Console.WriteLine("old zone = " + oldZoneId);
			
            if (zoneId == oldZoneId) 
            {
                mLogger.Warn("Object wants to change to zone it is already in.");
                return;
            }

			// Sessions interested in the zone this object came from
			List<Guid> sessionsInterestedInOldZone = new List<Guid>();
            if (oldZoneId != null)
            {
                sessionsInterestedInOldZone = mSessionManager.GetSessionIdsInterestedInZoneId(oldZoneId); 
            }
			
			// Sessions interested in the zone this object is going to
			List<Guid> sessionsInterestedInNewZone = mSessionManager.GetSessionIdsInterestedInZoneId(zoneId);

			// Record the new zone on this object in the repository
			mServerObjectRepository.SetObjectZone(obj, zoneId);
			
			// Send object create messages to all sessions interested in the new zone
			// but were not interested in the old zone
			
			List<Guid> sessionsNeedingCreates = new List<Guid>();
            List<Guid> sessionsNeedingDeletes = new List<Guid>();
			List<Guid> sessionsNeedingZoneChanges = new List<Guid>();
			

			foreach (Guid sessionId in sessionsInterestedInNewZone) 
			{
				//if no one is interested in the old zone, then we just need to send create messages
				if (sessionsInterestedInOldZone == null || !sessionsInterestedInOldZone.Contains(sessionId)) 
				{
					sessionsNeedingCreates.Add(sessionId);
				}
                else
                {
                    sessionsNeedingZoneChanges.Add(sessionId);
                }
			}
			SendCreateObjectMessage(obj, sessionsNeedingCreates);
            mServerObjectRepository.SendDistributedObjectStateUpdate(obj);
           
            foreach(Guid sessionId in sessionsInterestedInOldZone)
            {
                //if theres a client who was interested in the old zone but is not interested in the zone this object is being moved to, we need to delete it for that client
                if (!sessionsInterestedInNewZone.Contains(sessionId))
                {
                    sessionsNeedingDeletes.Add(sessionId);
                }
            }
            SendDestroyObjectMessage(obj, sessionsNeedingDeletes);
		}
		
        public void ProcessOpenZoneInterest(Guid sessionId, ZoneId zoneId)
		{
            if (zoneId == null)
            {
                mLogger.Error("ZoneId is null!!");
                return;
            }

			mLogger.Debug("sessionId: " + sessionId + " is now interested in zoneId: " + zoneId);
			if (mSessionManager.SessionIdHasInterest(sessionId, zoneId)) 
			{
                mLogger.Warn(String.Format("sessionId: {0} already has interest in zoneId: {1}", sessionId, zoneId));
				return;
			}
			
			// Record the new zone interest in our dictionaries
			mSessionManager.AddZoneIdInterestToSessionId(sessionId, zoneId);
			
			List<IServerDistributedObject> objects = mServerObjectRepository.GetObjectsInZone(zoneId);
			List<Guid> sessionIds = new List<Guid>();
			sessionIds.Add(sessionId);
			foreach (IServerDistributedObject obj in objects) 
			{
                mLogger.Debug("ProcessOpenInterest: sending create for objId " + obj.DistributedObjectId + " to session: " + sessionId);
                SendCreateObjectMessage(obj, sessionIds);
                mServerObjectRepository.SendDistributedObjectStateUpdate((ServerDistributedObject)obj);
			}	
		}

        private void SendCreateObjectMessage(IServerDistributedObject obj, List<Guid> sessionIds)
        {
            Message createMessage = new Message();
            createMessage.CreateObjectMessage(false, false, obj.DistributedObjectId, obj.Data);
            mSendMessageToReflectorCallback(createMessage, sessionIds);
        }

        private void SendDestroyObjectMessage(IServerDistributedObject obj, Guid sessionId)
        {
            List<Guid> sessionIds = new List<Guid>();
            sessionIds.Add(sessionId);
            SendDestroyObjectMessage(obj, sessionIds);
        }

        private void SendDestroyObjectMessage(IServerDistributedObject obj, List<Guid> sessionIds)
        {
            Message deleteObjectMessage = new Message();
            deleteObjectMessage.DeleteObjectMessage(false, obj.DistributedObjectId, obj.Data);
            mSendMessageToReflectorCallback(deleteObjectMessage, sessionIds);
        }

        public void ProcessCloseZoneInterest(Guid sessionIdToClose, ZoneId zoneId) 
        {
            List<IServerDistributedObject> distributedObjectsInZone = mServerObjectRepository.GetObjectsInZone(zoneId);
            foreach (IServerDistributedObject distributedObject in distributedObjectsInZone)
            {
                this.SendDestroyObjectMessage(distributedObject, sessionIdToClose);
            }
            mSessionManager.RemoveZoneIdInterestFromSessionId(sessionIdToClose, zoneId);
        }

        public void ProcessCloseZoneInterest(Guid sessionIdToClose, List<ZoneId> oldZoneIds)
        {
            //what should we do with objects that the client owns that are in the zone the client is removing interest from???!?
            // this may potentially be an error.

            foreach (ZoneId oldZone in oldZoneIds)
            {
                ProcessCloseZoneInterest(sessionIdToClose, oldZone);
            }
        }

		/// <summary>
        /// this is when a session is removed from the system, we need to find which zone's they were interested in and remove them from those zones
		/// </summary>
		/// <param name="sessionIdToClose"></param>
		public void ProcessDisconnectSession(Guid sessionIdToClose)
		{
            //find the distributed objects owned by the session
            List<DistributedObjectId> distributedObjectsOwnedBySession = mServerObjectRepository.GetDistributedObjectIdsOwnedBySessionId(sessionIdToClose);

            //find the zones this session was interested in...
            List<ZoneId> zonesToDisconnectSessionFrom = mSessionManager.GetZoneIdsFromInterestedSessionId(sessionIdToClose);
            foreach(ZoneId zoneId in zonesToDisconnectSessionFrom)
            {
                //...find everyone else in that zone...
                List<Guid> connectedSessionsInZone = mSessionManager.GetSessionIdsInterestedInZoneId(zoneId);
                //we remove the session to close from this list so we don't broadcast a message to a diconnected session
                connectedSessionsInZone.Remove(sessionIdToClose); 

                //...broadcast an object delete for all the objects belonging to the disconnected session id...
                foreach (DistributedObjectId distributedObjectId in distributedObjectsOwnedBySession)
                {
                    IServerDistributedObject distributedObjectToDelete = (IServerDistributedObject)mServerObjectRepository.GetObject(distributedObjectId);
                    this.SendDestroyObjectMessage(distributedObjectToDelete, connectedSessionsInZone);
                }
            }

            //take care of the backend cleanup
            mSessionManager.RemoveSession(sessionIdToClose);
            mServerObjectRepository.RemoveSessionId(sessionIdToClose);
		}
    }
}
