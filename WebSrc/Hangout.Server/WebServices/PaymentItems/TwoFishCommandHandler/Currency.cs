using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    internal class Currency : TwoFishCommandBase
    {
        public Currency() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Implements call to Twofish method FindCurrency
        /// </summary>
        /// <param name="currencyInfo">id, appId</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XmlDocument document containing the list of currency's for the application</returns>
        public XmlDocument FindCurrency(CurrencyInfo currencyInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);
            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            if (currencyInfo != null)
            {
                keyValues.Add("currency.id", currencyInfo.CurrencyId);
                keyValues.Add("currency.appId", currencyInfo.CurrencyAppId);
            }

            DebugLog("FindCurrency", keyValues);

            return (rest.GetXMLRequest("currency", keyValues));
        }
    }
}
