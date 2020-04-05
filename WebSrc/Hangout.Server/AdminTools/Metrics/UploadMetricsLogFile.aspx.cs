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

public partial class Metrics_UploadMetricLogFile : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}

	protected void SubmitButton_onClick(object sender, EventArgs e)
	{
		WebServiceRequest uploadLogFile = new WebServiceRequest(AdminWebConfig.WebServicesBaseUrl, "Logging", "LogBlobMetrics");
		uploadLogFile.AddParam("logFile", LogFile.FileBytes);
		uploadLogFile.GetWebResponse();
	}
}
