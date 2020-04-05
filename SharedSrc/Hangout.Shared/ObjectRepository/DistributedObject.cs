using System;
using System.Collections.Generic;
using Hangout.Shared;


namespace Hangout.Shared
{

	public abstract class DistributedObject : IDistributedObject
	{
        protected List<object> mObjectData = new List<object>();

        protected DistributedObjectTypes mObjectType;

		// Maps message names to Action deleagtes to call when receiving message
		protected Dictionary<int, Action<Message>> mMessageActions = new Dictionary<int, Action<Message>>();

		// The unique object id this DistributedObject is referenced by in all systems
		protected DistributedObjectId mDistributedObjectId = null;

		// Stores the most recent update received for every ram message on this object
		protected Dictionary<int, Message> mMessageState = new Dictionary<int, Message>();

		// Override this function to register your message actions all at once.
		// This is called in the constructor.
		protected abstract void RegisterMessageActions();

		public List<object> Data
		{
			get
			{
                return mObjectData;
			}
		}

		public DistributedObject(DistributedObjectId doId)
		{
			mDistributedObjectId = doId;
			RegisterMessageActions();
		}

		public DistributedObjectId DistributedObjectId
		{
			get { return mDistributedObjectId; }
		}

		protected void RegisterMessageAction(int actionIndex, Action<Message> callback)
		{
			mMessageActions[actionIndex] = callback;
		}

		public abstract void ProcessMessage(Message msg);

		public abstract void Dispose();
	}
}