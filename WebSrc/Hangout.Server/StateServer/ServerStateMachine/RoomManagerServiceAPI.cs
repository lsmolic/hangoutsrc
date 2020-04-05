using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
    public static class RoomManagerServiceAPI
    {
        private static ILog mLogger = LogManager.GetLogger("RoomManagerServiceAPI");

		private const string kIsEnabled = "IsEnabled";
        private const string kRoomId = "RoomId";
        private const string kRoomName = "roomName";
        private const string kAccountId = "AccountId";
        private const string kPrivacyLevel = "privacyLevel";
        private const string kDefaultRoomId = "defaultRoomId";
        private const string kRoomDna = "RoomDNA";
		private const string kAccountIdCsvList = "AccountIdCsvList";

        public static void GetDefaultRoomService(AccountId accountId, System.Action<XmlDocument> getDefaultRoomServiceCallback)
        {
            mLogger.DebugFormat("GetDefaultRoomService called accountId={0}", accountId);
            WebServiceRequest getDefaultRoomService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "GetRooms");
			getDefaultRoomService.AddParam("accountId", accountId.ToString());
            getDefaultRoomService.AddParam("isDefault", "1");
            getDefaultRoomService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("GetDefaultRoomService responded accountId={0} xmlResponse={1}", accountId, xmlResponse.OuterXml);
                getDefaultRoomServiceCallback(xmlResponse);
            });
        }

        public static void GetRoomService(RoomId roomId, System.Action<XmlDocument> getRoomServiceFinishedCallback)
        {
            mLogger.DebugFormat("GetRoomService called roomId={0}", roomId);
            WebServiceRequest getRoomService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "GetRooms");
            getRoomService.AddParam(kRoomId, roomId.ToString());
            getRoomService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("GetRoomService responded roomId={0} xmlResponse={1}", roomId, xmlResponse.OuterXml);
                getRoomServiceFinishedCallback(xmlResponse);
            });
        }

        public static void GetSessionOwnedRoomsService(AccountId accountId, System.Action<XmlDocument> getRoomServiceFinishedCallback)
        {
            mLogger.DebugFormat("GetSessionOwnedRoomsService called accountId={0}", accountId);
            WebServiceRequest getRoomList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "GetRooms");
            getRoomList.AddParam(kAccountId, accountId.ToString());
            getRoomList.GetWebResponseAsync(getRoomServiceFinishedCallback);
        }

		public static void GetSessionOwnedRoomsWithPrivacyService(IList<AccountId> accountIds, PrivacyLevel privacyLevel, System.Action<XmlDocument> getRoomServiceFinishedCallback)
		{
            mLogger.DebugFormat("GetSessionOwnedRoomsWithPrivacyService called accountIds={0} privacyLevel={1}", accountIds, privacyLevel);
            WebServiceRequest getRoomList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "GetRooms");
			AccountId[] accountIdArray = new AccountId[accountIds.Count];
			accountIds.CopyTo(accountIdArray, 0);
			string[] accountIdStringArray = Array.ConvertAll<AccountId, string>(accountIdArray, new Converter<AccountId, string>(
				delegate(AccountId accountId) 
				{ 
					return accountId.ToString(); 
				}
			));
			string accountIdsCommaSeperatedString = String.Join(",", accountIdStringArray);
			getRoomList.AddParam(kAccountIdCsvList, accountIdsCommaSeperatedString);
			if (privacyLevel != PrivacyLevel.Default)
			{
				getRoomList.AddParam(kPrivacyLevel, ((uint)privacyLevel).ToString());
			}
			getRoomList.GetWebResponseAsync(getRoomServiceFinishedCallback);
		}
		
		public static void GetSessionOwnedRoomsWithPrivacyService(AccountId accountId, PrivacyLevel privacyLevel, System.Action<XmlDocument> getRoomServiceFinishedCallback)
        {
            mLogger.DebugFormat("GetSessionOwnedRoomsWithPrivacyService called accountId={0} privacyLevel={1}", accountId, privacyLevel);
            WebServiceRequest getRoomList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "GetRooms");
            getRoomList.AddParam(kAccountId, accountId.ToString());
            if (privacyLevel != PrivacyLevel.Default)
            {
                getRoomList.AddParam(kPrivacyLevel, ((uint)privacyLevel).ToString());
            }
            getRoomList.GetWebResponseAsync(getRoomServiceFinishedCallback);
        }

        public static void UpdateRoomDnaService(RoomId roomId, XmlDocument roomDna, System.Action<XmlDocument> updateRoomDnaServiceFinishedCallback)
        {
            mLogger.DebugFormat("UpdateRoomDnaService called roomId={0} roomDna={1}", roomId, roomDna.OuterXml);
            WebServiceRequest updateRoomDna = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "UpdateRoomDNA");
            updateRoomDna.AddParam(kRoomId, roomId.ToString());
            updateRoomDna.AddParam(kRoomDna, roomDna.OuterXml);
            updateRoomDna.GetWebResponseAsync(updateRoomDnaServiceFinishedCallback);
        }

		public static void CreateNewRoomService(AccountId accountId, string roomName, PrivacyLevel privacyLevel, System.Action<XmlDocument> createRoomFinishedCallback)
		{
            mLogger.DebugFormat("CreateNewRoomService called accountId={0} roomName={1} privacyLevel={2}", accountId, roomName, privacyLevel);
            WebServiceRequest createRoomService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "CreateRoom");
			createRoomService.Encrypted = true;
			createRoomService.AddParam(kAccountId, accountId.ToString());
            createRoomService.AddParam(kRoomName, roomName);
			createRoomService.AddParam(kPrivacyLevel, ((int)privacyLevel).ToString());
            createRoomService.AddParam(kDefaultRoomId, "1");
			createRoomService.GetWebResponseAsync(createRoomFinishedCallback);
		}

		public static void GetAllSystemRoomsService(System.Action<XmlDocument> getSystemRoomFinishedCallback)
		{
            mLogger.DebugFormat("GetAllSystemRoomsService called");
            WebServiceRequest getSystemRoomService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "GetSystemRooms");
			getSystemRoomService.AddParam(kIsEnabled, "1");
			getSystemRoomService.GetWebResponseAsync(getSystemRoomFinishedCallback);
		}

		public static void DisableRoomService(AccountId accountId, RoomId roomId, System.Action<XmlDocument> disableRoomServiceCallback)
		{
			WebServiceRequest disableRoomInDatabaseService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Rooms", "DisableRoom");
			disableRoomInDatabaseService.AddParam(kRoomId, roomId.ToString());
			disableRoomInDatabaseService.AddParam(kAccountId, accountId.ToString());
			disableRoomInDatabaseService.GetWebResponseAsync(disableRoomServiceCallback);
		}
    }
}
