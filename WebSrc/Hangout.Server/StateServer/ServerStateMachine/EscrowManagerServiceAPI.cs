using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
	public static class EscrowManagerServiceAPI
	{
		private const string kFbIdCsvList = "toFacebookAccountIdCsvList";
		private const string kToAccountId = "toFacebookAccountId";
		private const string kFromAccountId = "fromAccountId";
		private const string kTransactionType = "transactionType";
		private const string kValue = "value";
		private const string kTransactionTimeStamp = "timeStamp";
		private const string kDeleteTransactions = "deleteTransactions";

		private static ILog mLogger = LogManager.GetLogger("EscrowManagerServiceAPI");

		public static void GetEscrowTransactions(long toFacebookAccountId, long fromFacebookAccountId, EscrowType transactionType, string timeStamp, bool clearTransactions,
			System.Action<XmlDocument> callback)
		{
			mLogger.DebugFormat("GetEscrowTransactions called to={0} from={1} type={2} ts={3}", toFacebookAccountId, fromFacebookAccountId, transactionType, timeStamp);
			WebServiceRequest request = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Escrow", "GetEscrowTransactions");
			request.AddParam(kToAccountId, toFacebookAccountId.ToString());
			if (fromFacebookAccountId == 0)
			{
				request.AddParam(kFromAccountId, "");
			} else {
				request.AddParam(kFromAccountId, fromFacebookAccountId.ToString());
			}
			request.AddParam(kTransactionType, transactionType.ToString());
			request.AddParam(kTransactionTimeStamp, timeStamp);
			// Do we want to immediately clear the transactions after getting them?
			if (clearTransactions)
			{
				request.AddParam(kDeleteTransactions, "1");
			} else {
				request.AddParam(kDeleteTransactions, "0");
			}
			request.GetWebResponseAsync(callback);
		}

		public static void CreateEscrowTransaction(ICollection<long> toFacebookAccountIdList, long fromFacebookAccountId, string transactionType, int value,
			System.Action<XmlDocument> callback)
		{
			string csvListOfFbIds = "";
			string delimiter = "";
			foreach (long friendFacebookId in toFacebookAccountIdList)
			{
				csvListOfFbIds += (delimiter + friendFacebookId.ToString());
				delimiter = ",";
			}
			mLogger.DebugFormat("CreateMultipleEscrowTransaction called to={0} from={1} type={2} value={3}", toFacebookAccountIdList, fromFacebookAccountId, transactionType, value);
			WebServiceRequest request = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Escrow", "CreateMultipleEscrowTransactions");
			request.AddParam(kFbIdCsvList, csvListOfFbIds);
			request.AddParam(kFromAccountId, fromFacebookAccountId.ToString());
			request.AddParam(kTransactionType, transactionType);
			request.AddParam(kValue, value.ToString());
			request.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
			{
				mLogger.DebugFormat("CreateMultipleEscrowTransactions responded xml={0}", xmlResponse.OuterXml);
				callback(xmlResponse);
			});
		}

		public static void CreateEscrowTransaction(long toFacebookAccountId, long fromFacebookAccountId, string transactionType, int value,
			System.Action<XmlDocument> callback)
		{
			mLogger.DebugFormat("CreateEscrowTransaction called to={0} from={1} type={2} value={3}", toFacebookAccountId, fromFacebookAccountId, transactionType, value);
			WebServiceRequest request = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Escrow", "CreateEscrowTransaction");
			request.AddParam(kToAccountId, toFacebookAccountId.ToString());
			request.AddParam(kFromAccountId, fromFacebookAccountId.ToString());
			request.AddParam(kTransactionType, transactionType);
			request.AddParam(kValue, value.ToString());
			request.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
			{
				mLogger.DebugFormat("CreateEscrowTransaction responded xml={0}", xmlResponse.OuterXml);
				callback(xmlResponse);
			});
		}

		public static void ClearEscrowTransactions(long toFacebookAccountId, long fromFacebookAccountId, string transactionType, string timeStamp,
			System.Action<XmlDocument> callback)
		{
			mLogger.DebugFormat("ClearEscrowTransactions called to={0} from={1} type={2} ts={3}", toFacebookAccountId, fromFacebookAccountId, transactionType, timeStamp);
			WebServiceRequest request = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Escrow", "ClearEscrowTransactions");
			request.AddParam(kToAccountId, toFacebookAccountId.ToString());
			if (fromFacebookAccountId == 0)
			{
				request.AddParam(kFromAccountId, "");
			}
			else
			{
				request.AddParam(kFromAccountId, fromFacebookAccountId.ToString());
			}
			request.AddParam(kTransactionType, transactionType);
			request.AddParam(kTransactionTimeStamp, timeStamp);
			request.GetWebResponseAsync(callback);
		}

	}
}
