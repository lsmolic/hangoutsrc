/*
 * ConfigManagerStateServer - State server interface to the ConfigManager.
 * 
 * Responsible for:
 * - Locating and reading config and group files
 * - Parsing config and group JSON data to produce a rule set (see ConfigManagerBase)
 * - Retrieving global or per-user config values from the rule set
 * - Casting config values to the type specified by the caller
 * 
 * See also:
 * - ConfigManagerBase
 * - ConfigManagerClient
 * - ConfigManagerASP
 * 
 */

using System.Collections.Generic;

namespace Hangout.Server
{
    public class ConfigManagerStateServer : Hangout.Shared.ConfigManagerBase
    {
        public ConfigManagerStateServer()
        {
            // Read from disk search paths both on desktop and in production
            // Later on this will switch to an HTTP query in production
            if (!InitFromLocalDisk())
            {
                string user = System.Environment.GetEnvironmentVariable("USER");
                if (user != "deploy" && user != "build")
                {
                    throw new System.Exception("Couldn't find a config.json in your search paths.  You need this file to run in development.  Ask Ian for help!  Your user was " + System.Environment.GetEnvironmentVariable("USER"));
                }
            }
        }


        // ---- Begin config property getters ----

        public object GetObjectGlobal(string cfgKey, object defaultValue)
        {
            return GetObject(cfgKey, null, null, defaultValue);
        }

        public T GetObjectGlobal<T>(string cfgKey, T defaultValue)
        {
            return GetObject<T>(cfgKey, null, null, defaultValue);
        }

        public List<object> GetListGlobal(string cfgKey, List<object> defaultValue)
        {
            return GetList(cfgKey, null, null, defaultValue);
        }

        public List<T> GetListGlobal<T>(string cfgKey, List<T> defaultValue)
        {
            return GetList<T>(cfgKey, null, null, defaultValue);
        }

        public string GetStringGlobal(string cfgKey, string defaultValue)
        {
            return GetString(cfgKey, null, null, defaultValue);
        }

        public bool GetBoolGlobal(string cfgKey, bool defaultValue)
        {
            return GetBool(cfgKey, null, null, defaultValue);
        }

        public int GetIntGlobal(string cfgKey, int defaultValue)
        {
            return GetInt(cfgKey, null, null, defaultValue);
        }

        public long GetLongGlobal(string cfgKey, long defaultValue)
        {
            return GetLong(cfgKey, null, null, defaultValue);
        }

        public float GetFloatGlobal(string cfgKey, float defaultValue)
        {
            return GetFloat(cfgKey, null, null, defaultValue);
        }

        public double GetDoubleGlobal(string cfgKey, double defaultValue)
        {
            return GetDouble(cfgKey, null, null, defaultValue);
        }



        public object GetObjectForUser(string cfgKey, long fbId, object defaultValue)
        {
            return GetObject(cfgKey, fbId, null, defaultValue);
        }

        public T GetObjectForUser<T>(string cfgKey, long fbId, T defaultValue)
        {
            return GetObject<T>(cfgKey, fbId, null, defaultValue);
        }

        public List<object> GetListForUser(string cfgKey, long fbId, List<object> defaultValue)
        {
            return GetList(cfgKey, fbId, null, defaultValue);
        }

        public List<T> GetListForUser<T>(string cfgKey, long fbId, List<T> defaultValue)
        {
            return GetList<T>(cfgKey, fbId, null, defaultValue);
        }

        public string GetStringForUser(string cfgKey, long fbId, string defaultValue)
        {
            return GetString(cfgKey, fbId, null, defaultValue);
        }

        public bool GetBoolForUser(string cfgKey, long fbId, bool defaultValue)
        {
            return GetBool(cfgKey, fbId, null, defaultValue);
        }

        public int GetIntForUser(string cfgKey, long fbId, int defaultValue)
        {
            return GetInt(cfgKey, fbId, null, defaultValue);
        }

        public long GetLongForUser(string cfgKey, long fbId, long defaultValue)
        {
            return GetLong(cfgKey, fbId, null, defaultValue);
        }

        public float GetFloatForUser(string cfgKey, long fbId, float defaultValue)
        {
            return GetFloat(cfgKey, fbId, null, defaultValue);
        }

        public double GetDoubleForUser(string cfgKey, long fbId, double defaultValue)
        {
            return GetDouble(cfgKey, fbId, null, defaultValue);
        }
    }
}
