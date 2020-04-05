using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client
{
    public abstract class ClientDistributedObject : DistributedObject, IClientDistributedObject
	{
		private IEntity mEntity;
		public IEntity Entity 
		{
			get { return mEntity; }
			//All objects inheriting CDO set this when the BuildEntity func executes.
			protected set { mEntity = value; }
		}
		
		private SendMessageCallback mSendMessage;
		protected SendMessageCallback SendMessage
		{
			get { return mSendMessage; }
		}

		private List<ITask> mCoroutines = new List<ITask>();
		protected List<ITask> Coroutines
		{
			get { return mCoroutines; }
		}


        public ClientDistributedObject(SendMessageCallback sendMessage, DistributedObjectId doId ) 
			: base(doId)
        {
			mSendMessage = sendMessage;
        }
		
		public override void Dispose()
		{
			if( mEntity != null )
			{
				mEntity.Dispose();
                mEntity = null;
			}
		}

		public abstract void BuildEntity();

		public override void ProcessMessage(Message message)
        {

            int actionIndex = message.Callback;

            Action<Message> callback = null;
            if (!mMessageActions.TryGetValue(actionIndex, out callback))
            {
				//TODO: Right now we're just filtering out the telemetry message for our local user since we don't want that.
				//Once joe fixes the server side garbage where it doesn't send the telemetry update back to the user who sent it
				//we may want to revisit this and have things fail if there is no callback. 
				
                //UnityEngine.Console.LogError("The callback associated with this messageType, " + message.MessageType + ", was not found.");
                return;
            }

            // If this is a ram update, store the message for future use
            // for clients that enter this zone late.
            // For debugging we could make this a stack and store all updates... that would be cool.
            if (message.StoreInRam)
            {
                base.mMessageState[actionIndex] = message;
            }

            // Call the Action stored for this message
            callback(message);
        }
    }
}
