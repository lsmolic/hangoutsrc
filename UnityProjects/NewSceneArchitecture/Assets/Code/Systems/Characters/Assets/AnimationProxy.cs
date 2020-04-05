/* 
 * Pherg 11/17/09
 * The spot where animations are stored.
 */

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using UnityEngine;
using Hangout.Shared;
using Hangout.Client.Gui;
using Hangout.Shared.Messages;

namespace Hangout.Client
{
	public class AnimationProxy : Proxy
	{
		private const string PATH_TO_EMOTE_LIST_XML = "assets://Animations/EmoteList.xml";
		private const string PATH_TO_MOOD_LIST_XML = "assets://Animations/MoodList.xml";
		private const string PATH_TO_ICON_LIST_XML = "assets://Animations/IconList.xml";

		private Dictionary<string, ImageAsset> mEmoticonLookUpTable = new Dictionary<string, ImageAsset>();
		private Dictionary<RigAnimationName, RigAnimationAsset> mRigAnimationLookUpTable = new Dictionary<RigAnimationName, RigAnimationAsset>();
		private Dictionary<FaceAnimationName, FaceAnimationAsset> mFaceAnimationLookUpTable = new Dictionary<FaceAnimationName, FaceAnimationAsset>();
		
		// Where emote gui's look for full list of emote and whether they can be played or not.
		private Dictionary<RigAnimationName, bool> mPlayableEmoteLookUpTable = new Dictionary<RigAnimationName,bool>();
		public Dictionary<RigAnimationName, bool> PlayableEmoteLookUpTable
		{
			get { return mPlayableEmoteLookUpTable; }
		}

		// Where emote gui's look for full list of emote and whether they can be played or not.
		private Dictionary<MoodAnimation, bool> mPlayableMoodLookUpTable = new Dictionary<MoodAnimation, bool>();
		public Dictionary<MoodAnimation, bool> PlayableMoodLookUpTable
		{
			get { return mPlayableMoodLookUpTable; }
		}

		private Dictionary<string, bool> mPlayableIconLookUpTable = new Dictionary<string,bool>();
		public Dictionary<string, bool> PlayableIconLookUpTable
		{
			get { return mPlayableIconLookUpTable; }
		}
		
		private Dictionary<RigAnimationName, AssetInfo> mEmoteToAssetInfoLookUpTable = new Dictionary<RigAnimationName, AssetInfo>();
		public Dictionary<RigAnimationName, AssetInfo> EmoteToAssetInfoLookUpTable
		{
			get { return mEmoteToAssetInfoLookUpTable; }
		}

		private Dictionary<MoodAnimation, List<AssetInfo>> mMoodAssetInfoLookUpTable = new Dictionary<MoodAnimation, List<AssetInfo>>();
		
		private Dictionary<string, AssetInfo> mEmoticonToAssetInfoLookUpTable = new Dictionary<string, AssetInfo>();
		
		private ClientAssetRepository mClientAssetRepository;
		
		private bool mEmotesParsed = false;
		private bool mIconsParsed = false;
		private bool mMoodsParsed = false;
		
		private bool mLoaded = false;
		public bool Loaded
		{
			get { return mLoaded;}
		}
		
		private Hangout.Shared.Action mOnFinishedLoading;

        public AnimationProxy(Hangout.Shared.Action onFinishedLoading)
        {
			mOnFinishedLoading = onFinishedLoading;
            mClientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
        }

        public void Init()
        {
			mClientAssetRepository.LoadAssetFromPath<XmlAsset>(PATH_TO_EMOTE_LIST_XML, delegate(XmlAsset emoteListXmlAsset)
			{
				// Parse out all the emotes and set them as false.  Make call to server to get list of owned Emotes.
				foreach (XmlNode emoteNode in emoteListXmlAsset.XmlDocument.SelectNodes("EmoteList/Emote"))
				{
					RigAnimationName emoteName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), emoteNode.InnerText);
					if (!mPlayableEmoteLookUpTable.ContainsKey(emoteName))
					{
						mPlayableEmoteLookUpTable.Add(emoteName, false);
					}
				}
				mEmotesParsed = true;
				CheckIfAllPossibleEmotesIconsMoodsHaveBeenParsed();
			});

