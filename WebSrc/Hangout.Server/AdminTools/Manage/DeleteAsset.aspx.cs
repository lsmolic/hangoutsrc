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
using Hangout.Shared;
using System.Xml;

public partial class Manage_DeleteAsset : AdminPageBaseClass
{
	protected override void Page_Load(object sender, EventArgs e)
	{
		string assetId = Request.QueryString["assetId"];
		if (String.IsNullOrEmpty(assetId))
		{
			throw new Exception("You must specify an AssetId that you wish to remove");
		}

		WebServiceRequest removeAssetFromDb = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Inventory", "DeleteAsset");
		removeAssetFromDb.AddParam("assetId", assetId);
		XmlDocument response = removeAssetFromDb.GetWebResponse();

		XmlNode fileNameNode = response.SelectSingleNode("//FileName");
		if (fileNameNode != null)
		{
			WebServiceRequest removeAssetFromAmazon = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "FileStore", "DeleteFileStoredAtAmazon");
			removeAssetFromAmazon.AddParam("fileName", fileNameNode.InnerText);
			removeAssetFromAmazon.GetWebResponse();
		}

		Response.Write("Asset with ID:" + assetId + ", has been removed from Amazon and the Database. <a href='ViewAssets.aspx'>View Assets</a>");
	}
}
