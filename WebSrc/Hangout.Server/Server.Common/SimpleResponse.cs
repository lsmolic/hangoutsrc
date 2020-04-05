using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Security;
using System.Reflection;

namespace Hangout.Server
{
	public class SimpleResponse : XmlDocument
	{
		public SimpleResponse(string nodeName, string nodeValue)
			: base()
		{
			try
			{
				this.LoadXml("<" + nodeName + ">" + nodeValue + "</" + nodeName + ">");
			}
			catch (System.Xml.XmlException xmlEx)
			{
				throw xmlEx;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		public SimpleResponse(MethodBase currentMethod, string nodeName, string nodeValue)
			: base()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(String.Format("<Response noun='{0}' verb='{1}'>", currentMethod.DeclaringType.Name, currentMethod.Name));
			sb.Append(String.Format("<{0}>{1}</{0}>", nodeName, nodeValue));
			sb.Append("</Response>");
			this.LoadXml(sb.ToString());
		}
	}
}
