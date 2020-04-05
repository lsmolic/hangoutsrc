using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public class Dna
	{
		private Dictionary<AssetSubType, AssetInfo> mDna = new Dictionary<AssetSubType, AssetInfo>();
		public Dictionary<AssetSubType, AssetInfo> DnaDictionary
		{
			get { return mDna; }
		}
		
        public Dna()
        {
        }

        /// <summary>
        /// Copy constructor
        /// Makes a copy of the dna dictionary.  This isn't a deep copy.
        /// This values of the copied dictionary point to the same AssetInfo's as the copied dna, but this is
        /// ok, as the AssetInfo's will only be read, and not modified.
        /// </summary>
        /// <param name="dna"></param>
        public Dna(Dna dna)
        {
            foreach (AssetSubType key in dna.DnaDictionary.Keys)
            {
                mDna[key] = dna.DnaDictionary[key];
            }
        }

		/// <summary>
        ///  Used to overwrite and add infos to the Dna.
		/// </summary>
		/// <param name="assetInfos"></param>
		public void UpdateDna(IEnumerable<AssetInfo> assetInfos)
		{
			foreach (AssetInfo dnaAssetInfo in assetInfos)
			{
				ApplyInfoToDna(dnaAssetInfo);
			}
		}

        /// <summary>
        ///  Used to overwrite and add infos to the Dna.
        /// </summary>
        /// <param name="dna"></param>
        public void UpdateDna(Dna dna)
        {
            foreach (AssetSubType key in dna.DnaDictionary.Keys)
            {
                mDna[key] = dna.DnaDictionary[key];
            }
        }

		
		// To be used on construction when asset Data is being set of the first time.
		public void SetDna(IEnumerable<AssetInfo> dna)
		{
			mDna.Clear();
			UpdateDna(dna);
		}
		
		public void ApplyInfoToDna(AssetInfo assetInfo)
		{
			if (mDna.ContainsKey(assetInfo.AssetSubType))
			{
				mDna[assetInfo.AssetSubType] = assetInfo;
			}
			else
			{
				mDna.Add(assetInfo.AssetSubType, assetInfo);
			}
		}

		public void RemoveFromDna(IEnumerable<AssetInfo> assetInfos)
		{
			foreach (AssetInfo assetInfo in assetInfos)
			{
				mDna.Remove(assetInfo.AssetSubType);
			}
		}

		public void RemoveFromDna(AssetInfo assetInfo)
		{
			mDna.Remove(assetInfo.AssetSubType);
		}

		public List<ItemId> GetItemIdList()
        {
            List<ItemId> itemIds = new List<ItemId>();
            foreach (KeyValuePair<AssetSubType, AssetInfo> kvp in mDna)
            {
                itemIds.Add(kvp.Value.ItemId);
            }
            return itemIds;
        }

		public XmlDocument GetDnaXml()
		{
            List<ItemId> itemIds = GetItemIdList();
			return AvatarXmlUtil.CreateAvatarDnaXml("", itemIds);
		}
	}
}
