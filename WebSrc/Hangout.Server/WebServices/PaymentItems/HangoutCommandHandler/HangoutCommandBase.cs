using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Configuration;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class HangoutCommandBase : CommandCommonBase
    { 
        public HangoutCommandBase(string loggerName) : base (loggerName) {}
        public HangoutCommandBase(MethodBase currentMethod) : base(currentMethod.DeclaringType) { }

        public override Type GetCommandClassType(string commandNoun)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return (base.GetCommandClassType(assembly, commandNoun));
        }

        /// <summary>
        /// Call the Twofish Service command and return the XMLDocument
        /// </summary>
        /// <param name="command">PaymentCommand to execute</param>
        /// <param name="currentMethod">Current Method</param>
        /// <returns>Twofish XML Response document</returns>
        protected XmlDocument CallTwoFishService(PaymentCommand command, MethodBase currentMethod)
        {
            XmlDocument response = null;

            try
            {
                TwoFishCommandBase parser = new TwoFishCommandBase(currentMethod);
                response = parser.ProcessRequest(command);
                response = UpdateResponseNounVerb(response, currentMethod.DeclaringType.Name, currentMethod.Name);
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("CallTwoFishService", ex);
            }

            return response;
        }


    }
}