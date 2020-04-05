using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class AssetId : ObjectId
    {
		public static string AssetIdNamespace
		{
			get { return "AssetId:"; }
		}
		
        protected override string SerializationString
        {
            get { return "AssetId"; }
        }

        public AssetId() : base() { }
        public AssetId(ItemId copyObject)
            : base(copyObject)
        {
        }

        public AssetId(uint itemIdValue)
            : base(itemIdValue)
        {
        }

        public AssetId(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        public string UniqueKey
        {
			get { return AssetIdNamespace + Value.ToString(); }
        }
    }
}
