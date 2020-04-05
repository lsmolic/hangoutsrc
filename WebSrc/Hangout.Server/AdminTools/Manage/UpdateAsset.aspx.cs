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
using Hangout.Shared;
using System.Text;
using System.IO;

public partial class Manage_UpdateAsset : AdminPageBaseClass
{
	protected override void Page_Load(object sender, EventArgs e)
	{
		if (Request.QueryString["assetId"] == null)
		{
			Response.Clear();
			Response.Write("Missing Argument: You must provide an assetId in the QueryString");
		}
		else
		{
			PopulateForm(Request.QueryString["assetId"]);
		}

		if (!IsPostBack)
		{
			AssetTypeDropDown.DataSource = Enum.GetNames(typeof(AssetType));
			AssetTypeDropDown.DataBind();

			AssetSubTypeDropDown.DataSource = Enum.GetNames(typeof(AssetSubType));
			AssetSubTypeDropDown.DataBind();
		}
	}

	private void PopulateForm(string assetId)
	{
		WebServiceRequest getAssetInfo = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Inventory", "GetAssetList");
		getAssetInfo.AddParam("assetId", assetId);
		XmlDocument webResponse = getAssetInfo.GetWebResponse();

		
		XmlNode assetNode = webResponse.SelectSingleNode("//Asset");
		if (assetNode == null)
		{
			throw new Exception("Asset Node doesn't exist in XmlDocument: " + webResponse.OuterXml);
		}

		string assetTypeString = assetNode.Attributes["AssetType"].Value;
		string assetSubTypeString = assetNode.Attributes["AssetSubType"].Value;
		string assetName = assetNode.Attributes["AssetName"].Value;
		string fileName = assetNode.Attributes["FileName"].Value;

		AssetType assetType;
		assetType = (AssetType)Enum.Parse(typeof(AssetType), assetTypeString);

		AssetSubType assetSubType;
		assetSubType = (AssetSubType)Enum.Parse(typeof(AssetSubType), assetSubTypeString);

		XmlNode assetDataNode = webResponse.SelectSingleNode("//AssetData");
		
		StringBuilder stringBuilder = new StringBuilder();

		if (assetDataNode != null)
		{
			// We will use stringWriter to push the formated xml into our StringBuilder
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				// We will use the Formatting of our xmlTextWriter to provide our indentation.
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					assetDataNode.WriteTo(xmlTextWriter);
				}
			}
		}


		AssetTypeDropDown.SelectedValue = assetType.ToString();
		AssetSubTypeDropDown.SelectedValue = assetSubType.ToString();
		AssetIdRangeStartId.Text = assetId;
		AssetName.Text = assetName;
		CurrentFileName.Text = fileName;
		AssetData.Text = stringBuilder.ToString();
		


	}

	protected void UpdateAssetButton_Clicked(object sender, EventArgs e)
	{
		bool removeOldFileOnAmazon = false;
		HttpPostedFile file = AssetFileData.PostedFile;
		byte[] postData = null;
		string extension = "";
		string newFileName = "";
		if (file != null && file.ContentLength > 0)
		{
			string[] fileparts = file.FileName.Split('.');
			extension = "." + fileparts[fileparts.Length - 1];
			postData = new Byte[file.ContentLength];
			file.InputStream.Read(postData, 0, file.ContentLength);
			newFileName = AssetIdRangeStartId.Text + extension;
			if (CurrentFileName.Text != newFileName)
			{
				removeOldFileOnAmazon = true;
			}
		}
		else
		{
			newFileName = CurrentFileName.Text;
		}



		WebServiceRequest updateAsset = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Inventory", "UpdateAsset");
		updateAsset.AddParam("assetType", AssetTypeDropDown.SelectedValue);
		updateAsset.AddParam("assetSubType", AssetSubTypeDropDown.SelectedValue);
		updateAsset.AddParam("assetId", AssetIdRangeStartId.Text);
		updateAsset.AddParam("assetName", AssetName.Text);
		updateAsset.AddParam("assetData", AssetData.Text);
		updateAsset.AddParam("newFileName", newFileName);
		XmlDocument updateResponse = updateAsset.GetWebResponse();

		XmlDocument removeResponse = null;
		if (removeOldFileOnAmazon)
		{
			WebServiceRequest removeFileOnAmazon = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "FileStore", "DeleteFileStoredAtAmazon");
			removeFileOnAmazon.AddParam("filename", CurrentFileName.Text);
			removeResponse = removeFileOnAmazon.GetWebResponse();
		}
		XmlDocument replaceResponse = null;
		if (postData != null)
		{
			WebServiceRequest replaceFileOnAmazon = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "FileStore", "AddFileToAmazon");
			replaceFileOnAmazon.AddParam("fileName", newFileName);
			replaceFileOnAmazon.AddParam("fileToUpload", postData);
			replaceResponse = replaceFileOnAmazon.GetWebResponse();
		}
		
		

		StringBuilder stringBuilder = new StringBuilder();
		// We will use stringWriter to push the formated xml into our StringBuilder
		using (StringWriter stringWriter = new StringWriter(stringBuilder))
		{
			// We will use the Formatting of our xmlTextWriter to provide our indentation.
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				updateResponse.WriteTo(xmlTextWriter);
				if (removeResponse != null)
				{
					removeResponse.WriteTo(xmlTextWriter);
				}
				if (replaceResponse != null)
				{
					replaceResponse.WriteTo(xmlTextWriter);
				}
			}
		}

		ServerResponse.Text = stringBuilder.ToString();

		PopulateForm(AssetIdRangeStartId.Text);
	}
}
