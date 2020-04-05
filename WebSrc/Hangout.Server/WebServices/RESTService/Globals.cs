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

namespace Hangout.Server.WebServices
{
	public static class Globals
	{
		private static int mNumberOfCurrentRestRequests = 0;
		private static int mNumberOfCurrentRemotingRequests = 0;
		private static long mTotalRequests = 0;
		private static long mTotalRestRequests = 0;
		private static long mTotalRemotingRequests = 0;


		public static int GetCurrentRestRequests
		{
			get { return mNumberOfCurrentRestRequests; }
		}
		public static long GetTotalRestRequests
		{
			get { return mTotalRestRequests; }
		}
		public static void AddCurrentRestRequest()
		{
			mNumberOfCurrentRestRequests++;
			try
			{
				mTotalRequests++;
				mTotalRestRequests++;
			}
			catch { }
		}
		public static void RemoveCurrentRestRequest()
		{
			mNumberOfCurrentRestRequests--;
		}


		public static int GetCurrentRemotingRequests
		{
			get { return mNumberOfCurrentRemotingRequests; }
		}
		public static long GetTotalRemotingRequests
		{
			get { return mTotalRemotingRequests; }
		}
		public static void AddCurrentRemotingRequest()
		{
			mNumberOfCurrentRemotingRequests++;
			try
			{
				mTotalRequests++;
				mTotalRemotingRequests++;
			}
			catch { }
		}
		public static void RemoveCurrentRemotingRequest()
		{
			mNumberOfCurrentRemotingRequests--;
		}
	}
}
