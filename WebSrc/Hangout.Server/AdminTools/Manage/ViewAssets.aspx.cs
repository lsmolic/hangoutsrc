using System;
using System.Collections;
using System.Collections.Generic;
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
using System.IO;
using System.Text;

public partial class Manage_ViewAssets : AdminPageBaseClass
{
	private XmlDocument mAssetListXmlDoc = null;
	private List<string> mCheckedBoxes = new List<string>();
	
	protected override void Page_Load(object sender, EventArgs e)
	{
		if(!IsPostBack)
		{
			AssetTypeDropDown.DataSource = Enum.GetNames(typeof(AssetType));
			AssetTypeDropDown.DataBind();

			AssetSubTypeDropDown.DataSource = Enum.GetNames(typeof(AssetSubType));
			AssetSubTypeDropDown.DataBind();
		}

		XmlDocument assetListXmlDoc = GetAssetList();
		mAssetListXmlDoc = assetListXmlDoc;
		ListAssetsByType(assetListXmlDoc);

	}

	private XmlDocument GetAssetList()
	{
		WebServiceRequest getAssetList = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Inventory", "GetAssetList");
		return getAssetList.GetWebResponse();
	}

	private void ListAssetsByType(XmlDocument xmlDocument)
	{
		

		string subTypeXPath = " and @AssetSubType='"+ AssetSubTypeDropDown.SelectedValue +"']";
		if (AssetSubTypeDropDown.SelectedValue == "NotSet")
		{
			subTypeXPath = "]";
		}
		XmlDataSource assetsXmlDataSource = new XmlDataSource();
		assetsXmlDataSource.Data = xmlDocument.OuterXml;
		assetsXmlDataSource.XPath = "Assets/Asset[@AssetType='" + AssetTypeDropDown.SelectedValue + "'" + subTypeXPath;
		assetsXmlDataSource.ID = "AssetsXmlDataSource";
		assetsXmlDataSource.EnableCaching = false;
		AssetGridView.DataSource = assetsXmlDataSource;
		AssetGridView.DataBind();
	}



	protected void AssetType_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		AssetSubTypeDropDown.SelectedIndex = 0;
		ListAssetsByType(mAssetListXmlDoc);
	}

	protected void AssetSubType_OnSelectedIndexChanged(object sender, EventArgs e)
	{

		ListAssetsByType(mAssetListXmlDoc);
	}
}
