/**  --------------------------------------------------------  *
 *   LevelGui.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.FashionGame;
using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class LevelGui : IDisposable
	{
		private const string OUT_OF_ENERGY_GUI = "resources://GUI/Minigames/Fashion/OutOfEnergy.gui";
		private const string HIRED_FRIEND_MODEL_FEED_COPY = "feed_hired_model";
		private const string HIRED_FRIEND_HAIR_FEED_COPY = "feed_hired_hair";
		private const string HIRED_FRIEND_MAKEUP_FEED_COPY = "feed_hired_makeup";
		private const string HIRED_FRIEND_SEAMSTRESS_FEED_COPY = "feed_hired_seamstress";
		private const string LEVEL_UP_FEED_COPY = "feed_level_up";
		private readonly XmlDocument mLevelXml;
		private readonly FashionLevel mLevel;

		private GuiController mOutOfEnergyGui = null;
		private GuiController mFirstTimeLevelGui = null;
		private Button mStartLevelButton = null;

		private GuiController mLevelCompleteGui = null;

		private readonly IGuiManager mGuiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();

		private const string LEVEL_COMPLETE_SFX_PATH = "assets://FashionMinigame/Sounds/success_light_03.ogg";
		private AudioClip mLevelCompleteSfx;

		private const string LEVEL_UP_SFX_PATH = "assets://Sounds/highlight_08.ogg";
		private AudioClip mLevelUpSfx;

		private AudioClip mMusicClip;
		private GameObject mMusicGameObject;

		private SortedList<string, FacebookFriendInfo> mPossibleHires = new SortedList<string, FacebookFriendInfo>();
		private Textbox mUserNameFilterBox = null;
		private IGuiFrame mHireFriendPrototypeFrame = null;
		private Vector2 mFriendListingStartPosition;
		private Jobs mJobToHireFor;
		private IGuiFrame mHireFrame;
		private Label mHireFeedbackLabel;

		/// <summary>
		/// True if any Dialog (Out of Energy, First Time Level, Level Complete) is showing.
		/// </summary>
		public bool DialogShowing
		{
			get
			{
				return !(mOutOfEnergyGui == null || !mOutOfEnergyGui.MainGui.Showing ||
						mFirstTimeLevelGui == null || !mFirstTimeLevelGui.MainGui.Showing ||
						mLevelCompleteGui == null || !mLevelCompleteGui.MainGui.Showing);
			}
		}
		
		public LevelGui(XmlDocument levelXml, FashionLevel level)
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

			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			assetRepo.LoadAssetFromPath<SoundAsset>(LEVEL_COMPLETE_SFX_PATH, delegate(SoundAsset asset)
			{
				mLevelCompleteSfx = asset.AudioClip;
			});

			assetRepo.LoadAssetFromPath<SoundAsset>(LEVEL_UP_SFX_PATH, delegate(SoundAsset asset)
			{
				mLevelUpSfx = asset.AudioClip;
			});
		}

		public void StartLevel(bool firstTimePlayed, bool needsToHire, Hangout.Shared.Action onStartLevel)
		{
			if (needsToHire || firstTimePlayed)
			{
				BuildFirstTimeHireGui(onStartLevel);
			}
			else
			{
				BuildReplayLevelGui(onStartLevel);
			}

			Label energyRequirementLabel = mFirstTimeLevelGui.MainGui.SelectSingleElement<Label>("**/EnergyRequirement");
			energyRequirementLabel.Text = String.Format(energyRequirementLabel.Text, mLevel.RequiredEnergy);

			string musicPath = mLevelXml.SelectSingleNode("Level/Music/@path").InnerText;
			ClientAssetRepository assetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			assetRepo.LoadAssetFromPath<SoundAsset>(musicPath, delegate(SoundAsset asset)
			{
				mMusicClip = asset.AudioClip;
				mMusicGameObject = new GameObject("music game object");
				AudioSource audioSource = (AudioSource)mMusicGameObject.AddComponent(typeof(AudioSource));
				audioSource.clip = mMusicClip;
				audioSource.loop = true;
				audioSource.volume = 0.9f;
				audioSource.Play();
			});
		}

		public void SetWave(ModelWave wave, int waveNumber, int waveCount)
		{
			bool lastWave = (waveNumber == waveCount);
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().SetWave(waveNumber, waveCount);
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().EnableNextWave(!lastWave);
		}

		public void ShareLevelUp(string levelName)
		{
			GameFacade.Instance.RetrieveMediator<FacebookFeedMediator>().PostFeed
			(
                null,
				FashionMinigame.FACEBOOK_FEED_COPY_PATH,
				LEVEL_UP_FEED_COPY,
				delegate() { },
				levelName
			);
			// TODO: Add mixpanel funnel metrics?
		}

		public void CompleteLevel(uint startXp, uint entourageBonusXp, PlayerProgression progression, int coinEarned, int entourageBonusCoins, Hangout.Shared.Action onNextLevelPressed)
		{
			IGuiManager manager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			string levelCompleteGuiPath = mLevelXml.SelectSingleNode("Level/LevelCompleteGui/@path").InnerText;
			mLevelCompleteGui = new GuiController(manager, levelCompleteGuiPath);

			// If we leveled up, show the complete frame, otherwise show the replay frame
			GuiFrame levelCompleteFrame = mLevelCompleteGui.MainGui.SelectSingleElement<GuiFrame>("**/LevelCompleteFrame");
			GuiFrame levelReplayFrame = mLevelCompleteGui.MainGui.SelectSingleElement<GuiFrame>("**/LevelReplayFrame");
			GuiFrame activeFrame = null;
			
			// How much total we have now
			uint currentXP = progression.XP;
			
			// How much we just earned
			uint earnedXP = (currentXP - startXp);
			
			// How much total we need to get to the next level
			uint nextLevelXP = progression.GetNextLevelXP();
									
			// How much we need to finish this level
			uint pointsToNextLevel = nextLevelXP - currentXP;

			progression.SaveExperienceToServer(earnedXP, progression.IsLeveledUp());

			if (progression.IsLeveledUp())
			{
				// DID level up
				if (mLevelUpSfx != null)
				{
					GameObject mainCamera = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>().MainCamera;
					AudioSource.PlayClipAtPoint(mLevelUpSfx, mainCamera.transform.position, 0.5f);
				}

				levelCompleteFrame.Showing = true;
				levelReplayFrame.Showing = false;
				activeFrame = levelCompleteFrame;

				// Facebook Share button
				Button shareButton = activeFrame.SelectSingleElement<Button>("**/ShareButton");
				shareButton.AddOnPressedAction(delegate()
				{
					// Now that they've shared, disable the button
					shareButton.Disable();
					ShareLevelUp(mLevel.Name);
				});
			} 
			else
			{
				// Did NOT level up
				if (mLevelCompleteSfx != null)
				{
					GameObject mainCamera = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>().MainCamera;
					AudioSource.PlayClipAtPoint(mLevelCompleteSfx, mainCamera.transform.position, 0.5f);
				}
				levelCompleteFrame.Showing = false;
				levelReplayFrame.Showing = true;
				activeFrame = levelReplayFrame;
				Label levelDetailsLabel = activeFrame.SelectSingleElement<Label>("**/LevelDetails"); 
				levelDetailsLabel.Text = String.Format(levelDetailsLabel.Text, pointsToNextLevel.ToString());
			}

			// Update coins
			Label coinLabel = activeFrame.SelectSingleElement<Label>("**/CoinsEarnedLabel");
			coinLabel.Text = String.Format(coinLabel.Text, coinEarned.ToString());

			// Entourage bonus
			Label bonusCoinsLabel = activeFrame.SelectSingleElement<Label>("**/EntourageBonusCoinLabel");
			bonusCoinsLabel.Text = String.Format(bonusCoinsLabel.Text, entourageBonusCoins.ToString());
			Label bonusXpLabel = activeFrame.SelectSingleElement<Label>("**/EntourageBonusXpLabel");
			bonusXpLabel.Text = String.Format(bonusXpLabel.Text, entourageBonusXp.ToString());

			Button inviteFriendsButton = mLevelCompleteGui.MainGui.SelectSingleElement<Button>("**/InviteFriendsButton");
			inviteFriendsButton.Text = Translation.ENTOURAGE_INVITE_FRIENDS_BUTTON_TEXT;
			inviteFriendsButton.AddOnPressedAction(delegate()
			{
				InviteFriendsToEntourage();
			});

			// Next level button
			Button nextLevelButton = mLevelCompleteGui.MainGui.SelectSingleElement<Button>("**/NextLevelButton");
			nextLevelButton.AddOnPressedAction(onNextLevelPressed);
			nextLevelButton.AddOnPressedAction(CleanupLevelCompleteGui);

			Button exitToRunwayButton = mLevelCompleteGui.MainGui.SelectSingleElement<Button>("**/ExitToRunwayButton");
			if (exitToRunwayButton != null)
			{
				exitToRunwayButton.AddOnPressedAction(delegate()
				{
					FashionMinigame.GoToRunway();
				});
			}
		}

		private bool mFriendHired = false;
		private void BuildFirstTimeHireGui(Hangout.Shared.Action onStartLevel)
		{
			XmlNode guiPathNode = mLevelXml.SelectSingleNode("Level/FirstTimeLevelGui/@path");
			if (guiPathNode == null)
			{
				throw new Exception("Level (" + mLevel.Name + ") has no FirstTimeLevelGui/@path node");
			}
			IGuiManager manager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mFirstTimeLevelGui = new GuiController(manager, guiPathNode.InnerText);

			mFirstTimeLevelGui.MainGui.AddOnCloseAction(delegate()
			{
				mFriendHired = false;
			});

			HandleHireGui(onStartLevel);
		}

		private void BuildReplayLevelGui(Hangout.Shared.Action onStartLevel)
		{
			XmlNode guiPathNode = mLevelXml.SelectSingleNode("Level/ReplayLevelGui/@path");
			if (guiPathNode == null)
			{
				throw new Exception("Level (" + mLevel.Name + ") has no FirstTimeLevelGui/@path node");
			}
			IGuiManager manager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mFirstTimeLevelGui = new GuiController(manager, guiPathNode.InnerText);

			mStartLevelButton = mFirstTimeLevelGui.MainGui.SelectSingleElement<Button>("**/StartLevelButton");
			mStartLevelButton.AddOnPressedAction(onStartLevel);
			mStartLevelButton.AddOnPressedAction(mFirstTimeLevelGui.Dispose);
		}

		public bool HiresNewAvatar
		{
			get
			{
				bool result = false;
				if( mFirstTimeLevelGui != null )
				{
					result = mFirstTimeLevelGui.MainGui.SelectSingleElement<IGuiElement>("**/HireModel") != null ||
							 mFirstTimeLevelGui.MainGui.SelectSingleElement<IGuiElement>("**/HireMakeup") != null ||
							 mFirstTimeLevelGui.MainGui.SelectSingleElement<IGuiElement>("**/HireHair") != null ||
							 mFirstTimeLevelGui.MainGui.SelectSingleElement<IGuiElement>("**/HireSeamstress") != null;
				}
				return result;
			}
		}

		private void HandleHireGui(Hangout.Shared.Action onStartLevel)
		{
			IGuiFrame hireFrame = mFirstTimeLevelGui.MainGui.SelectSingleElement<IGuiFrame>("**/HireList");
			mStartLevelButton = mFirstTimeLevelGui.MainGui.SelectSingleElement<Button>("**/StartLevelButton");

			if (hireFrame != null)
			{
				string query = "Level/FirstTimeLevelGui/@hires";
				XmlNode hireForJob = mLevelXml.SelectSingleNode(query);
				if( hireForJob == null )
				{
					throw new Exception(mLevel.Name + " has a GUI that contains a hire friend dialog, but that level doesn't have any hire information at " + query);
				}

                // Add mixpanel funnel metrics
                FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_FRIEND_HIRE, FunnelGlobals.HIRE_FRIEND_POPUP, "{\"level\":\"" + mLevel.Name + "\"}");

				SetupHireFriendFrame(hireFrame, (Jobs)Enum.Parse(typeof(Jobs), hireForJob.InnerText));
			
				mStartLevelButton.AddOnPressedAction(delegate()
				{
					if( mFriendHired )
					{
						onStartLevel();
						CleanupFirstTimeGui();

                        // Add mixpanel funnel metrics
                        FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_FRIEND_HIRE, FunnelGlobals.CLICKED_PLAY, "{\"level\":\"" + mLevel.Name + "\"}");
					}
					else
					{
						throw new Exception("Need to Hire a friend first");
					}
				});
			}
			else
			{
				mStartLevelButton.AddOnPressedAction(onStartLevel);
				mStartLevelButton.AddOnPressedAction(CleanupFirstTimeGui);
			}
		}

		private void SetupHireFriendFrame(IGuiFrame hireFrame, Jobs job)
		{
			mStartLevelButton.Disable();

			mHireFrame = hireFrame;
			mJobToHireFor = job;
			mUserNameFilterBox = hireFrame.GetContainer<ITopLevel>().SelectSingleElement<Textbox>("**/FriendSearchBox");
			// Hide the feedback label until after the hiring is complete.
			mHireFeedbackLabel = hireFrame.GetContainer<ITopLevel>().SelectSingleElement<Label>("**/HireFeedbackLabel");
			mHireFeedbackLabel.Showing = false;
			if( mUserNameFilterBox != null )
			{
				mUserNameFilterBox.AddTextChangedCallback(LayoutFriends);
			}

			mFriendHired = false;
			mHireFriendPrototypeFrame = hireFrame.SelectSingleElement<IGuiFrame>("HireFriendPrototypeFrame");
			if (mHireFriendPrototypeFrame == null )
			{
				throw new Exception("No HireFriendPrototypeFrame was found in the level GUI for this level");
			}
			mFriendListingStartPosition = hireFrame.GetChildPosition(mHireFriendPrototypeFrame);
			hireFrame.RemoveChildWidget(mHireFriendPrototypeFrame);

			GetFriendsToHire(job, delegate(IDictionary<long, FacebookFriendInfo> possibleHires)
			{
				// SortedList doesn't support multiple keys with the same value, so to support 
				// friends that have the same name, we need to make the last name fields unique
				int uniqueifyingKeySuffix = 0;

				foreach (KeyValuePair<long, FacebookFriendInfo> possibleHire in possibleHires)
				{
					mPossibleHires.Add(possibleHire.Value.FirstName + possibleHire.Value.LastName + uniqueifyingKeySuffix++, possibleHire.Value);
				}

				LayoutFriends();

				Scrollbar scrollbar = ((IGuiContainer)hireFrame.Parent).SelectSingleElement<Scrollbar>("ScrollBar");
				if (scrollbar != null)
				{
					scrollbar.Percent = 0.0f;
				}
			});
		}

		private void GetFriendsToHire(Jobs job, Action<IDictionary<long, FacebookFriendInfo>> result)
		{
			FashionGameCommands.GetFriendsToHire(job, delegate(Message msg)
			{
				if (msg.Data.Count == 0)
				{
					throw new Exception("Malformed FriendsToHire Message, Data[0] must be a Dictionary<long,FacebookFriendInfo>");
				}

				IDictionary<long, FacebookFriendInfo> possibleHires = null;

				// There is a compatibility bug serializing an empty dict between .NET and mono, 
				// so if the dict is empty we just pass down false.
				if ((msg.Data[0] is bool) && ((bool)msg.Data[0] == false))
				{
					possibleHires = new Dictionary<long, FacebookFriendInfo>();
				}
				else
				{
					possibleHires = (IDictionary<long, FacebookFriendInfo>)msg.Data[0];
				}

				result(possibleHires);
			});
		}

		private void LayoutFriends()
		{
			string filter = "";
			if( mUserNameFilterBox != null )
			{
				filter = mUserNameFilterBox.Text;
			}

			ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			Vector2 nextGuiPosition = mFriendListingStartPosition;
			mHireFrame.ClearChildWidgets();

			List<Regex> searchFilters = new List<Regex>();
			foreach(string filterSplit in filter.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries))
			{
				searchFilters.Add(new Regex(filterSplit, RegexOptions.IgnoreCase | RegexOptions.Compiled));
			}

			foreach (FacebookFriendInfo facebookFriend in mPossibleHires.Values)
            {
				bool matches = true;
				foreach(Regex rx in searchFilters)
				{
					if( !rx.IsMatch(facebookFriend.FirstName) && !rx.IsMatch(facebookFriend.LastName) )
					{
						matches = false;
						break;
					}
				}

				if (matches)
				{
					IGuiFrame friendListing = (IGuiFrame)mHireFriendPrototypeFrame.Clone();
					(friendListing.SelectSingleElement<ITextGuiElement>("FriendNameLabel")).Text = facebookFriend.FirstName + " " + facebookFriend.LastName;

					Button hireFriendButton = friendListing.SelectSingleElement<Button>("HireFriendButton");
					Image friendListingImage = friendListing.SelectSingleElement<Image>("FriendImage");
					if (facebookFriend.ImageUrl != "")
					{
						clientAssetRepository.LoadAssetFromPath<ImageAsset>
						(
							facebookFriend.ImageUrl,
							delegate(ImageAsset image)
							{
								friendListingImage.Texture = image.Texture2D;
							}
						);
					}
					hireFriendButton.AddOnPressedAction(new HireFriendClosure(this, friendListing, mJobToHireFor, facebookFriend).ExecuteClosure);

					mHireFrame.AddChildWidget(friendListing, new FixedPosition(nextGuiPosition));
					nextGuiPosition.y += friendListing.ExternalSize.y + mFriendListingStartPosition.y;
				}
            }
		}

		private class HireFriendClosure
		{
			private LevelGui mThisRef;
			private Jobs mJob;
			private FacebookFriendInfo mFriendInfo;

			public HireFriendClosure(LevelGui thisRef, IGuiFrame friendListing, Jobs job, FacebookFriendInfo friendInfo)
			{
				mThisRef = thisRef;
				mJob = job;
				mFriendInfo = friendInfo;
			}

			public void ExecuteClosure()
			{
				if (!mThisRef.mFriendHired)
				{
					mThisRef.mFriendHired = true;

					// Cleanup the hire GUI to up framerate through the rest of this function
					IGuiFrame hireParent = (IGuiFrame)mThisRef.mHireFrame.Parent;
					((IGuiFrame)hireParent.Parent).RemoveChildWidget(hireParent);
					hireParent = null;
					mThisRef.mHireFrame = null;

					mThisRef.mHireFeedbackLabel.Showing = true;
					mThisRef.mHireFeedbackLabel.Text = "Hiring " + mFriendInfo.FirstName + " " + mFriendInfo.LastName + ". Please wait...";

					FashionNpcMediator npcFactory = GameFacade.Instance.RetrieveMediator<FashionNpcMediator>();
					FashionGameCommands.HireFriend(mFriendInfo.FbAccountId, mJob, delegate(Message message)
					{
						npcFactory.HiredFriend(message, mThisRef.mLevel);
					});

					string feedType = "";
					switch (mJob)
					{
						case Jobs.Model:
							feedType = HIRED_FRIEND_MODEL_FEED_COPY;
							break;
						case Jobs.Hair:
							feedType = HIRED_FRIEND_HAIR_FEED_COPY;
							break;
						case Jobs.Makeup:
							feedType = HIRED_FRIEND_MAKEUP_FEED_COPY;
							break;
						case Jobs.Seamstress:
							feedType = HIRED_FRIEND_SEAMSTRESS_FEED_COPY;
							break;
					}

					GameFacade.Instance.RetrieveMediator<FacebookFeedMediator>().PostFeed
					(
                        mFriendInfo.FbAccountId,
						FashionMinigame.FACEBOOK_FEED_COPY_PATH,
						feedType,
						delegate(){},
						mFriendInfo.FirstName + " " + mFriendInfo.LastName
					);
					
                    // Add mixpanel funnel metrics
                    FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_FRIEND_HIRE, FunnelGlobals.CLICKED_HIRE, "{\"level\":\"" + mThisRef.mLevel.Name + "\"}");
				}
			}
		}

		public void HiredFriendLoaded()
		{
			mHireFeedbackLabel.Text = "Hiring complete! Press Play to continue your fashion show.";
			mStartLevelButton.Enable();
		}

		private void SelectFrame(IGuiFrame friendListing)
		{
			GuiPath selectEverything = new GuiPath("**/*");
			foreach (Button button in selectEverything.SelectElements<Button>((IGuiContainer)friendListing.Parent))
			{
				button.Disable();
			}

			IGuiStyle buttonStyle = new GuiStyle(mGuiManager.GetDefaultStyle(typeof(Button)), "ButtonStyle");
			IGuiStyle frameStyle = new GuiStyle(friendListing.Style, "FrameStyle");
			foreach( IGuiStyle style in mFirstTimeLevelGui.AllStyles )
			{
			    if (style.Name == "SecondaryButtonStyle")
			    {
			        buttonStyle = new GuiStyle(style, "ButtonStyle");
			    }
			    else if( style.Name == "SelectedFrameStyle" )
			    {
			        frameStyle = new GuiStyle(style, "FrameStyle");
			    }
			}

			foreach (Button button in selectEverything.SelectElements<Button>(friendListing))
			{
			    button.Style = buttonStyle;
			    button.Enable();
			}
			friendListing.Style = frameStyle;
		}

		public void BuildTopGui(Hangout.Shared.Action onNextPressed)
		{
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().SetLevel(mLevel.Name);
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().SetNextWaveAction(onNextPressed);
			GameFacade.Instance.RetrieveMediator<FashionGameGui>().EnableNextWave(false);
		}

		public void ShowOutOfEnergyGui()
		{
			mLevel.SendNotification(GameFacade.HIDE_POPUP);

			mOutOfEnergyGui = new GuiController(mGuiManager, OUT_OF_ENERGY_GUI);
			Button leaveGameButton = mOutOfEnergyGui.MainGui.SelectSingleElement<Button>("**/LeaveGameButton");
			leaveGameButton.AddOnPressedAction(FashionMinigame.Exit);

			Button nextLevelButton = mOutOfEnergyGui.MainGui.SelectSingleElement<Button>("**/NextLevelButton"); // refill energy

            nextLevelButton.AddOnPressedAction(GetMoreEnergyPressed);
		}

        private void GetMoreEnergyPressed()
		{
			mOutOfEnergyGui.Dispose();

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.GetStoreInventory
			(
				InventoryGlobals.ENERGY_STORE_CASH,
				ItemType.ENERGY_REFILL,
				0,
				1,
				delegate(Message buyEnergyResponseMessage)
				{
					XmlDocument xml = new XmlDocument();
					xml.LoadXml((string)buyEnergyResponseMessage.Data[0]);

					List<object> args = new List<object>();
					args.Add(FashionGameTranslation.BUY_ENERGY);
					args.Add
					(
						String.Format
						(
							FashionGameTranslation.REFILL_ENERGY_MESSAGE, 
							(int)float.Parse(xml.SelectSingleNode // stupid floor action
							(
								"Response/store/itemOffers/itemOffer/price/money/@amount"
							).InnerText)
						)
					);
					Hangout.Shared.Action okcb = delegate()
					{
						BuyEnergy(xml.SelectSingleNode("Response/store/itemOffers/itemOffer/@id").InnerText);
					};
					Hangout.Shared.Action cancelcb = ShowOutOfEnergyGui;
					args.Add(okcb);
					args.Add(cancelcb);
					args.Add(FashionGameTranslation.BUY);
					args.Add(FashionGameTranslation.CANCEL);

					GameFacade.Instance.SendNotification(GameFacade.SHOW_CONFIRM, args);
				}
			);
        }

		private void BuyEnergy(string itemOffer)
		{
			mLevel.SendNotification(GameFacade.HIDE_POPUP);

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.PurchaseRequest(itemOffer, InventoryGlobals.CASH, delegate(Message message)
			{
				XmlDocument xmlResponse = new XmlDocument();
				xmlResponse.LoadXml((string)message.Data[0]);

				inventoryProxy.HidePurchaseModel();
				XmlNode errorCode = xmlResponse.SelectSingleNode("Response/errors/error/@code");
				if (errorCode == null)
				{
					List<object> args = new List<object>();
					args.Add(FashionGameTranslation.SUCCESS);
					args.Add(FashionGameTranslation.PURCHASE_COMPLETE);
					Hangout.Shared.Action okcb = delegate()
					{
						mLevel.SendNotification(GameFacade.HIDE_POPUP);
						FashionGameGui gui = GameFacade.Instance.RetrieveMediator<FashionGameGui>();
						gui.EnergyRefilled();
						mLevel.Gameplay.StartWaves();
					};
					args.Add(okcb);
					args.Add(FashionGameTranslation.PLAY_LEVEL);
					GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);
				}
				else if (int.Parse(errorCode.InnerText) == 270007) // Attempt to go below account minimum balance.
				{
					// Leave the minigame completely and go to the webpage for buying more cash
					BuyCoinUtility.GoToBuyCashPage
					(
						FashionGameTranslation.NOT_ENOUGH_CASH_TITLE, 
						FashionGameTranslation.NOT_ENOUGH_CASH_MESSAGE,
						delegate(string jsResult)
						{
							ShowOutOfEnergyGui();
						},
						ShowOutOfEnergyGui
					);
				}
			});
		}

		private void CleanupFirstTimeGui()
		{
			if( mFirstTimeLevelGui != null )
			{
				mFirstTimeLevelGui.Dispose();
				mFirstTimeLevelGui = null;
			}
		}

		private void CleanupLevelCompleteGui()
		{
			if (mLevelCompleteGui != null)
			{
				mLevelCompleteGui.Dispose();
				mLevelCompleteGui = null;
			}
		}

		private void InviteFriendsToEntourage()
		{
			JSDispatcher jsd = new JSDispatcher();
			string inviteCampaign = "invite_entourage_fc_level_gui";
			jsd.RequestFriendInviter
			(
				inviteCampaign,
				delegate(string jsResponse)
				{
					EventLogger.Log(LogGlobals.CATEGORY_FACEBOOK, LogGlobals.FACEBOOK_INVITE, inviteCampaign, jsResponse);
					Console.Log("Invite to entourage result = " + jsResponse);
				}
			);
		}

		public void Dispose()
		{
			CleanupFirstTimeGui();
			CleanupLevelCompleteGui();

			if (mOutOfEnergyGui != null)
			{
				mOutOfEnergyGui.Dispose();
				mOutOfEnergyGui = null;
			}

			if( mMusicGameObject )
			{
				GameObject.Destroy(mMusicGameObject);
			}
		}
	}
}
