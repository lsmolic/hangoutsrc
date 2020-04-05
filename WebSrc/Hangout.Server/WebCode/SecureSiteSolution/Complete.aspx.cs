using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Complete : PurchaseBasePage
{
    protected override void Page_Load(object sender, EventArgs e)
    {
		string transactionComplete = "<img width=\"266px\" height=\"36px\" src=\"/Images/transaction_complete.png\" />";
        SetPageExpireNow();

        if (!Page.IsPostBack)
        {
            string message = Request.QueryString["Msg"];
            if (!String.IsNullOrEmpty(message))
            {
                switch (message)
                {
                    case "1":
						lblComplete.Text = transactionComplete;
                        messageId.Text = PaymentResources.GetStringFromResourceFile("resCreditCardSuccess");
                        break;

                    case "2":
						lblComplete.Text = transactionComplete;
                        messageId.Text = PaymentResources.GetStringFromResourceFile("resPayPalCompleted"); 
                        break;

                    case "3":
						lblComplete.Text = transactionComplete;
                        messageId.Text = PaymentResources.GetStringFromResourceFile("resPayPalCanceled");
                        break;

                    case "4":
						lblComplete.Text = transactionComplete;
                        messageId.Text = PaymentResources.GetStringFromResourceFile("resZongCompleted");
                        break;
                }
            }
        }
    }

	protected void return_button_onclick(object sender, EventArgs e)
	{
		Response.Clear();
		Response.Write("<script type=\"text/javascript\">parent.Hangout.closeChildFrame();</script>");
	}
}
