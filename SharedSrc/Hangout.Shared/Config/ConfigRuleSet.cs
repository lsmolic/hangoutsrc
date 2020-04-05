using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace Hangout.Shared
{
    public class ConfigRuleSet
    {
        private Dictionary<string, ConfigValue> mRules = new Dictionary<string, ConfigValue>();
        private Dictionary<string, List<long>> mGroupToUsers = new Dictionary<string, List<long>>();
        private Dictionary<long, List<string>> mUserToGroups = new Dictionary<long, List<string>>();

        public ConfigRuleSet(string rulesJSON, string groupsJSON)
        {
            mRules = ConfigParser.ParseRules(rulesJSON);
            mGroupToUsers = ConfigParser.ParseGroups(groupsJSON);
            foreach (string group in mGroupToUsers.Keys)
            {
                foreach (long member in mGroupToUsers[group])
                {
                    if (!mUserToGroups.ContainsKey(member))
                    {
                        mUserToGroups.Add(member, new List<string>());
                    }
                    mUserToGroups[member].Add(group);
                }
            }
        }

        public JsonType GetType(string cfgKey, long? fbId, long? nonIdentifyingHash)
        {
            if (!mRules.ContainsKey(cfgKey))
            {
                throw new ArgumentException("No value found for config key" + cfgKey +".  Indeterminate type!");
            }

            List<string> groups = null;

            if (fbId != null && mUserToGroups.ContainsKey((long)fbId))
            {
                groups = mUserToGroups[(long)fbId];
            }

            return mRules[cfgKey].GetType(fbId, nonIdentifyingHash, groups);
        }

        public object GetObject(string cfgKey, long? fbId, long? nonIdentifyingHash, object defaultValue)
        {
            if (!mRules.ContainsKey(cfgKey))
            {
                return defaultValue;
            }

            List<string> groups = null;

            if (fbId != null && mUserToGroups.ContainsKey((long)fbId))
            {
                groups = mUserToGroups[(long)fbId];
            }

            object result = mRules[cfgKey].GetObject(fbId, nonIdentifyingHash, groups);

            if (result != null)
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public T GetObject<T>(string cfgKey, long? fbId, long? nonIdentifyingHash, T defaultValue)
        {
            if (!mRules.ContainsKey(cfgKey))
            {
                return defaultValue;
            }

            List<string> groups = null;

            if (fbId != null && mUserToGroups.ContainsKey((long)fbId))
            {
                groups = mUserToGroups[(long)fbId];
            }

            object result = mRules[cfgKey].GetObject(fbId, nonIdentifyingHash, groups);

            if (result != null)
            {
                return (T) result;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
