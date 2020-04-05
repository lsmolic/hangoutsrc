using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using Hangout.Server;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class HangoutStore : HangoutCommandBase 
    {
        public HangoutStore() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }


        /// <summary>
        /// Get the Store Inventory (Offers) 
        /// </summary>
        /// <param name="storeName">The name of store to retreive the offers from</param>
        /// <returns>XMLDocument containing the Store Inventory (Offers)</returns>
        public XmlDocument GetStoreInventory(string storeName, ResultFilter filter)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Store";
                cmd.Verb = "FindStore";

                cmd.Parameters.Add("storeName", storeName);

                cmd.Parameters.Add("itemTypeNames", filter.ItemTypeNames);
                cmd.Parameters.Add("startIndex", filter.StartIndex);
                cmd.Parameters.Add("blockSize", filter.BlockSize);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
            }

            catch (Exception ex)
            {
                logError("GetStoreInventory", ex);
            }

            return response;
        }

    }
}
