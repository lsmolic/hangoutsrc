using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class CallBacks_purchaseGambitCallback : PurchaseBasePage
{
    protected override void Page_Load(object sender, EventArgs e)
    {
        //http://example.com/gambit_postback?uid=21&amount=2000&time=1229281667&oid=1021

        string[] userInfo = GetUserInfoFromSecureInfo(Request.QueryString["uid"]);
        string xmlData = GetRequestXmlString(userInfo);
        string responseCode = ProcessGambitCallBack(xmlData);
        
        Response.Clear();
        Response.Write(responseCode);
        Response.End();
    }


    private string GetRequestXmlString(string[] userInfo)
    {
        StringBuilder sb = new StringBuilder("<Response command='gambit' >");

        sb.Append(String.Format("<data name='sessionId' value='{0}'></data>", userInfo[0]));
        sb.Append(String.Format("<data name='hangoutUserId' value='{0}'></data>", userInfo[1]));
        sb.Append(String.Format("<data name='paymentItemsUserId' value='{0}'></data>", userInfo[2]));

        foreach (string name in Request.QueryString)
        {
            string value = "";
            if (Request.QueryString[name] != "")
            {
                value = Request.QueryString[name];
            }
            sb.Append(String.Format("<data name='{0}' value='{1}'> </data>", name, value));
        }
        sb.Append(String.Format("<data name='ipAddress' value='{0}'></data>", GetIpAddress()));

        sb.Append("</Response>");

        return sb.ToString();
    }
}
    