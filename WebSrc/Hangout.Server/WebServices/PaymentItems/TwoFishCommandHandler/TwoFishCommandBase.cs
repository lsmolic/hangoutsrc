using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace Hangout.Server.WebServices
{
    public class TwoFishCommandBase : CommandHandlerBase
    { 
        public TwoFishCommandBase(string loggerName) : base (loggerName) {}
        public TwoFishCommandBase(MethodBase currentMethod) : base(currentMethod.DeclaringType) { }
        /// <summary>
        /// Get the Command Class Type
        /// </summary>
        /// <param name="commandNoun">The command noun (class)</param>
        /// <returns>return the command noun class</returns>
        public override Type GetCommandClassType(string commandNoun)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return (base.GetCommandClassType(assembly, commandNoun));
        }

        /// <summary>
        /// Retrieves the twoFish Application name from the app config file
        /// </summary>
        /// <returns>return the AppName or blank if the AppName key is not found.</returns>
        protected string GetAppName()
        {
            return (GetConfigurationAppSetting("AppName", ""));
        }

        /// <summary>
        /// Retrieves the twoFish Application id from the app config file.
        /// </summary>
        /// <returns>return the AppId or blank if the AppId key is not found.</returns>
        protected string GetAppId()
        {
            return (GetConfigurationAppSetting("AppId", ""));
        }
    }
}
