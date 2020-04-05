using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using Hangout.Server;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class HangoutUsers : HangoutCommandBase
    {
        public HangoutUsers() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Create a new user and give the user and initial amount of virtual coin
        /// </summary>
        /// <param name="userInfo">Name, IPAddress, CountryCode (optional), ExternalKey (optional), Gender (optional), 
        ///  DateOfBirth (optional), EmailAddress (optional), Tags (optional)</param>
        /// <param name="initialAmount">Number of virtual coin to give to the user</param>
        /// <returns>XML Response document with user infomation and users virtual coin balance</returns>
        public XmlDocument CreateNewUser(UserInfo userInfo, string initialCoinAmount, string initialCashAmount)
        {
            XmlDocument response = null;

            try
            {
                XmlDocument responseNewUser = CreateNewUserCommand(userInfo);

                string coinCurrencyId = GetConfigurationAppSetting("CoinCurrencyId", "");
                string xPathCoinAccount = String.Format("/Response/user/accounts/account[@currencyId='{0}']", coinCurrencyId);
                XmlNode currencyCoinNode = responseNewUser.SelectSingleNode(xPathCoinAccount);

                string cashCurrencyId = GetConfigurationAppSetting("CashCurrencyId", "");
                string xPathCashAccount = String.Format("/Response/user/accounts/account[@currencyId='{0}']", cashCurrencyId);
                XmlNode currencyCashNode = responseNewUser.SelectSingleNode(xPathCashAccount);

                response = RemoveResponseNodes(responseNewUser, "/Response/user", "accounts");

                if (IsNewAccount(response, currencyCoinNode, currencyCashNode))
                {
                    AddAttribute(response, "/Response/user", "NewAccount", "true");
                    response = AddNode(response, "/Response/user", "InitialCoinAmount", String.Format("Value='{0}'", initialCoinAmount));
                    response = AddNode(response, "/Response/user", "InitialCashAmount", String.Format("Value='{0}'", initialCashAmount));

                    string coinDebitAccountId = GetConfigurationAppSetting("CoinDebitAccountId", "");
                    AddFundsToUserAccount(currencyCoinNode, initialCoinAmount, coinDebitAccountId, userInfo.IPAddress);

                    string cashDebitAccountId = GetConfigurationAppSetting("CashDebitAccountId", "");
                    AddFundsToUserAccount(currencyCashNode, initialCashAmount, cashDebitAccountId, userInfo.IPAddress);
                }
                else
                {
                    AddAttribute(response, "/Response/user", "NewAccount", "false");
                    string userId = response.SelectSingleNode("/Response/user").Attributes["id"].InnerText;
                    string secureId = GetUsersSecureKey(userId, userInfo.IPAddress);
                    response.SelectSingleNode("/Response/user").Attributes["secureKey"].InnerText = secureId;
               }
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("CreateNewUser", ex);
            }

            return response;

        }

        /// <summary>
        /// Is this a new Account or is this an existing account
        /// </summary>
        /// <param name="response">The response document for the CreateNewUser call</param>
        /// <param name="currencyVirtualCoinNode">The virtual code node of the response document</param>
        /// <returns>true if the account is new else false</returns>
        private bool IsNewAccount(XmlDocument response, XmlNode currencyCoinNode, XmlNode currencyCashNode)
        {
            bool newUser = false;

            if (response.SelectSingleNode("/Response/user").Attributes["secureKey"].InnerText.Trim().Length > 0)
            {
                newUser = true;

                if (currencyCoinNode != null)
                {
                   string sBalance = currencyCoinNode.Attributes["balance"].InnerText;
                   if (!sBalance.StartsWith("0.0"))
                   {
                       newUser = false;
                   }
                }

                if (currencyCashNode != null)
                {
                    string sBalance = currencyCashNode.Attributes["balance"].InnerText;
                    if (!sBalance.StartsWith("0.0"))
                    {
                        newUser = false;
                    }
                }
            }
            return newUser;
        }
        
        /// <summary>
        /// Get the users secure key
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <returns>The users secure key</returns>
        private string GetUsersSecureKey(string userId, string ipAddress)
        {
            string secureKey = "";

            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Users";
                cmd.Verb = "SecureKey";

                cmd.Parameters.Add("userId", userId);
                cmd.Parameters.Add("ipAddress", ipAddress);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                secureKey = response.SelectSingleNode("/Response/user").Attributes["secureKey"].InnerText;
            }

            catch (Exception ex)
            {
                logError("GetUsersSecureKey", ex);
            }

            return secureKey;
        }
        
        /// <summary>
        /// Get User Balance for all there accounts.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>XML Document returning users account balances</returns>
        public XmlDocument GetUserBalance(string userId)
        {
            XmlDocument response = null;
            try
            {
                logDebug("GetUserBalance", userId); 
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Users";
                cmd.Verb = "UserInfo";

                cmd.Parameters.Add("userId", userId);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                response = RemoveResponseNodes(response, "/Response/user", "accounts");
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("GetUserBalance", ex);
            }

            return response;
        }

        /// <summary>
        /// Get User Inventory
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>XML Document containing the user inventory</returns>
        public XmlDocument GetUserInventory(string userId, ResultFilter filter)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Items";
                cmd.Verb = "ItemInstance";
  
                cmd.Parameters.Add("userId", userId);

                if (String.IsNullOrEmpty(filter.Filter))
                {
                    filter.Filter = "all";
                }
                cmd.Parameters.Add("filter", filter.Filter);

               // cmd.Parameters.Add("itemTypeName", filter.ItemTypeNames);
               // cmd.Parameters.Add("startIndex", filter.StartIndex);
               // cmd.Parameters.Add("blockSize", filter.BlockSize);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                int totalItems = 0;
                XmlNodeList itemInstanceNodes = response.SelectNodes("Response/itemInstances/itemInstance");
                if (itemInstanceNodes != null)
                {
                    totalItems = itemInstanceNodes.Count;
                }

                int startIndex = -1;
                int tempOut = -1;
                if (int.TryParse(filter.StartIndex, out tempOut))
                {
                    startIndex = tempOut;
                }

                int blockSize = -1;
                tempOut = -1;
                if (int.TryParse(filter.BlockSize, out tempOut))
                {
                    blockSize = tempOut;
                }

                AddAttribute(response, "/Response/itemInstances", "startIndex", startIndex.ToString());
                AddAttribute(response, "/Response/itemInstances", "blockSize", blockSize.ToString());
                AddAttribute(response, "/Response/itemInstances", "total", totalItems.ToString());
                AddAttribute(response, "/Response/itemInstances", "itemTypeNames", filter.ItemTypeNames);
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("GetUserInventory", ex);
            }

            return response;
        }

        /// <summary>
        /// Purchase Game Items 
        /// Currency Name can be used instead of the AccountId.
        /// Uses the AccountId (optional) specified and if not specified finds the account from the CurrencyName.
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferIds, ExternalTxnId (optional), AccountId (optional)</param>
        /// <param name="currencyInfo">CurrencyName</param>
        /// <returns>XML document with the purchase results</returns>
        public XmlDocument PurchaseItems(UserId userId, PurchaseInfo purchaseInfo, CurrencyInfo currencyInfo)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Store";
                cmd.Verb = "PurchaseItems";

                cmd.Parameters.Add("userId", userId.ToString());

                if (currencyInfo == null || currencyInfo.CurrencyName.Length == 0)
                {
                    cmd.Parameters.Add("accountId", purchaseInfo.AccountId);
                }
                else
                {
                    cmd.Parameters.Add("accountId", GetAccountIdFromUserIdAndCurrencyName(userId.ToString(), currencyInfo.CurrencyName));
                }
                cmd.Parameters.Add("offerIds", purchaseInfo.OfferIds);
                cmd.Parameters.Add("externalTxnId", purchaseInfo.ExternalTxnId);
                cmd.Parameters.Add("ipAddress", userId.IPAddress);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                response = UpdateResponseForErrors(response, cmd.Parameters);
                AddUserAccountBalances(userId, response, "/Response/errors");
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("PurchaseItems", ex);
            }

            return response;
        }

        /// <summary>
        /// Purchase Game Items  as a Gift
        /// Currency Name can be used instead of the AccountId.
        /// Uses the AccountId (optional) specified and if not specified finds the account from the CurrencyName.
        /// </summary>
        /// <param name="userId">userId, IPAddress</param>
        /// <param name="purchaseInfo">OfferId, RecipientUserId, note (optional), ExternalTxnId (optional), AccountId (optional)</param>
        /// <param name="currencyInfo">CurrencyName</param>
        /// <returns>XML document with the purchase results</returns>
        public XmlDocument PurchaseGift(UserId userId, PurchaseInfo purchaseInfo, CurrencyInfo currencyInfo)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Store";
                cmd.Verb = "PurchaseGift";

                cmd.Parameters.Add("userId", userId.ToString());

                if (currencyInfo == null || currencyInfo.CurrencyName.Length == 0)
                {
                    cmd.Parameters.Add("accountId", purchaseInfo.AccountId);
                }
                else
                {
                    cmd.Parameters.Add("accountId", GetAccountIdFromUserIdAndCurrencyName(userId.ToString(), currencyInfo.CurrencyName));
                }


                if (purchaseInfo.OfferId.Length > 0)
                {
                    cmd.Parameters.Add("offerId", purchaseInfo.OfferId);
                }
                else
                {
                    cmd.Parameters.Add("offerId", purchaseInfo.OfferIds);
                }
                cmd.Parameters.Add("recipientUserId", purchaseInfo.RecipientUserId);
                cmd.Parameters.Add("note", purchaseInfo.NoteToRecipient);
                cmd.Parameters.Add("externalTxnId", purchaseInfo.ExternalTxnId);
                cmd.Parameters.Add("ipAddress", userId.IPAddress);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                response = UpdateResponseForErrors(response, cmd.Parameters);
                AddUserAccountBalances(userId, response, "/Response/errors");
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("PurchaseGift", ex);
            }

            return response;
        }

        /// <summary>
        /// Add Virtual Coin to the User account
        /// </summary>
        /// <param name="userInfo">UserId, IPAddress</param>
        /// <param name="amount">The amount of coin to add</param>
        /// <returns>XML Document containing users new balance</returns>
        public XmlDocument AddVirtualCoinForUser(UserInfo userInfo, string amount)
        {
            XmlDocument response = null;
            XmlDocument responseBalance = null;

            try
            {
                responseBalance = GetUserBalance(userInfo.UserId);

                UpdateResponseNounVerb(responseBalance, "HangoutUsers", "AddVirtualCoinForUser");

                string coinCurrencyId = GetConfigurationAppSetting("CoinCurrencyId", "");

                string xPathCurrency = String.Format("/Response/user/accounts/account[@currencyId='{0}']", coinCurrencyId);
                string creditAccountId = responseBalance.SelectSingleNode(xPathCurrency).Attributes["id"].InnerText;
                string coinDebitAccountId = GetConfigurationAppSetting("CoinDebitAccountId", "");

                int newBalanceValue = AddFundsForUser(creditAccountId, coinDebitAccountId, amount, userInfo.IPAddress);

                if (newBalanceValue > 0)
                {
                    creditAccountId = responseBalance.SelectSingleNode(xPathCurrency).Attributes["balance"].InnerText = newBalanceValue.ToString();
                }
                logDebug("AddVirtualCoinForUser", String.Format("userId={0} | vcoin={1}", userInfo.UserId, amount));

                response = responseBalance;
            }
            catch (Exception ex)
            {
                string errorText = "";

                if (responseBalance != null)
                {
                    errorText = responseBalance.InnerXml;
                }
                response = CreateErrorDoc(String.Format("{0} {1}", ex.Message, errorText));
                logError("AddVirtualCoinForUser", ex);
            }

            return response;
        }


        /// <summary>
        /// Get the Users Email Address stored at twofish
        /// </summary>
        /// <param name="userId">The PaymentItems userid</param>
        /// <returns>The xml document containing the users Email address</returns>
        public XmlDocument GetUserEmailAddress(UserId userId)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Users";
                cmd.Verb = "UserInfo";

                cmd.Parameters.Add("userId", userId.ToString());

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                string userIdReturned = response.SelectSingleNode("/Response/user").Attributes["id"].InnerText;
                string emailAddress = response.SelectSingleNode("/Response/user").Attributes["emailAddress"].InnerText;

                response.LoadXml(String.Format("<response noun='Users' verb='UserInfo'><user id='{0}' emailAddress='{1}' /></response>", userIdReturned, emailAddress));
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("GetUserEmailAddress", ex);
            }

            return response;
        }

        /// <summary>
        /// Update the users email address at twofish
        /// </summary>
        /// <param name="userInfo">UserInfo containing tne userId, ipAddressm and emailAddress</param>
        /// <returns>The xml results document containing the email address</returns>
        public XmlDocument UpdateUserEmail(UserInfo userInfo)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();

                cmd.Noun = "Users";
                cmd.Verb = "UpdateUser";

                cmd.Parameters.Add("userId", userInfo.UserId);
                cmd.Parameters.Add("ipAddress", userInfo.IPAddress);
                cmd.Parameters.Add("emailAddress", userInfo.EmailAddress);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                string userIdReturned = response.SelectSingleNode("/Response/user").Attributes["id"].InnerText;
                string emailAddress = response.SelectSingleNode("/Response/user").Attributes["emailAddress"].InnerText;

                response.LoadXml(String.Format("<response noun='Users' verb='UserInfo'><user id='{0}' emailAddress='{1}' /></response>", userIdReturned, emailAddress));
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("UpdateUserEmail", ex);
            }

            return response;
       }

        /// <summary>
        /// Create and Exexutes Twofish command to Create a New User
        /// </summary>
        /// <param name="userInfo">Name, IPAddress, CountryCode (optional), ExternalKey (optional), Gender (optional), 
        ///  DateOfBirth (optional), EmailAddress (optional), Tags (optional)</param>
        /// <returns>XML Response document with user infomation</returns>
        private XmlDocument CreateNewUserCommand(UserInfo userInfo)
        {
            XmlDocument response = null;

            try
            {
                PaymentCommand cmd = new PaymentCommand();

                cmd.Noun = "Users";
                cmd.Verb = "CreateUser";

                cmd.Parameters.Add("name", userInfo.Name);
                cmd.Parameters.Add("namespaceId", "0");
                cmd.Parameters.Add("countryCode", userInfo.CountryCode);
                cmd.Parameters.Add("externalKey", userInfo.ExternalKey);
                cmd.Parameters.Add("gender", userInfo.Gender);
                cmd.Parameters.Add("dateOfBirth", userInfo.DateOfBirth);
                cmd.Parameters.Add("emailAddress", userInfo.EmailAddress);
                cmd.Parameters.Add("tags", userInfo.Tags);
                cmd.Parameters.Add("ipAddress", userInfo.IPAddress);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("CreateNewUserCommand", ex);
            }

            return response;
        }


        /// <summary>
        /// Add funds to the user account
        /// </summary>
        /// <param name="currencyNode">The currency node for the currecny to use</param>
        /// <param name="amountToAdd">The amount to add to the users account</param>
        /// <param name="debitAccountId">The debit account to use</param>
        /// <param name="ipAddress">The users IPaddress</param>
        internal void AddFundsToUserAccount(XmlNode currencyNode, string amountToAdd, string debitAccountId, string ipAddress)
        {
            string creditAccountId = currencyNode.Attributes["id"].InnerText;

            int amountToAddValue = -1;
            int.TryParse(amountToAdd, out amountToAddValue);

            if (amountToAddValue > 0)
            {
                int newBalanceValue = AddFundsForUser(creditAccountId, debitAccountId, amountToAddValue.ToString(), ipAddress);

                if (newBalanceValue > 0)
                {
                    currencyNode.Attributes["balance"].InnerText = newBalanceValue.ToString();
                }
            }
        }

        /// <summary>
        /// Add funds to the user account
        /// </summary>
        /// <param name="creditAccountId">Account to add the funds to</param>
        /// <param name="amount">Amount of virtual currency to add</param>
        /// <param name="userIPAddress"></param>
        /// <returns>Integer with the new balance</returns>
        private int AddFundsForUser(string creditAccountId, string debitAccountId, string amount, string userIPAddress)
        {
            int amountValue = -1;
            int newBalanceValue = -1;
            int.TryParse(amount, out amountValue);

            if (amountValue > 0)
            {
                XmlDocument responseFunds = TransferFundsForUser(creditAccountId, debitAccountId, amountValue.ToString(), userIPAddress);

                string xPathBalance = String.Format("/Response/transaction/details/detail/creditAccount/account[@id='{0}']", creditAccountId);
                string newBalance = responseFunds.SelectSingleNode(xPathBalance).Attributes["balance"].InnerText;

                string[] newBalanceArray = newBalance.Split('.');
            
                int.TryParse(newBalanceArray[0], out newBalanceValue);
              }

            return newBalanceValue;
        }


        /// <summary>
        /// Call the Twofish service to add funds to the user account
        /// </summary>
        /// <param name="creditAccountId">account to credit</param>
        /// <param name="debitAccountId">account to debit</param>
        /// <param name="amount">amount to transfer from debit to credit account</param>
        /// <param name="userIPAddress">users IP Address</param>
        /// <returns>XML Document with the new balances</returns>
        private XmlDocument TransferFundsForUser(string creditAccountId, string debitAccountId, string amount, string userIPAddress)
        {
            XmlDocument response = null;

            try
            {
                PaymentCommand cmd = new PaymentCommand();

                cmd.Noun = "Transfer";
                cmd.Verb = "CreateTransfer";

                cmd.Parameters.Add("amount", amount);
                cmd.Parameters.Add("debitAccountId", debitAccountId);
                cmd.Parameters.Add("creditAccountId", creditAccountId);
                cmd.Parameters.Add("transferType", "INITIAL_AMOUNT");
                cmd.Parameters.Add("ipAddress", userIPAddress);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
            }
            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("AddFundsForUser", ex);
            }

            return response;
        }

        /// <summary>
        /// Given the userID and currencyName retrieve the users accountId.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="currencyName">curencyName</param>
        /// <returns>string containing the accountID</returns>
        private string GetAccountIdFromUserIdAndCurrencyName(string userId, string currencyName)
        {
            string accountId = "";
            string currencyId = "";

            try
            {
                if (currencyName == "HOUTS")
                {
                    currencyId = GetConfigurationAppSetting("CashCurrencyId", "");
                }
                else
                {
                    currencyId = GetConfigurationAppSetting("CoinCurrencyId", "");
                }

                XmlDocument responseBalance = GetUserBalance(userId);
                string xPathCurrency = String.Format("/Response/user/accounts/account[@currencyId='{0}']" , currencyId);
                accountId = responseBalance.SelectSingleNode(xPathCurrency).Attributes["id"].InnerText;
            }
            catch (Exception ex)
            {
                logError("GetAccountIdFromUserIdAndCurrencyName", ex);
            }

            return accountId;
        }


        /// <summary>
        /// Add the users account balance node to the xml response document
        /// </summary>
        /// <param name="userId">The userID</param>
        /// <param name="response">The xml response document to add the account balance node to.</param>
        /// <param name="errorNode">The error node verifies that no errors occured in the transaction</param>
        private void AddUserAccountBalances(UserId userId, XmlDocument response, string errorNode)
        {
            XmlNode node = null;
            try
            {
                if (errorNode != null)
                {
                    node = response.SelectSingleNode(errorNode);
                }
                if (node == null)
                {
                    XmlDocument balanceNode = GetUserBalance(userId.ToString());
                    XmlNode accounts = balanceNode.SelectSingleNode("/Response/user/accounts");

                    AppendNode(response, "/Response", accounts);
                }
            }
            catch (Exception ex)
            {
                logError("AddUserAccountBalances", ex);
            }
        }
    }
 }

