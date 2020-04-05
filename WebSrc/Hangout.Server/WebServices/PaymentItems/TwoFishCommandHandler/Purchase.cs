using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    internal class Purchase : TwoFishCommandBase
    {
        public Purchase() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }


        /// <summary>
        /// Implements call to Twofish method purchase to retreive the user payment history
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="filter">StartIndex, BlockSize</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XmlDocument document containing the users purchase history</returns>
        public XmlDocument PurchaseHistory(UserId userId, ResultFilter filter, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            keyValues.Add("id", userId.ToString());

            if (filter != null)
            {
                keyValues.Add("fetchRequest.startIndex", filter.StartIndex);
                keyValues.Add("fetchRequest.blockSize", filter.BlockSize);
            }
            DebugLog("purchase", keyValues);

            return (rest.GetXMLRequest("purchase", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/paypal/checkout to start the Paypal checkout process
        /// Purchase a foreign exchange offer 
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferId, ExternalTxnId</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing Paypal url used to continue the purchase process</returns>
        public XmlDocument PayPalCheckout(UserId userId, PurchaseInfo purchaseInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {

            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);

            DebugLog("PayPalCheckout", keyValues);

            return (rest.PostXMLRequest("purchase/paypal/checkout", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/paypal to complete the Paypal checkout process
        /// This is called from the Paypal callback method
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferId, Token, PayerId, ExternalTxnId</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing purchase results</returns>
        public XmlDocument PayPalPurchase(UserId userId, PurchaseInfo purchaseInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("token", purchaseInfo.Token);
            keyValues.Add("payerId", purchaseInfo.PayerId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);

            DebugLog("PayPalPurchase", keyValues);

            return (rest.PostXMLRequest("purchase/paypal", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/paypal to cancel the Paypal checkout process
        /// This is called from the Paypal callback method
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferId, Token, ExternalTxnId</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing canel purchase results</returns>
        public XmlDocument PayPalCancel(UserId userId, PurchaseInfo purchaseInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("token", purchaseInfo.Token);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);

            DebugLog("PayPalCancel", keyValues);

            return (rest.DeleteXMLRequest("purchase/paypal", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/paypal/checkout/recurring to start the Paypal recurring payment purchase checkout process
        /// Recurring purchase of a foreign exchange 
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferId, ExternalTxnId, BillingDescription, StartDate, NumPayments, PayFrequency</param>
        /// <param name="recurringInfo">BillingDescription, StartDate, NumPayments, PayFrequency</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing Paypal url used to continue the recurring payment purchase process</returns>
        public XmlDocument PayPalRecurringCheckout(UserId userId, PurchaseInfo purchaseInfo, PurchaseRecurringInfo recurringInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("billingDescription", recurringInfo.BillingDescription);
            keyValues.Add("startDateString", recurringInfo.StartDate);
            keyValues.Add("numPayments", recurringInfo.NumPayments);
            keyValues.Add("payFrequency", recurringInfo.PayFrequency);

            DebugLog("PayPalRecurringCheckout", keyValues);

            return (rest.PostXMLRequest ("purchase/paypal/checkout/recurring", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/paypal/recurring to complete the Paypal recurring payment purchase checkout process
        /// This is called from the Paypal callback method
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferId, ExternalTxnId, Token, PayerId</param>
        /// <param name="recurringInfo">BillingDescription, StartDate, NumPayments, PayFrequency</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing recurring purchase results</returns>
        public XmlDocument PayPalRecurringPurchase(UserId userId, PurchaseInfo purchaseInfo, PurchaseRecurringInfo recurringInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("token", purchaseInfo.Token);
            keyValues.Add("payerId", purchaseInfo.PayerId);
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("billingDescription", recurringInfo.BillingDescription);
            keyValues.Add("startDateString", recurringInfo.StartDate);
            keyValues.Add("numPayments", recurringInfo.NumPayments);
            keyValues.Add("payFrequency", recurringInfo.PayFrequency);

            DebugLog("PayPalRecurringPurchase", keyValues);

            return (rest.PostXMLRequest("purchase/paypal/recurring", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/creditCard to complete a credit card purchase 
        /// Purchase of a foreign exchange offer 
        /// </summary>
        /// <param name="userId">userId, IPAddress, SecureKey (optional)</param>
        /// <param name="purchaseInfo">OfferId, ExternalTxnId, Token, PayerId</param>
        /// <param name="creditCardInfo">CreditCardnumber, CreditCardtype, ExpireDate, SecurityCode, FirstName, MiddleName, LastName, Address, City, StateProvince, ZipCode, CountryCode, PhoneNumber</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing credit card purchase results</returns>
        public XmlDocument PurchaseCreditCard(UserId userId, PurchaseInfo purchaseInfo, CreditCardInfo creditCardInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("secureKey", userId.SecureKey);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("cc.number", creditCardInfo.CreditCardnumber);
            keyValues.Add("cc.type", creditCardInfo.CreditCardtype);
            keyValues.Add("cc.expDate", creditCardInfo.ExpireDate);
            keyValues.Add("cc.securityCode", creditCardInfo.SecurityCode);
            keyValues.Add("cc.firstName", creditCardInfo.FirstName);
            keyValues.Add("cc.middleName", creditCardInfo.MiddleName);
            keyValues.Add("cc.surname", creditCardInfo.LastName);
            keyValues.Add("cc.address", creditCardInfo.Address);
            keyValues.Add("cc.city", creditCardInfo.City);
            keyValues.Add("cc.stateProvince", creditCardInfo.StateProvince);
            keyValues.Add("cc.zip", creditCardInfo.ZipCode);
            keyValues.Add("cc.countryCode", creditCardInfo.CountryCode);
            keyValues.Add("cc.phoneNumber", creditCardInfo.PhoneNumber);

            //Do not log the credit card number in plain text
            string[] itemsToEncrypt = new string[] { "cc.number", "cc.securityCode" };
            DebugLogEncryptList("PurchaseCreditCard", keyValues, itemsToEncrypt);

            return (rest.PostXMLRequest("purchase/creditCard", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/creditCard to complete a recurring credit card purchase 
        /// Recurring purchase of a foreign exchange offer
        /// </summary>
        /// <param name="userId">userId, IPAddress, SecureKey (optional)</param>
        /// <param name="purchaseInfo">OfferId, ExternalTxnId, Token, PayerId</param>
        /// <param name="creditCardInfo">CreditCardnumber, CreditCardtype, ExpireDate, SecurityCode, FirstName, MiddleName, LastName, Address, City, StateProvince, ZipCode, CountryCode, PhoneNumber</param>
        /// <param name="recurringInfo">StartDate, NumPayments, PayFrequency</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing recurring credit card purchase results</returns>
        public XmlDocument PurchaseCreditCardRecurring(UserId userId, PurchaseInfo purchaseInfo, CreditCardInfo creditCardInfo, PurchaseRecurringInfo recurringInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("secureKey", userId.SecureKey);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("cc.number", creditCardInfo.CreditCardnumber);
            keyValues.Add("cc.type", creditCardInfo.CreditCardtype);
            keyValues.Add("cc.expDate", creditCardInfo.ExpireDate);
            keyValues.Add("cc.securityCode", creditCardInfo.SecurityCode);
            keyValues.Add("cc.firstName", creditCardInfo.FirstName);
            keyValues.Add("cc.middleName", creditCardInfo.MiddleName);
            keyValues.Add("cc.surname", creditCardInfo.LastName);
            keyValues.Add("cc.address", creditCardInfo.Address);
            keyValues.Add("cc.city", creditCardInfo.City);
            keyValues.Add("cc.stateProvince", creditCardInfo.StateProvince);
            keyValues.Add("cc.zip", creditCardInfo.ZipCode);
            keyValues.Add("cc.countryCode", creditCardInfo.CountryCode);
            keyValues.Add("cc.phoneNumber", creditCardInfo.PhoneNumber);
            keyValues.Add("startDateString", recurringInfo.StartDate);
            keyValues.Add("numPayments", recurringInfo.NumPayments);
            keyValues.Add("payFrequency", recurringInfo.PayFrequency);

            //Do not log the credit card number in plain text
            string[] itemsToEncrypt = new string[] { "cc.number", "cc.securityCode"};
            DebugLogEncryptList("PurchaseCreditCard", keyValues, itemsToEncrypt);

            return (rest.PostXMLRequest("purchase/creditCardRecurring", keyValues));
        }

        /// <summary>
        /// Implements call to Twofish method purchase/oneClick to complete a one click credit card purchase 
        /// User SecureKey is required
        /// Purchase of a foreign exchange offer 
        /// </summary>
        /// <param name="userId">userId, IPAddress, SecureKey</param>
        /// <param name="purchaseInfo">OfferId, ExternalTxnId</param>
        /// <param name="creditCardInfo">SecurityCode</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML document containing one click credit card purchase results</returns>
        public XmlDocument PurchaseCreditCardOneClick(UserId userId, PurchaseInfo purchaseInfo, CreditCardInfo creditCardInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("secureKey", userId.SecureKey);
            keyValues.Add("offerId", purchaseInfo.OfferId);
            keyValues.Add("externalTxnId", purchaseInfo.ExternalTxnId);
            keyValues.Add("cc.securityCode", creditCardInfo.SecurityCode);

            //Do not log the credit card number in plain text
            string[] itemsToEncrypt = new string[] { "cc.securityCode" };
            DebugLogEncryptList("PurchaseCreditCard", keyValues, itemsToEncrypt);

            return (rest.PostXMLRequest("purchase/oneClick", keyValues));
        }  

        public XmlDocument PaymentServiceURL(UserId userId, PurchaseInfo purchaseInfo, GatewayInfo gateWayInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress);
            keyValues.Add("forexOfferId", purchaseInfo.OfferId);
            keyValues.Add("paymentGatewayConfigId", gateWayInfo.PaymentGatewayConfigId);
            keyValues.Add("returnURL", gateWayInfo.ReturnURL);
            keyValues.Add("cancelURL", gateWayInfo.CancelURL);

            DebugLog("PaymentServiceURL", keyValues);

            return (rest.GetXMLRequest("purchase/paymentServiceURL", keyValues));
        }  
    }
}
