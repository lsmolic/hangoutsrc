using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;

namespace Hangout.Server.WebServices
{
	public class ServiceConnectUtil
	{
		public bool RegisterWellKnownServiceType(Type typeToRegister, string typeName, WellKnownObjectMode wellKnownObjectMode)
		{
			bool registered = false;
			try
			{
				RemotingConfiguration.RegisterWellKnownServiceType(typeToRegister, typeName, wellKnownObjectMode);
				//Console.WriteLine("PaymentItemConnect RegisterWellKnownServiceType PaymentItemHandler");
				registered = true;
			}

			catch (Exception ex)
			{
				//Console.WriteLine("PaymentItemConnect RegisterWellKnownServiceType Error:{1}", ex.Message);
			}

			return registered;
		}
	}
}
