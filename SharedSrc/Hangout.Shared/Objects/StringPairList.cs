using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Hangout.Shared
{
    [Serializable]
    public class StringPairList : IEnumerable<StringPair>
    {
        private readonly IDictionary<string, string> parent;
        public StringPairList(IDictionary<string, string> parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            this.parent = parent;
        }
        public void Add(StringPair item)
        {
            parent.Add(item.Key, item.Value);
        }
        public IEnumerator<StringPair> GetEnumerator()
        {
            foreach (KeyValuePair<string, string> pair in parent)
            {
                StringPair stringPair = new StringPair();
                stringPair.Key = pair.Key;
                stringPair.Value = pair.Value;

                yield return stringPair;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    [Serializable]
    public class NameValueCollectionSerializable : IEnumerable
    {
        private NameValueCollection parent = new NameValueCollection();

        public NameValueCollectionSerializable(NameValueCollection parameters)
        {
            if (parameters == null) throw new ArgumentNullException("parameters");
            this.parent = parameters;
        }

        public void Add(StringPair item)
        {
            parent.Add(item.Key, item.Value);
        }
        public IEnumerator<StringPair> GetEnumerator()
        {
            foreach (string key in parent)
            {
                StringPair stringPair = new StringPair();
                stringPair.Key = key;
                stringPair.Value = parent[key];

                yield return stringPair;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
