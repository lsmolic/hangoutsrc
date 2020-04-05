/**  --------------------------------------------------------  *
 *   ServerEngineTest.cs  
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

    public class DistributedTestObject : ServerDistributedObject
    {
        public DistributedTestObject(DistributedObjectId doId) : base(null, doId) {}
        protected override void RegisterMessageActions() {}
		public override void Dispose() { }
    }

    [TestFixture]
    public class ServerEngineTest
    {

        private void MockSendMessageToReflector(Message message, Guid sessionId) {}
        private void MockSendMessageToReflector(Message message, List<Guid> sessionIds) {}

        [Test]
        public void ObjectZoneChangingWithNobodyListening()
        {
            // Setup the server engine and its required dependencies
			ServerStateMachine serverStateMachine = new TestServerStateMachine();
			SessionManager sessionManager = new SessionManager();
            ServerObjectRepository objectRepo = new ServerObjectRepository(serverStateMachine);
            ServerEngine serverEngine = new ServerEngine(sessionManager, objectRepo, MockSendMessageToReflector);

            // Create a distributed object to play with
            DistributedObjectId doId = new DistributedObjectId(100);
            DistributedTestObject obj = new DistributedTestObject(doId);

            ZoneId zoneId1 = new ZoneId(100);
            ZoneId zoneId2 = new ZoneId(200);

            // Start by putting the object in the first zone
            serverEngine.ProcessZoneChange(obj, zoneId1);
            Assert.IsTrue(objectRepo.GetZone(obj) == zoneId1);

            // Test changing to the zone the object is already in
            serverEngine.ProcessZoneChange(obj, zoneId1);
            Assert.IsTrue(objectRepo.GetZone(obj) == zoneId1);

            // Now change to a new zone
            serverEngine.ProcessZoneChange(obj, zoneId2);
            Assert.IsTrue(objectRepo.GetZone(obj) == zoneId2);

            // Now remove the object from the zone
            Assert.IsTrue(serverEngine.RemoveObjectFromZone(obj) == zoneId2);
            Assert.IsTrue(objectRepo.GetZone(obj) == null);

            // Try a duplicate removal - should be safe to do
            Assert.IsTrue(serverEngine.RemoveObjectFromZone(obj) == null);
            Assert.IsTrue(objectRepo.GetZone(obj) == null);

            // Now put the object back in the first zone
            serverEngine.ProcessZoneChange(obj, zoneId1);
            Assert.IsTrue(objectRepo.GetZone(obj) == zoneId1);
        }
        
        [Test]
        public void SessionInterestOpenAndClose()
        {
            // Setup the server engine and its required dependencies
			ServerStateMachine serverStateMachine = new TestServerStateMachine();
            SessionManager sessionManager = new SessionManager();
            ServerObjectRepository objectRepo = new ServerObjectRepository(serverStateMachine);
            ServerEngine serverEngine = new ServerEngine(sessionManager, objectRepo, MockSendMessageToReflector);

            Guid sessionId1 = new Guid();

            ZoneId zoneId1 = new ZoneId(100);
            ZoneId zoneId2 = new ZoneId(200);
            ZoneId zoneId3 = new ZoneId(300);

            // Simple open and close interest
            // Tests interest changing in an empty zone
            serverEngine.ProcessOpenZoneInterest(sessionId1, zoneId1);
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId1, zoneId1));
            serverEngine.ProcessCloseZoneInterest(sessionId1, zoneId1);
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId1, zoneId1));

            // Now test interest in two zones at once
            serverEngine.ProcessOpenZoneInterest(sessionId1, zoneId1);
            serverEngine.ProcessOpenZoneInterest(sessionId1, zoneId2);
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId1, zoneId1));
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId1, zoneId2));

            // Now close them as a list
            List<ZoneId> zoneIds = new List<ZoneId>();
            zoneIds.Add(zoneId1);
            zoneIds.Add(zoneId2);
            serverEngine.ProcessCloseZoneInterest(sessionId1, zoneIds);
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId1, zoneId1));
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId1, zoneId2));

            // Open the same interest twice
            serverEngine.ProcessOpenZoneInterest(sessionId1, zoneId1);
            serverEngine.ProcessOpenZoneInterest(sessionId1, zoneId1);
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId1, zoneId1));
            // Close it
            serverEngine.ProcessCloseZoneInterest(sessionId1, zoneId1);
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId1, zoneId1));

            // Close interest in a zone you didn't have interest in already
            serverEngine.ProcessCloseZoneInterest(sessionId1, zoneId3);
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId1, zoneId3));
        }

        [Test]
        public void DisconnectSession()
        {
            // Setup the server engine and its required dependencies
			ServerStateMachine serverStateMachine = new TestServerStateMachine();
			SessionManager sessionManager = new SessionManager();
            ServerObjectRepository objectRepo = new ServerObjectRepository(serverStateMachine);
            ServerEngine serverEngine = new ServerEngine(sessionManager, objectRepo, MockSendMessageToReflector);

            // Create a distributed object to play with
            DistributedObjectId doId = new DistributedObjectId(100);
            DistributedTestObject obj = new DistributedTestObject(doId);

            ZoneId zoneId = new ZoneId(200);
            Guid sessionId = new Guid();

			Assert.IsTrue(sessionManager.AddSession(sessionId, new ServerAccount(new AccountId(0), 0, "0", "0", "schmear", "mein", "schmeer", null)));
            objectRepo.AddObjectToSessionId(sessionId, obj);

            serverEngine.ProcessOpenZoneInterest(sessionId, zoneId);
            serverEngine.ProcessZoneChange(obj, zoneId);

            serverEngine.ProcessDisconnectSession(sessionId);

            // See if the object repo got cleaned up
            // Object repo should no longer have the object
            Assert.IsFalse(objectRepo.ContainsObject(obj));
            Assert.IsTrue(objectRepo.GetObject(doId) == null);
            // Object should not be in the zone
            Assert.IsFalse(objectRepo.GetObjectsInZone(zoneId).Contains(obj));
            Assert.IsTrue(objectRepo.GetDistributedObjectIdsOwnedBySessionId(sessionId).Count == 0);

            // See if the session manager got cleaned up
            // Session manager should no longer have the session
            Assert.IsFalse(sessionManager.ContainsSession(sessionId));
            // Zone interest should be cleaned up
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId, zoneId));
            Assert.IsFalse(sessionManager.GetSessionIdsInterestedInZoneId(zoneId).Contains(sessionId));
            Assert.IsFalse(sessionManager.GetZoneIdsFromInterestedSessionId(sessionId).Contains(zoneId));

        }

    }
}