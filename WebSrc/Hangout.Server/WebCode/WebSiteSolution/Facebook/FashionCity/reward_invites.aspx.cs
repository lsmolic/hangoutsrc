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
using System.Net;

public partial class Facebook_FashionCity_reward_invites : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

		string inviterId = Request.QueryString["inviter"];
		string inviteType = Request.QueryString["invite_type"];
		string[] allKeys = Request.QueryString.AllKeys;
		
		string delimiter = "";
		string csvInvites = "";
		for( int i=0; i<allKeys.Length; i++)
		{
			if( allKeys[i] == "ids[]" )
			{
				csvInvites += delimiter + Request.QueryString[i];
				delimiter = ",";
			}
		}
		if(csvInvites != "")
		{
			string inviteReward = ConfigurationManager.AppSettings["invite_reward"];

			Uri serviceCall = new Uri(WebConfig.WebServicesBaseUrl + "Escrow/CreateMultipleEscrowTransactions"
						+ "?fromAccountId=" + inviterId + "&transactionType=" + inviteType
						+ "&toFacebookAccountIdCsvList=" + csvInvites + "&value=" + inviteReward);

			HttpWebRequest sendInvites = (HttpWebRequest)WebRequest.Create(serviceCall);
			sendInvites.GetResponse();
		}
    }
}
