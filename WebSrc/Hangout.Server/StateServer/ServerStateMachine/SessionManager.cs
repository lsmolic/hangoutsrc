using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public class SessionManager
    {
        private static ILog mLogger = LogManager.GetLogger("SessionManager");
        private Dictionary<Guid, ServerAccount> mSessionIdsToServerAccounts = new Dictionary<Guid, ServerAccount>();
        private Dictionary<AccountId, ServerAccount> mAccountIdsToServerAccounts = new Dictionary<AccountId, ServerAccount>();

        // For each session, stores a list of zones that session is interested in
        private Dictionary<Guid, List<ZoneId>> mSessionsIdsToZoneIdInterests = new Dictionary<Guid, List<ZoneId>>();

        // For each zone, stores a list of which sessions are interested in that zone
        private Dictionary<ZoneId, List<Guid>> mZoneIdsToInterestedSessions = new Dictionary<ZoneId, List<Guid>>();

        public SessionManager()
        {
        }

        public ServerAccount GetServerAccountFromAccountId(AccountId accountId)
        {
            ServerAccount serverAccount = null;
			if (accountId != null)
			{
				mAccountIdsToServerAccounts.TryGetValue(accountId, out serverAccount);
			}
            return serverAccount;
        }

        public ServerAccount GetServerAccountFromSessionId(Guid sessionId)
        {
            ServerAccount serverAccount = null;
            mSessionIdsToServerAccounts.TryGetValue(sessionId, out serverAccount);
            return serverAccount;
        }

        public bool AddSession(Guid sessionId, ServerAccount serverAccount)
        {
            if (mSessionIdsToServerAccounts.ContainsKey(sessionId))
            {
                mLogger.Warn("WARNING: SessionManager already contains sessionId " + sessionId.ToString() + " in mSessionIdsToServerAccounts");
                return false;
            }
            mSessionIdsToServerAccounts.Add(sessionId, serverAccount);
            Metrics.Log(LogGlobals.CATEGORY_CONNECTION, LogGlobals.EVENT_CONNECTED, serverAccount.AccountId.ToString());
            if (mAccountIdsToServerAccounts.ContainsKey(serverAccount.AccountId))
            {
                mLogger.Warn("WARNING: SessionManager already contains accountId " + serverAccount.AccountId.ToString() + " in mAccountIdsToServerAccounts");
                return false;
            }
            mAccountIdsToServerAccounts.Add(serverAccount.AccountId, serverAccount);
            return true;
        }

        public bool ContainsSession(Guid sessionId)
        {
            return mSessionIdsToServerAccounts.ContainsKey(sessionId);
        }

        public bool RemoveSession(Guid sessionId)
        {
            ServerAccount serverAccountToRemove = null;
            if (mSessionIdsToServerAccounts.TryGetValue(sessionId, out serverAccountToRemove))
            {
				Metrics.Log(LogGlobals.CATEGORY_CONNECTION, LogGlobals.EVENT_DISCONNECTED, "ElapsedTime", serverAccountToRemove.LoggedInTimeLength.ToString(), serverAccountToRemove.AccountId.ToString());
                mAccountIdsToServerAccounts.Remove(serverAccountToRemove.AccountId);
            }
            bool containedSession = mSessionIdsToServerAccounts.Remove(sessionId);

            List<ZoneId> zonesToProcess = GetZoneIdsFromInterestedSessionId(sessionId);
            foreach (ZoneId zoneId in zonesToProcess)
            {
                List<Guid> sessions = GetSessionIdsInterestedInZoneId(zoneId);
                sessions.Remove(sessionId);
            }

            mSessionsIdsToZoneIdInterests.Remove(sessionId);
            return containedSession;
        }

        public List<Guid> GetSessionIdsInterestedInZoneId(ZoneId zoneId)
        {
            List<Guid> sessionIds = null;
            if (!mZoneIdsToInterestedSessions.TryGetValue(zoneId, out sessionIds))
            {
                sessionIds = new List<Guid>();
            }
            return sessionIds;
        }

        public List<ZoneId> GetZoneIdsFromInterestedSessionId(Guid sessionId)
        {
            List<ZoneId> zoneIds = null;
            if (!mSessionsIdsToZoneIdInterests.TryGetValue(sessionId, out zoneIds))
            {
                zoneIds = new List<ZoneId>();
            }
            return zoneIds;
        }

        public bool SessionIdHasInterest(Guid sessionId, ZoneId zoneId)
        {
            List<ZoneId> zoneIdInterests = null;
            if (!mSessionsIdsToZoneIdInterests.TryGetValue(sessionId, out zoneIdInterests))
            {
                // We dont have any zone interests for this sessionId
                return false;
            }
            // Is the zone in our interest list?
            return zoneIdInterests.Contains(zoneId);
        }

        public void AddZoneIdInterestToSessionId(Guid sessionId, ZoneId zoneId)
        {
            // Update the dictionary from sessions -> zoneIds
            List<ZoneId> zoneIdInterests = null;
            if (!mSessionsIdsToZoneIdInterests.TryGetValue(sessionId, out zoneIdInterests))
            {
                zoneIdInterests = new List<ZoneId>();
                mSessionsIdsToZoneIdInterests[sessionId] = zoneIdInterests;
            }
            zoneIdInterests.Add(zoneId);

            // Update the dictionary from zoneIds -> sessions
            List<Guid> sessions = null;
            if (!mZoneIdsToInterestedSessions.TryGetValue(zoneId, out sessions))
            {
                sessions = new List<Guid>();
                mZoneIdsToInterestedSessions[zoneId] = sessions;
            }
            sessions.Add(sessionId);
        }

        public void RemoveZoneIdInterestFromSessionId(Guid sessionId, ZoneId zoneId)
        {
            List<ZoneId> zoneIdInterests = null;
            if (mSessionsIdsToZoneIdInterests.TryGetValue(sessionId, out zoneIdInterests))
            {
                zoneIdInterests.Remove(zoneId);
            }

            List<Guid> sessions = null;
            if (mZoneIdsToInterestedSessions.TryGetValue(zoneId, out sessions))
            {
                sessions.Remove(sessionId);
            }
        }
    }
}
