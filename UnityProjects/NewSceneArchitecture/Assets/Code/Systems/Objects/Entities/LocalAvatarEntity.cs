/**  --------------------------------------------------------  *
 *   LocalAvatarEntity.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class LocalAvatarEntity : AvatarEntity
	{		
		private Dna mAvatarDna;
		protected Dna AvatarDna
		{
			get { return mAvatarDna; }
		}
		private Dna mChangesToDna;
		protected Dna ChangesToDna
		{
			get { return mChangesToDna; }
		}

		protected Dna mDefaultDna = new Dna();
		
		private AssetInfo mEmptySkinnedMeshAssetInfo;
		
		public LocalAvatarEntity(GameObject avatarBodyGameObject, GameObject headGameObject)
			: base(avatarBodyGameObject, headGameObject)
		{
			mAvatarDna = new Dna();
			mChangesToDna = new Dna();

			List<AssetInfo> assetInfos = new List<AssetInfo>();
			XmlDocument defaultsDoc = XmlUtility.LoadXmlDocument("resources://Avatar/DefaultAssetList");
			foreach (XmlNode defaultAssetNode in defaultsDoc.SelectNodes("DefaultAssets/Asset"))
			{
				string stringSubType = XmlUtility.GetStringAttribute(defaultAssetNode, "AssetSubType");
				if (stringSubType == null)
				{
					throw new XmlException("Could not retrieve asset sub type string from node: " + defaultAssetNode);
				}

				AssetInfo assetInfo = new ClientAssetInfo(defaultAssetNode);
				assetInfos.Add(assetInfo);
			}
			mDefaultDna.SetDna(assetInfos);
		}

		public void RemoveAssets(IEnumerable<AssetInfo> assetInfos)
		{
			List<AssetInfo> defaultAssetsToApply = new List<AssetInfo>();
			foreach (AssetInfo assetInfo in assetInfos)
			{
				AssetInfo defaultAssetInfo;
				if (mDefaultDna.DnaDictionary.TryGetValue(assetInfo.AssetSubType, out defaultAssetInfo))
				{
					defaultAssetsToApply.Add(defaultAssetInfo);
				}
				else
				{
					Console.LogError("The previous avatar DNA and the DefaultsDictionary has no assetInfo for AssetSubType: " + assetInfo.AssetSubType);
				}
			}
			UpdateAssets(defaultAssetsToApply);
		}

		/// <summary>
		/// Updates the DNA and applies the assets.
		/// </summary>
		/// <param name="assetInfos"></param>
		public override void UpdateAssets(IEnumerable<AssetInfo> assetInfos)
		{
			mAvatarDna.UpdateDna(assetInfos);
			mChangesToDna.RemoveFromDna(assetInfos);
			mClientAssetRepository.GetAssets<Asset>(assetInfos, delegate(IEnumerable<Asset> assets)
			{
				this.ApplyAssets(assetInfos, assets);
			});
		}

		/// <summary>
		/// Updates the DNA and applies the assets and calls back when assets are finished being set.
		/// </summary>
		/// <param name="assetInfos"></param>
		/// <param name="callback"></param>
		public void UpdateAssetsWithCallback(IEnumerable<AssetInfo> assetInfos, Action<AvatarEntity> callback)
		{
			mAvatarDna.UpdateDna(assetInfos);
			mChangesToDna.RemoveFromDna(assetInfos);
			this.SetAssetsWithCallback(assetInfos, callback);
		}

		/// <summary>
		/// Trying on assets.
		/// </summary>
		/// <param name="assetInfos"></param>
		public void ApplyTempAssetInfos(IEnumerable<AssetInfo> assetInfos)
		{
			mClientAssetRepository.GetAssets<Asset>(assetInfos, delegate(IEnumerable<Asset> assets)
			{
				ApplyTempAssets(assetInfos, assets);
			});
		}
		
		/// <summary>
		/// Removes a temp clothing item which was tried on in the store.
		/// </summary>
		public void RemoveTempAssetInfos(IEnumerable<AssetInfo> assetInfos)
		{
			List<AssetInfo> assetInfosToRevert = new List<AssetInfo>();
			foreach (AssetInfo info in assetInfos)
			{
				// Find if the asset info is actually in the temp data.
				AssetInfo assetInfo;
				if (mChangesToDna.DnaDictionary.TryGetValue(info.AssetSubType, out assetInfo))
				{
					// Find the previous asset being used.
					AssetInfo previousAssetInfo;
					if (mAvatarDna.DnaDictionary.TryGetValue(info.AssetSubType, out previousAssetInfo))
					{
						assetInfosToRevert.Add(previousAssetInfo);
					}
					// If there was no asset previously use the default.
					else if (mDefaultDna.DnaDictionary.TryGetValue(info.AssetSubType, out previousAssetInfo))
					{
						assetInfosToRevert.Add(previousAssetInfo);
					}
					mChangesToDna.RemoveFromDna(assetInfo);
				}
				else
				{
					Debug.LogError("Trying to remove a temp asset which was never set in mChangesDna.");
				}
			}
			if (assetInfosToRevert.Count > 0)
			{
				mClientAssetRepository.GetAssets<Asset>(assetInfosToRevert, delegate(IEnumerable<Asset> assets)
				{
					this.SetAssetsOverFrames(assets);
				});
			}
		}

		private void ApplyTempAssets(IEnumerable<AssetInfo> assetInfos, IEnumerable<Asset> assets)
		{
			mChangesToDna.UpdateDna(assetInfos);
			this.SetAssetsOverFrames(assets);
		}

		/// <summary>
		/// Puts clothes on temporarily (updates the tempDna only)
		/// </summary>
		/// <param name="assetsNode"></param>
		public void ApplyTempClothingToAvatar(XmlNode assetsNode)
		{
			List<AssetInfo> assetInfos = new List<AssetInfo>();
			foreach (XmlNode assetNode in assetsNode.SelectNodes("Asset"))
			{
				AssetInfo assetInfo = new ClientAssetInfo(assetNode);
				assetInfos.Add(assetInfo);
			}
			ApplyTempAssetInfos(assetInfos);
		}
		
		
        /// <summary>
        /// Puts clothes on the avatar permanently and updates the dna.
        /// </summary>
        /// <param name="assetsNode"></param>
        public void ApplyAssetsToAvatar(XmlNode assetsNode)
        {
            List<AssetInfo> assetInfos = new List<AssetInfo>();
            foreach (XmlNode assetNode in assetsNode.SelectNodes("Asset"))
            {
                AssetInfo assetInfo = new ClientAssetInfo(assetNode);
                assetInfos.Add(assetInfo);
            }
            UpdateAssets(assetInfos);
        }

        public void RemoveClothingFromAvatar(XmlNode assetsNode)
        {
            List<AssetInfo> assetInfos = new List<AssetInfo>();
            foreach (XmlNode assetNode in assetsNode.SelectNodes("Asset"))
            {
                AssetInfo assetInfo = new ClientAssetInfo(assetNode);
                assetInfos.Add(assetInfo);
            }
            RemoveAssets(assetInfos);
        }

		// Use on construction of DNA.  Blows away old changes.
		public void SetDnaAndBuildAvatar(IEnumerable<AssetInfo> assetInfos, Action<AvatarEntity> avatarAssetsSet)
		{
			//mLoadAvatarCallback = avatarAssetsLoaded;
			mAvatarDna = new Dna();
			mChangesToDna = new Dna();
			mAvatarDna.UpdateDna(assetInfos);
			this.SetAssetsWithCallback(assetInfos, avatarAssetsSet);
		}

		public XmlDocument GetDnaXml()
		{
			return mAvatarDna.GetDnaXml();
		}

		/// <summary>
		/// Used when you are saving the temp changes to the DNA.
		/// </summary>
		public void SaveChangesToDna()
		{
			mAvatarDna.UpdateDna(mChangesToDna.DnaDictionary.Values);
			mChangesToDna = new Dna();
		}

		public void RevertDnaChanges()
		{
			List<AssetInfo> assetInfosToRevert = new List<AssetInfo>();
			// TODO: Ensure that we have a defaultDna which has a 1 to 1 mapping of assetSubTypes to assets.
			// This list here is added to when we cannot find an asset to revert to in the avatarDna, or default
			// dna.  This ensures that changes syncs up with the actual DNA.
			List<AssetInfo> assetsNotFoundToRemoveFromChanges = new List<AssetInfo>();
			foreach (KeyValuePair<AssetSubType, AssetInfo> change in mChangesToDna.DnaDictionary)
			{
				if (mAvatarDna.DnaDictionary.ContainsKey(change.Key))
				{
					assetInfosToRevert.Add(mAvatarDna.DnaDictionary[change.Key]);
				}
				else if (mDefaultDna.DnaDictionary.ContainsKey(change.Key))
				{
					assetInfosToRevert.Add(mDefaultDna.DnaDictionary[change.Key]);
				}
				else
				{
					assetsNotFoundToRemoveFromChanges.Add(change.Value);
				}
			}
			mChangesToDna.RemoveFromDna(assetsNotFoundToRemoveFromChanges);
			UpdateAssets(assetInfosToRevert);
		}
	}
}
