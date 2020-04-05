using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Text;
using System.Net;
using Hangout.Shared;
using Hangout.Server;

/// <summary>
/// Summary description for PurchaseBasePage
/// </summary>
public class PurchaseBasePage : System.Web.UI.Page
{
    private static readonly string mBaseServicesRoot = ConfigurationManager.AppSettings["WebServicesBaseUrl"];
    private static readonly string mPaymentItemZongCallback = ConfigurationManager.AppSettings["PaymentItemZongCallback"];
    private static readonly string mPaymentItemGambitCallback = ConfigurationManager.AppSettings["PaymentItemGambitCallback"];
    private static readonly string mSupportUrl = ConfigurationManager.AppSettings["SupportURL"];
    private string mPaymentItemUserId = "";
    private string mOfferId = "";
    private string mSecureId = "";
    private string mHangoutUserId = "";
    private string mSessionGuid = "";
    private string mCreditCardTypes = "";
    private string mEmailAddress = "";

    protected string PaymentItemUserId
    {
        get { return mPaymentItemUserId; }
        set
        {
            mPaymentItemUserId = value;
            ViewState["piUserId"] = mPaymentItemUserId;
        }
    }

    protected string PaymentItemUserIdFromViewState
    {
        get { return (string)ViewState["piUserId"]; }
    }

    protected string OfferId
    {
        get { return mOfferId; }
        set
        {
            mOfferId = value;
            ViewState["offerId"] = mOfferId;
        }
    }

    protected string OfferIdFromViewState
    {
        get { return (string)ViewState["offerId"]; }
    }

    protected string SecureId
    {
        get { return mSecureId; }
        set
        {
            mSecureId = value;
            ViewState["secureId"] = mSecureId;
        }
    }

    protected string SecureIdFromViewState
    {
        get { return (string)ViewState["secureId"]; }
    }

    protected string HangoutUserId
    {
        get { return mHangoutUserId; }
        set
        {
            mHangoutUserId = value;
            ViewState["hUserId"] = mHangoutUserId;
        }
    }

    protected string HangoutUserIdFromViewState
    {
        get { return (string)ViewState["hUserId"]; }
    }

    protected string SessionGuid
    {
        get { return mSessionGuid; }
        set
        {
            mSessionGuid = value;
            ViewState["sessionGuid"] = mSessionGuid;
        }
    }

    protected string CreditCardTypesFromViewState
    {
        get { return (string)ViewState["creditCardTypes"]; }
    }


    protected string CreditCardTypes 
    {
        get { return mCreditCardTypes; }
        set
        {
            mCreditCardTypes = value;
            ViewState["creditCardTypes"] = mCreditCardTypes;
        }
    }

    protected string PaymentItemEmailAddress
    {
        get { return mEmailAddress; }
        set
        {
            mEmailAddress = value;
            ViewState["piEmail"] = mEmailAddress;
        }
    }

    protected string PaymentItemEmailAddressFromViewState
    {
        get { return (string)ViewState["piEmail"]; }
    }

    protected string SessionGuidFromViewState
    {
        get { return (string)ViewState["sessionGuid"]; }
    }

    protected string UserIP
    {
        get { return Request.ServerVariables["remote_addr"]; }
    }


    public PurchaseBasePage()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    protected string BaseServicesRoot
    {
        get { return mBaseServicesRoot; }
    }

    protected string PaymentItemZongCallbackUrl
    {
        get { return HttpUtility.UrlEncode(mPaymentItemZongCallback); }
    }   

        protected string PaymentItemGambitCallbackUrl
    {
        get { return mPaymentItemGambitCallback; }
    }

    protected string SupportUrl
    {
        get { return mSupportUrl; }
    }   
        
