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
using System.Xml;
using System.Text;
using System.IO;

public partial class Manage_RemoveUser : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}
	protected void SubmitButton_Click(object sender, EventArgs e)
	{
		WebServiceRequest removeUsers = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Accounts", "RemoveAllTracesOfAccount");
		removeUsers.AddParam("accountId",AccountId.Text);
		removeUsers.AddParam("accountIdCsvList",AccountIdCSVList.Text);
		removeUsers.AddParam("fbAccountId",FbAccountId.Text);
		removeUsers.AddParam("nickName",NickName.Text);
		XmlDocument responseXml = removeUsers.GetWebResponse();

		StringBuilder stringBuilder = new StringBuilder();
		// We will use stringWriter to push the formated xml into our StringBuilder
		using (StringWriter stringWriter = new StringWriter(stringBuilder))
		{
			// We will use the Formatting of our xmlTextWriter to provide our indentation.
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				responseXml.WriteTo(xmlTextWriter);
			}
		}

		ServerResponse.Text = stringBuilder.ToString();
	}
}
