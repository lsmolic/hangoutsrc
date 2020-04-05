/**  --------------------------------------------------------  *
 *   SessionManagerTest.cs  
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
    public class SessionManagerTest
    {
		private ServerAccount mTestServerAccount = new ServerAccount(new AccountId(0), 0, "0", "0", "schmear", "mein", "schmeer", null);
        [Test]
        // Test adding and removing a session from the manager
        public void AddAndRemoveSessionId()
        {
            SessionManager sessionManager = new SessionManager();
            Guid sessionId = new Guid();
            Assert.IsTrue(sessionManager.AddSession(sessionId, mTestServerAccount));
            Assert.IsTrue(sessionManager.ContainsSession(sessionId));

            ServerAccount returnedServerAccount = sessionManager.GetServerAccountFromSessionId(sessionId);
            Assert.Equals(returnedServerAccount, mTestServerAccount);

            Assert.IsTrue(sessionManager.RemoveSession(sessionId));
            Assert.IsFalse(sessionManager.ContainsSession(sessionId));
            // Trying to remove it again should return false
            Assert.IsFalse(sessionManager.RemoveSession(sessionId));
        }

        [Test]
        // Test adding a session twice should only really add it once
        // It should act like a set
        public void AddASessionTwice()
        {
            SessionManager sessionManager = new SessionManager();
            Guid sessionId = new Guid();
            Assert.IsTrue(sessionManager.AddSession(sessionId, mTestServerAccount));
            Assert.IsTrue(sessionManager.ContainsSession(sessionId));
            // This should return false because it has already been added
            Assert.IsFalse(sessionManager.AddSession(sessionId, mTestServerAccount));
            Assert.IsTrue(sessionManager.ContainsSession(sessionId));
            Assert.IsTrue(sessionManager.RemoveSession(sessionId));
            // Now it should be gone, if it only got added once
            Assert.IsFalse(sessionManager.ContainsSession(sessionId));
        }

        [Test]
        // Test adding and removing interest in a couple zones
        public void OpenAndCloseInterest()
        {
            SessionManager sessionManager = new SessionManager();
            Guid sessionId = new Guid();
            ZoneId zoneId1 = new ZoneId(100);
            ZoneId zoneId2 = new ZoneId(200);

            Assert.IsTrue(sessionManager.AddSession(sessionId, mTestServerAccount));

            // List should start out empty
            Assert.IsTrue(sessionManager.GetSessionIdsInterestedInZoneId(zoneId1).Count == 0);

            // Session should not already have interest
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId, zoneId1));
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId, zoneId2));

            sessionManager.AddZoneIdInterestToSessionId(sessionId, zoneId1);

            // List should now contain one session
            Assert.IsTrue(sessionManager.GetSessionIdsInterestedInZoneId(zoneId1).Count == 1);

            // Session should now have interest
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId, zoneId1));

            // Session should be interested in that single zone
            List<ZoneId> zoneIds = sessionManager.GetZoneIdsFromInterestedSessionId(sessionId);
            Assert.IsTrue(zoneIds.Count == 1);
            Assert.IsTrue(zoneIds.Contains(zoneId1));

            // Lets test a second zone
            sessionManager.AddZoneIdInterestToSessionId(sessionId, zoneId2);

            // Session should now have interest
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId, zoneId2));

            // Session should be interested in two zones
            Assert.IsTrue(sessionManager.GetZoneIdsFromInterestedSessionId(sessionId).Count == 2);

            // Remove interest in first zone
            sessionManager.RemoveZoneIdInterestFromSessionId(sessionId, zoneId1);

            // Session should no longer have interest
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId, zoneId1));

            // Back to one
            Assert.IsTrue(sessionManager.GetSessionIdsInterestedInZoneId(zoneId2).Count == 1);

            // Remove interest in the second zone
            sessionManager.RemoveZoneIdInterestFromSessionId(sessionId, zoneId2);

            // Session should no longer have interest
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId, zoneId2));

            // Back to empty
            Assert.IsTrue(sessionManager.GetSessionIdsInterestedInZoneId(zoneId1).Count == 0);
            
            sessionManager.RemoveSession(sessionId);

        }

        [Test]
        // Test adding and removing interest in a zone
        public void ExtraneousRemoveInterestCalls()
        {
            SessionManager sessionManager = new SessionManager();
            Guid sessionId = new Guid();
            ZoneId zoneId1 = new ZoneId(100);
            ZoneId zoneId2 = new ZoneId(200);

            sessionManager.AddZoneIdInterestToSessionId(sessionId, zoneId1);

            // Session should now have interest
            Assert.IsTrue(sessionManager.SessionIdHasInterest(sessionId, zoneId1));

            // Remove it once
            sessionManager.RemoveZoneIdInterestFromSessionId(sessionId, zoneId1);

            // Session should no longer have interest
            Assert.IsFalse(sessionManager.SessionIdHasInterest(sessionId, zoneId1));

            // Try to remove it again even though it is not there... this should be allowed
            sessionManager.RemoveZoneIdInterestFromSessionId(sessionId, zoneId1);

            // Try to remove another zone it never had interest in. Should be allowed.
            sessionManager.RemoveZoneIdInterestFromSessionId(sessionId, zoneId2);

        }
    }
}
