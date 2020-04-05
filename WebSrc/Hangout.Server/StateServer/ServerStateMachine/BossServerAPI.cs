using System;
using System.Xml;
using Hangout.Server.StateServer;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public class BossServerAPI
    {
        private static ILog mLogger = LogManager.GetLogger("BossServerAPI");

        public static void InitStateServer(string ipAddress, string port, Action<XmlDocument> initStateServerCallback)
        {
            mLogger.DebugFormat("InitStateServer called ipAddress={0} port={1}", ipAddress, port);
            WebServiceRequest bossService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Boss", "InitStateServer");
            bossService.AddParam("stateServerIp", ipAddress);
            bossService.AddParam("port", port);
            bossService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.DebugFormat("InitStateServer responded ipAddress={0} port={1}", ipAddress, port);
                initStateServerCallback(xmlResponse);
            });
        }

        public static void UpdateStateServer(string stateServerId, int population, string isEnabled, Action<XmlDocument> updateStateServerCallback)
        {
            mLogger.DebugFormat("UpdateStateServer called stateServerId={0} population={1} isEnabled={2}", stateServerId, population, isEnabled);
            WebServiceRequest bossService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Boss", "UpdateStateServer");
            bossService.AddParam("stateServerId", stateServerId);
            bossService.AddParam("population", population.ToString());
            bossService.AddParam("isEnabled", isEnabled);
            bossService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.DebugFormat("UpdateStateServer responded stateServerId={0} population={1} isEnabled={2}", stateServerId, population, isEnabled);
                updateStateServerCallback(xmlResponse);
            });
        }

        public static void RegisterNewSession(AccountId accountId, string sessionId, string status, string zone, string stateServerId, Action<XmlDocument> updateStateServerCallback)
        {
            mLogger.DebugFormat("RegisterNewSession called accountId={0} sessionId={1} status={2} zone={3} stateServerId={4}", accountId, sessionId, status, zone, stateServerId);
            WebServiceRequest bossService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Boss", "RegisterNewSession");
            bossService.AddParam("accountId", accountId.ToString());
            bossService.AddParam("sessionId", sessionId);
            bossService.AddParam("status", status);
            bossService.AddParam("zone", zone);
            bossService.AddParam("stateServerId", stateServerId);
            bossService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.DebugFormat("RegisterNewSession responded accountId={0} sessionId={1} status={2} zone={3} stateServerId={4}", accountId, sessionId, status, zone, stateServerId);
                updateStateServerCallback(xmlResponse);
            });
        }

        public static void UpdateSession(AccountId accountId, string sessionId, string status, string zone, string stateServerId, Action<XmlDocument> updateStateServerCallback)
        {
            mLogger.DebugFormat("UpdateSession called accountId={0} sessionId={1} status={2} zone={3} stateServerId={4}", accountId, sessionId, status, zone, stateServerId);
            WebServiceRequest bossService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Boss", "UpdateSession");
            bossService.AddParam("accountId", accountId.ToString());
            bossService.AddParam("sessionId", sessionId);
            bossService.AddParam("status", status);
            bossService.AddParam("zone", zone);
            bossService.AddParam("stateServerId", stateServerId);
            bossService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.DebugFormat("UpdateSession responded accountId={0} sessionId={1} status={2} zone={3} stateServerId={4}", accountId, sessionId, status, zone, stateServerId);
                updateStateServerCallback(xmlResponse);
            });
        }

        public static void RemoveSession(AccountId accountId, string sessionId, Action<XmlDocument> updateStateServerCallback)
        {
            mLogger.DebugFormat("RemoveSession called accountId={0} sessionId={1}", accountId, sessionId);
            WebServiceRequest bossService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Boss", "RemoveSession");
            bossService.AddParam("accountId", accountId.ToString());
            bossService.AddParam("sessionId", sessionId);
            bossService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.DebugFormat("RemoveSession responded accountId={0} sessionId={1}", accountId, sessionId);
                updateStateServerCallback(xmlResponse);
            });
        }

    }
}
