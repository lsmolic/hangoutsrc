using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for WebConfig
/// </summary>
public class WebConfig
{
	public static readonly string AmazonUrlRoot = ConfigurationManager.AppSettings["AmazonBaseAddress"] + ConfigurationManager.AppSettings["AmazonS3BucketName"];
	public static readonly string WebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
	public static readonly string AssetBaseUrl = ConfigurationSettings.AppSettings["AssetBaseUrl"];
	public static readonly string StateServerIp = ConfigurationSettings.AppSettings["StateServerIp"];
	public static readonly string FacebookAPIKey = ConfigurationSettings.AppSettings["APIKey"];
	public static readonly string FacebookSecret = ConfigurationManager.AppSettings["Secret"];
	public static readonly string MetricsAPIKey = ConfigurationSettings.AppSettings["MetricsAPIKey"];
	public static readonly string WebHostBaseURL = ConfigurationSettings.AppSettings["WebHostBaseURL"];
	public static readonly string FacebookBaseURL = ConfigurationSettings.AppSettings["FacebookBaseURL"];
	public static readonly string Unity3DFileLoc = ConfigurationSettings.AppSettings["Unity3DFileLoc"];
	public static readonly string JavascriptErrorMode = ConfigurationSettings.AppSettings["JavascriptErrorMode"];
	public static readonly string InstanceName = ConfigurationSettings.AppSettings["InstanceName"];
}
