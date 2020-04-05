using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
    public class ClientMockDistributedObject : ClientDistributedObject
    {
		public ClientMockDistributedObject(SendMessageCallback sendMessage, DistributedObjectId doId ) : base(sendMessage, doId )
        {
        }

		public override void BuildEntity()
		{
			throw new NotImplementedException( "Deprecated.  Need to make a MockEntity class to use this.");
		}

        protected override void RegisterMessageActions()
        {
            base.RegisterMessageAction(-1, new Action<Message>(TestFromServer));
        }

        public void TestFromServer(Message msg)
        {
            Console.WriteLine("Test message from server processed.");
        }

		public override void Dispose()
		{
			
		}
    }
}
