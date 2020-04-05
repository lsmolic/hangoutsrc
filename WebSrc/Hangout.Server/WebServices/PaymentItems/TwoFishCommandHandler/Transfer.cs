using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

// TwoFish command class to call Transfer methods. Create User, Find User(s), Update User, User Widget, Secure Key etc..

namespace Hangout.Server.WebServices
{
    internal class Transfer : TwoFishCommandBase
    {
        public Transfer() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Retreive information about a transfer of funds
        /// </summary>
        /// <param name="id">id of transfer to find</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing the information about the transfer</returns>
        public XmlDocument FindTransfer(string id, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);
            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            keyValues.Add("id", id);

            DebugLog("FindTransfer", keyValues);

            return (rest.GetXMLRequest("transfer", keyValues));
        }


        /// <summary>
        /// Does a tranfer of funds from one account to another account of the same currency.
        /// </summary>
        /// <param name="transferInfo">Amount, DebitAccountId, CreditAccountId, TransferType, ExternalTxnId, IpAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing results of the transfer</returns>
        public XmlDocument CreateTransfer(TransferInfo transferInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);
            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            keyValues.Add("amount", transferInfo.Amount);
            keyValues.Add("debitAccountId", transferInfo.DebitAccountId);
            keyValues.Add("creditAccountId", transferInfo.CreditAccountId);
            keyValues.Add("transferType", transferInfo.TransferType);
            keyValues.Add("externalTxnId", transferInfo.ExternalTxnId);
            keyValues.Add("ipAddress", transferInfo.IpAddress);

            DebugLog("CreateTransfer", keyValues);

            return (rest.PostXMLRequest("transfer", keyValues));
        }

    }
}


