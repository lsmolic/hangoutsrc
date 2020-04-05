/**  --------------------------------------------------------  *
 *   PlayerProgression.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/19/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Client.Gui;
using Hangout.Shared;

using PureMVC.Patterns;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class PlayerProgression : Mediator
	{
		private class LevelInfo
		{
			private readonly string mLevelDataPath;
			public string LevelDataPath
			{
				get { return mLevelDataPath; }
			}

			private readonly float mEnergy;
			public float Energy
			{
				get { return mEnergy; }
			}

			public LevelInfo(string levelDataPath, float energy)
			{
				mLevelDataPath = levelDataPath;
				mEnergy = energy;
			}
		}

		private class ExperienceData
		{
			private readonly uint mExperienceAmount;
			public uint ExperienceAmount
			{
				get { return mExperienceAmount; }
			}

			private readonly string mDescription;
			public string Description
			{
				get { return mDescription; }
			}

			public ExperienceData(uint experienceAmount, string description)
			{
				mExperienceAmount = experienceAmount;
				mDescription = description;
			}
		}

		private const string PROGRESSION_SETTINGS_PATH = "resources://Fashion Game Settings/Progression";
		private const string EXPERIENCE_REWARDS_PATH = "resources://Fashion Game Settings/ExperienceRewards";

		private const string LAST_LEVEL_DATA_KEY = "LastLevelStarted";
		private readonly SortedList<uint, LevelInfo> mExperienceToLevel = new SortedList<uint, LevelInfo>();

		private uint mXP = 0;
		public uint XP
		{
			get { return mXP; }
		}

		private const string LEVEL_UP_SFX_PATH = "assets://Sounds/highlight_08.ogg";
		private AudioClip mLevelUpSfx;

		private readonly Dictionary<ExperienceType, ExperienceData> mExperiencePerType = new Dictionary<ExperienceType, ExperienceData>();

		private readonly uint mEntourageSize;
		public uint EntourageSize
		{
			get { return mEntourageSize; }
		}

		public PlayerProgression(uint initialPlayerExperience, uint entourageSize)
		{
			mXP = initialPlayerExperience;
			mEntourageSize = entourageSize;

			XmlDocument progressionSettingsXml = XmlUtility.LoadXmlDocument(PROGRESSION_SETTINGS_PATH);
			
			foreach( XmlNode levelNode in progressionSettingsXml.SelectNodes("//PlayerLevel") )
			{
				mExperienceToLevel.Add
				(
					uint.Parse(levelNode.Attributes["xp"].InnerText), 
					new LevelInfo
					(
						levelNode.Attributes["level"].InnerText, 
						float.Parse(levelNode.Attributes["energy"].InnerText)
					)
				);
			}

			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			assetRepo.LoadAssetFromPath<SoundAsset>(LEVEL_UP_SFX_PATH, delegate(SoundAsset asset)
			{
				mLevelUpSfx = asset.AudioClip;
			});

			ParseExperienceRewards();
		}

		private void ParseExperienceRewards()
		{
			XmlDocument doc = XmlUtility.LoadXmlDocument(EXPERIENCE_REWARDS_PATH);
			foreach(XmlNode experienceNode in doc.SelectNodes("ExperienceRewards/*"))
			{
				string description = "";

				XmlNode descriptionAttribute = experienceNode.SelectSingleNode("@description");
				if( descriptionAttribute != null )
				{
					description = descriptionAttribute.InnerText;
				}

				mExperiencePerType.Add
				(
					(ExperienceType)Enum.Parse(typeof(ExperienceType), experienceNode.Name), 
					new ExperienceData
					(
						uint.Parse(experienceNode.Attributes["value"].InnerText),
						description
					)
				);
			}
		}

		private uint mXpForCoins = 0;
		private bool mLeveledSoundPlayed = false;
		private bool mLeveledThisLevel = false;

		public void EarnedXP(uint points)
		{
			// Check if these points will cause you to level up
			bool levelUp = WillLevelUp(points);
			// Play a sfx if we leveled up.
			if (levelUp)
			{
				if (mLevelUpSfx != null && !mLeveledSoundPlayed)
				{
					mLeveledSoundPlayed = true;
					GameObject mainCamera = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>().MainCamera;
					AudioSource.PlayClipAtPoint(mLevelUpSfx, mainCamera.transform.position, 0.5f);
				}
				// Only do this once or you might keep skipping levels every time this is called
				if (!mLeveledThisLevel)
				{
					// Set your XP to the cap of this level
					mXP = GetNextLevelXP();
					mLeveledThisLevel = true;
				}

				// Still count this XP towards your coins
				mXpForCoins += points;
			}
			else
			{
				mXP += points;
			}
			UpdateProgressGUI(levelUp);
		}

		public override IList<string> ListNotificationInterests()
		{
			List<string> result = new List<string>(base.ListNotificationInterests());
			result.Add(FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION);
			return result;
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			switch (notification.Name)
			{
				case FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION:
					if( !(notification.Body is ExperienceInfo) )
					{
						throw new Exception("EARNED_EXPERIENCE_NOTIFICATION expected an ExperienceInfo object as the body of the notification. Found " + notification.Body.ToString());
					}
					HandleExperienceGain((ExperienceInfo)notification.Body);
					break;
				default:
					base.HandleNotification(notification);
					break;
			}
		}

		private void HandleExperienceGain(ExperienceInfo experience)
		{
			uint points = 0;
			string description = "";

			switch(experience.Type)
			{
				case ExperienceType.PerfectLevel:
					points = GameFacade.Instance.RetrieveMediator<FashionLevel>().PerfectCompleteBonus;
					description = FashionGameTranslation.PERFECT_LEVEL;
					break;
				default:
					ExperienceData experienceForType = mExperiencePerType[experience.Type];
					if( experience.MultipleEvent != null )
					{
						points = ((uint)experience.MultipleEvent) * experienceForType.ExperienceAmount;
					}
					else
					{
						points = experienceForType.ExperienceAmount;
					}
					description = experienceForType.Description;
					break;
			}

			Vector3 windowPosition;
			if( experience.Position != null )
			{
				windowPosition = (Vector3)experience.Position;
			}
			else
			{
				FashionCameraMediator fashionCamera = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>();
				windowPosition = fashionCamera.Camera.ViewportPointToRay(Vector3.one * 0.5f).GetPoint(1.0f);
			}

			EarnedXP(points);

			// TODO: Hard coded value
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().SpawnFloatingWindow(windowPosition, points, description, new Color(0.69f, 0.79f, 0.36f));
		}

		public void UpdateProgressGUI(bool levelUp)
		{
			// Compute how much xp the last level took
			uint previousLevelXP = GetPreviousLevelXP();
			// How much xp have we earned on this level alone?
			uint thisLevelXP = mXP - previousLevelXP;
			// How much xp do we need for this level alone?
			uint thisLevelXPNeeded = GetNextLevelXP() - previousLevelXP;
			// Now set the progress by float dividing the two so we
			// show progress through this specific level
			// Debug.Log(mXP.ToString() + " " + previousLevelXP.ToString() + " " + thisLevelXP.ToString() + " " + thisLevelXPNeeded.ToString() + " " + GetNextLevelXP().ToString());
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().SetExperience(thisLevelXP, thisLevelXPNeeded, levelUp);
		}

		public void SaveExperienceToServer(uint xpEarnedOnThisLevel, bool leveledUp)
		{
			FashionGameCommands.SetLevelComplete(mXP, xpEarnedOnThisLevel + mXpForCoins, leveledUp, delegate(Message message)
			{
				//string coinsEarned = (string)message.Data[0];
				string totalCoins = (string)message.Data[1];
				SendNotification(GameFacade.RECV_USER_BALANCE, new string[] { totalCoins, "" });
			});

			mXpForCoins = 0;

			if (leveledUp)
			{
				FashionGameGui gui = GameFacade.Instance.RetrieveMediator<FashionGameGui>();
				gui.LeveledUp();
			}
        }

		public void GetNextLevel(Action<FashionLevel> fashionLevelResult)
		{
			// Make the first level that this player qualifies for
			int levelNumber = 0;
			
			FashionGameCommands.GetPlayerData(LAST_LEVEL_DATA_KEY, delegate(Message message)
			{
                //Console.WriteLine(Time.realtimeSinceStartup.ToString("f2") + "> Received Player Data in PlayerProgression");
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(message.Data[0].ToString());
				int lastLevel = 0;
				XmlNode levelDataNode = xml.SelectSingleNode("//DataKey[@KeyName='" + LAST_LEVEL_DATA_KEY + "']");
				if( levelDataNode != null )
				{
					int.TryParse(levelDataNode.InnerText, out lastLevel);
				}

				KeyValuePair<uint, LevelInfo> nextLevel = default(KeyValuePair<uint, LevelInfo>);
				foreach (KeyValuePair<uint, LevelInfo> experienceLevel in mExperienceToLevel)
				{
					levelNumber++;
					nextLevel = experienceLevel;

					if( experienceLevel.Key > mXP )
					{
						break;
					}
				}

				bool firstTimePlayed = levelNumber > lastLevel;

				//Load Level Xml
				XmlDocument levelXml = XmlUtility.LoadXmlDocument(nextLevel.Value.LevelDataPath);
				XmlNode feedCopyNode = levelXml.SelectSingleNode("Level/FeedOnFirstTimePlayed/@feedCopyName");

				if (firstTimePlayed && feedCopyNode != null)
				{
					GameFacade.Instance.RetrieveMediator<FacebookFeedMediator>().PostFeed
					(
                        null,
						FashionMinigame.FACEBOOK_FEED_COPY_PATH,
						feedCopyNode.InnerText,
						delegate(){},
						""
					);
				}
				else
				{
					mLevelStartXp = mXP;

					EarnedXP(0); // Cause the XP bar to update
					GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine
					(
						BuildLevel
						(
							levelXml,
							nextLevel.Value,
							firstTimePlayed,
							fashionLevelResult
						)
					);

					FashionGameCommands.SetServerData(LAST_LEVEL_DATA_KEY, levelNumber.ToString());

					//Console.WriteLine(Time.realtimeSinceStartup.ToString("f2") + "> Received Next Level Completed");
				}
			});
		}

		private IEnumerator<IYieldInstruction> BuildLevel
		(
			XmlDocument levelXml, 
			LevelInfo levelInfo, 
			bool firstTimePlayed, 
			Action<FashionLevel> fashionLevelResult
		)
		{
            //Console.WriteLine(Time.realtimeSinceStartup.ToString("f2") + "> Building Level");
			
			// Start loading the clothing assets
			ClothingMediator clothingMediator = GameFacade.Instance.RetrieveMediator<ClothingMediator>();
			clothingMediator.LoadClothing
			(
				Functionals.Map<ItemId>
				(
					delegate(object xmlNode)
					{
						return new ItemId(XmlUtility.GetUintAttribute((XmlNode)xmlNode, "id"));
					},
					levelXml.SelectNodes("Level/Clothing/ClothingItem")
				),
				levelInfo.LevelDataPath
			);
			
			yield return new YieldWhile(delegate()
			{
				return !clothingMediator.ClothingLoaded;
			});

			FashionNpcMediator fashionModelFactory = GameFacade.Instance.RetrieveMediator<FashionNpcMediator>();

			// Clear this value
			mLeveledThisLevel = false;
			
			// Make the first level that this player qualifies for
			FashionLevel result = new FashionLevel(levelInfo.LevelDataPath, fashionModelFactory, firstTimePlayed, levelInfo.Energy);

			fashionLevelResult(result);
            //Console.WriteLine(Time.realtimeSinceStartup.ToString("f2") + "> Build Level Completed");
		}

		private uint mLevelStartXp = 0;
		public bool IsLeveledUp()
		{
			// Special case for first level
			if ((mLevelStartXp == 0) && (mXP != 0))
			{
				return true;
			}
			// If your current xp level is greater than starting xp, you must have leveled up
			return (GetNextLevelXP(mXP) > GetNextLevelXP(mLevelStartXp));
		}

		private bool WillLevelUp(uint additionalXP)
		{
			// Special case for first level
			if ((mLevelStartXp == 0) && (mXP != 0))
			{
				return true;
			}
			// If your current xp level is greater than starting xp, you must have leveled up
			return (GetNextLevelXP(mXP + additionalXP) > GetNextLevelXP(mLevelStartXp));
		}

		public uint GetNextLevelXP()
		{
			// Use the class member variable by default
			return GetNextLevelXP(mXP);
		}

		public uint GetNextLevelXP(uint currentXP)
		{
			// Return the total XP required at the next level
			foreach (KeyValuePair<uint, LevelInfo> experienceLevel in mExperienceToLevel)
			{
				if (experienceLevel.Key > currentXP)
				{
					return experienceLevel.Key;
				}
			}

			return currentXP;
		}

		public uint GetPreviousLevelXP()
		{
			// Use the class member variable by default
			return GetPreviousLevelXP(mXP);
		}

		public uint GetPreviousLevelXP(uint currentXP)
		{
			uint previousXP = 0;
			// Return the total XP required at the next level
			foreach (KeyValuePair<uint, LevelInfo> experienceLevel in mExperienceToLevel)
			{
				if (experienceLevel.Key > currentXP)
				{
					return previousXP;
				} 
				else 
				{
					previousXP = experienceLevel.Key;
				}
			}

			return currentXP;
		}
	}
}
