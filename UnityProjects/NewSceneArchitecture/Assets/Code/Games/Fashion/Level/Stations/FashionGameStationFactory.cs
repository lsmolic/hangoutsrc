/**  --------------------------------------------------------  *
 *   FashionGameStationFactory.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class FashionGameStationFactory : IDisposable
	{
		private class StationInfo
		{
			private readonly float mTime;
			public float Time
			{
				get { return mTime; }
			}

			private readonly Vector3 mGuiOffset;
			public Vector3 GuiOffset
			{
				get { return mGuiOffset; }
			}

			private readonly string mType;
			public string Type
			{
				get { return mType; }
			}

			private readonly string mImagePath;
			public string ImagePath
			{
				get { return mImagePath; }
			}
			
			private readonly GameObject mAssetPrototype;
			public GameObject InstantiateAssets()
			{
				GameObject result = (GameObject)GameObject.Instantiate(mAssetPrototype);
				result.SetActiveRecursively(true);
				return result;
			}

			private readonly List<string> mSoundPaths = new List<string>();
			public ICollection<string> SoundPaths
			{
				get { return mSoundPaths; }
			}

			private readonly string mIdleAnimationPath;
			public string IdleAnimationPath
			{
				get { return mIdleAnimationPath; }
			}

			private readonly string mWorkingAnimationPath;
			public string WorkingAnimationPath
			{
				get { return mWorkingAnimationPath; }
			}

			public StationInfo(XmlNode stationNode, GameObject assetsPrototypeRoot)
			{
				// Type (Mandatory)
				XmlNode typeNode = stationNode.SelectSingleNode("@type");
				if (typeNode != null)
				{
					mType = typeNode.InnerText;
					if( mType != "Hair" &&
						mType != "Makeup" &&
						mType != "Sewing" &&
						mType != "Holding" )
					{
						throw new Exception("Unknown Fashion Game Station Type (" + mType + ")");
					}
				}
				else
				{
					throw new Exception("No type attribute found on Station node. Cannot create station info.");
				}

				// Asset (Mandatory)
				XmlNode assetNode = stationNode.SelectSingleNode("@asset");
				if( assetNode != null )
				{
					mAssetPrototype = GameObjectUtility.GetNamedChild(assetNode.InnerText, assetsPrototypeRoot);
				} 
				else
				{
					throw new Exception("No asset attribute found on Station node. Cannot create station info.");
				}

				// Time (Optional)
				XmlNode timeNode = stationNode.SelectSingleNode("@time");
				if( timeNode != null )
				{
					mTime = Rangef.Parse(timeNode.InnerText).RandomValue();
				}
				else
				{
					mTime = 0.0f;
				}

				// Icon (Mandatory)
				XmlNode imagePathNode = stationNode.SelectSingleNode("Icon/@path");
				if( imagePathNode != null )
				{
					mImagePath = ProtocolUtility.ResolvePath(imagePathNode.InnerText);
				}
				else
				{
					mImagePath = null;
				}

				// Gui Offset (Optional)
				XmlNode guiOffsetNode = stationNode.SelectSingleNode("@guiOffset");
				if( guiOffsetNode != null )
				{
					mGuiOffset = SerializationUtility.ToVector3(guiOffsetNode.InnerText);
				}
				else
				{
					mGuiOffset = Vector3.zero;
				}

				// Sounds (Optional, Multiple Supported)
				XmlNodeList soundNodes = stationNode.SelectNodes("ActivationSound/@path");
				foreach(XmlNode soundNode in soundNodes)
				{
					mSoundPaths.Add(soundNode.InnerText);
				}

				// WorkingAnimation (Optional)
				XmlNode workingAnimationNode = stationNode.SelectSingleNode("WorkingAnimation/@path");
				if (workingAnimationNode != null)
				{
					mWorkingAnimationPath = workingAnimationNode.InnerText;
				}
				else
				{
					mWorkingAnimationPath = null;
				}

				// IdleAnimation (Optional)
				XmlNode idleAnimationNode = stationNode.SelectSingleNode("IdleAnimation/@path");
				if (idleAnimationNode != null)
				{
					mIdleAnimationPath = idleAnimationNode.InnerText;
				}
				else
				{
					mIdleAnimationPath = null;
				}
			}
		}

		private const string STATION_DESCRIPTION_PATH = "resources://Fashion Game Settings/Stations";
		private const string STATION_ASSETS_PATH = "assets://FashionMinigame/FashionFrenzyStations.lba";
		private readonly IDictionary<string, StationInfo> mStationInfos = new Dictionary<string, StationInfo>();

		public FashionGameStationFactory(GameObject stationsAssets)
		{
			XmlDocument stationDoc = XmlUtility.LoadXmlDocument(STATION_DESCRIPTION_PATH);
			foreach (XmlNode stationNode in stationDoc.SelectNodes("//Station"))
			{
				StationInfo newInfo = new StationInfo(stationNode, stationsAssets);
				mStationInfos.Add(newInfo.Type, newInfo);
			}
		}

		public void BuildStation(string type, Pair<Vector3> location, string name, Action<FashionGameStation> onResult)
		{
			if( onResult == null )
			{
				throw new ArgumentNullException("onResult");
			}

			FashionGameStation resultStation = null;

			if( !mStationInfos.ContainsKey(type) )
			{
				throw new Exception("Unexpected station type (" + type + "). Stations can only be of types defined in " + STATION_DESCRIPTION_PATH);
			}

			StationInfo info = mStationInfos[type];

			Texture2D stationIcon = null;
			if( info.ImagePath != null )
			{
				stationIcon = (Texture2D)Resources.Load(info.ImagePath);
			}
			switch(type)
			{
				case "Holding":
					resultStation = new HoldingStation(location, name, info.Time, info.GuiOffset, info.InstantiateAssets());
					break;

				case "Sewing":
					resultStation = new TailorStation(location, name, stationIcon, info.Time, info.GuiOffset, info.InstantiateAssets());
					break;

				case "Hair":
					if (stationIcon == null)
					{
						throw new Exception("Unable to load the texture at " + info.ImagePath);
					}
					resultStation = new HairStation(location, name, stationIcon, info.Time, info.GuiOffset, info.InstantiateAssets());
					break;
				case "Makeup":
					if (stationIcon == null)
					{
						throw new Exception("Unable to load the texture at " + info.ImagePath);
					}
					resultStation = new MakeupStation(location, name, stationIcon, info.Time, info.GuiOffset, info.InstantiateAssets());
					break;
			}

			// If this station requires late bound animations or sounds, load them, otherwise just return the station
			if( info.SoundPaths.Count > 0 || info.WorkingAnimationPath != null || info.IdleAnimationPath != null )
			{
				GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(LoadExternalAssetsForStation(info, resultStation, onResult));
			}
			else
			{
				onResult(resultStation);
			}
		}

		private IEnumerator<IYieldInstruction> LoadExternalAssetsForStation(StationInfo stationInfo, FashionGameStation station, Action<FashionGameStation> onResult)
		{
			
			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			int loadingSounds = 0;
			foreach(string soundPath in stationInfo.SoundPaths)
			{
				loadingSounds++;
				assetRepo.LoadAssetFromPath<SoundAsset>(soundPath, delegate(SoundAsset asset)
				{
					loadingSounds--;
					station.AddActivationSound(asset.AudioClip);
				});
			}

			int loadingAnimation = 0;

			if (stationInfo.WorkingAnimationPath != null)
			{
				loadingAnimation++;
				SetupWorkerAnimation(stationInfo.WorkingAnimationPath, stationInfo, station, delegate(AnimationClip clip)
				{
					loadingAnimation--;
					station.SetWorkingAnimation(clip);
				});
			}

			if (stationInfo.IdleAnimationPath != null)
			{
				loadingAnimation++;
				SetupWorkerAnimation(stationInfo.IdleAnimationPath, stationInfo, station, delegate(AnimationClip clip)
				{
					loadingAnimation--;
					station.SetIdleAnimation(clip);
				});
			}

			yield return new YieldWhile(delegate() 
			{
				return loadingSounds != 0 || loadingAnimation != 0;
			});

			onResult(station);
		}
		
		private void SetupWorkerAnimation(string animationPath, StationInfo stationInfo, FashionGameStation station, Action<AnimationClip> applyAnimation)
		{
			if (!station.RequiresWorker)
			{
				throw new Exception("Station (" + stationInfo.Type + ") has an animation node, but it isn't a type of station that has a worker.");
			}

			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			assetRepo.LoadAssetFromPath<RigAnimationAsset>(animationPath, delegate(RigAnimationAsset asset)
			{
				applyAnimation(asset.AnimationClip);
			});
		}

		private List<IReceipt> mLoadingAssets = new List<IReceipt>();
		public void Dispose()
		{
			foreach(IReceipt r in mLoadingAssets )
			{
				r.Exit();
			}
		}
	}
}
