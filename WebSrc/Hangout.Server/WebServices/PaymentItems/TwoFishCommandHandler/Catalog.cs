using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

// TwoFish command class to call Catalog methods.  FindCatalog, CreateCatalog

namespace Hangout.Server.WebServices
{
    internal class Catalog : TwoFishCommandBase 
    {
        public Catalog() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Implements call to Twofish method FindCatalog 
        /// Retreive Twofish items catalog
        /// </summary>
        /// <param name="itemsInfo">ItemTypeNames, IPAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing the Twofish CSV file listing the Items Catalog</returns>
        public XmlDocument FindCatalog(ItemsInfo itemsInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            keyValues.Add("appName", GetAppName());
            keyValues.Add("itemTypeNames", itemsInfo.ItemTypeNames);
            keyValues.Add("ipAddress", itemsInfo.IPAddress);

            DebugLog("FindCatalog", keyValues);

            string catalogData = rest.GetStringRequest("catalog", keyValues);

            return (rest.WrapDataXML("Catalog", "FindCatalog", "Catalog", catalogData));
        }

        /// <summary>
        /// Implements call to Twofish method CreateCatalog
        /// </summary>
        /// <param name="itemsInfo">IPAddress</param>
        /// <param name="fileData">Twofish catalog csv file</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>Twofish CreateCatalog XML response document</returns>
        public XmlDocument CreateCatalog(ItemsInfo itemsInfo, PostFile fileData, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(true);
            keyValues.Add("ipAddress", itemsInfo.IPAddress);

            DebugLog("CreateCatalog", keyValues);

            return (rest.MultiPartFormXMLPost("catalog", keyValues, fileData.FileData));

        }
    }
}
