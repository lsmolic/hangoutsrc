using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Hangout.Server.WebServices
{
    public static class WebConfig
    {
        public static readonly string InventoryDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutInventoryDatabase"].ConnectionString;
		public static readonly string AvatarsDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutAvatarsDatabase"].ConnectionString;
		public static readonly string RoomsDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutRoomsDatabase"].ConnectionString;
		public static readonly string AccountsDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutAccountsDatabase"].ConnectionString;
		public static readonly string MiniGameDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutMiniGameDatabase"].ConnectionString;
        public static readonly string LoggingDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutLoggingDatabase"].ConnectionString;
		public static readonly string BossServerDBConnectionString = ConfigurationManager.ConnectionStrings["HangoutBossServer"].ConnectionString;
		
		public static readonly string AmazonS3BucketName = ConfigurationManager.AppSettings["AmazonS3BucketName"];
		public static readonly string AmazonBaseAddress = ConfigurationManager.AppSettings["AmazonBaseAddress"];
		public static readonly string AmazonSecretAccessKey = ConfigurationManager.AppSettings["AmazonSecretAccessKey"];
		public static readonly string AmazonAccessKeyId = ConfigurationManager.AppSettings["AmazonAccessKeyId"];

		public static readonly string AssetBaseUrl = ConfigurationManager.AppSettings["AssetBaseUrl"];

		public static readonly string StateServerPopulationCap = ConfigurationManager.AppSettings["StateServerPopulationCap"];
		public static readonly string StateServerDesiredPopulationCap = ConfigurationManager.AppSettings["StateServerDesiredPopulationCap"];
		public static readonly string RemotingListenPort = ConfigurationManager.AppSettings["RemotingListenPort"];

		public static readonly string FacebookAPIKey = ConfigurationManager.AppSettings["APIKey"];
		public static readonly string FacebookSecret = ConfigurationManager.AppSettings["Secret"];
		public static readonly string TransactionLogKey = ConfigurationManager.AppSettings["TransactionLogKey"];
    }
}
