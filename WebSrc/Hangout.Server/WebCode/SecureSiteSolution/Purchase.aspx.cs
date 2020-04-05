using System;
using System.Data;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Hangout.Server;

public partial class Purchase : PurchaseBasePage 
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        SetPageExpireNow();

        divErrorPopUp.Visible = false;

        base.Page_Load(sender, e);

        divPackages.Visible = GetPaymentEnabledState("Packages");
        btnCreditCard.Visible = GetPaymentEnabledState("CreditCard");
        btnPayPal.Visible = GetPaymentEnabledState("PayPal");
        divGambit.Visible = GetPaymentEnabledState("Gambit");
        divZong.Visible = GetPaymentEnabledState("Zong");
    
        if (!Page.IsPostBack)
        {
            string param = "";
            if (!String.IsNullOrEmpty(Request.Form["pValueId"]))
            {
                param = Request.Form["pValueId"];

                string[] valuesArray = GetUserInfoFromSecureInfo(param);
                SessionGuid = valuesArray[0];
                HangoutUserId = valuesArray[1];
                PaymentItemUserId = valuesArray[2];
                SecureId = valuesArray[3];
                PaymentItemEmailAddress = GetUserEmailAddress(PaymentItemUserId);
            }
            else
            {
                param = CreatePValueId();
            }
            if (String.IsNullOrEmpty(param) == false)
            {
                frameGambit.Attributes["src"] = String.Format("{0}panel?k=63e3de5170cda3f347ad0e72b5ffbf13&uid={1}", PaymentItemGambitCallbackUrl, param);
            }
            PopulatePaymentOffers((string)Context.Items["offerId"]);
            //btnCreditCard.Text = PaymentResources.GetStringFromResourceFile("resBuyCreditCard");
            //btnPayPal.Text = PaymentResources.GetStringFromResourceFile("resBuyPayPal");
            //lblSelectPayment.Text = PaymentResources.GetStringFromResourceFile("resSelectPayment");
            //lblSelectPackage.Text = PaymentResources.GetStringFromResourceFile("resSelectPackage");
            //btnZong.Text = "Zong";
      
        }
    }
    
    private string GetFormData(string formKey)  
    {
        string formValue = "";

        if (!String.IsNullOrEmpty(Request.Form[formKey]))
        {
            formValue = Request.Form[formKey];
        }

        return formValue;
    }
    
    protected void btnPayPal_Click(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;

        string offerId = GetOfferId();
        if (!CheckForErrors(offerId, PaymentItemUserIdFromViewState, HangoutUserIdFromViewState, SessionGuidFromViewState))
        {
            PurchaseGameCurrencyPayPal(PaymentItemUserIdFromViewState, HangoutUserIdFromViewState, SessionGuidFromViewState, PaymentItemEmailAddressFromViewState, offerId);
        }
    }

	protected void backButton_click(object sender, EventArgs e)
	{
		Response.Clear();
		Response.Write("<script type=\"text/javascript\">parent.Hangout.closeChildFrame();</script>");
	}

    protected void btnCreditCard_Click(object sender, EventArgs e)
    {
        string offerId = GetOfferId();
        if (!CheckForErrors(offerId, PaymentItemUserIdFromViewState, HangoutUserIdFromViewState, SessionGuidFromViewState))
        {
            Context.Items["offerId"] = offerId;
            Context.Items["creditCardTypes"] = CreditCardTypesFromViewState;
            ServerTransfer("Creditcard.aspx");
        }
    }

    protected void btnZong_Click(object sender, EventArgs e)
    {
        ServerTransfer("zongOffers.aspx");
    }
    
    private void PopulatePaymentOffers(string offerId)
    {
        XmlDocument xmlCashDoc = GetPaymentOffers(PaymentItemUserId, "CASH");
        SetTheCreditCardTypes(xmlCashDoc);
        psCash.PopulatePaymentOffersRadioButtons(xmlCashDoc, offerId);
        
      //  XmlDocument xmlCoinDoc = GetPaymentOffers(PaymentItemUserId, "COIN");
       // psCoin.PopulatePaymentOffersRadioButtons(xmlCoinDoc);
    }   

    private string GetOfferId()
    {
        string offerId = psCash.SelectedItem;

     //   if (String.IsNullOrEmpty(offerId))
      //  {
      //      offerId = psCoin.SelectedItem;
      //  }

        return offerId;
    }

    private bool CheckForErrors(string offerId, string PaymentItemUserId, string HangoutUserId, string SessionGuid)
    {

        bool errors = false;
        string message = "";

        if (String.IsNullOrEmpty(offerId))
        {
            errors = true;
            message = PaymentResources.GetStringFromResourceFile("resNoOfferSelected");

            if (!String.IsNullOrEmpty(message))
            {
                ErrorMessage(message);
            }
        }

        return errors;
     }

    private void ErrorMessage(string message)
    {
        divErrorPopUp.Visible = true;
        textError.Text = message;
    }

  }
