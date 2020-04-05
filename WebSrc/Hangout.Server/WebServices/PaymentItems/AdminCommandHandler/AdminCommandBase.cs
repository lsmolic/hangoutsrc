using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class AdminCommandBase : CommandHandlerBase
    {
        public AdminCommandBase(string loggerName) : base(loggerName) { }
        public AdminCommandBase(MethodBase currentMethod) : base(currentMethod.DeclaringType) { }

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


        /// <summary>
        /// Update the Response node noun and verb.  
        /// Used to convert twoFish noun and verb to Hangout noun and verb.
        /// </summary>
        /// <param name="responseXML">Response XML document</param>
        /// <param name="noun">The command noun</param>
        /// <param name="verb">The command verb</param>
        /// <returns>The updated XML document</returns>
        protected XmlDocument UpdateResponseNounVerb(XmlDocument responseXML, string noun, string verb)
        {
            XmlNode node = responseXML.SelectSingleNode("/Response");
            node.Attributes["noun"].InnerText = noun;
            node.Attributes["verb"].InnerText = verb;

            node.Attributes.RemoveNamedItem("request");

            return responseXML;
        }

    }
}


