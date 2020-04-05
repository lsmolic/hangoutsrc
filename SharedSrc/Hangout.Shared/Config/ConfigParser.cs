using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;

namespace Hangout.Shared
{
    internal class ConfigParser
    {
        private static bool CheckSimpleString(string str)
        {
            return Regex.IsMatch(str, "^[a-zA-Z0-9_]+$");
        }

        private static bool CheckSelector(string str)
        {
            return Regex.IsMatch(str, "^DEFAULT$|^MOD[0-9]+==[0-9]+$|^FBID:[0-9]+$|^GROUP:[a-zA-Z0-9_]+$");
        }

        public static Dictionary<string, List<long>> ParseGroups(string json)
        {
            Dictionary<string, List<long>> result = new Dictionary<string, List<long>>();
            Dictionary<string, List<JsonData>> groups = JsonMapper.ToObject<Dictionary<string, List<JsonData>>>(json);

            foreach (string groupName in groups.Keys)
            {
                if (!CheckSimpleString(groupName))
                {
                    throw new ArgumentException("Error parsing groups.  Every group name must be a simple string.");
                }

                result.Add(groupName, new List<long>());

                foreach (JsonData memberId in groups[groupName])
                {
                    if (memberId.IsLong)
                    {
                        result[groupName].Add((long)memberId);
                    }
                    else if (memberId.IsInt)
                    {
                        result[groupName].Add((long)(int)memberId);
                    }
                    else
                    {
                        throw new ArgumentException("Error parsing groups.  Every member must be an int or long!");
                    }
                }
            }

            return result;
        }

        private static void ConsumeToken(JsonReader reader, JsonToken token)
        {
            reader.Read();
            CheckToken(reader, token);

        }

        private static void CheckToken(JsonReader reader, JsonToken token)
        {
            if (reader.Token != token)
            {
                throw new Exception("Parsing error, did not receive expected token " + token + ".  Instead received " + reader.Token);
            }
        }

/*
         Token      Value             Type
------------------------------------------
  PropertyName ShirtColor    System.String
        String       blue    System.String
  PropertyName PhoneNumber    System.String
           Int      12345     System.Int32
  PropertyName PantsColor    System.String
   ObjectStart
  PropertyName    DEFAULT    System.String
        String       blue    System.String
  PropertyName FBID:98765    System.String
        String        red    System.String
  PropertyName GROUP:admin    System.String
        String      green    System.String
  PropertyName    MOD5==2    System.String
        String      black    System.String
     ObjectEnd
     ObjectEnd
*/


        public static Dictionary<string, ConfigValue> ParseRules(string json)
        {
            Dictionary<string, ConfigValue> ruleSet = new Dictionary<string, ConfigValue>();

            JsonReader reader = new JsonReader(json);

            ConsumeToken(reader, JsonToken.ObjectStart);

            reader.Read();

            while (reader.Token != JsonToken.ObjectEnd)
            {
                // Store config key
                CheckToken(reader, JsonToken.PropertyName);
                string cfgKey = (string)reader.Value;
                CheckSimpleString(cfgKey);
                reader.Read();

                // Store config value
                ConfigValue cfgVal = new ConfigValue();
                if (reader.Token == JsonToken.ObjectStart)
                {
                    reader.Read();
                    while (reader.Token != JsonToken.ObjectEnd)
                    {
                        CheckToken(reader, JsonToken.PropertyName);
                        string selector = (string)reader.Value;
                        reader.Read();
                        cfgVal.ParseCase(selector, reader);
                    }
                }
                else
                {
                    cfgVal.ParseConst(reader);
                }

                ruleSet.Add(cfgKey, cfgVal);
            }

            return ruleSet;
        }
    }
}
