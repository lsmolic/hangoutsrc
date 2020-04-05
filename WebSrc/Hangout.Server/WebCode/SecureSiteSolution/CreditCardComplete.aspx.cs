using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Text.RegularExpressions;
using Hangout.Shared;

public partial class CreditCardComplete : PurchaseBasePage
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        SetPageExpireNow(); 

        base.Page_Load(sender, e);
        if (!Page.IsPostBack)
        {
            string transactionId = (string)Context.Items["transactionId"];
            CreditCardInfo creditCardInfo = (CreditCardInfo)Context.Items["creditCardInfo"];
            string offerId = (string)Context.Items["offerId"];

            lThankYou.Text = PaymentResources.GetStringFromResourceFile("resCCPurchaseThankYou");

            lblTransaction.Text = PaymentResources.GetStringFromResourceFile("resCCTransaction");
            lTransaction.Text = GetDisplayTransactionID(transactionId);
            lblName.Text = PaymentResources.GetStringFromResourceFile("resCCName");
            lName.Text = String.Format("{0} {1}", creditCardInfo.FirstName, creditCardInfo.LastName);
            lblCardType.Text = PaymentResources.GetStringFromResourceFile("resCCCardType");
            lCardType.Text = creditCardInfo.CreditCardtype;
            lblExpireDate.Text = PaymentResources.GetStringFromResourceFile("resCCExpireDate"); 
            lExpireDate.Text = String.Format("{0}/{1}", creditCardInfo.ExpireDate.Substring(0, 2), creditCardInfo.ExpireDate.Substring(2));
            lblEmail.Text = PaymentResources.GetStringFromResourceFile("resCCEmail"); 
            lEmail.Text = PaymentItemEmailAddress;

            lblCreditCard.Text = PaymentResources.GetStringFromResourceFile("resCCCreditCard"); 
            lCreditCard.Text = GetCreditCardNumberMasked(creditCardInfo.CreditCardnumber);

            lblPackage.Text = PaymentResources.GetStringFromResourceFile("resCCPackage");
            lPackage.Text = GetOfferDescription(offerId)[0];

            lSupport.Text = PaymentResources.GetStringFromResourceFile("resCCSupport") + " ";  
            hlSupport.Text = SupportUrl;
            hlSupport.NavigateUrl = "mailto:" + SupportUrl;           
        }
    }

    private string GetCreditCardNumberMasked(string cardNumber)
    {
        string maskedCreditCard = "";
        if (cardNumber.StartsWith("3"))
        {
            maskedCreditCard = String.Format("xxxx-xxxxxx-x{0}", cardNumber.Substring(cardNumber.Length - 4));
        }
        else
        {
            maskedCreditCard = String.Format("xxxx-xxxx-xxxx-{0}", cardNumber.Substring(cardNumber.Length - 4));
        }

        return maskedCreditCard;
    }

    private string GetDisplayTransactionID(string transactionId)
    {
        string displayTransactionId = transactionId;

        int indexOfDash = transactionId.IndexOf("-");
        if (indexOfDash > 0)
        {
            displayTransactionId = transactionId.Substring(0, indexOfDash);
        }

        return displayTransactionId;
    }
}
