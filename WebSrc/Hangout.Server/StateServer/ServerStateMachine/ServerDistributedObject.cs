using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Collections;
using log4net;

namespace Hangout.Server
{
    public abstract class ServerDistributedObject : DistributedObject, IServerDistributedObject
    {
        protected ServerObjectRepository mServerObjectRepository = null;
        protected static ILog mLogger = LogManager.GetLogger("ServerDistributedObject");

        public ServerDistributedObject(ServerObjectRepository serverObjectRepository, DistributedObjectId doId)
            : base(doId)
        {
            mServerObjectRepository = serverObjectRepository;
        }

		// Incoming message from the client
        public override void ProcessMessage(Message message)
        {
            //TODO: abstract this to be in MessageUtil!!!!
            int actionIndex = message.Callback;

            Action<Message> callback = null;
            if (!mMessageActions.TryGetValue(actionIndex, out callback))
            {
				// Warning
				mLogger.Warn("Got unexpected actionIndex " + actionIndex + " with no callback");
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

        /// <summary>
        /// This function is intended to update clients who are first joining a zone / room.
        /// The client need to know what the current state of the distributed object is upon joining,
        /// so every distributed object must be able to report it's current state to broadcast to all interested client sessions
        /// The default implementation is to do nothing.
        /// To return false means that the message being generated inside the function is not valid.
        /// Subclasses should return true.
        /// </summary>
        /// <param name="sessionIds"></param>
        public virtual bool DistributedObjectStateUpdate(out Message outgoingMessage)
        {
            outgoingMessage = null;
            return false;
        }

        protected void BroadcastMessage(Message message)
        {
            mServerObjectRepository.BroadcastDistributedObjectUpdateMessageToZone(message, this.DistributedObjectId);
        }

		public override void Dispose() { }
    }
}
