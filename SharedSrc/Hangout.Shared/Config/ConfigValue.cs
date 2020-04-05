using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;

namespace Hangout.Shared
{
    internal class ConfigValue
    {
        private SortedList<ConfigSelector, Pair<JsonType, object>> valueMap = new SortedList<ConfigSelector, Pair<JsonType, object>>();

        public ConfigValue()
        {
        }

        private void ParseValueObject(JsonReader reader, out JsonType type, out object value)
        {
            switch (reader.Token)
            {
                case JsonToken.Boolean:
                    type = JsonType.Boolean;
                    value = reader.Value;
                    break;
                case JsonToken.Double:
                    type = JsonType.Double;
                    value = reader.Value;
                    break;
                case JsonToken.Int:
                    type = JsonType.Int;
                    value = reader.Value;
                    break;
                case JsonToken.Long:
                    type = JsonType.Long;
                    value = reader.Value;
                    break;
                case JsonToken.Null:
                    type = JsonType.None;
                    value = reader.Value;
                    break;
                case JsonToken.String:
                    type = JsonType.String;
                    value = reader.Value;
                    break;
                case JsonToken.ArrayStart:
                    type = JsonType.Array;
                    List<object> result = new List<object>();
                    reader.Read();
                    while (reader.Token != JsonToken.ArrayEnd)
                    {
                        JsonType elemType;
                        object elemValue;
                        ParseValueObject(reader, out elemType, out elemValue);
                        result.Add(elemValue);
                    }
                    value = result;
                    break;
                default:
                    throw new System.ArgumentException("Invalid type " + reader.Token + " when parsing config value.  Boolean/double/int/long/null/string/array supported.");
            }
            reader.Read();
        }

        public void ParseCase(string selectorStr, JsonReader reader)
        {
            ConfigSelector selector = new ConfigSelector(selectorStr);

            JsonType type;
            object val;
            ParseValueObject(reader, out type, out val);

            valueMap.Add(selector, new Pair<JsonType, object>(type, val));
        }

        public void ParseConst(JsonReader reader)
        {
            ParseCase("DEFAULT", reader);
        }

        public object GetObject(long? fbId, long? nonIdentifyingHash, List<string> groups)
        {
            foreach (KeyValuePair<ConfigSelector, Pair<JsonType, object>> kvp in valueMap)
            {
                if (kvp.Key.Matches(fbId, nonIdentifyingHash, groups))
                {
                    return kvp.Value.Second;
                }
            }
            throw new Exception("No matching ConfigSelector found for ConfigValue.  Did you forget to specify a DEFAULT?");
        }

        public JsonType GetType(long? fbId, long? nonIdentifyingHash, List<string> groups)
        {
            foreach (KeyValuePair<ConfigSelector, Pair<JsonType, object>> kvp in valueMap)
            {
                if (kvp.Key.Matches(fbId, nonIdentifyingHash, groups))
                {
                    return kvp.Value.First;
                }
            }
            throw new Exception("No matching ConfigSelector found for ConfigValue.  Did you forget to specify a DEFAULT?");
        }
    }
}
