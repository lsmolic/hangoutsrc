using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hangout.Shared
{
    public class UserInfo
    {
        private string userId;
        private string name;
        private string namespaceId;
        private string countryCode;
        private string externalKey;
        private string gender;              //  M or F
        private string dateOfBirth;         //  yyyy/MM/dd
        private string emailAddress;
        private string tags;
        private string ipAddress;

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string NamespaceId
        {
            get { return namespaceId; }
            set { namespaceId = value; }
        }

        public string CountryCode
        {
            get { return countryCode; }
            set { countryCode = value; }
        }

        public string ExternalKey
        {
            get { return externalKey; }
            set { externalKey = value; }
        }

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public string DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }

        public string Tags
        {
            get { return tags; }
            set { tags = value; }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
    }

    public class UserId
    {
        private string userId;
        private string ipAddress;
        private string secureKey;

        public UserId(string userId)
        {
            this.userId = userId;
        }

        public string Id
        {
            get { return userId; }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public string SecureKey
        {
            get { return secureKey; }
            set { secureKey = value; }
        }

        public override string ToString()
        {
            return userId.ToString();
        }
    }


	public class ItemsInfo
	{
		private string itemName;
		private string itemTypeNames;
		private string itemTypeName;
		private string buttonName;
		private string description;
		private string smallImageUrl;
		private string mediumImageUrl;
		private string largeImageUrl;
		private string available;
		private string storeName;
		private string ipAddress;

		public string ItemName
		{
			get { return itemName; }
			set { itemName = value; }
		}

		public string ItemTypeNames
		{
			get { return itemTypeNames; }
			set { itemTypeNames = value; }
		}

		public string ItemTypeName
		{
			get { return itemTypeName; }
			set { itemTypeName = value; }
		}

		public string ButtonName
		{
			get { return buttonName; }
			set { buttonName = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public string SmallImageUrl
		{
			get { return smallImageUrl; }
			set { smallImageUrl = value; }
		}

		public string MediumImageUrl
		{
			get { return mediumImageUrl; }
			set { mediumImageUrl = value; }
		}

		public string LargeImageUrl
		{
			get { return largeImageUrl; }
			set { largeImageUrl = value; }
		}

		public string Available
		{
			get { return available; }
			set { available = value; }
		}

		public string StoreName
		{
			get { return storeName; }
			set { storeName = value; }
		}

		public string IPAddress
		{
			get { return ipAddress; }
			set { ipAddress = value; }
		}
	}

    public class PropertiesInfo
    {
        private string color;

        public string Color
        {
            get { return color; }
            set { color = value; }
        }
    }
        
    public class CurrencyInfo
    {
        private string currencyId;
        private string currencyAppId;
        private string currencyName;

        public string CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public string CurrencyAppId
        {
            get { return currencyAppId; }
            set { currencyAppId = value; }
        }

        public string CurrencyName
        {
            get { return currencyName; }
            set { currencyName = value; }
        }
    }

    public class PaymentInfo
    {
        private string countryCode;
        private string currencyCode;
        private string virtualCurrencyIds;

        public string CountryCode
        {
            get { return countryCode; }
            set { countryCode = value; }
        }
        
        public string CurrencyCode
        {
            get { return currencyCode; }
            set { currencyCode = value; }
        }

        public string VirtualCurrencyIds
        {
            get { return virtualCurrencyIds; }
            set { virtualCurrencyIds = value; }
        }
    }

    public class PurchaseInfo
    {
        private string accountId;
        private string offerIds;
        private string externalTxnId;
        private string recipientUserId;
        private string noteToRecipient;
        private string token;
        private string payerId;
       
        public string AccountId
        {
            get { return accountId; }
            set { accountId = value; }
        }

        // used for gifting
        public string RecipientUserId  
        {
            get { return recipientUserId; }
            set { recipientUserId = value; }
        }

        // used for gifting
        public string NoteToRecipient
        {
            get { return noteToRecipient; }
            set { noteToRecipient = value; }
        }
        
        public string OfferId
        {
            get { return offerIds; }
            set { offerIds = value; }
        }
        
        public string OfferIds
        {
            get { return offerIds; }
            set { offerIds = value; }
        }

        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        public string PayerId
        {
            get { return payerId; }
            set { payerId = value; }
        }
        
        public string ExternalTxnId
        {
            get { return externalTxnId; }
            set { externalTxnId = value; }
        }

    }
    
    public class GatewayInfo
    {
        private string paymentGatewayConfigId;
        private string returnURL;
        private string cancelURL;

        public string PaymentGatewayConfigId
        {
            get { return paymentGatewayConfigId; }
            set { paymentGatewayConfigId = value; }
        }

        public string ReturnURL
        {
            get { return returnURL; }
            set { returnURL = value; }
        }

        public string CancelURL
        {
            get { return cancelURL; }
            set { cancelURL = value; }
        }
    }

    public class PurchaseRecurringInfo
    {
        private string billingDescription;
        private string startDate;
        private string numPayments;
        private string payFrequency;

        public string BillingDescription
        {
            get { return billingDescription; }
            set { billingDescription = value; }
        }

        public string StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public string NumPayments
        {
            get { return numPayments; }
            set { numPayments = value; }
        }

        public string PayFrequency
        {
            get { return payFrequency; }
            set { payFrequency = value; }
        }
    }

    public class ResultFilter
    {
        private string startIndex;
        private string blockSize;
        private string filter;
        private string maxRemaining;
        private string latestEndDate;
        private string orderBy;
        private string descending;
        private string itemTypeNames;

        public string StartIndex
        {
            get { return startIndex; }
            set { startIndex = value; }
        }

        public string BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }

        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        public string ItemTypeNames
        {
            get { return itemTypeNames; }
            set { itemTypeNames = value; }
        }

        public string MaxRemaining
        {
            get { return maxRemaining; }
            set { maxRemaining = value; }
        }

        public string LatestEndDate
        {
            get { return latestEndDate; }
            set { latestEndDate = value; }
        }

        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        public string Descending
        {
            get { return descending; }
            set { descending = value; }
        }
    }

    public class CatalogInfo
    {
        private string fileName;
        private string appId;
        private string itemTypeNames;
        private string ipAddress;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string AppId
        {
            get { return appId; }
            set { appId = value; }
        }

        public string ItemTypeNames
        {
            get { return itemTypeNames; }
            set { itemTypeNames = value; }
        }

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
    }

    public class StoreInfo
    {
        private string storeName;
        private string ipAddress;

        public string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
    }

    public class TransferInfo
    {
        private string amount;
        private string debitAccountId;
        private string creditAccountId;
        private string transferType;
        private string externalTxnId;
        private string ipAddress;

        public string Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string DebitAccountId
        {
            get { return debitAccountId; }
            set { debitAccountId = value; }
        }

        public string CreditAccountId
        {
            get { return creditAccountId; }
            set { creditAccountId = value; }
        }

        public string TransferType
        {
            get { return transferType; }
            set { transferType = value; }
        }

        public string ExternalTxnId
        {
            get { return externalTxnId; }
            set { externalTxnId = value; }
        }


        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
    }

    public class CreditCardInfo
    {
        private string creditCardnumber;
        private string creditCardtype;
        private string expireDate;
        private string securityCode;
        private string firstName;
        private string middleName;
        private string lastName;
        private string address;
        private string zipCode;
        private string city;
        private string stateProvince;
        private string countryCode;
        private string phoneNumber;

        public string CreditCardnumber
        {
            get { return creditCardnumber; }
            set { creditCardnumber = value; }
        }

        public string CreditCardtype
        {
            get { return creditCardtype; }
            set { creditCardtype = value; }
        }

        public string ExpireDate
        {
            get { return expireDate; }
            set { expireDate = value; }
        }

        public string SecurityCode
        {
            get { return securityCode; }
            set { securityCode = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string StateProvince
        {
            get { return stateProvince; }
            set { stateProvince = value; }
        }

        public string CountryCode
        {
            get { return countryCode; }
            set { countryCode = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
    }
}