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

public partial class FFrenzy : System.Web.UI.Page
{
	protected string UnityFileLocation
	{
		get { return "/TempUnityLocation/hangout.unity3d" + GetUnityFileQueryString(); }
	}
    protected void Page_Load(object sender, EventArgs e)
    {
		
    }
	private string GetUnityFileQueryString()
	{
		string selectedAvatar = Request.QueryString["SelectedAvatar"];
		string fbSessionId = Request.QueryString["fb_sig_session_key"];
		string fbAccountId = Request.QueryString["fb_sig_user"];

		return "?SelectedAvatar=" + selectedAvatar + 
				"&FbSessionId=" + fbSessionId + 
				"&FbAccountId=" + fbAccountId +
				"&AssetBaseUrl=" + WebConfig.AmazonUrlRoot +
				"&StateServerIp=" + WebConfig.StateServerIp + 
				"&WebServicesBaseUrl=" + WebConfig.WebServicesBaseUrl;
	}
}
