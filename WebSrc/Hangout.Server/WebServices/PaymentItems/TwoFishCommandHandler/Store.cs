using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

// TwoFish command class to call Store methods. FindStore, PurchaseItems, StoreBulkGet, StoreBulkLoad.

namespace Hangout.Server.WebServices
{
    internal class Store : TwoFishCommandBase
    {
        public Store() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Retrieve all of the store offer for a given store
        /// </summary>
        /// <param name="storeName">storeName<storeName/param>
        /// <param name="filter">ItemTypeNames, MaxRemaining, LatestEndDate, OrderBy, Descending, StartIndex, BlockSize</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML doucment containing stores offers</returns>
        public XmlDocument FindStore(String storeName, ResultFilter filter, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);
            keyValues.Add("name", storeName);
            keyValues.Add("itemTypeNames", filter.ItemTypeNames);
            keyValues.Add("maxRemaining", filter.MaxRemaining);
            keyValues.Add("latestEndDate", filter.LatestEndDate);
            keyValues.Add("orderBy", filter.OrderBy);
            keyValues.Add("descending", filter.Descending);
            keyValues.Add("fetchRequest.startIndex", filter.StartIndex);
            keyValues.Add("fetchRequest.blockSize", filter.BlockSize);

            DebugLog("Store", keyValues);

            return (rest.GetXMLRequest("store", keyValues));
        }

        /// <summary>
        /// Purchase an item or items from a store.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="purchaseInfo">AccountId, OfferIds, ExternalTxnId, IPAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing purchase results</returns>
        public XmlDocument PurchaseItems(UserId userId, PurchaseInfo purchaseInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("accountId", purchaseInfo.AccountId);
            keyValues.Add("offerIds", purchaseInfo.OfferIds);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("ipAddress", userId.IPAddress);

            DebugLog("PurchaseItems", keyValues);

            return (rest.PostXMLRequest("store/purchaseItems", keyValues));
        }

        /// <summary>
        /// Purchase an item as a gift from a store.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="purchaseInfo">RecipientUserId, NoteToRecipient AccountId, OfferIds, ExternalTxnId, IPAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing purchase gift results</returns>
        public XmlDocument PurchaseGift(UserId userId, PurchaseInfo purchaseInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("recipientUserId", purchaseInfo.RecipientUserId);
            keyValues.Add("note", purchaseInfo.NoteToRecipient);
            keyValues.Add("accountId", purchaseInfo.AccountId);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("ipAddress", userId.IPAddress);

            DebugLog("PurchaseGift", keyValues);

            return (rest.PostXMLRequest("store/purchaseGift", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method StoreBulkGet 
        /// Retreive Twofish Store offers
        /// </summary>
        /// <param name="storeInfo">StoreName, IpAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing the Twofish CSV file listing all store offers</returns>
        public XmlDocument StoreBulkGet(StoreInfo storeInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);
            keyValues.Add("name", storeInfo.StoreName);
            keyValues.Add("ipAddress", storeInfo.IpAddress);

            DebugLog("StoreBulkGet", keyValues);

            string storeData = rest.GetStringRequest("store/bulk", keyValues);

            return (rest.WrapDataXML("Store", "StoreBulkGet", storeInfo.StoreName, storeData));
        }

        /// <summary>
        /// Implements call to Twofish method StoreBulkLoad
        /// Loads a store catalog
        /// </summary>
        /// <param name="storeInfo">IpAddress</param>
        /// <param name="fileData">Twofish store csv file</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish store bulk load XML response document</returns>
        public XmlDocument StoreBulkLoad(StoreInfo storeInfo, PostFile fileData, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("ipAddress", storeInfo.IpAddress);
            keyValues.Add("submit", "User Bulk Load (pretty format)");
            DebugLog("StoreBulkLoad", keyValues);

            return (rest.MultiPartFormXMLPost("store/bulk", keyValues, fileData.FileData));
        }
    }
}
