using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
	public static class ServerAssetRepositoryServiceAPI
	{
        private static ILog mLogger = LogManager.GetLogger("ServerAssetRepositoryServiceAPI");
		
        private static string kInventory = "Inventory";
		private static string kGetItemList = "GetItemList";
		private static string kGetAssetList = "GetAssetList";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getItemsServiceCallback"></param>
		public static void GetItemsService(System.Action<XmlDocument> getItemsServiceCallback)
		{
            mLogger.Info("GetItemsService called");
            WebServiceRequest getItemsList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, kInventory, kGetItemList);
            getItemsList.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.Info("GetItemsService responded");
                getItemsServiceCallback(xmlResponse);
            });
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getAssetsServiceCallback"></param>
		public static void GetAssetsService(System.Action<XmlDocument> getAssetsServiceCallback)
		{
            mLogger.Info("GetAssetsService called");
            WebServiceRequest getAssetList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, kInventory, kGetAssetList);
			getAssetList.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.Info("GetAssetsService responded");
                getAssetsServiceCallback(xmlResponse);
            });
		}
	}
}
