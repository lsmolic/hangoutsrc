using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

// TwoFish command class to call user methods. Create User, Find User(s), Update User, User Widget, Secure Key etc..

namespace Hangout.Server.WebServices
{
    internal class Users : TwoFishCommandBase
    {
        public Users() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Implements call to user method to retreive user information 
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing user information</returns>
        public XmlDocument UserInfo(UserId userId, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("id", userId.ToString());

            DebugLog("UserInfo", keyValues);

            return (rest.GetXMLRequest("user", keyValues));   //twofish Command FindUser
        }

        /// <summary>
        /// Implements call to user/secureKey method to generate a new secure key for the user. 
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing user new secure key</returns>
        public XmlDocument SecureKey(UserId userId, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("ipAddress", userId.IPAddress );

            DebugLog("SecureKey", keyValues);

            return (rest.PostXMLRequest("user/secureKey", keyValues));
        }

        /// <summary>
        /// Implements call to user method to create a new Twofish user account
        /// </summary>
        /// <param name="userInfo">Name, NamespaceId, CountryCode, ExternalKey, Gender, DateOfBirth, EmailAddress, Tags, IPAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing user information</returns>
        public XmlDocument CreateUser(UserInfo userInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("user.name", userInfo.Name);
            keyValues.Add("user.namespaceId", userInfo.NamespaceId);
            keyValues.Add("user.countryCode", userInfo.CountryCode);
            keyValues.Add("user.externalKey", userInfo.ExternalKey);
            keyValues.Add("user.gender", userInfo.Gender);
            keyValues.Add("user.dateOfBirth", userInfo.DateOfBirth);
            keyValues.Add("user.emailAddress", userInfo.EmailAddress);
            keyValues.Add("user.tags", userInfo.Tags);
            keyValues.Add("ipAddress", userInfo.IPAddress);

            DebugLog("CreateUser", keyValues);

            return (rest.PostXMLRequest("user", keyValues));
        }

        /// <summary>
        /// Implements call to user/update method to update a Twofish user account
        /// </summary>
        /// <param name="userInfo">UserId, CountryCode, ExternalKey, Gender, DateOfBirth, EmailAddress, Tags, IPAddress</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing user information</returns>
        public XmlDocument UpdateUser(UserInfo userInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);
            keyValues.Add("user.id", userInfo.UserId);
            keyValues.Add("user.countryCode", userInfo.CountryCode);
            keyValues.Add("user.externalKey", userInfo.ExternalKey);
            keyValues.Add("user.gender", userInfo.Gender);
            keyValues.Add("user.dateOfBirth", userInfo.DateOfBirth);
            keyValues.Add("user.emailAddress", userInfo.EmailAddress);
            keyValues.Add("user.tags", userInfo.Tags);
            keyValues.Add("ipAddress", userInfo.IPAddress);

            DebugLog("UpdateUser", keyValues);

            return (rest.PostXMLRequest("user/update", keyValues));
        }

        /// <summary>
        /// Implements call to user/widget method to retreive user foreign exchange offers and payment methods
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="virtualCurrencyId">virtualCurrencyId</param>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XML response document containing user foreign exchange offers and payment methods</returns>
        public XmlDocument UserWidget(UserId userId, String virtualCurrencyId, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false, "DefaultPaymentGateway");
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("virtualCurrencyId", virtualCurrencyId);

            DebugLog("UserWidget", keyValues);

            return (rest.GetXMLRequest("user/widget", keyValues));
        }


        public XmlDocument PaymentOptions(UserId userId, string PaymentGatewayName, PaymentInfo paymentInfo, CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false, PaymentGatewayName);
            keyValues.Add("userId", userId.ToString());
            keyValues.Add("countryCode", paymentInfo.CountryCode);
            keyValues.Add("currencyCode", paymentInfo.CurrencyCode);
            keyValues.Add("virtualCurrencyIds", paymentInfo.VirtualCurrencyIds);

            DebugLog("PaymentOptions", keyValues);

            return (rest.GetXMLRequest("user/paymentOptions", keyValues));
        }
    }
}
