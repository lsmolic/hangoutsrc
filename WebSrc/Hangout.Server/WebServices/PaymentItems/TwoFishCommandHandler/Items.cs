using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    internal class Items : TwoFishCommandBase
    {
        public Items() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }


        /// <summary>
        /// Implements call to Twofish method item to find an item. 
        /// </summary>
        /// <param name="itemsInfo">ItemName</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish XML response document containing detailed information about the Item</returns>
        public XmlDocument FindItem(ItemsInfo itemsInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);

            keyValues.Add("name", itemsInfo.ItemName);

            DebugLog("Item", keyValues);

            return (rest.GetXMLRequest("item", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method item to create an item
        /// </summary>
        /// <param name="itemsInfo">ItemName, ItemTypeName, ButtonName, Description, SmallImageUrl, MediumImageUrl, LargeImageUrl, Available, IPAddress</param>
        /// <param name="appName">The name of the application to contain the item</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish XML response document containing Create Item response</returns>
        public XmlDocument CreateItem(ItemsInfo itemsInfo, string appName, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            keyValues.Add("item.name", itemsInfo.ItemName);
            keyValues.Add("item.appName", appName);
            keyValues.Add("item.itemTypeName", itemsInfo.ItemTypeName);
            keyValues.Add("item.buttonName", itemsInfo.ButtonName);
            keyValues.Add("item.description", itemsInfo.Description);
            keyValues.Add("item.smallImageUrl", itemsInfo.SmallImageUrl);
            keyValues.Add("item.mediumImageUrl", itemsInfo.MediumImageUrl);
            keyValues.Add("item.largeImageUrl", itemsInfo.LargeImageUrl);
            keyValues.Add("item.available", itemsInfo.Available);
            keyValues.Add("ipAddress", itemsInfo.IPAddress);
          //  keyValues.Add("p.color", "Blue");  // Test for additional properties.

            DebugLog("CreateItem", keyValues);

            return (rest.PostXMLRequest("item", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method item/update to update an item 
        /// </summary>
        /// <param name="itemsInfo">ItemName, ItemTypeName, ButtonName, Description, SmallImageUrl, MediumImageUrl, LargeImageUrl, Available, IPAddress</param>
        /// <param name="appName">The name of the application to contain the item</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish XML response document containing Update Item response</returns>
        public XmlDocument UpdateItem(ItemsInfo itemsInfo, string appName, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            keyValues.Add("item.name", itemsInfo.ItemName);
            keyValues.Add("item.appName", appName);
            keyValues.Add("item.itemTypeName", itemsInfo.ItemTypeName);
            keyValues.Add("item.buttonName", itemsInfo.ButtonName);
            keyValues.Add("item.description", itemsInfo.Description);
            keyValues.Add("item.smallImageUrl", itemsInfo.SmallImageUrl);
            keyValues.Add("item.mediumImageUrl", itemsInfo.MediumImageUrl);
            keyValues.Add("item.largeImageUrl", itemsInfo.LargeImageUrl);
            keyValues.Add("item.available", itemsInfo.Available);
            keyValues.Add("ipAddress", itemsInfo.IPAddress);
            //   keyValues.Add("p.color", "Blue");  // Test for additional properties.

            DebugLog("UpdateItem", keyValues);

            return (rest.PostXMLRequest("item/update", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method item/prices to retrieve the item price
        /// </summary>
        /// <param name="itemsInfo">ItemName, IPAddress, StoreName</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish XML response document containing item price</returns>
        public XmlDocument ItemPrices(ItemsInfo itemsInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);

            keyValues.Add("itemName", itemsInfo.ItemName );
            keyValues.Add("ipAddress", itemsInfo.IPAddress);
            keyValues.Add("storeName", itemsInfo.StoreName);

            DebugLog("ItemPrices", keyValues);

            return (rest.GetXMLRequest("item/prices", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method itemInstances to retrieve the item instance information for a user and item
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="itemsInfo">ItemName, ItemTypeName, IPAddress</param>
        /// <param name="filter">Filter</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish XML response document containing item instances information for a user and item</returns>
        public XmlDocument ItemInstance(UserId userId, ItemsInfo itemsInfo, ResultFilter filter, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);

            keyValues.Add("userId", userId.ToString());
            keyValues.Add("itemName", itemsInfo.ItemName);
            keyValues.Add("itemTypeName", itemsInfo.ItemTypeName);
            keyValues.Add("ipAddress", itemsInfo.IPAddress);
            keyValues.Add("filter", filter.Filter);
            keyValues.Add("fetchRequest.startIndex", filter.StartIndex);
            keyValues.Add("fetchRequest.blockSize", filter.BlockSize);

            DebugLog("ItemInstance", keyValues);

            return (rest.GetXMLRequest("itemInstances", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method ItemTypes to retrieve the item type information for a given store
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="commmonKeyValue"></param>
        /// <param name="baseAddress"></param>
        /// <returns>Twofish XML response document containing item type information for a given store</returns>
        public XmlDocument ItemTypes(string storeName, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);

            keyValues.Add("storeName", storeName);
     
            DebugLog("ItemTypes", keyValues);

            return (rest.GetXMLRequest("itemTypes", keyValues));
       }
    }
}
