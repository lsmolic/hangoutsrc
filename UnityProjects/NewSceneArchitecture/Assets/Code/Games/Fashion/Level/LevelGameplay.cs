/**  --------------------------------------------------------  *
 *   LevelGameplay.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;
using Hangout.Shared.FashionGame;

using UnityEngine;


namespace Hangout.Client.FashionGame
{
	public class LevelGameplay : IDisposable
	{
		private readonly static System.Random mRandom = new System.Random();
		private readonly XmlDocument mLevelXml;
		private readonly FashionLevel mLevel;
		private readonly IScheduler mScheduler;
		private readonly TaskCollection mSpawnModels = new TaskCollection();
		private int mMissedModels = 0;
		private bool mComplete = false;
		private bool mNextWaveButtonPressed = false;
		private readonly FashionNpcMediator mModelFactory;

		private readonly List<ModelWave> mWaves = new List<ModelWave>();
		public ICollection<ModelWave> Waves
		{
			get { return mWaves; }
		}

		private readonly uint mPerfectLevelBonus;
		public uint PerfectLevelBonus
		{
			get { return mPerfectLevelBonus; }
		}

		private readonly float mTimeBetweenWaves;

		private float mNextWaveTime = float.NegativeInfinity;

		private ITask mSpawnWaves = null;
		private ITask mFacebookFeedUpdate = null;

		// TODO: Hard coded value
		private static readonly float mAvoidDuplicateWavesThreshold = 2.0f;

		private readonly float mCloseSaveDistance;
		public float CloseSaveDistance
		{
			get { return mCloseSaveDistance; }
		}

		// All the models that are currently in the level
		private readonly IDictionary<GameObject, FashionModel> mActiveModels = new Dictionary<GameObject, FashionModel>();
		public ICollection<FashionModel> ActiveModels
		{
			get { return mActiveModels.Values; }
		}

		public LevelGameplay(XmlDocument levelXml, FashionLevel level, FashionNpcMediator modelFactory)
		{
			if( levelXml == null )
			{
				throw new ArgumentNullException("levelXml");
			}
			mLevelXml = levelXml;

			if (level == null)
			{
				throw new ArgumentNullException("level");
			}
			mLevel = level;

			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;

			XmlNode timeBetweenWaves = mLevelXml.SelectSingleNode("Level/Waves/@timeBetweenWaves");
			if (timeBetweenWaves == null)
			{
				throw new Exception("Unable to load level (" + mLevel.Name + "), no Level/Waves/@timeBetweenWaves node found.");
			}
			mTimeBetweenWaves = float.Parse(timeBetweenWaves.InnerText);

			XmlNode perfectLevelBonusNode = mLevelXml.SelectSingleNode("Level/PerfectLevelBonus/@value");
			if (perfectLevelBonusNode != null)
			{
				mPerfectLevelBonus = uint.Parse(perfectLevelBonusNode.InnerText);
			}
			else
			{
				mPerfectLevelBonus = 0;
			}

			XmlNode closeSaveDistanceNode = mLevelXml.SelectSingleNode("Level/CloseSaveDistance/@value");
			if (closeSaveDistanceNode == null)
			{
				throw new Exception("Cannot load level (" + mLevel.Name + "), no Level/@path attribute found.");
			}
			mCloseSaveDistance = float.Parse(closeSaveDistanceNode.InnerText);

			if (modelFactory == null)
			{
				throw new ArgumentNullException("modelFactory");
			}
			mModelFactory = modelFactory;

			mLevel.Gui.BuildTopGui(delegate()
			{
				if (Time.time - mNextWaveTime > mAvoidDuplicateWavesThreshold)
				{
					mNextWaveButtonPressed = true;

					EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.GAMEPLAY_BEHAVIOR, "NextWaveButtonPress", mLevel.Name);
				}
			});
		}

		public bool IsComplete
		{
			get { return mComplete; }
		}

		public FashionModel GetModelFromGameObject(GameObject gameObject)
		{
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}

			FashionModel result;
			mActiveModels.TryGetValue(gameObject, out result);

			return result;
		}

		public void SetupWaves()
		{
			foreach (XmlNode node in mLevelXml.SelectNodes("Level/Waves/Wave"))
			{
				mWaves.Add(new ModelWave(node, mModelFactory, mLevel.ModelStations));
			}
		}

		public void StartLevel()
		{
			FashionGameCommands.TryUseEnergy(mLevel.RequiredEnergy, delegate(Message useEnergyResultMessage)
			{
				Energy.UseRequestResult useEnergyResult = (Energy.UseRequestResult)Enum.Parse(typeof(Energy.UseRequestResult), (string)useEnergyResultMessage.Data[0]);

				switch (useEnergyResult)
				{
					case Energy.UseRequestResult.Success:
						StartWaves();
						break;

					case Energy.UseRequestResult.NotEnoughEnergy:
						EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.OUT_OF_ENERGY, "Level", mLevel.Name);
						mLevel.Gui.ShowOutOfEnergyGui();
						return;

					default:
						throw new NotImplementedException("Unhandled Energy.UseRequestResult");
				}

				GameFacade.Instance.RetrieveMediator<FashionGameGui>().SetEnergy
				(
					float.Parse((string)useEnergyResultMessage.Data[1]),
					float.Parse((string)useEnergyResultMessage.Data[2]),
					DateTime.Parse((string)useEnergyResultMessage.Data[3])
				);
			});
		}

		public void StartWaves()
		{
			mSpawnWaves = mScheduler.StartCoroutine(SpawnWaves());
		}

		private DateTime mLevelStartTime;
		private IEnumerator<IYieldInstruction> SpawnWaves()
		{
			PlayerProgression progression = GameFacade.Instance.RetrieveMediator<PlayerProgression>();
			mLevelStartTime = DateTime.UtcNow;
			
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_STARTED, "Level", mLevel.Name);

			uint startXp = progression.XP;

			yield return new YieldWhile(delegate()
			{
				return !mLevel.IsLoaded;
			});

			FashionGameGui fashionGameGui = GameFacade.Instance.RetrieveMediator<FashionGameGui>();
			int waveNum = 0;
			foreach (ModelWave wave in mWaves)
			{
				mNextWaveTime = Time.time + mTimeBetweenWaves;

				waveNum++;
				mLevel.Gui.SetWave(wave, waveNum, mWaves.Count);

				yield return new YieldForSeconds(mTimeBetweenWaves);

				// Pair: clothing name and need fixin chance
				List<Pair<ItemId, float>> clothesThisWave = new List<Pair<ItemId, float>>();
				foreach (FashionModelNeeds needs in wave.Needs)
				{
					foreach (ItemId itemId in needs.Clothing)
					{
						clothesThisWave.Add(new Pair<ItemId, float>(itemId, needs.NeedFixinChance));
					}
				}

				ClothingMediator clothingMediator = GameFacade.Instance.RetrieveMediator<ClothingMediator>();

				// Scramble the list and make some of the clothing need fixin
				while (clothesThisWave.Count > 0)
				{
					Pair<ItemId, float> item = clothesThisWave[mRandom.Next(0, clothesThisWave.Count)];
					clothesThisWave.Remove(item);

					ClothingItem newItem = clothingMediator.BuildClothingItem(item.First);

					if (UnityEngine.Random.value < item.Second)
					{
						newItem.MakeNeedFixin();
						mLevel.NeedsFixinTotal++;
					}

					fashionGameGui.PutItemInGui(newItem);

					yield return new YieldUntilNextFrame(); // Spreads out the texture copies over a few frames to avoid slowdowns
				}

				ITask spawnModelTask = mScheduler.StartCoroutine(SpawnModels(wave));
				mSpawnModels.Add(spawnModelTask);

				// Yield while there are any active models left from this wave, 
				//  or if a model is still at a station.
				yield return new YieldWhile
				(
					delegate()
					{
						bool keepWaiting = true;

						if (mNextWaveButtonPressed)
						{
							int modelsLeft = 0;
							foreach (FashionModel model in mActiveModels.Values)
							{
								if (!model.Ready)
								{
									modelsLeft++;
								}
							}

							if (modelsLeft > wave.Models.Count)
							{
								modelsLeft = wave.Models.Count;
							}

							GameFacade.Instance.SendNotification
							(
								FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
								new ExperienceInfo
								(
									ExperienceType.NextWave,
									(uint)modelsLeft + 1
								)
							);
							keepWaiting = false;

							mNextWaveButtonPressed = false;
						}
						else if (wave.AllModelsSpawned)
						{
							bool allModelsDone = true;
							foreach (FashionModel model in mActiveModels.Values)
							{
								if (!model.Ready)
								{
									allModelsDone = false;
									break;
								}
							}

							bool allStationsFree = true;
							foreach (ModelStation station in mLevel.ModelStations)
							{
								if (station.InUse)
								{
									allStationsFree = false;
									break;
								}
							}
							keepWaiting = !(allModelsDone && allStationsFree);
						}
						return keepWaiting;
					}
				);
			}

			if (mMissedModels == 0)
			{
				GameFacade.Instance.SendNotification
				(
					FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
					new ExperienceInfo
					(
						ExperienceType.PerfectLevel
					)
				);
			}

			// This is a prediction of the same math the server should be doing and get the same result.
			int experienceFromLevel = (int)(progression.XP - startXp);
			int coinsEarned = Rewards.GetCoinsFromExperience(experienceFromLevel);

			uint entourageBonusXp = (uint)Rewards.GetEntourageExperienceBonus(experienceFromLevel, (int)progression.EntourageSize);
			int entourageBonusCoins = Rewards.GetCoinsFromExperience((int)entourageBonusXp);

			progression.EarnedXP((uint)entourageBonusXp);

			// Log for metrics
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_COMPLETE, "Level", mLevel.Name);
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_COMPLETE,
				LogGlobals.MINIGAME_TOTAL_XP, progression.XP.ToString());
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_COMPLETE,
				LogGlobals.MINIGAME_COINS, (coinsEarned + entourageBonusCoins).ToString());
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_COMPLETE,
				LogGlobals.MISSED_MODELS_IN_LEVEL, mMissedModels.ToString());
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_COMPLETE,
				LogGlobals.TIME_TO_COMPLETE_LEVEL, (DateTime.UtcNow - mLevelStartTime).ToString());
			EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVEL_COMPLETE,
				LogGlobals.UNFIXED_CLOTHING, mLevel.UnfixedClothing + "/" + mLevel.NeedsFixinTotal);
			if (progression.IsLeveledUp())
			{
				EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.LEVELED_UP_EVENT, "Level", mLevel.Name);
			}

			mLevel.Gui.CompleteLevel(startXp, entourageBonusXp, progression, coinsEarned, entourageBonusCoins, delegate()
			{
				mComplete = true;
			});
		}

		public void ModelInactive(FashionModel inactiveModel)
		{
			mActiveModels.Remove(inactiveModel.UnityGameObject);
		}

		private IEnumerator<IYieldInstruction> SpawnModels(ModelWave wave)
		{
			foreach (Pair<string, FashionModelNeeds> model in wave.Models)
			{
				yield return new YieldWhile(delegate() 
				{
					return !mModelFactory.HasInactiveModels; 
				});

				FashionModel newModel = mModelFactory.GetModel(mLevel, model.Second, model.First);

				mActiveModels.Add(newModel.UnityGameObject, newModel); 
				newModel.WalkToEndTarget();

				newModel.AddOnTargetReachedAction((new ClosureBugWorkaround(newModel, this)).ExecuteClosureHack);
				wave.ModelSpawned();

				yield return new YieldForSeconds(wave.TimeBetweenSpawns);
			}
		}

		// TODO: get rid of this class when Unity upgrades its Mono runtime (this bug was fixed like 2 years ago by Mono)
		private class ClosureBugWorkaround
		{
			private FashionModel mModel;
			private LevelGameplay mLevelGameplay;
			public ClosureBugWorkaround(FashionModel model, LevelGameplay level)
			{
				mModel = model;
				mLevelGameplay = level;
			}

			// This is the function that was an anonymous delegate, mLevelGameplay was passed implicitly at the place this function is now executed.
			public void ExecuteClosureHack()
			{
				if (!mModel.Ready)
				{
					mLevelGameplay.mMissedModels++;
					FashionGameGui gui = GameFacade.Instance.RetrieveMediator<FashionGameGui>();
					foreach (ItemId item in mModel.RequiredClothes)
					{
						gui.RemoveClothingItem(item);
					}

					EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.MODEL_COMPLETE,
						LogGlobals.CLOTHING_MISSED, mModel.RequiredClothes.Count.ToString());
					
					EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.MODEL_COMPLETE,
						LogGlobals.STATIONS_MISSED, mModel.RequiredStations.Count.ToString());
				}
				mLevelGameplay.mLevel.ModelInactive(mModel);
				mModel.SetToInactive();
				mModel.ClearNeeds();
			}
		}

		public void Dispose()
		{
			if( mFacebookFeedUpdate != null )
			{
				mFacebookFeedUpdate.Exit();
			}

			if (mSpawnWaves != null)
			{
				mSpawnWaves.Exit();
			}

			mSpawnModels.Dispose();

			List<KeyValuePair<GameObject, FashionModel>> activeModelList = new List<KeyValuePair<GameObject, FashionModel>>(mActiveModels);
			foreach (KeyValuePair<GameObject, FashionModel> activeModel in activeModelList)
			{
				if (activeModel.Value == null)
				{
					throw new Exception("There's a null activeModel.Value in the level");
				}
				activeModel.Value.SetToInactive();
			}
			mActiveModels.Clear();
		}
	}
}
