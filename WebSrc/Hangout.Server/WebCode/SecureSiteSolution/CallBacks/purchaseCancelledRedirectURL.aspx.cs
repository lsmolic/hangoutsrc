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

public partial class paypal_purchaseCancelledRedirectURL : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Server.Transfer(@"..\complete.aspx?Msg=3", true); 
    }
}
