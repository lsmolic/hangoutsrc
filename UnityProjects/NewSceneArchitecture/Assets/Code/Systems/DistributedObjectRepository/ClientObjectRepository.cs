using System;
using System.Collections.Generic;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
    public delegate void RequestDistributedObjectReference(DistributedObjectId requestedDistributedObjectId, System.Action<IClientDistributedObject> retrieveDistributedObjectCallback);
    public class ClientObjectRepository : ObjectRepository, IMessageRouter
    {

		private SendMessageCallback mSendMessage = null;
		private DistributedObjectFactory mDistributedObjectFactory = null;
		
        //this list is to cache distributed object ids to callbacks.. 
        // so if a distributed object A needs a reference to distributed object B, but B wasn't loaded yet,
        // then B's distributed object id would be cached in association with A's callback
        private Dictionary<DistributedObjectId, List<System.Action<IClientDistributedObject>> > mAnticipatedDistributedObjectIdsToCallbacks = null;
		
        /// <summary>
        /// Created by the ClientMessageProcessor after making a connection to the server,
        /// so we have a sessionId
        /// </summary>
        /// <param name="loadingManager"></param>
        /// <param name="localSessionId"></param>
		public ClientObjectRepository( Guid localSessionId )
		{
            mAnticipatedDistributedObjectIdsToCallbacks = new Dictionary< DistributedObjectId, List<System.Action<IClientDistributedObject>> >();
			//What we are about to do here is temporary.  When we figure out how the server
			//is going to send us assets we MUST change this to make sense.  Palabras motha muchachos.

            mDistributedObjectFactory = new DistributedObjectFactory(localSessionId);
		}
		
		/*******************************/		
		// IMessageRouter Functions
		/*******************************/		
		public void ReceiveMessage( Message message )
		{
	
			switch(message.MessageType)
			{
                case MessageType.Create:
					IClientDistributedObject distributedObject = mDistributedObjectFactory.CreateDistributedObject( message.DistributedObjectId, message.Data );
					if ( distributedObject != null )
					{
                        this.AddObject(distributedObject);
                        List<System.Action<IClientDistributedObject>> requestedObjectCallbacks = null;
                        if (mAnticipatedDistributedObjectIdsToCallbacks.TryGetValue(distributedObject.DistributedObjectId, out requestedObjectCallbacks))
                        {
                            foreach (System.Action<IClientDistributedObject> requestedObjectCallback in requestedObjectCallbacks)
                            {
                                requestedObjectCallback(distributedObject);
                            }
                            mAnticipatedDistributedObjectIdsToCallbacks.Remove(distributedObject.DistributedObjectId);
                        }
					}
					break;

				case MessageType.Update:
					DistributedObjectId doId = message.DistributedObjectId;
					IDistributedObject updateObject = null;
					if ( mObjectIds.TryGetValue(doId, out updateObject) ) 
					{
						updateObject.ProcessMessage(message);
					}
					else
					{
						Console.LogError("Message update but objectId " + doId + " not found in repository");
					}
					break;
					
				case MessageType.Delete:
					DistributedObjectId deletedoId = message.DistributedObjectId;
					IDistributedObject deleteObject;
					
					if ( mObjectIds.TryGetValue(deletedoId, out deleteObject) ) 
					{	
						if ( deleteObject is ClientDistributedObject )
						{
							ClientDistributedObject deleteClientObject = deleteObject as ClientDistributedObject;
							if ( deleteClientObject != null )
							{
								deleteClientObject.Dispose();
							}
						}
                        this.RemoveObject(deleteObject);
					}
					else
					{
						Console.LogError("Message delete but objectId " + deletedoId.ToString() + " not found in repository");
					}
					break;
			}
		}
		
		public void RegisterSendMessageCallback( SendMessageCallback sendMessageCallback )
		{
			mSendMessage = sendMessageCallback;
			mDistributedObjectFactory.RegisterSendMessageCallback(mSendMessage);
		}
		
		//Take in an entity message.
		public void ReceiveEntityUpdateMessage ( EntityUpdateMessage entityUpdateMessage )
		{
			ProcessEntityUpdateMessage();
		}
		//Process the entity update message, turn it into a normal message and route it to the processor to head to the server.
		private void ProcessEntityUpdateMessage()
		{
			throw new NotImplementedException();
		}

        private void GetDistributedObjectReference(DistributedObjectId requestedDistributedObjectId, System.Action<IClientDistributedObject> retrieveDistributedObjectCallback)
        {
            IClientDistributedObject clientDistributedObject = this.GetObject(requestedDistributedObjectId) as IClientDistributedObject;
            //if the resulting object is null, we need to cache this request for when the object is eventually downloaded
            if (clientDistributedObject == null)
            {
                if (!mAnticipatedDistributedObjectIdsToCallbacks.ContainsKey(requestedDistributedObjectId))
                {
                    mAnticipatedDistributedObjectIdsToCallbacks.Add(requestedDistributedObjectId, new List<System.Action<IClientDistributedObject>>());
                }
                mAnticipatedDistributedObjectIdsToCallbacks[requestedDistributedObjectId].Add(retrieveDistributedObjectCallback);
            }
            else
            {
                retrieveDistributedObjectCallback(clientDistributedObject);
            }
        }

		public void DestroyAllDistributedObjects()
		{
			foreach(KeyValuePair<DistributedObjectId, IDistributedObject> distributedObjectIdToDistributedObject in this.mObjectIds)
			{
				distributedObjectIdToDistributedObject.Value.Dispose();
			}
			mObjectIds.Clear();
		}
    }
}
