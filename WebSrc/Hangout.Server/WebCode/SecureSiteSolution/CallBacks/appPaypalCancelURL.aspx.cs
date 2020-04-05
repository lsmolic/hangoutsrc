using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class paypal_appPaypalCancelURL : PurchaseBasePage
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        SetPageExpireNow();

        divResponse.InnerText = ProcessPayPalCallBack(Request, "PaypalCancel");

        lblPayPalCancel.Text = PaymentResources.GetStringFromResourceFile("resPayPalWaitCancel");
    }

    protected override string PayPalCallBack(string xmlInfo)
    {
       ///  DebugClient.Client debugClient = new DebugClient.Client();
        //  debugClient.Connect();
        //  string callBackResponse = debugClient.PayPalCallBack("appPaypalCancelURL", sb.ToString());
        // debugClient.Disconnect();

        return base.PayPalCallBack(xmlInfo);
    }

}
