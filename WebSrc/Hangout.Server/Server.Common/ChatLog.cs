using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace Hangout.Server
{
	public static class ChatLog
	{
        private const string mDelimiter = "|";
		private static ILog mChatLog = LogManager.GetLogger("Chat");

        private static void LogGoodChat(string senderId, string locationId, string msg)
        {
            mChatLog.Info(mDelimiter + "G" + mDelimiter + senderId + mDelimiter + locationId + mDelimiter + msg);
        }

        private static void LogBadChat(string senderId, string locationId, string originalMsg, string filteredMsg)
        {
            mChatLog.Info(mDelimiter + "B" + mDelimiter + senderId + mDelimiter + locationId + mDelimiter + originalMsg + mDelimiter + filteredMsg);
        }

		public static void LogChatResult(string senderId, string locationId, string originalMsg, string filteredMsg)
		{
            if (originalMsg == filteredMsg)
            {
                LogGoodChat(senderId, locationId, originalMsg);
            }
            else
            {
                LogBadChat(senderId, locationId, originalMsg, filteredMsg);
            }
		}
	}
}
