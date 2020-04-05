/**  --------------------------------------------------------  *
 *   FashionNpcMediator.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;
using Hangout.Shared.FashionGame;

using UnityEngine;
using Hangout.Shared.Messages;

namespace Hangout.Client.FashionGame
{
	public class FashionNpcMediator : CharacterMediator
	{
		private const string MODEL_TYPES_PATH = "resources://Fashion Game Settings/Models";
		private readonly IDictionary<string, FashionModelInfo> mFashionModelTypes = new Dictionary<string, FashionModelInfo>();
		private readonly System.Random mRand = new System.Random();

		// Pairs of ItemTypes that serve the same purpose in gameplay
		private readonly static Pair<string>[] mSynonyms = new Pair<string>[]
		{
			new Pair<string>(ItemType.PANTS, ItemType.SKIRT)
		};

		private bool mAvatarsDownloading = false;

		private List<FacebookFriendInfo> mHiredFacebookFriendInfos = new List<FacebookFriendInfo>();
		public List<FacebookFriendInfo> HiredFacebookFriendInfos
		{
			get { return mHiredFacebookFriendInfos; }
		}

		private interface INpcPool
		{
			bool HasNpc(long uniqueId);
			bool HasInactiveNpcs { get; }
			int Count { get; }
		}

		private class NpcPool<T> : INpcPool, IDisposable
			where T : INpc
		{
			private readonly System.Random mRand = new System.Random(typeof(T).GetHashCode() * (int)DateTime.Now.Ticks);
			private readonly Dictionary<long, T> mNpcs = new Dictionary<long, T>();
			private readonly List<Asset> mDefaultAssets;

			public NpcPool(IEnumerable<Asset> defaultAssets)
			{
				mDefaultAssets = new List<Asset>(defaultAssets);
			}

			public int Count
			{
				get { return mNpcs.Count; }
			}

			public bool AddNpc(long uniqueId, INpc npc)
			{
				return AddNpc(uniqueId, (T)npc);
			}

			public bool AddNpc(long uniqueId, T npc)
			{
				ResetNpc(npc);
				if (!mNpcs.ContainsKey(uniqueId))
				{
					mNpcs.Add(uniqueId, npc);
					return true;
				}
				return false;
			}

			public bool HasNpc(long uniqueId)
			{
				return mNpcs.ContainsKey(uniqueId);
			}

			public void ResetNpc(T npc)
			{
				//npc.HeadController.SetAssets(mDefaultAssets);
				npc.BodyController.SetAssets(mDefaultAssets);
				//npc.HeadController.UpdateAssets();
				npc.BodyController.UpdateAssets();
			}

			public T GetNpc()
			{
				List<T> inactiveNpcs = new List<T>();
				foreach (T npc in mNpcs.Values)
				{
					if (!npc.Active)
					{
						inactiveNpcs.Add(npc);
					}
				}

				if (inactiveNpcs.Count == 0)
				{
					throw new Exception("There are no inactive " + typeof(T).Name + "s available");
				}

				int index = mRand.Next(0, inactiveNpcs.Count);
				T result = inactiveNpcs[index];
				return result;
			}

			public ICollection<T> GetListOfNpcs()
			{
				return mNpcs.Values;
			}

			public bool HasInactiveNpcs
			{
				get
				{
					bool result = false;
					foreach (T npc in mNpcs.Values)
					{
						if (!npc.Active)
						{
							result = true;
							break;
						}
					}
					return result;
				}
			}

			public void Dispose()
			{
				foreach (KeyValuePair<long, T> kvp in mNpcs)
				{
					kvp.Value.Dispose();
				}

				mDefaultAssets.RemoveAll(delegate(Asset asset)
				{
					asset.Dispose();
					return true;
				});
			}
		}

		private NpcPool<StationWorker> mHairStationWorkers = null;
		private NpcPool<StationWorker> mMakeupStationWorkers = null;
		private NpcPool<StationWorker> mSewingStationWorkers = null;
		private NpcPool<FashionModel> mFashionModels = null;
		public ICollection<FashionModel> FashionModelList
		{
			get
			{
				return mFashionModels.GetListOfNpcs();
			}
		}

		// TODO: Hard coded value
		private static readonly GameObject mRigPrototype = (GameObject)Resources.Load("Avatar/Avatar Rig");

		private readonly TaskCollection mActiveTasks = new TaskCollection();
		private readonly IScheduler mScheduler;

		private AnimationClip mImpatientSitIdle = null;
		private AnimationClip mSittingIdle = null;

		private float mMediumHurriedSpeed = 1.0f;
		private AnimationClip mMediumHurried = null;

		private float mCatWalkSpeed = 1.0f;
		private AnimationClip mCatWalk = null;

		private bool mModelAnimationsDownloaded = false;

		private IEnumerable<Asset> mDefaultWorkerClothes = null;
		private IEnumerable<Asset> mDefaultModelClothes = null;

		public FashionNpcMediator()
		{
			XmlDocument modelsXmlDoc = XmlUtility.LoadXmlDocument(MODEL_TYPES_PATH);
			foreach (XmlNode modelInfoNode in modelsXmlDoc.SelectNodes("//Model"))
			{
				FashionModelInfo info = new FashionModelInfo(modelInfoNode);
				mFashionModelTypes.Add(info.Name, info);
			}

			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mActiveTasks.Add(mScheduler.StartCoroutine(LoadAnimations(modelsXmlDoc)));
		}

		private AnimationClip GetClipFromAsset(Asset asset)
		{
			if (asset is UnityEngineAsset)
			{
				UnityEngine.Object unityObject = ((UnityEngineAsset)asset).UnityObject;
				GameObject gameObject = (GameObject)GameObject.Instantiate(unityObject);
				try
				{
					Animation animation = (Animation)gameObject.GetComponentInChildren(typeof(Animation));
					if (animation == null)
					{
						throw new Exception("Error loading late bound asset file, there's no animation component on the asset (" + gameObject.name + ")");
					}

					AnimationClip result = animation.GetClip(unityObject.name);
					if (result == null)
					{
						throw new Exception("Cannot find clip (" + unityObject.name + ") in Animation component on " + animation.name);
					}
					return result;
				}
				finally
				{
					GameObject.Destroy(gameObject);
				}
			}
			else if(asset is RigAnimationAsset)
			{
				return ((RigAnimationAsset)asset).AnimationClip;
			}
			else
			{
				throw new ArgumentException("Unexpected asset type: " + asset.GetType().Name);
			}
		}

		private IEnumerator<IYieldInstruction> LoadAnimations(XmlDocument modelsXmlDoc)
		{
			uint downloadingAssets = 0;
			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			foreach (XmlNode animationNode in modelsXmlDoc.SelectSingleNode("Models/Animations").ChildNodes)
			{
				downloadingAssets++;
				string nodeName = animationNode.Name;

				XmlNode speedNode = animationNode.SelectSingleNode("@speed");
				assetRepo.LoadAssetFromPath<RigAnimationAsset>
				(
					animationNode.Attributes["path"].InnerText,
					delegate(RigAnimationAsset loadedAsset)
					{
						downloadingAssets--;

						AnimationClip clip = GetClipFromAsset(loadedAsset);
						switch (nodeName)
						{
							case "ImpatientSitIdle":
								mImpatientSitIdle = clip;
								break;
							case "SittingIdle":
								mSittingIdle = clip;
								break;
							case "MediumHurried":
								mMediumHurriedSpeed = float.Parse(speedNode.InnerText);
								mMediumHurried = clip;
								break;
							case "CatWalk":
								mCatWalkSpeed = float.Parse(speedNode.InnerText);
								mCatWalk = clip;
								break;
							default:
								throw new Exception("Unable to load the Animation (" + nodeName + ") from " + MODEL_TYPES_PATH);
						}
					}
				);
			}

			yield return new YieldWhile(delegate()
			{
				return downloadingAssets != 0;
			});
			mModelAnimationsDownloaded = true;
		}

		private INpcPool GetPoolForStation(FashionGameStation station)
		{
			NpcPool<StationWorker> pool;
			if (station is HairStation)
			{
				pool = mHairStationWorkers;
			}
			else if (station is MakeupStation)
			{
				pool = mMakeupStationWorkers;
			}
			else if (station is TailorStation)
			{
				pool = mSewingStationWorkers;
			}
			else
			{
				throw new ArgumentException("Unexpected station type (" + station.GetType().Name + ")");
			}

			return pool;
		}

		public bool HasActiveWorkerForStation(FashionGameStation station)
		{
			return GetPoolForStation(station).HasInactiveNpcs;
		}

		public bool HasWorkerForStation(FashionGameStation station)
		{
			return GetPoolForStation(station).Count > 0;
		}

		public bool HasModelsForLevel(FashionLevel level)
		{
			if (level.ModelsRequired > mFashionModels.Count)
			{
				return false;
			}

			bool result = true;
			foreach (FashionGameStation station in level.Stations)
			{
				if (!(station is HoldingStation) && !HasWorkerForStation(station))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public void SetStationWorkerDefaultClothes(IEnumerable<Asset> defaultClothes)
		{
			mDefaultWorkerClothes = defaultClothes;

			// For now all the station workers share defaults
			mHairStationWorkers = new NpcPool<StationWorker>(mDefaultWorkerClothes);
			mMakeupStationWorkers = new NpcPool<StationWorker>(mDefaultWorkerClothes);
			mSewingStationWorkers = new NpcPool<StationWorker>(mDefaultWorkerClothes);
		}

		public void SetModelDefaultClothes(IEnumerable<Asset> defaultClothes)
		{
			mDefaultModelClothes = defaultClothes;
			mFashionModels = new NpcPool<FashionModel>(mDefaultModelClothes);
		}


		public bool Downloading
		{
			get
			{
				return mAvatarsDownloading || !mModelAnimationsDownloaded;
			}
		}

		public void BuildNpcsForLevel(FashionLevel level)
		{
			mAvatarsDownloading = true;

			FashionGameCommands.GetAllHiredAvatars(delegate(Message message)
			{
				GetHiredNpcs(message, level, delegate() { mAvatarsDownloading = false; });
			});
		}


		private void GetHiredNpcs(Message message, FashionLevel level, Hangout.Shared.Action onComplete)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			// Expecting the XML for a single avatar in each item in message.Data
			Dictionary<Jobs, List<XmlNode>> avatarsPerJob = new Dictionary<Jobs, List<XmlNode>>();

			foreach (XmlDocument doc in MessageUtil.GetXmlDocumentsFromMessage(message))
			{
				XmlNode avatarNode = doc.SelectSingleNode("//Avatar");

				if (avatarNode == null)
				{
					throw new Exception("Unable to find avatar node in messageData: " + doc.OuterXml);
				}

				XmlNode jobNode = avatarNode.SelectSingleNode("@job");
				if (jobNode == null)
				{
					throw new Exception("Unable to find job attribute on XmlNode: " + avatarNode.OuterXml);
				}

				Jobs job = (Jobs)Enum.Parse(typeof(Jobs), jobNode.InnerText);
				List<XmlNode> avatarsList;
				if (!avatarsPerJob.TryGetValue(job, out avatarsList))
				{
					avatarsList = new List<XmlNode>();
					avatarsPerJob.Add(job, avatarsList);
				}
				avatarsList.Add(avatarNode);
				AccountId accountId;
				if (avatarNode.Attributes["AccountId"].InnerText != "")
				{
					accountId = new AccountId(XmlUtility.GetUintAttribute(avatarNode, "AccountId"));
				}
				else
				{
					accountId = new AccountId(0u);
				}
				
				long fbAccountId = XmlUtility.GetLongAttribute(avatarNode, "FBID");
				string firstName = XmlUtility.GetStringAttribute(avatarNode, "FirstName");
				string lastName = XmlUtility.GetStringAttribute(avatarNode, "LastName");
				if (XmlUtility.XmlNodeHasAttribute(avatarNode, "ImageUrl"))
				{
					string imageUrl = XmlUtility.GetStringAttribute(avatarNode, "ImageUrl");
					FacebookFriendInfo facebookFriendInfo = new FacebookFriendInfo(accountId, fbAccountId, firstName, lastName, imageUrl);
					mHiredFacebookFriendInfos.Add(facebookFriendInfo);
				}
			}

			foreach (KeyValuePair<Jobs, List<XmlNode>> avatarListing in avatarsPerJob)
			{
				mActiveTasks.Add
				(
					mScheduler.StartCoroutine
					(
						BuildNpcs
						(
							avatarListing.Key,
							level,
							avatarListing.Value,
							onComplete
						)
					)
				);
			}
		}

		/// <summary>
		/// Receives messages from the server that report the assets of a single friend that was just hired by the user
		/// </summary>
		public void HiredFriend(Message message, FashionLevel level)
		{
			Jobs job = (Jobs)Enum.Parse(typeof(Jobs), message.Data[0].ToString());

			// Parse the XML from the message and build the avatars it describes
			mActiveTasks.Add
			(
				mScheduler.StartCoroutine
				(
					BuildNpcs
					(
						job,
						level,
						Functionals.Reduce<XmlDocument, List<XmlNode>>
						(
							delegate(List<XmlNode> accumulator, XmlDocument docItem)
							{
								foreach (XmlNode node in docItem.SelectNodes("//Avatar"))
								{
									accumulator.Add(node);
								}
								return accumulator;
							},
							MessageUtil.GetXmlDocumentsFromMessage(message)
						),
						level.Gui.HiredFriendLoaded
					)
				)
			);
		}

		private bool JobHasNpc(Jobs job, long npcId)
		{
			bool result = false;
			switch (job)
			{
				case Jobs.Hair:
					result = mHairStationWorkers.HasNpc(npcId);
					break;

				case Jobs.Makeup:
					result = mMakeupStationWorkers.HasNpc(npcId);
					break;

				case Jobs.Model:
					result = mFashionModels.HasNpc(npcId);
					break;

				case Jobs.Seamstress:
					result = mSewingStationWorkers.HasNpc(npcId);
					break;
			}
			return result;
		}

		private static readonly List<AssetSubType> mIgnoredAssets = new List<AssetSubType>(new AssetSubType[]
		{
			AssetSubType.RigAnimation,
			AssetSubType.EmoteAnimation,
			AssetSubType.Emoticon,
			AssetSubType.BagSkinnedMesh,
			AssetSubType.BagTexture,
			AssetSubType.IdleAnimation,
			AssetSubType.WalkAnimation,
			AssetSubType.SoundEffect,
			AssetSubType.SitAnimation,
			AssetSubType.RoomBackgroundThumbnail,
			AssetSubType.RoomBackgroundTexture,
			AssetSubType.RigIdleAnimation,
			AssetSubType.RigWalkAnimation,
			AssetSubType.PropAnimation,
			AssetSubType.PropMesh,
			AssetSubType.PropTexture
		});

		private IEnumerator<IYieldInstruction> BuildNpcs
		(
			Jobs job,
			FashionLevel level,
			IEnumerable<XmlNode> modelsXml,
			Hangout.Shared.Action onBuildingNpcsComplete
		)
		{
			// Key = FBID, Value = name, assets for model
			Dictionary<long, Pair<string, IEnumerable<Asset>>> modelInfos = new Dictionary<long, Pair<string, IEnumerable<Asset>>>();
			uint gettingAssets = 0;
			ClientAssetRepository repo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			foreach (XmlNode modelXml in modelsXml)
			{
				string name;
				XmlNode fbidNode = modelXml.SelectSingleNode(".//@FBID");
				if (fbidNode == null)
				{
					throw new Exception("modelXml doesn't have an FBID attribute: " + modelXml.OuterXml);
				}

				long uniqueId = long.Parse(fbidNode.InnerText);
				name = modelXml.Attributes["FirstName"].InnerText;// +" " + modelXml.Attributes["LastName"].InnerText;

				// Only grab the avatars we don't already have loaded
				if (!JobHasNpc(job, uniqueId) && !modelInfos.ContainsKey(uniqueId))
				{
					gettingAssets++;
					List<AssetInfo> assetInfos = new List<AssetInfo>();
					foreach(AssetInfo assetInfo in ClientAssetInfo.Parse(modelXml))
					{
						if( !mIgnoredAssets.Contains(assetInfo.AssetSubType) )
						{
							assetInfos.Add(assetInfo);
						}
					}
					
					repo.GetAssets<Asset>(assetInfos, delegate(IEnumerable<Asset> modelAssets)
					{
						gettingAssets--;
						if (!modelInfos.ContainsKey(uniqueId))
						{
							modelInfos.Add(uniqueId, new Pair<string, IEnumerable<Asset>>(name, modelAssets));
						}
						else
						{
							Console.WriteLine("Attempted to download model info for " + uniqueId + " twice at the same time.");
						}
					});
				}
			}

			yield return new YieldWhile(delegate()
			{
				return gettingAssets != 0;
			});

			int spreadFrames = 25;
			List<Pair<long, INpc>> newNpcs = new List<Pair<long, INpc>>();
			foreach (KeyValuePair<long, Pair<string, IEnumerable<Asset>>> assets in modelInfos)
			{

				// This object will be cleaned up by the fashion model
				GameObject displayObject = (GameObject)GameObject.Instantiate(mRigPrototype);
				displayObject.SetActiveRecursively(false);

				HeadController headController = new HeadController(GameObjectUtility.GetNamedChildRecursive("Head", displayObject));
				BodyController bodyController = new BodyController(displayObject);

				bodyController.SetAssets(assets.Value.Second);
				headController.SetAssets(assets.Value.Second);

				bodyController.UpdateAssetsOverFrames(spreadFrames);
				headController.UpdateAssetsOverFrames(spreadFrames);

				INpc newNpc;
				if (job == Jobs.Model)
				{
					newNpc = new FashionModel(assets.Value.First, level, displayObject, headController, bodyController);
				}
				else
				{
					newNpc = new StationWorker(assets.Value.First, displayObject, headController, bodyController);
				}

				CharacterMediator.AddAnimationClipToRig(newNpc.DisplayObject, "Avatar/F_walk_normal_loop");
				CharacterMediator.AddAnimationClipToRig(newNpc.DisplayObject, "Avatar/F_idle_normal_loop");

				newNpcs.Add(new Pair<long, INpc>(assets.Key, newNpc));
			}

			yield return new YieldWhile(delegate()
			{
				return spreadFrames-- != 0 || !mModelAnimationsDownloaded;
			});

			foreach (Pair<long, INpc> npc in newNpcs)
			{
				switch (job)
				{
					case Jobs.Hair:
						mHairStationWorkers.AddNpc(npc.First, npc.Second);
						break;
					case Jobs.Makeup:
						mMakeupStationWorkers.AddNpc(npc.First, npc.Second);
						break;
					case Jobs.Model:
						FashionModel newModel = (FashionModel)npc.Second;

						newModel.SetImpatientSitIdle(mImpatientSitIdle);
						newModel.SetSittingIdle(mSittingIdle);
						newModel.SetMediumHurried(mMediumHurried, mMediumHurriedSpeed);
						newModel.SetCatWalk(mCatWalk, mCatWalkSpeed);

						mFashionModels.AddNpc(npc.First, npc.Second);
						break;
					case Jobs.Seamstress:
						mSewingStationWorkers.AddNpc(npc.First, npc.Second);
						break;
				}
			}

			if (onBuildingNpcsComplete != null)
			{
				onBuildingNpcsComplete();
			}
		}

		public bool HasInactiveModels
		{
			get { return mFashionModels.HasInactiveNpcs; }
		}

		public FashionModel GetModel(FashionLevel level, FashionModelNeeds needs, string type)
		{
			FashionModelInfo modelTypeInfo = mFashionModelTypes[type];
			FashionModel resultModel = mFashionModels.GetNpc();

			resultModel.SetActive(needs, level);
			resultModel.WalkSpeed = modelTypeInfo.Speed;
			resultModel.UnityGameObject.SetActiveRecursively(true);

			return resultModel;
		}

		public StationWorker GetStationWorker(FashionGameStation station)
		{
			StationWorker result = null;
			if (station is HoldingStation)
			{
				throw new ArgumentException("HoldingStations don't have workers", "station");
			}
			else if (station is HairStation)
			{
				result = mHairStationWorkers.GetNpc();
			}
			else if (station is MakeupStation)
			{
				result = mMakeupStationWorkers.GetNpc();
			}
			else if (station is TailorStation)
			{
				result = mSewingStationWorkers.GetNpc();
			}
			else
			{
				throw new ArgumentException("Unexpected ModelStation (" + station.GetType().Name + ")");
			}

			return result;
		}

		public override void OnRemove()
		{
			base.OnRemove();

			mActiveTasks.Dispose();

			mHairStationWorkers.Dispose();
			mMakeupStationWorkers.Dispose();
			mSewingStationWorkers.Dispose();
			mFashionModels.Dispose();

			foreach (Asset asset in mDefaultWorkerClothes)
			{
				asset.Dispose();
			}
			foreach (Asset asset in mDefaultWorkerClothes)
			{
				asset.Dispose();
			}
		}

		public FashionModelNeeds BuildNeedsForType(string modelType, IEnumerable<ModelStation> modelStations)
		{
			FashionModelInfo modelInfo = mFashionModelTypes[modelType];
			return BuildNeedsFromInfo(modelInfo, modelStations);
		}

		private FashionModelNeeds BuildNeedsFromInfo(FashionModelInfo info, IEnumerable<ModelStation> modelStations)
		{
			FashionModelNeeds result = new FashionModelNeeds();
			int clothingNeeds = mRand.Next(info.ClothingNeeds.Low, info.ClothingNeeds.High + 1);
			int stationNeeds = mRand.Next(info.StationNeeds.Low, info.StationNeeds.High + 1);

			ClothingMediator clothingMediator = GameFacade.Instance.RetrieveMediator<ClothingMediator>();
			List<ModelStation> stations = new List<ModelStation>();

			foreach (ModelStation station in modelStations)
			{
				if (!(station is HoldingStation))
				{
					stations.Add(station);
				}
			}

			List<string> clothingTypes = new List<string>(clothingMediator.ClothingItemTypes);

			// Setup Clothing Needs
			for (int i = 0; i < clothingNeeds; ++i)
			{
				string clothingType = clothingTypes[mRand.Next(0, clothingTypes.Count)];
				clothingTypes.Remove(clothingType);

				foreach (Pair<string> synonym in mSynonyms)
				{
					if (synonym.First == clothingType)
					{
						clothingTypes.Remove(synonym.Second);
					}
					else if (synonym.Second == clothingType)
					{
						clothingTypes.Remove(synonym.First);
					}
				}

				List<ClothingItem> clothesForThisType = new List<ClothingItem>(clothingMediator.ClothingForType(clothingType));
				if (clothesForThisType.Count == 0)
				{
					continue;
				}

				ItemId clothing = clothesForThisType[mRand.Next(0, clothesForThisType.Count)].ItemId;
				result.Add(clothing);

				if (clothingTypes.Count == 0)
				{
					break;
				}
			}

			// Setup Station Needs
			if (stations.Count != 0)
			{
				for (int i = 0; i < stationNeeds; ++i)
				{
					ModelStation station = stations[mRand.Next(0, stations.Count)];

					string stationName = station.Name;
					stations.RemoveAll(delegate(ModelStation stationInList)
					{
						return stationInList.Name == stationName;
					});

					result.Add(station);

					if (stations.Count == 0)
					{
						break;
					}
				}
			}

			result.NeedFixinChance = info.NeedFixinChance;

			return result;
		}
	}
}
