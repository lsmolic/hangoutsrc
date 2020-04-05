using System;
using System.Collections.Generic;
using System.Text;
using Facebook;
using Microsoft.Xml.Schema.Linq;


namespace Hangout.Server
{
	public class FacebookUser : Facebook.Schema.user
	{
		private string onlinePresence = "offline";
		/// <summary>
		/// returns "active","idle","offline" or "error"
		/// </summary>
		///
		public string online_presence
		{
			get { return onlinePresence; }
			set { onlinePresence = value; }
		}
	}
}
