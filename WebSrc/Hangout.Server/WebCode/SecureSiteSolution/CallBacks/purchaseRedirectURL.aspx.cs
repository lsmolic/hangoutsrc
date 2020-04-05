using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class paypal_purchaseRedirectURL : PurchaseBasePage 
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        Server.Transfer(@"..\complete.aspx?Msg=2", true); 
    }

}
