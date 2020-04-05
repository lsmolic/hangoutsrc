using System;
using System.Collections;
using System.Collections.Specialized;
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

public partial class Zong : PurchaseBasePage
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        SetPageExpireNow();

        base.Page_Load(sender, e);

        if (!Page.IsPostBack)
        {
            if (IsZongCallback(Request))
            {
                Response.Write("<script>window.parent.location='complete.aspx?Msg=4';</script>");
                Response.End();
            }
            else
            {
                string offerUrl = (string)Context.Items["offerUrl"];

                Uri tempUri = new Uri(offerUrl);
                NameValueCollection queryString = HttpUtility.ParseQueryString(tempUri.Query);

                string transactionRef = queryString["transactionRef"];
                string offerDesc = queryString["itemDesc"];

                PurchaseGameCurrencyZong(PaymentItemUserId, HangoutUserId, SessionGuid, offerDesc, transactionRef, PaymentItemEmailAddress);
            
                frameZong.Attributes["src"] = offerUrl;
            }
        }

    }

    protected bool IsZongCallback(HttpRequest request)
    {
        bool zongCallback = false;
        if (!String.IsNullOrEmpty(request["transactionRef"]))
        {
            zongCallback = true;
        }
        return zongCallback;
    }

}
