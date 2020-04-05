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
using System.IO;
using System.Xml;
using System.Text;

public partial class Manage_UploadNewAsset : AdminPageBaseClass
{
	protected override void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			AssetTypeDropDown.DataSource = Enum.GetNames(typeof(AssetType));
			AssetTypeDropDown.DataBind();

			AssetSubTypeDropDown.DataSource = Enum.GetNames(typeof(AssetSubType));
			AssetSubTypeDropDown.DataBind();
		}
	}

	public void SubmitAssetButton_Clicked(object sender, EventArgs e)
	{ 
		string newFileName = AddNewAssetToDatabase();

		StringBuilder stringBuilder = new StringBuilder();
		HttpPostedFile file = AssetFileData.PostedFile;
		if (file != null && file.ContentLength > 0)
		{
			byte[] postData = null;
			postData = new Byte[file.ContentLength];
			file.InputStream.Read(postData, 0, file.ContentLength);
	
			WebServiceRequest uploadNewAssetToAmazon = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "FileStore", "AddFileToAmazon");
			uploadNewAssetToAmazon.AddParam("fileName", newFileName);
			uploadNewAssetToAmazon.AddParam("fileToUpload", postData);
			XmlDocument xmlResponse = uploadNewAssetToAmazon.GetWebResponse();
			if (xmlResponse != null)
			{
				
				// We will use stringWriter to push the formated xml into our StringBuilder bob.
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					// We will use the Formatting of our xmlTextWriter to provide our indentation.
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						xmlTextWriter.Formatting = Formatting.Indented;
						xmlResponse.WriteTo(xmlTextWriter);
					}
				}
			}
		}
		
		ServerResponse.Text += "\r\n" + stringBuilder.ToString();
	}

	private string AddNewAssetToDatabase()
	{
		HttpPostedFile file = AssetFileData.PostedFile;
		string extention = null;

		if (file != null && file.ContentLength > 0)
		{
			string[] fileparts = file.FileName.Split('.');
			extention = "." + fileparts[fileparts.Length-1];
		}

		WebServiceRequest createNewAsset = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Inventory", "AddNewAsset");
		createNewAsset.AddParam("assetType", AssetTypeDropDown.SelectedItem.Text);
		createNewAsset.AddParam("assetSubType", AssetSubTypeDropDown.SelectedItem.Text);
		createNewAsset.AddParam("assetName", AssetName.Text);
		createNewAsset.AddParam("assetExtention", extention);
		createNewAsset.AddParam("assetData", AssetData.Text.Replace("\n","").Replace("\r",""));
		
		XmlDocument xmlResponse = createNewAsset.GetWebResponse();

		StringBuilder stringBuilder = new StringBuilder();
		if (xmlResponse != null)
		{
			
			// We will use stringWriter to push the formated xml into our StringBuilder bob.
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				// We will use the Formatting of our xmlTextWriter to provide our indentation.
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlResponse.WriteTo(xmlTextWriter);
				}
			}
		}
		ServerResponse.Text = "";
		ServerResponse.Text += "\r\n" + stringBuilder.ToString();
		XmlNode newFileName = xmlResponse.SelectSingleNode("//NewAssetId");
		string returnValue = "";
		if (newFileName != null)
		{
			returnValue = newFileName.InnerText;
			if (!String.IsNullOrEmpty(extention))
			{
				returnValue += extention;
			}
		}
		else
		{
			// If there's no NewAssetId, something went wrong, just output the whole XML
			returnValue = xmlResponse.InnerXml;
		}
		return returnValue;
	}
}
