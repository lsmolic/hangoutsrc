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
using System.IO;
using System.Text;

public partial class PaymentItems_GenerateItemsListCSV : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}
	protected void GenerateFileButton_Click(object sender, EventArgs e)
	{

	
		string csv = "";

		WebServiceRequest getCSVList = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Inventory", "GeneratePaymentItemsCSV");
		XmlDocument csvXml = getCSVList.GetWebResponse();

		if(csvXml != null)
		{
			XmlNode csvNode = csvXml.SelectSingleNode("CSV");
			if(csvNode != null)
			{
				csv = csvNode.InnerText;
				System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
				byte[] csvByteArray = encoding.GetBytes(csv);


				Response.Clear();
				Response.AddHeader("Content-Disposition", "attachment; filename=CSVFile.csv");
				Response.AddHeader("Content-Length", csvByteArray.Length.ToString());
				Response.ContentType = "application/octet-stream";
				Response.BinaryWrite(csvByteArray);

			}
			else
			{
				Response.Write("CSV node is Null... FAIL!");
			}
		}
		
	}
}
