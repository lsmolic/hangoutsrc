/*
 * ConfigManagerBase - Base class for client, state server, and ASP config managers. 
 * 
 * Responsible for:
 * - Locating and reading config and group files from disk when running on a dev desktop
 * - Parsing config and group JSON data to produce a rule set (see ConfigRuleSet)
 * - Retrieving global or per-user config values from the rule set
 * - Casting config values to the type specified by the caller
 * 
 * See also:
 * - ConfigManagerStateServer
 * - ConfigManagerClient
 * - ConfigManagerASP
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace Hangout.Shared
{
    public class ConfigManagerBase
    {
        protected ConfigRuleSet mRuleSet;

        protected ConfigManagerBase()
        { }

        protected virtual void InitFromJSON(string rulesJSON, string groupsJSON)
        {
            mRuleSet = new ConfigRuleSet(rulesJSON, groupsJSON);
        }

        protected List<string> GetSearchPaths()
        {
            List<string> searchDirs = new List<string>();
            if (System.Environment.GetEnvironmentVariable("CONFIG_JSON_PATH") != null)
            {
                searchDirs.Add(System.Environment.GetEnvironmentVariable("CONFIG_JSON_PATH"));
            }
            if (System.Environment.GetEnvironmentVariable("HOME") != null)
            {
                searchDirs.Add(System.Environment.GetEnvironmentVariable("HOME"));
            }
            if (System.Environment.GetEnvironmentVariable("HOMEPATH") != null)
            {
                searchDirs.Add(System.Environment.GetEnvironmentVariable("HOMEPATH"));
            }
            searchDirs.Add(@"C:");
            return searchDirs;
        }

        // Call this when running on a dev desktop.
        // Checks desktop search paths and sets up personal dev settings.
        protected bool InitFromLocalDisk()
        {
            List<string> searchDirs = GetSearchPaths();

            string rulesJSON = "{}";
            string groupsJSON = "{}";
            bool fileFound = false;

            foreach (string path in searchDirs)
            {
                if (File.Exists(path + "/config.json"))
                {
                    StreamReader reader = new StreamReader(path + "/config.json");
                    rulesJSON = reader.ReadToEnd();
                    reader.Close();

                    if (File.Exists(path + "/config_groups.json"))
                    {
                        reader = new StreamReader(path + "/config_groups.json");
                        groupsJSON = reader.ReadToEnd();
                        reader.Close();
                    }
                    fileFound = true;
                    break;
                }
            }

            InitFromJSON(rulesJSON, groupsJSON);
            return fileFound;
        }


        // ---- Begin config property getters ----


        // Retrieve a config value of unknown type
        protected object GetObject(string cfgKey, long? fbId, long? nonIdentifyingHash, object defaultValue)
        {
            return mRuleSet.GetObject(cfgKey, fbId, nonIdentifyingHash, defaultValue);
        }

        // Retrieve a config value and cast to the type specified by the caller
        protected T GetObject<T>(string cfgKey, long? fbId, long? nonIdentifyingHash, T defaultValue)
        {
            return mRuleSet.GetObject<T>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
        }

        // Retrieve a config value as a list with elements of unknown type
        protected List<object> GetList(string cfgKey, long? fbId, long? nonIdentifyingHash, List<object> defaultValue)
        {
            return GetObject<List<object>>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
        }

        // Retrieve a config value as a list with elements of the type specified by the caller
        protected List<T> GetList<T>(string cfgKey, long? fbId, long? nonIdentifyingHash, List<T> defaultValue)
        {
            List<object> defaultList = null;
            if (defaultValue != null)
            {
                defaultList = new List<object>();
                foreach (object obj in defaultValue)
                {
                    defaultList.Add(obj);
                }
            }

            List<object> vals = GetObject<List<object>>(cfgKey, fbId, nonIdentifyingHash, defaultList);
            List<T> result = new List<T>();

            foreach (object obj in vals)
            {
                result.Add((T)obj);
            }

            return result;
        }

        // Retrieve a config value as a string
        protected string GetString(string cfgKey, long? fbId, long? nonIdentifyingHash, string defaultValue)
        {
            return GetObject<string>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
        }

        // Retrieve a config value as a boolean
        protected bool GetBool(string cfgKey, long? fbId, long? nonIdentifyingHash, bool defaultValue)
        {
            return GetObject<bool>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
        }

        // Retrieve a config value as an int32
        protected int GetInt(string cfgKey, long? fbId, long? nonIdentifyingHash, int defaultValue)
        {
            return GetObject<int>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
        }

        // Retrieve a config value as an int64
        protected long GetLong(string cfgKey, long? fbId, long? nonIdentifyingHash, long defaultValue)
        {
            try
            {
                return GetObject<long>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
            }
            catch (InvalidCastException ex)
            {
                return (long)GetObject<int>(cfgKey, fbId, nonIdentifyingHash, (int)defaultValue);
            }
        }

        // Retrieve a config value as a float
        protected float GetFloat(string cfgKey, long? fbId, long? nonIdentifyingHash, float defaultValue)
        {
            try
            {
                return GetObject<float>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
            }
            catch (InvalidCastException ex)
            {
                try
                {
                    return (float)GetObject<double>(cfgKey, fbId, nonIdentifyingHash, (double)defaultValue);
                }
                catch (InvalidCastException ex2)
                {
                    try
                    {
                        return (float)GetObject<int>(cfgKey, fbId, nonIdentifyingHash, (int)defaultValue);
                    }
                    catch (InvalidCastException ex3)
                    {
                        return (float)GetObject<long>(cfgKey, fbId, nonIdentifyingHash, (long)defaultValue);
                    }
                }
            }
        }

        // Retrieve a config value as a double
        protected double GetDouble(string cfgKey, long? fbId, long? nonIdentifyingHash, double defaultValue)
        {
            try
            {
                return GetObject<double>(cfgKey, fbId, nonIdentifyingHash, defaultValue);
            }
            catch (InvalidCastException ex)
            {
                try
                {
                    return (double)GetObject<float>(cfgKey, fbId, nonIdentifyingHash, (float)defaultValue);
                }
                catch (InvalidCastException ex2)
                {
                    try
                    {
                        return (double)GetObject<int>(cfgKey, fbId, nonIdentifyingHash, (int)defaultValue);
                    }
                    catch (InvalidCastException ex3)
                    {
                        return (double)GetObject<long>(cfgKey, fbId, nonIdentifyingHash, (long)defaultValue);
                    }
                }
            }
        }
    }
}
