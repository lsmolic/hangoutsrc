using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Hangout.Server;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{

    /// <summary>
    /// The command Parser and dispatcher for PaymentItems commands
    /// The command handelers input is in a noun (refering to the class) and a verb referring to the method. 
    /// This class users reflection to determine the paramters to parse and pass to the PaymentItem.
    /// Each input type for a method has a method to parese the PaymentCommand arguments for the correct input variables.
    /// The output is the XML result returned from the invoked PaymentItem command.
    /// </summary>
    public class CommandParser : Logger 
    {
        public CommandParser()
        {
            SetLogger("CommandParser");
        }

        /// <summary>
        /// PaymentCommand parser and dispatcher
        /// </summary>
        /// <param name="command">PaymentCommand object</param>
        /// <returns>Response XML returned from the command</returns>
        public virtual XmlDocument ProcessRequest(PaymentCommand command)
        {
            XmlDocument paymentItemResponse = null;
            XmlDocument response = null;

            try
            {
                Type commandClassType = GetCommandClassType(command.Noun);
                if (commandClassType == null)
                {
                    throw new ArgumentException("PaymentItem noun could not be found");
                }

                MethodInfo methodInfo = commandClassType.GetMethod(command.Verb);
                if (methodInfo == null)
                {
                    throw new ArgumentException("PaymentItem verb could not be found");
                }

                object[] args = GetCommandTheArgs(methodInfo, command.Parameters, command.FileData);
                if (args == null)
                {
                    throw new ArgumentException("PaymentItem args could not be found");
                }

                Object invokeParam1 = Activator.CreateInstance(commandClassType);

                paymentItemResponse = (XmlDocument)methodInfo.Invoke(invokeParam1, args);

                response = UpdateResponseNode(paymentItemResponse, command.Noun, command.Verb);
            }

            catch (Exception ex)
            {
                logError("ProcessRequest", ex);
                response = CreateErrorDoc(ex.Message);
            }

            return response;
        }
        /// <summary>
        /// GetCommandClassType a virtual methods that needs to be implemented by the inherriting class.
        /// This method retreives the type for the class that holds the noun.
        /// </summary>
        /// <param name="commandNoun"></param>
        /// <returns>null</returns>
        public virtual Type GetCommandClassType(string commandNoun)
        {
            return null;
        }

        public Type GetCommandClassType(Assembly assembly, string commandNoun)
        {
            Type commandClassType = null;

            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                try
                {
                    if (type.IsClass && (type.Name == commandNoun))
                    {
                        commandClassType = type;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    logError("GetCommandClassType", ex);
                }
            }
            return commandClassType;
        }

         // loop though each parameter in the method and look for argIndex corresponding
         // 
        /// <summary>
        /// GetCommandTheArgs loops though each parameter in the method and look for argIndex corresponding
        /// parameter are in the command object or standard commands.
        /// </summary>
        /// <param name="methodInfo">Reflection MethodInfo used to retrieve the parameter types for the method to be invoked</param>
        /// <param name="parameters">Dictionary of the input parmeters</param>
        /// <param name="fileData">MemoryStream containing file data. In most cases null.</param>
        /// <returns>object containing the arguments for the method to be invoked</returns>
        private object[] GetCommandTheArgs(MethodInfo methodInfo, NameValueCollection parameters, MemoryStream fileData)
         {
             int numArgs = methodInfo.GetParameters().Length;
             object[] args = new object[numArgs];

             try
             {
                 for (int argIndex = 0; argIndex < numArgs; argIndex++)
                 {
                     ParameterInfo parameterInfo = methodInfo.GetParameters()[argIndex];

                     if (parameterInfo.ParameterType == typeof(UserId))
                     {
                         args[argIndex] = GetUserIdParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(CommmonKeyValues))
                     {
                         args[argIndex] = new CommmonKeyValues(); ;
                     }
                     else if (parameterInfo.ParameterType == typeof(BaseAddress))
                     {
                         args[argIndex] = new BaseAddress();
                     }
                     else if (parameterInfo.ParameterType == typeof(UserInfo))
                     {
                         args[argIndex] = GetUserInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(ItemsInfo))
                     {
                         args[argIndex] = GetItemsInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(CurrencyInfo))
                     {
                         args[argIndex] = GetCurrencyInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(PurchaseInfo))
                     {
                         args[argIndex] = GetPurchaseInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(PaymentInfo))
                     {
                         args[argIndex] = GetPaymentInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(CatalogInfo))
                     {
                         args[argIndex] = GetCatalogInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(StoreInfo))
                     {
                         args[argIndex] = GetStoreInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(ResultFilter))
                     {
                         args[argIndex] = GetResultFilterParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(TransferInfo))
                     {
                         args[argIndex] = GetTransferInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(CreditCardInfo))
                     {
                         args[argIndex] = GetCreditCardInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(PurchaseRecurringInfo))
                     {
                         args[argIndex] = GetPurchaseRecurringInfoParamaters(parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(GatewayInfo))
                     {
                         args[argIndex] = GetGatewayInfoParamaters(parameters);
                     } 
                     else if (parameterInfo.ParameterType == typeof(String))
                     {
                         args[argIndex] = GetStringParamater(parameterInfo.Name, parameters);
                     }
                     else if (parameterInfo.ParameterType == typeof(PostFile))
                     {
                         PostFile postFile = new PostFile();
                         fileData.Position = 0;
                         postFile.FileData =  fileData;
                         args[argIndex] = postFile;
                     }
                 }
             }

             catch (Exception ex)
             {
                 logError("GetCommandTheArgs", ex);
             }

             return args;

         }


        /// <summary>
         /// Retreives the UserID object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
         /// <returns>UserID object</returns>
        private UserId GetUserIdParamaters(NameValueCollection Parameters)
        {
            string userIdValue = GetValue(Parameters, "userId", "");
            UserId userId = new UserId(userIdValue);
            userId.IPAddress = GetValue(Parameters, "ipAddress", "");
            userId.SecureKey = GetValue(Parameters, "secureKey", "");

            return (userId);
        }

        /// <summary>
        /// Retreives the UserInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>UserInfo object</returns>
        private UserInfo GetUserInfoParamaters(NameValueCollection Parameters)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.UserId = GetValue(Parameters, "userId", "");
            userInfo.Name = GetValue(Parameters, "name", "");
            userInfo.NamespaceId = GetValue(Parameters, "namespaceId", "0");
            userInfo.CountryCode = GetValue(Parameters, "countryCode", "");
            userInfo.ExternalKey = GetValue(Parameters, "externalKey", "");
            userInfo.Gender = GetValue(Parameters, "gender", "");
            userInfo.DateOfBirth = GetValue(Parameters, "dateOfBirth", "");
            userInfo.EmailAddress = GetValue(Parameters, "emailAddress", "");
            userInfo.Tags = GetValue(Parameters, "tags", "");
            userInfo.IPAddress = GetValue(Parameters, "ipAddress", "");

            return (userInfo);
        }

        /// <summary>
        /// Retreives the CurrencyInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>CurrencyInfo object</returns>
        private CurrencyInfo GetCurrencyInfoParamaters(NameValueCollection Parameters)
        {
            CurrencyInfo currencyInfo = new CurrencyInfo();
            currencyInfo.CurrencyId = GetValue(Parameters, "currencyId", "");
            currencyInfo.CurrencyAppId = GetValue(Parameters, "currencyAppId", "");
            currencyInfo.CurrencyName = GetValue(Parameters, "currencyName", "");

            return (currencyInfo);
        }

        /// <summary>
        /// Retreives the ItemsInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>ItemsInfo object</returns>
        private ItemsInfo GetItemsInfoParamaters(NameValueCollection Parameters)
        {
            ItemsInfo itemsInfo = new ItemsInfo();
            itemsInfo.ItemName = GetValue(Parameters, "itemName", "");
            itemsInfo.ItemTypeName = GetValue(Parameters, "itemTypeName", "");
            itemsInfo.ItemTypeNames = GetValue(Parameters, "itemTypeNames", "");
            itemsInfo.ButtonName = GetValue(Parameters, "buttonName", "");
            itemsInfo.Description = GetValue(Parameters, "description", "");
            itemsInfo.SmallImageUrl = GetValue(Parameters, "smallImageUrl", "");
            itemsInfo.MediumImageUrl = GetValue(Parameters, "mediumImageUrl", "");
            itemsInfo.LargeImageUrl = GetValue(Parameters, "largeImageUrl", "");
            itemsInfo.Available = GetValue(Parameters, "available", "");
            itemsInfo.IPAddress = GetValue(Parameters, "ipAddress", "");

            return (itemsInfo);
        }

        /// <summary>
        /// Retreives the PurchaseInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>PurchaseInfo object</returns>
        private PurchaseInfo GetPurchaseInfoParamaters(NameValueCollection Parameters)
        {
            PurchaseInfo purchaseInfo = new PurchaseInfo();
            purchaseInfo.AccountId = GetValue(Parameters, "accountId", "");
            purchaseInfo.OfferId = GetValue(Parameters, "offerId", "");
            if ((purchaseInfo.OfferId == null)  || (purchaseInfo.OfferId.Length == 0))
            {
                purchaseInfo.OfferIds = GetValue(Parameters, "offerIds", "");
            }
            purchaseInfo.ExternalTxnId = GetValue(Parameters, "externalTxnId", "");
            purchaseInfo.RecipientUserId = GetValue(Parameters, "recipientUserId", "");
            purchaseInfo.NoteToRecipient = GetValue(Parameters, "noteToRecipient", "");
            purchaseInfo.Token = GetValue(Parameters, "token", "");
            purchaseInfo.PayerId = GetValue(Parameters, "payerId", "");

            return (purchaseInfo);
        }

        private GatewayInfo GetGatewayInfoParamaters(NameValueCollection Parameters)
        {
            GatewayInfo gatewayInfo = new GatewayInfo();
            gatewayInfo.PaymentGatewayConfigId = GetValue(Parameters, "paymentGatewayConfigId", "");
            gatewayInfo.ReturnURL = GetValue(Parameters, "returnURL", "");
            gatewayInfo.CancelURL = GetValue(Parameters, "cancelURL", "");

            return (gatewayInfo);
        }

        /// <summary>
        /// Retreives the PaymentInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>PaymentInfo object</returns>
        private PaymentInfo GetPaymentInfoParamaters(NameValueCollection Parameters)
        {
            PaymentInfo paymentInfo = new PaymentInfo();
            paymentInfo.VirtualCurrencyIds = GetValue(Parameters, "virtualCurrencyIds", "");
            paymentInfo.CountryCode = GetValue(Parameters, "countryCode", "");
            paymentInfo.CurrencyCode = GetValue(Parameters, "currencyCode", "");

            return (paymentInfo);
        }

        /// <summary>
        /// Retreives the ResultFilter object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>ResultFilter object</returns>
        private ResultFilter GetResultFilterParamaters(NameValueCollection Parameters)
        {
            ResultFilter resultFilterInfo = new ResultFilter();
            resultFilterInfo.ItemTypeNames = GetValue(Parameters, "itemTypeNames", "");
            resultFilterInfo.StartIndex = GetValue(Parameters, "startIndex", "");
            resultFilterInfo.BlockSize = GetValue(Parameters, "blockSize", "");
            resultFilterInfo.Filter = GetValue(Parameters, "filter", "");
            resultFilterInfo.OrderBy = GetValue(Parameters, "orderBy", "");

            return (resultFilterInfo);
        }

        /// <summary>
        /// Retreives the StoreInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>StoreInfo object</returns>
        private StoreInfo GetStoreInfoParamaters(NameValueCollection Parameters)
        {
            StoreInfo storeInfo = new StoreInfo();
            storeInfo.StoreName = GetValue(Parameters, "storeName", "");
            storeInfo.IpAddress = GetValue(Parameters, "ipAddress", "");

            return (storeInfo);
        }

        /// <summary>
        /// Retreives the CatalogInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>CatalogInfo object</returns>
        private CatalogInfo GetCatalogInfoParamaters(NameValueCollection Parameters)
        {
            CatalogInfo catalogInfo = new CatalogInfo();
            catalogInfo.FileName = GetValue(Parameters, "fileName", "");
            catalogInfo.AppId = GetValue(Parameters, "AppId", "");
            catalogInfo.ItemTypeNames = GetValue(Parameters, "itemTypeNames", "");
            catalogInfo.IpAddress = GetValue(Parameters, "ipAddress", "");

            return (catalogInfo);
        }

        /// <summary>
        /// Retreives the TransferInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>TransferInfo object</returns>
        private TransferInfo GetTransferInfoParamaters(NameValueCollection Parameters)
        {
            TransferInfo transferInfo = new TransferInfo();
            transferInfo.Amount = GetValue(Parameters, "amount", "");
            transferInfo.DebitAccountId = GetValue(Parameters, "debitAccountId", "");
            transferInfo.CreditAccountId = GetValue(Parameters, "creditAccountId", "");
            transferInfo.TransferType = GetValue(Parameters, "transferType", "");
            transferInfo.ExternalTxnId = GetValue(Parameters, "externalTxnId", "");
            transferInfo.IpAddress = GetValue(Parameters, "ipAddress", "");

            return (transferInfo);
        }

        /// <summary>
        /// Retreives the CreditCardInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>CreditCardInfo object</returns>
        private CreditCardInfo GetCreditCardInfoParamaters(NameValueCollection Parameters)
        {
            CreditCardInfo creditCardInfo = new CreditCardInfo();
            creditCardInfo.CreditCardnumber = GetValue(Parameters, "creditCardNumber", "");
            creditCardInfo.CreditCardtype = GetValue(Parameters, "creditCardType", "");
            creditCardInfo.ExpireDate = GetValue(Parameters, "expireDate", "");
            creditCardInfo.SecurityCode = GetValue(Parameters, "securityCode", "");
            creditCardInfo.FirstName = GetValue(Parameters, "firstName", "");
            creditCardInfo.MiddleName = GetValue(Parameters, "middleName", "");
            creditCardInfo.LastName = GetValue(Parameters, "lastName", "");
            creditCardInfo.Address = GetValue(Parameters, "address", "");
            creditCardInfo.City = GetValue(Parameters, "city", "");
            creditCardInfo.StateProvince = GetValue(Parameters, "state", "");
            creditCardInfo.ZipCode = GetValue(Parameters, "zipCode", "");
            creditCardInfo.CountryCode = GetValue(Parameters, "countryCode", "");
            creditCardInfo.PhoneNumber = GetValue(Parameters, "phoneNumber", "");

            return (creditCardInfo);
        }

        /// <summary>
        /// Retreives the PurchaseRecurringInfo object data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>PurchaseRecurringInfo object</returns>
        private PurchaseRecurringInfo GetPurchaseRecurringInfoParamaters(NameValueCollection Parameters)
        {
            PurchaseRecurringInfo purchaseRecurringInfo = new PurchaseRecurringInfo();
            purchaseRecurringInfo.BillingDescription = GetValue(Parameters, "billingDescription", "");
            purchaseRecurringInfo.StartDate = GetValue(Parameters, "startDate", "");
            purchaseRecurringInfo.NumPayments = GetValue(Parameters, "numPayments", "");
            purchaseRecurringInfo.PayFrequency = GetValue(Parameters, "payFrequency", "");

            return (purchaseRecurringInfo);
        }

        /// <summary>
        /// Retreives the String data from the Parameters Dictionary
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns>String</returns>
        private String GetStringParamater(string parameterName, NameValueCollection Parameters)
        {
           return (GetValue(Parameters, parameterName, ""));
        }

        /// <summary>
        /// Uppated the Response node to change the node name to "Response" and
        /// add attribures for the request noun and the request verb.
        /// </summary>
        /// <param name="twoFishResponse">twoFish Response XML document</param>
        /// <param name="noun">The request noun</param>
        /// <param name="verb">The request verb</param>
        /// <returns>XmlDocument containing the modified response</returns>
        private XmlDocument UpdateResponseNode(XmlDocument twoFishResponse, string noun, string verb)
        {
            XmlDocument response = new XmlDocument();

            try
            {
                XmlElement element = (XmlElement)twoFishResponse.GetElementsByTagName("response")[0];

                if (element == null)
                {
                    response = twoFishResponse;
                }
                else
                {
                    XmlElement newElement = CopyElementToName(element, "Response");
                    XmlNode responseNode = response.ImportNode(newElement, true);

                    response.AppendChild(responseNode);

                    response = AddAttributeToNode(response, "noun", noun);
                    response = AddAttributeToNode(response, "verb", verb);
                }
            }

            catch (Exception ex)
            {
                logError("UpdateResponseNode", ex);
            }

            return response;
        }


        /// <summary>
        /// Does a deep element copy changing the name of the top node element.
        /// </summary>
        /// <param name="element">The xml element to copy</param>
        /// <param name="tagName">The new top node element name</param>
        /// <returns>The modified element after the copy and element name change </returns>
        private XmlElement CopyElementToName(XmlElement element, string tagName) 
        {
            XmlElement newElement = element.OwnerDocument.CreateElement(tagName);
            for (int i = 0; i < element.Attributes.Count; i++) 
            {
                newElement.SetAttributeNode((XmlAttribute)
                element.Attributes[i].CloneNode(true));
            }

            for (int i = 0; i < element.ChildNodes.Count; i++) 
            {
                newElement.AppendChild(element.ChildNodes[i].CloneNode(true));
            }
            return newElement;
        }


        /// <summary>
        /// Add an attribute to the top level element of the document
        /// </summary>
        /// <param name="doc">The document to add the attribute to</param>
        /// <param name="attributeName">Attribute name to add</param>
        /// <param name="attributeValue">Attribute value to add</param>
        /// <returns>XML document with the attribute added</returns>
        private XmlDocument AddAttributeToNode(XmlDocument doc, string attributeName, string attributeValue)
        {
            XmlAttribute attribute = doc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;

            doc.DocumentElement.SetAttributeNode(attribute);

            return doc;
        }


        /// <summary>
        /// Get the value from the dictionary of Parameters
        /// </summary>
        /// <param name="Parameters">Dictionary containing Parameters</param>
        /// <param name="key">The key to locate in the Dictionary</param>
        /// <param name="defaultValue">The default value returned if the key is not found</param>
        /// <returns>The value for the key from the Parameters dictionary, if no key is found then the default value</returns>
        private string GetValue(NameValueCollection Parameters, string key, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                value = Parameters[key];
            }

            catch { }

            return value;
        }
        
        /// <summary>
        /// Creates and error XML document response document from a message.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Error XML document</returns>
        protected XmlDocument CreateErrorDoc(string message)
        {
            XmlDocument response = new XmlDocument();
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<response><Error><Message>" + message + "</Message></Error></response>");
                response.LoadXml(sb.ToString());
            }
            catch
            {
                response.LoadXml("<response><Error>unknown</Error></response>");
            }

            return response;
        }

   }
}

 