    protected virtual void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PaymentItemUserId = (string)Context.Items["piUserId"];
            SecureId = (string)Context.Items["secureId"];
            HangoutUserId = (string)Context.Items["hUserId"];
            SessionGuid = (string)Context.Items["sessionGuild"];
            PaymentItemEmailAddress = (string)Context.Items["piEmail"];
        }
    }

    protected void ServerTransfer(string pageName)
    {
        ServerTransfer(PaymentItemUserIdFromViewState, HangoutUserIdFromViewState, SessionGuidFromViewState, SecureIdFromViewState, PaymentItemEmailAddressFromViewState, pageName);
    }

    protected void ServerTransfer(string piUserId, string hangoutUserID, string sessionGuid, string secureId, string emailAddress, string pageName)
    {
        Context.Items["piUserId"] = piUserId;
        Context.Items["secureId"] = secureId;
        Context.Items["hUserId"] = hangoutUserID;
        Context.Items["sessionGuild"] = sessionGuid;
        Context.Items["piEmail"] = emailAddress;
        Server.Transfer(pageName, false);
    }

    protected bool GetPaymentEnabledState(string paymentType)
    {
        bool paymentEnabled = true; 
        string paymentConfig = ConfigurationManager.AppSettings[paymentType];
        if (String.IsNullOrEmpty(paymentType) == false)
        {
            if (paymentConfig.Trim().ToLower() == "off")
            {
                paymentEnabled = false;
            }
        }

        return paymentEnabled;
    }

    protected XmlDocument GetPaymentOffers(string piUserId, string paymentType)
    {
        PaymentItemsRequest request = new PaymentItemsRequest();
        return (request.GetPaymentOffers(piUserId, paymentType, BaseServicesRoot));
    }

    protected XmlDocument GetZongPaymentOffers(string piUserId, string paymentType)
    {
        PaymentItemsRequest request = new PaymentItemsRequest();
        return (request.GetZongOffers(piUserId, paymentType, BaseServicesRoot));
    }

    protected void PurchaseGameCurrencyPayPal(string piUserId, string hangoutUserId, string sessionGuid, string emailAddress, string offerId)
    {
        PaymentItemsRequest request = new PaymentItemsRequest();
        string paypalURL = request.PurchaseGameCurrencyPayPal(piUserId, hangoutUserId, sessionGuid, emailAddress, offerId, UserIP, BaseServicesRoot);

        Response.Redirect(paypalURL, true);
    }

    protected string PurchaseGameCurrencyCreditCard(string transactionId, string piUserId, string hangoutUserId, string sessionGuid, string offerId, CreditCardInfo creditCardInfo, string emailAddress)
    {
        PaymentItemsRequest request = new PaymentItemsRequest();
        string response = request.PurchaseGameCurrencyCreditCard(transactionId, piUserId, hangoutUserId, sessionGuid, emailAddress, offerId, creditCardInfo, UserIP, BaseServicesRoot);

        return response;
    }

    protected void PurchaseGameCurrencyZong(string piUserId, string hangoutUserId, string sessionGuid, string offerDesc, string transactionRef, string emailAddress)
    {
        PaymentItemsRequest request = new PaymentItemsRequest();
        request.PurchaseGameCurrencyZong(piUserId, hangoutUserId, sessionGuid, emailAddress, offerDesc, transactionRef, UserIP, BaseServicesRoot);

    }

    protected string GetExternalTransaction(XmlDocument response)
    {
        string externalTransactionId = "";

        if (response.SelectSingleNode("/Response/purchaseResult") != null)
        {
            if (response.SelectSingleNode("/Response/purchaseResult").Attributes["externalTxnId"] != null)
            {
                externalTransactionId = response.SelectSingleNode("/Response/purchaseResult").Attributes["externalTxnId"].InnerText;
            }
        }
        return externalTransactionId;
    }

    protected void SetPageExpireNow()
    {
        Response.Buffer = true;  
        Response.ExpiresAbsolute = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)) ;
        Response.Expires = 0 ;
        Response.CacheControl = "no-cache" ;
    }


    protected void GetTheUserInformationFromHangoutId(string hangoutUserId)
    {
        try
        {
            PaymentItemsRequest request = new PaymentItemsRequest();
            XmlDocument userInformation = request.GetTheUserInformationFromHangoutId(hangoutUserId, BaseServicesRoot);

            if (userInformation.SelectSingleNode("/Accounts/Account") != null)
            {
                PaymentItemUserId = userInformation.SelectSingleNode("/Accounts/Account").Attributes["PIAccountId"].InnerText;
                SecureId = userInformation.SelectSingleNode("/Accounts/Account").Attributes["PISecureKey"].InnerText;
                PaymentItemEmailAddress = GetUserEmailAddress(PaymentItemUserId);
            }
        }
        catch  { }
    }


    protected string[] GetUserInfoFromSecureInfo(string secureInfo)
    {
        SimpleCrypto crypt = new SimpleCrypto();
        string[] returnValueArray = new string[4];

        string values = crypt.TDesDecrypt(secureInfo);
        string[] valuesArray = values.Split('&');

        returnValueArray[0] = valuesArray[0];  //SessionId
        returnValueArray[1] = valuesArray[1];  //Hangout userId

        PaymentItemsRequest request = new PaymentItemsRequest();
        XmlDocument userInformation = request.GetTheUserInformationFromHangoutId(valuesArray[1], BaseServicesRoot);
        if (userInformation.SelectSingleNode("/Accounts/Account") != null)
        {
            returnValueArray[2] = userInformation.SelectSingleNode("/Accounts/Account").Attributes["PIAccountId"].InnerText;
            returnValueArray[3] = userInformation.SelectSingleNode("/Accounts/Account").Attributes["PISecureKey"].InnerText;
       }

        return returnValueArray;
    }

    protected string CreatePValueId()
    { 
        SimpleCrypto crypt = new SimpleCrypto();

        string pValueId = String.Format("{0}&{1}", SessionGuid, HangoutUserId);

        string pValueIdEncrypt = crypt.TDesEncrypt(pValueId);

        return pValueIdEncrypt;
    }

    protected string GetUserEmailAddress(string userId)
    {
        string emailAddress = "";
        try
        {
            PaymentItemsRequest request = new PaymentItemsRequest();
            emailAddress = request.GetEmailAddress(userId, BaseServicesRoot);
        }
        catch {}

        return emailAddress;
    }


    protected void UpdateEmailAddress(string userId, string emailAddress)
    {
        try
        {
            PaymentItemsRequest request = new PaymentItemsRequest();
            request.UpdateEmailAddess(userId, UserIP, emailAddress, BaseServicesRoot);
        }
        catch {}

    }

    protected string GetIpAddress()
    {
        string ipAddress = "";

        try
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ipAddrList = Dns.GetHostAddresses(hostName);
            if (ipAddrList.Length > 0)
            {
                ipAddress = ipAddrList[0].ToString();
            }
        }
        catch { }

        return ipAddress;
    }


    protected void SetTheCreditCardTypes(XmlDocument xmlDoc)
    {
        string creditCardTypes = "";

        try
        {
            string delim = "";
            XmlNodeList paymentMethodsNodeList = xmlDoc.SelectNodes("/Response/paymentMethods/paymentMethod");

            foreach (XmlNode paymentMethodNode in paymentMethodsNodeList)
            {
                creditCardTypes += delim + paymentMethodNode.Attributes["name"].InnerText;
                delim = ",";
            }
        }

        catch
        {
            creditCardTypes = "";
        }

        CreditCardTypes = creditCardTypes;
    }

    protected string ProcessPayPalCallBack(HttpRequest request, string command)
    {
        string response = "";
        StringBuilder sb = new StringBuilder();

        sb.Append(String.Format("<Response command='{0}'>", command));

        foreach (string name in Request.QueryString)
        {
            string value = "";
            if (Request.QueryString[name] != "")
            {
                value = Request.QueryString[name];
            }
            sb.Append(String.Format("<data name='{0}' value='{1}'> </data>", name.Replace("PayerID", "payerId"), value));
        }

        string hostName = Dns.GetHostName();
        IPAddress[] ipAddrList = Dns.GetHostAddresses(hostName);
        if (ipAddrList.Length > 0)
        {
            sb.Append(String.Format("<data name='ipAddress' value='{0}'></data>", ipAddrList[0]));
        }

        sb.Append("</Response>");

        response = PayPalCallBack(sb.ToString());

        // compare response to http:// or https:// to determine if url or is error message
        if (response.StartsWith(String.Format("{0}://", Request.Url.Scheme)))  
        {
            Response.Redirect(response);
        }

        return response;
    }


     protected virtual string PayPalCallBack(string xmlInfo)
     {
          string response = "";

          PayPalCallbackHandler handler = new PayPalCallbackHandler();
          response = handler.PayPalCallBack(xmlInfo, BaseServicesRoot);

          return response;
      }


     protected string ProcessGambitCallBack(string xmlInfo)
     {
         string response = "";

         PaymentItemsRequest handler = new PaymentItemsRequest();
         response = handler.GambitCallBack(xmlInfo, BaseServicesRoot);

         return response;
     }

     protected string GetRequiredFieldMessage(ValidatorCollection validatorCollection)
     {
        string message = "";

        foreach (IValidator validator in validatorCollection)
        {
            if (!validator.IsValid)
            {
                message = PaymentResources.GetStringFromResourceFile("resRequiredFieldError");
                break;
            }
        }
        return message;
    }

     protected string[] GetOfferDescription(string offerId)
     {
         string[] description = new string[3];
         description[0] = offerId;

         //<Offer id="64" usd="9.95" vMoney="4500"><Description>9.95 USD for 4500 Cash </Description></Offer>

         try
         {
             XmlDocument xmlOffers = GetPaymentOffers(PaymentItemUserId, "CASH");
             XmlNode xmlOfferNode = xmlOffers.SelectSingleNode("/Response/offers/Offer[@id =" + offerId + "]");
             description[0] = xmlOfferNode.SelectSingleNode("Description").InnerText;
             description[1] = xmlOfferNode.Attributes["usd"].InnerText;
             description[2] = xmlOfferNode.Attributes["vMoney"].InnerText;
         }
         catch { }

         return description;
     }
}
