using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class Zong : PurchaseBasePage
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        SetPageExpireNow();

        divErrorPopUp.Visible = false;

        base.Page_Load(sender, e);

        if (!Page.IsPostBack)
        {
            PopulateZongOffers();
        }
    }

    private void PopulateZongOffers()
    {
        XmlDocument xmlCashDoc = GetZongPaymentOffers(PaymentItemUserId, "CASH");
        psCash.PopulatePaymentOffersRadioButtons(xmlCashDoc, "");

       // XmlDocument xmlCoinDoc = GetZongPaymentOffers(PaymentItemUserId, "COIN");
      //  psCoin.PopulatePaymentOffersRadioButtons(xmlCoinDoc);
    }
        
    protected void btnZong_Click(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;


        string ZongBaseOfferUrl = GetOfferUrl();

        if (!CheckForErrors(ZongBaseOfferUrl, PaymentItemZongCallbackUrl, PaymentItemUserIdFromViewState, HangoutUserIdFromViewState, SessionGuidFromViewState))
        {
            string zongOfferUrl = String.Format("{0}&redirect={1}", ZongBaseOfferUrl, PaymentItemZongCallbackUrl);
            if (!String.IsNullOrEmpty(zongOfferUrl))
            {
                Context.Items["offerUrl"] = zongOfferUrl;
            } 
            ServerTransfer("zong.aspx");
        }
    }

    private string GetOfferUrl()
    {
        string offerUrl = psCash.SelectedUrl;

     //   if (String.IsNullOrEmpty(offerUrl))
     //   {
      //      offerUrl = psCoin.SelectedUrl;
     //   }

        return offerUrl;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ServerTransfer("purchase.aspx");
    }

    private string GetZongOffer()
    {
        string zongOffer = psCash.SelectedItem;

       // if (String.IsNullOrEmpty(zongOffer))
     //   {
      //      zongOffer = psCoin.SelectedItem;
      //  }

        return zongOffer;
    }

    private bool CheckForErrors(string offerBaseUrl, string callBackUrl, string PaymentItemUserId, string HangoutUserId, string SessionGuid)
    {
        bool errors = false;
        string message = "";

        if (String.IsNullOrEmpty(offerBaseUrl) || String.IsNullOrEmpty(callBackUrl))
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
