/**  --------------------------------------------------------  *
 *   FashionGameGui.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;

using UnityEngine;

using PureMVC.Patterns;

namespace Hangout.Client.FashionGame
{
	public class FashionLevel : Mediator
	{
		private Pair<Vector3> mStart;
		private float mStartWidth;
		private Pair<Vector3> mEnd;
		private float mEndWidth;

		public Pair<Vector3> Start
		{
			get { return mStart; }
		}

		public float StartWidth
		{
			get { return mStartWidth; }
		}

		public Pair<Vector3> End
		{
			get { return mEnd; }
		}

		public float EndWidth
		{
			get { return mEndWidth; }
		}

		private FashionGameStationFactory mFactory = null;
		private XmlDocument mLevelXml;
		private readonly string mLevelXmlPath;

		private string mRunwayBackgroundPath;
		public string RunwayBackgroundPath
		{
			get { return mRunwayBackgroundPath; }
		}
		
		private string mLocationName;
		public string LocationName
		{
			get { return mLocationName; }
		}

		private string mRunwayMusicPath;
		public string RunwayMusicPath
		{
			get { return mRunwayMusicPath; }
		}
		
		private readonly LevelGameplay mLevelGameplay;
		public LevelGameplay Gameplay
		{
			get { return mLevelGameplay; }
		}

		private readonly IScheduler mScheduler;
		private readonly FashionNpcMediator mFashionNpcMediator;

		private readonly float mRequiredEnergy;
		public float RequiredEnergy
		{
			get { return mRequiredEnergy; }
		}
		
		private readonly bool mFirstTimePlayed;
		public bool FirstTimePlayed
		{
			get { return mFirstTimePlayed; }
		}

		private readonly int mModelsRequired;
		public int ModelsRequired
		{
			get { return mModelsRequired; }
		}
		private readonly TaskCollection mLevelTasks = new TaskCollection();

		private bool mLevelSetup = false;
		public bool IsLoaded
		{
			get { return mLevelSetup; }
		}
		private GameObject mLevelRoot = null;
		private UnityEngineAsset mLevelAsset = null;

		public uint PerfectCompleteBonus
		{
			get { return mLevelGameplay.PerfectLevelBonus; }
		}

		private readonly LevelGui mLevelGui;
		public LevelGui Gui
		{
			get { return mLevelGui; }
		}
		private readonly string mName;
		public string Name
		{
			get { return mName; }
		}

		public ICollection<ModelWave> Waves
		{
			get { return mLevelGameplay.Waves; }
		}

		public float CloseSaveDistance
		{
			get { return mLevelGameplay.CloseSaveDistance; }
		}

		public bool HiresNewAvatar
		{
			get { return mLevelGui.HiresNewAvatar; }
		}

		private uint mNeedsFixinTotal = 0;
		public uint NeedsFixinTotal
		{
			get { return mNeedsFixinTotal; }
			set { mNeedsFixinTotal = value; }
		}

		private uint mUnfixedClothing = 0;
		public uint UnfixedClothing
		{
			get { return mUnfixedClothing; }
			set { mUnfixedClothing = value; }
		}
		// All the stations' gameObjects in this level
		private readonly IDictionary<GameObject, FashionGameStation> mStations = new Dictionary<GameObject, FashionGameStation>();

		public bool HasStations<T>() where T : FashionGameStation
		{
			foreach(FashionGameStation station in mStations.Values)
			{
				if( station is T )
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerable<T> GetStations<T>() where T : FashionGameStation
		{
			// Filter to return only 1 of each station
			List<T> stations = new List<T>();
			foreach (FashionGameStation station in mStations.Values)
			{
				if (station is T)
				{
					T stationT = (T)station;
					if (!stations.Contains(stationT))
					{
						stations.Add(stationT);
					}
				}
			}
			return stations;
		}

		public IEnumerable<FashionGameStation> Stations
		{
			get { return GetStations<FashionGameStation>(); }
		}

		public IEnumerable<HoldingStation> HoldingStations
		{
			get { return GetStations<HoldingStation>(); }
		}

		public IEnumerable<ModelStation> ModelStations
		{
			get { return GetStations<ModelStation>(); }
		}

		public IEnumerable<TailorStation> TailorStations
		{
			get { return GetStations<TailorStation>(); }
		}

		public FashionLevel(string levelDataPath, FashionNpcMediator fashionNpcMediator, bool firstTimePlayed, float requiredEnergy)
		{
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			
			mLevelXmlPath = levelDataPath;
			mLevelXml = XmlUtility.LoadXmlDocument(mLevelXmlPath);

			XmlNode backgroundNode = mLevelXml.SelectSingleNode("Level/RunwayBackground");
			if (backgroundNode != null)
			{
				mRunwayBackgroundPath = backgroundNode.InnerText;
			}
			else
			{
				Debug.LogError("No background url node in level xml.");
			}

			XmlNode locationNode = mLevelXml.SelectSingleNode("Level/LocationName");
			if (backgroundNode != null)
			{
				mLocationName = locationNode.InnerText;
			}
			else
			{
				Debug.LogError("No location node in level xml.");
			}

			XmlNode runwayMusicNode = mLevelXml.SelectSingleNode("Level/RunwayMusic");
			if (runwayMusicNode != null)
			{
				mRunwayMusicPath = runwayMusicNode.InnerText;
			}
			else
			{
				Debug.LogError("No runway music node in level xml.");
			}

			XmlNode levelAssetsPath = mLevelXml.SelectSingleNode("Level/@path");
			if (levelAssetsPath == null)
			{
				throw new Exception("Cannot load level from " + levelDataPath + ", no Level/@path attribute found.");
			}

			ClientAssetRepository clientAssetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			clientAssetRepo.LoadAssetFromPath<UnityEngineAsset>(levelAssetsPath.InnerText, BuildLevelAssets);

			XmlNode startNode = mLevelXml.SelectSingleNode("Level/ModelSpawn");
			mStartWidth = float.Parse(startNode.Attributes["width"].InnerText);
			mEnd = XmlUtility.ParsePositionDirection(mLevelXml.SelectSingleNode("Level/ModelDrain"));
			mEndWidth = float.Parse(startNode.Attributes["width"].InnerText);
			mName = mLevelXml.SelectSingleNode("Level/@name").InnerText;
			mStart = XmlUtility.ParsePositionDirection(startNode);
			mModelsRequired = int.Parse(mLevelXml.SelectSingleNode("Level/Waves/@requiredModels").InnerText);

			mLevelGui = new LevelGui(mLevelXml, this);
			if( fashionNpcMediator == null )
			{
				throw new ArgumentNullException("fashionModelMediator");
			}
			mFashionNpcMediator = fashionNpcMediator;
			mLevelGameplay = new LevelGameplay(mLevelXml, this, mFashionNpcMediator);
			mFirstTimePlayed = firstTimePlayed;

			mRequiredEnergy = requiredEnergy;
		}
		
		public void StartLevel(bool needsToHire)
		{
			mLevelGui.StartLevel(mFirstTimePlayed, needsToHire, mLevelGameplay.StartLevel);
		}

		private void BuildLevelAssets(Asset asset)
		{
			mScheduler.StartCoroutine(BuildLevelAssetsCoroutine(asset));
		}

		private IEnumerator<IYieldInstruction> BuildLevelAssetsCoroutine(Asset asset)
		{
			mLevelAsset = (UnityEngineAsset)asset;
			GameObject fashionGameAssets = (GameObject)mLevelAsset.UnityObject;
			mLevelRoot = (GameObject)GameObject.Instantiate(GameObjectUtility.GetNamedChild("Environment", fashionGameAssets));
			mLevelRoot.transform.position = Vector3.zero;

			GameObject stationsRoot = GameObjectUtility.GetNamedChild("Stations", fashionGameAssets);
			mFactory = new FashionGameStationFactory(stationsRoot);

			int loadingStations = 0;
			List<string> labels = new List<string>();
			foreach (XmlNode node in mLevelXml.SelectNodes("Level/Stations/Station"))
			{
				XmlNode labelNode = node.Attributes["label"];
				string label = null;
				if (labelNode != null)
				{
					label = labelNode.InnerText;

					if (labels.Contains(label))
					{
						throw new Exception("Fashion Minigame does not support multiple stations with the same label.");
					}
					labels.Add(label);
				}

				loadingStations++;
				mFactory.BuildStation
				(
					node.Attributes["type"].InnerText, 
					XmlUtility.ParsePositionDirection(node), 
					label, 
					delegate(FashionGameStation station)
					{
						loadingStations--;
						foreach (Component component in station.UnityGameObject.GetComponentsInChildren(typeof(Collider)))
						{
							mStations.Add(component.gameObject, station);
						}
						
						if( station.RequiresWorker )
						{
							mLevelTasks.Add(mScheduler.StartCoroutine(AssignNpcWhenAvailable(station)));
						}
					}
				);
			}

			ClothingMediator clothingMediator = GameFacade.Instance.RetrieveMediator<ClothingMediator>();
			yield return new YieldWhile(delegate() 
			{
				return loadingStations != 0 || !clothingMediator.ClothingLoaded;
			});

			mLevelGameplay.SetupWaves();
			ReloadStationPositions();

			mLevelSetup = true;
		}

		private IEnumerator<IYieldInstruction> AssignNpcWhenAvailable(FashionGameStation station)
		{
			yield return new YieldWhile(delegate()
			{
				return !mFashionNpcMediator.HasActiveWorkerForStation(station);
			});

			station.AssignWorker(mFashionNpcMediator.GetStationWorker(station));
		}

		private void SetupCamera(XmlNode cameraXmlNode)
		{
			if (cameraXmlNode == null)
			{
				throw new ArgumentNullException("cameraXmlNode");
			}
			FashionCameraMediator cam = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>();

			Vector3 position = SerializationUtility.ToVector3(cameraXmlNode.SelectSingleNode("Position/@vector3").InnerText);
			Vector3 rotation = SerializationUtility.ToVector3(cameraXmlNode.SelectSingleNode("Rotation/@vector3").InnerText);
			float fov = float.Parse(cameraXmlNode.SelectSingleNode("FOV/@float").InnerText);

			cam.SetupCamera(position, Quaternion.Euler(rotation), fov);
		}

		public void ReloadStationPositions()
		{
			mLevelXml = XmlUtility.LoadXmlDocument(mLevelXmlPath);

			foreach (XmlNode node in mLevelXml.SelectNodes("Level/Stations/Station"))
			{
				XmlNode labelNode = node.Attributes["label"];
				string label = null;
				if (labelNode != null)
				{
					label = labelNode.InnerText;
				}

				GetStationByName(label).SetPositionDirection(XmlUtility.ParsePositionDirection(node));
			}

			SetupCamera(mLevelXml.SelectSingleNode("Level/Camera"));
		}

		private FashionGameStation GetStationByName(string name)
		{
			foreach (FashionGameStation station in this.Stations)
			{
				if (station.Name == name)
				{
					return station;
				}
			}
			return null;
		}

		public bool IsComplete
		{
			get { return mLevelGameplay.IsComplete; }
		}

		public void ModelInactive(FashionModel model)
		{
			mLevelGameplay.ModelInactive(model);
		}

		public ICollection<FashionModel> ActiveModels
		{
			get { return mLevelGameplay.ActiveModels; }
		}

		public FashionModel GetModelFromGameObject(GameObject gameObject)
		{
			return mLevelGameplay.GetModelFromGameObject(gameObject);
		}

		public FashionGameStation GetStationFromGameObject(GameObject gameObject)
		{
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}

			FashionGameStation result;
			mStations.TryGetValue(gameObject, out result);
			return result;
		}

		public override void OnRemove()
		{
			base.OnRemove();

			foreach (FashionGameStation station in mStations.Values)
			{
                if (station == null)
                {
                    throw new Exception("There's a null station in the level");
                }
				station.Dispose();
			}
			mStations.Clear();

			UnityEngine.GameObject.Destroy(mLevelRoot);

            if (mLevelAsset != null)
            {
                mLevelAsset.Dispose();
            }

			mLevelGui.Dispose();
			mLevelGameplay.Dispose();
			mLevelTasks.Dispose();
		}
	}
}