			mClientAssetRepository.LoadAssetFromPath<XmlAsset>(PATH_TO_MOOD_LIST_XML, delegate(XmlAsset emoteListXmlAsset)
			{
				// Parse out all the emotes and set them as false.  Make call to server to get list of owned Emotes.
				foreach (XmlNode moodNode in emoteListXmlAsset.XmlDocument.SelectNodes("MoodList/Mood"))
				{
					MoodAnimation moodAnimationName = (MoodAnimation)Enum.Parse(typeof(MoodAnimation), moodNode.InnerText);
					if (!mPlayableMoodLookUpTable.ContainsKey(moodAnimationName))
					{
						if (moodAnimationName.ToString() == "Happy")
						{
							XmlNodeList assetNodes = moodNode.SelectNodes("//Assets/Asset");
							ClientAssetInfo faceAnimation = new ClientAssetInfo(assetNodes[0]);
							ClientAssetInfo idle = new ClientAssetInfo(assetNodes[1]);
							ClientAssetInfo walk = new ClientAssetInfo(assetNodes[2]);
							List<AssetInfo> assets = new List<AssetInfo>();
							assets.Add(faceAnimation);
							assets.Add(idle);
							assets.Add(walk);
							mMoodAssetInfoLookUpTable.Add(moodAnimationName, assets);
							mPlayableMoodLookUpTable.Add(moodAnimationName, true);
						}
						else
						{
							mPlayableMoodLookUpTable.Add(moodAnimationName, false);
						}
					}
				}
				mMoodsParsed = true;
				CheckIfAllPossibleEmotesIconsMoodsHaveBeenParsed();
			});

