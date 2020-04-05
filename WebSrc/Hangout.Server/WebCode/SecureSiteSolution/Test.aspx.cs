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
using Hangout.Server;

public partial class Test : System.Web.UI.Page
{
    private string mSessionGuid;

    protected void Page_Load(object sender, EventArgs e)
    {
       // mSessionGuid = (string)ViewState["sessionGuid"];

        if (mSessionGuid == null)
        {
            string sessionGuid = Guid.NewGuid().ToString();
            mSessionGuid = sessionGuid;
            ClientScript.RegisterHiddenField("sessionGuid", sessionGuid);
        }

       // ViewState["sessionGuid"] = mSessionGuid;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SimpleCrypto crypt = new SimpleCrypto();

        string url = "~/PurchaseGw.aspx?p=";
        string urlParams = String.Format("{0}&{1}", mSessionGuid, hUid.Text);

        string urlParamsEncrypt = crypt.TDesEncrypt(urlParams);

        Response.Redirect(url + urlParamsEncrypt);

    }
}       
