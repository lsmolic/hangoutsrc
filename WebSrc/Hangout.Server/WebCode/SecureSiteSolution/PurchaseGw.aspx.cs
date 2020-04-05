using System;
using System.Web;
using System.Web.UI;

public partial class purchasegw : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(Request.QueryString["p"]))
        {
            pValueId.Value = Request.QueryString["p"];

            Random randomIndex = new Random();
            sValue.Value = randomIndex.Next(9999).ToString();
        }
    }
}