			mClientAssetRepository.LoadAssetFromPath<XmlAsset>(PATH_TO_ICON_LIST_XML, delegate(XmlAsset iconListXmlAsset)
			{
				// Parse out all the emotes and set them as false.
				foreach (XmlNode iconNode in iconListXmlAsset.XmlDocument.SelectNodes("IconList/Icon"))
				{
					if (!mPlayableIconLookUpTable.ContainsKey(iconNode.InnerText))
					{
						mPlayableIconLookUpTable.Add(iconNode.InnerText, false);
					}
				}
				mIconsParsed = true;
				CheckIfAllPossibleEmotesIconsMoodsHaveBeenParsed();
			});
		}
		
		/// <summary>
		/// When Emotes, moods, and Icons have been pre-populated the call is made to
		/// get the asset infos for the moods, emotes, and icons we can play.
		/// </summary>
		private void CheckIfAllPossibleEmotesIconsMoodsHaveBeenParsed()
		{
			if (mMoodsParsed && mIconsParsed && mEmotesParsed)
			{
				string itemTypes = ItemType.MOOD + "," + ItemType.EMOTE + "," + ItemType.EMOTICON;
				InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
				inventoryProxy.GetPlayerAnimations(itemTypes, inventoryProxy.HandlePlayerAnimationResponse);
			}
		}

        public void SetOwnedEmotesAndMoodsXml(XmlDocument emoteXml)
		{
			foreach (XmlNode itemNode in emoteXml.SelectNodes("Response/itemInstances/itemInstance/item"))
			{
				string itemType = XmlUtility.GetStringAttribute(itemNode, "itemTypeName");
				switch (itemType)
				{
					case "Emote":
						XmlNode assetInfoNode = itemNode.SelectSingleNode("Assets/Asset");
                        AssetInfo assetInfo = new ClientAssetInfo(assetInfoNode);
						string animationNameString = assetInfoNode.SelectSingleNode("AssetData/AnimationName").InnerText;
						RigAnimationName rigAnimationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), animationNameString);
						// Add asset info.
						if (!mEmoteToAssetInfoLookUpTable.ContainsKey(rigAnimationName))
						{
							mEmoteToAssetInfoLookUpTable.Add(rigAnimationName, assetInfo);
						}
						// Add to owned lookup table.
						// TODO: Get a true or false with the response to actually set it.
						if (!mPlayableEmoteLookUpTable.ContainsKey(rigAnimationName))
						{
							mPlayableEmoteLookUpTable.Add(rigAnimationName, true);
						}
						else
						{
							mPlayableEmoteLookUpTable[rigAnimationName] = true;
						}
						break;
					case "Mood":
						List<AssetInfo> moodAssetInfos = new List<AssetInfo>();
						string moodNameString = XmlUtility.GetStringAttribute(itemNode, "buttonName");
						moodNameString = moodNameString.Split(' ')[0];
						MoodAnimation moodName = (MoodAnimation)Enum.Parse(typeof(MoodAnimation), moodNameString);
						foreach (XmlNode assetNode in itemNode.SelectNodes("Assets/Asset"))
						{
                            AssetInfo moodAssetInfo = new ClientAssetInfo(assetNode);
                            // If the client Asset repo has the walk animation cached these walk and idle animations (happens when
                            /// game states are changed), the animations wouldn't have been added to this proxy.
							if (moodAssetInfo.AssetSubType == AssetSubType.RigAnimation || moodAssetInfo.AssetSubType == AssetSubType.RigWalkAnimation || moodAssetInfo.AssetSubType == AssetSubType.RigIdleAnimation)
							{
								SetRigAnimationAssetInfo(moodAssetInfo);
							}
							moodAssetInfos.Add(moodAssetInfo);
						}
						if (!mMoodAssetInfoLookUpTable.ContainsKey(moodName))
						{
							mMoodAssetInfoLookUpTable.Add(moodName, moodAssetInfos);
						}
						// Add to owned lookup table.
						// TODO: Get a true or false with the response to actually set it.
						if (!mPlayableMoodLookUpTable.ContainsKey(moodName))
						{
							mPlayableMoodLookUpTable.Add(moodName, true);
						}
						else
						{
							mPlayableMoodLookUpTable[moodName] = true;
						}
						break;
					case "Emoticon":
						XmlNode emoticonInfoNode = itemNode.SelectSingleNode("Assets/Asset");
						if (emoticonInfoNode == null)
						{
							Debug.LogError("emoticonInfoNode is null in Xml: " + itemNode.OuterXml);
							break;
						}
						AssetInfo emoticonInfo = new ClientAssetInfo(emoticonInfoNode);
						if (!mEmoticonToAssetInfoLookUpTable.ContainsKey(emoticonInfo.Path))
						{
							mEmoticonToAssetInfoLookUpTable.Add(emoticonInfo.Path, emoticonInfo);
						}

						if (!mPlayableIconLookUpTable.ContainsKey(emoticonInfo.Path))
						{
							mPlayableIconLookUpTable.Add(emoticonInfo.Path, true);
						}
						else
						{
							mPlayableIconLookUpTable[emoticonInfo.Path] = true;
						}
						break;
					default:
						Debug.LogError("Do not yet know how to parse type of: " + itemType);
						break;
				}
			}
            // Parse the xml into Asset Infos and store list with AnimationName enum.
			// Fill this out mEmoteToAssetInfoLookUpTable here.
            GameFacade.Instance.SendNotification(GameFacade.ANIMATION_PROXY_LOADED);
			mOnFinishedLoading();
            mLoaded = true;
        }
        
        /// <summary>
        /// Adds the asset info to the RigAnimationName.
        /// </summary>
        /// <param name="rigAnimationName"></param>
        /// <param name="rigAnimationAssetInfo"></param>
        public void SetRigAnimationAssetInfo(RigAnimationName rigAnimationName, AssetInfo rigAnimationAssetInfo)
        {
			if (!mEmoteToAssetInfoLookUpTable.ContainsKey(rigAnimationName))
			{
				mEmoteToAssetInfoLookUpTable.Add(rigAnimationName, rigAnimationAssetInfo);
			}
			else
			{
				mEmoteToAssetInfoLookUpTable[rigAnimationName] = rigAnimationAssetInfo;
			}
        }
        
        private void SetRigAnimationAssetInfo(AssetInfo rigAnimationAssetInfo)
        {
			string animationNameString = rigAnimationAssetInfo.AssetData.SelectSingleNode("AnimationName").InnerText;
			RigAnimationName rigAnimationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), animationNameString);
			SetRigAnimationAssetInfo(rigAnimationName, rigAnimationAssetInfo);
        }

		/// <summary>
		/// Adds the asset info to the MoodAnimation.
		/// </summary>
		/// <param name="moodAnimationName"></param>
		/// <param name="moodAnimationAssetInfos"></param>
		public void SetMoodAnimationAssetInfo(MoodAnimation moodAnimationName, List<AssetInfo> moodAnimationAssetInfos)
		{
			if (!mMoodAssetInfoLookUpTable.ContainsKey(moodAnimationName))
			{
				mMoodAssetInfoLookUpTable.Add(moodAnimationName, moodAnimationAssetInfos);
			}
		}
		
		/// <summary>
		/// Adds the asset info to the Emoticon path..
		/// </summary>
		/// <param name="rigAnimationName"></param>
		/// <param name="rigAnimationAssetInfo"></param>
		public void SetEmoticonAssetInfo(AssetInfo assetInfo)
		{
			if (!mEmoticonToAssetInfoLookUpTable.ContainsKey(assetInfo.Path))
			{
				mEmoticonToAssetInfoLookUpTable.Add(assetInfo.Path, assetInfo);
			}
		}

		public void SetOwnedEmote(RigAnimationName rigAnimationName)
        {
			if (mPlayableEmoteLookUpTable.ContainsKey(rigAnimationName))
			{
				mPlayableEmoteLookUpTable[rigAnimationName] = true;
			}
			else if (mEmoteToAssetInfoLookUpTable.ContainsKey(rigAnimationName))
			{
				mPlayableEmoteLookUpTable.Add(rigAnimationName, true);
			}
			else
			{
				Debug.LogError("mEmoteToAssetInfoLookUpTable does not contain asset info, therefore we cannot add rig animation name: " + rigAnimationName + " to mPlayableEmoteLookUpTable");
			}
        }

		public void SetOwnedMood(MoodAnimation moodAnimation)
		{
			if (mPlayableMoodLookUpTable.ContainsKey(moodAnimation))
			{
				mPlayableMoodLookUpTable[moodAnimation] = true;
			}
			else if (mMoodAssetInfoLookUpTable.ContainsKey(moodAnimation))
			{
				mPlayableMoodLookUpTable.Add(moodAnimation, true);
			}
			else
			{
				Debug.LogError("mMoodAssetInfoLookUpTable does not contain asset info, therefore we cannot add mood animation name: " + moodAnimation + " to mPlayableEmoteLookUpTable");
			}
		}
        
        public void SetOwnedEmoticon(string emoticon)
        {
			if (mPlayableIconLookUpTable.ContainsKey(emoticon))
			{
				mPlayableIconLookUpTable[emoticon] = true;
			}
			else if (mEmoticonLookUpTable.ContainsKey(emoticon))
			{
				mPlayableIconLookUpTable.Add(emoticon, true);
			}
			else
			{
				Debug.LogError("mEmoticonLookUpTable does not contain asset info, therefore we cannot add emoticon path: " + emoticon + " to mPlayableIconLookUpTable");
			}
        }
		
		/// <summary>
		/// Adds rig animation asset to the look up table.
		/// </summary>
		/// <param name="rigAnimationAsset"></param>
		public void AddRigAnimation(RigAnimationAsset rigAnimationAsset)
		{
			if (mRigAnimationLookUpTable.ContainsKey(rigAnimationAsset.AnimationName))
			{	
				mRigAnimationLookUpTable[rigAnimationAsset.AnimationName] = rigAnimationAsset;
			}
			else
			{
				mRigAnimationLookUpTable.Add(rigAnimationAsset.AnimationName, rigAnimationAsset);
			}
		}
		
		public void GetRigAnimation(RigAnimationName animation, Action<AnimationClip> foundAnimationClipCallback)
		{
			// TODO: This is a bit of a hack should probably be removed.
			if (animation == RigAnimationName.None)
			{
				Debug.LogError("animation being passed into GetRigAnimation is 'None'.");
				return;
			}
			// Check if we have the the rig animation asset already.
			if (mRigAnimationLookUpTable.ContainsKey(animation))
			{
				foundAnimationClipCallback(mRigAnimationLookUpTable[animation].AnimationClip);
				return;
			}
			
			// If we don't have the rig animation asset already grab the info from the lookup table and loaded it from the repo.
			// The repo calls the AddRigAnimation function in the class which puts it in the rig animation lookup table. 
			if (!mEmoteToAssetInfoLookUpTable.ContainsKey(animation))
			{
				Debug.LogError("There is no asset info for the animation: " + animation.ToString());
				return;
			}
			mClientAssetRepository.GetAsset<RigAnimationAsset>(mEmoteToAssetInfoLookUpTable[animation], delegate(RigAnimationAsset asset)
			{
				foundAnimationClipCallback(asset.AnimationClip);
			});
		}
		
		public List<AssetInfo> GetMoodAssetInfos(MoodAnimation moodName)
		{
			if (mMoodAssetInfoLookUpTable.ContainsKey(moodName))
			{
				return mMoodAssetInfoLookUpTable[moodName];
			}
			else
			{
				Debug.LogError("Mood named: " + moodName + " is not found in mMoodAssetInfoLookUpTable");
				return new List<AssetInfo>();
			}
		}

		// TODO: FaceAnimation stuff has not been tested yet.  We will do that soon.
		public void AddFaceAnimation(FaceAnimationAsset faceAnimationAsset)
		{
			if (!mFaceAnimationLookUpTable.ContainsKey(faceAnimationAsset.FaceAnimationName))
			{
				mFaceAnimationLookUpTable.Add(faceAnimationAsset.FaceAnimationName, faceAnimationAsset);
			}
		}
// 
// 		public void GetFaceAnimation(FaceAnimationName animationName, Action<FaceAnimation> foundAnimationClipCallback)
// 		{
// 			if (mFaceAnimationLookUpTable.ContainsKey(animationName))
// 			{
// 				foundAnimationClipCallback(mFaceAnimationLookUpTable[animationName].FaceAnimation);
// 				return;
// 			}
// 		}
	}
}
