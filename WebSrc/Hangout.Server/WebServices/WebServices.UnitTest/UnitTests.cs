using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Shared;
using Hangout.Server;

namespace Hangout.Server.WebServices
{
	public class UnitTests : ServiceBaseClass
	{
		public SimpleResponse TestBasicTypes(int intA, uint uintB, string stringC)
		{
			StringBuilder responseXml = new StringBuilder();

			responseXml.AppendFormat("<intA>{0}</intA>", intA);
			responseXml.AppendFormat("<uintB>{0}</uintB>", uintB);
			responseXml.AppendFormat("<stringC>{0}</stringC>", stringC);

			return new SimpleResponse("returnValues", responseXml.ToString());
		}
	}
}
