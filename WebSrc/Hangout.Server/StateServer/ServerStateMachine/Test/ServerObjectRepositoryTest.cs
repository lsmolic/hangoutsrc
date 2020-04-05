/**  --------------------------------------------------------  *
 *   ServerObjectRepositoryTest.cs  
 *
 *   Author: Joe Shochet, Hangout Industries
 *   Date: 08/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Server;

// namespace Hangout.Server

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class ServerObjectRepositoryTest
    {
        public class DistributedTestObject : ServerDistributedObject
        {
            public DistributedTestObject(DistributedObjectId doId) : base(null, doId) { }
            protected override void RegisterMessageActions() { }
			public override void Dispose() {}
        }


        [Test]
        public void ZoneHandling()
        {
            ServerStateMachine serverStateMachine = new TestServerStateMachine();
            ServerObjectRepository objRepo = new ServerObjectRepository(serverStateMachine);

            // Create a distributed object to play with
            DistributedObjectId doId = new DistributedObjectId(100);
            DistributedTestObject obj = new DistributedTestObject(doId);

            ZoneId zoneId1 = new ZoneId(100);
            ZoneId zoneId2 = new ZoneId(200);

            objRepo.AddObject(obj);

            // No zone set yet
            Assert.IsTrue(objRepo.GetZone(obj) == null);
            Assert.IsTrue(objRepo.GetZone(doId) == null);
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId1).Count == 0);

            // Set the first zone
            objRepo.SetObjectZone(obj, zoneId1);
            Assert.IsTrue(objRepo.GetZone(obj) == zoneId1);
            Assert.IsTrue(objRepo.GetZone(doId) == zoneId1);
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId1).Count == 1);

            // Now change to the second zone
            objRepo.SetObjectZone(obj, zoneId2);
            Assert.IsTrue(objRepo.GetZone(obj) == zoneId2);
            Assert.IsTrue(objRepo.GetZone(doId) == zoneId2);
            // Should no longer be in the first zone
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId1).Count == 0);
            // Should be in the second zone
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId2).Count == 1);

            // Try setting the same zone again
            objRepo.SetObjectZone(obj, zoneId2);
            Assert.IsTrue(objRepo.GetZone(obj) == zoneId2);
            Assert.IsTrue(objRepo.GetZone(doId) == zoneId2);
            // Should be in the second zone, only once
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId2).Count == 1);

            // Remove obj
            objRepo.RemoveObject(obj);
            Assert.IsFalse(objRepo.ContainsObject(obj));
            // Should not be in either zone
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId1).Count == 0);
            Assert.IsTrue(objRepo.GetObjectsInZone(zoneId2).Count == 0);
        }


        [Test]
        // Get an object update but the object is not in the repository
        public void ReceiveMessageForMissingObject()
        {
			ServerStateMachine serverStateMachine = new TestServerStateMachine();
			ServerObjectRepository objectRepo = new ServerObjectRepository(serverStateMachine);

            DistributedObjectId doId = new DistributedObjectId(100);

            Message updateMessage = new Message();
            List<object> data = new List<object>();
            data.Add("Test");
            updateMessage.UpdateObjectMessage(true, false, doId, (int)MessageSubType.Chat, data);

            objectRepo.ReceiveRequest(updateMessage, Guid.Empty);
        }
    }
}
