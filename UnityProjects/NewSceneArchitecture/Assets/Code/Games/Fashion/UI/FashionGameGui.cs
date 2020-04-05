/**  --------------------------------------------------------  *
 *   FashionGameGui.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Shared;
using Hangout.Shared.FashionGame;
using Hangout.Client;
using Hangout.Client.Gui;

using UnityEngine;

using PureMVC.Interfaces;

namespace Hangout.Client.FashionGame
{
	public class FashionGameGui : GuiController, IMediator
	{
		public const string FASHION_GUI_PATH = "resources://GUI/Minigames/Fashion/Fashion.gui";
		private const float ENERGY_UPDATE_RATE = 1.0f;

		private readonly FashionGameInput mInput;
		private readonly IScheduler mScheduler;

		private readonly Window mMainWindow;
		private readonly Window mScoreWindow;
		private readonly IGuiFrame mMainFrame;
		private readonly PushButton mClothingButtonPrototype;
		private readonly List<Pair<ClothingItem, PushButton>> mClothingButtonPrototypes = new List<Pair<ClothingItem, PushButton>>();
		private readonly List<List<Pair<ClothingItem, PushButton>>> mActiveClothes = new List<List<Pair<ClothingItem, PushButton>>>();

		private readonly Label mWaveLabel;
		private readonly Button mNextWaveButton;
		private readonly Label mLevelLabel;
		private readonly ProgressIndicator mExperienceMeter;
		private readonly Label mExperienceLabel;
		private readonly Label mEnergyLabel;
		private readonly Label mEnergyTimerLabel;
		private readonly ProgressIndicator mEnergyMeter;
		private readonly IGuiStyle mProgressStyle;
		private readonly IGuiStyle mProgressCompleteStyle;
		private readonly TaskCollection mTasks = new TaskCollection();

		private bool mMouseUp = true;
		private string mWaveString = "";

		private float mLastKnownEnergy = 0.0f;
		private float mMaxEnergy = Energy.INITIAL_MAX_ENERGY;
		private DateTime? mLastEnergyTime = null;

		public FashionGameGui()
			: base(GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>(), FASHION_GUI_PATH)
		{
			mInput = GameFacade.Instance.RetrieveMediator<FashionGameInput>();

			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			this.MainGui.Showing = true;

			foreach (ITopLevel topLevel in AllGuis)
			{
				switch (topLevel.Name)
				{
					case "FashionGameGui":
						mMainWindow = (Window)topLevel;
						mMainWindow.Showing = true;
						break;

					case "FashionScoreGui":
						mScoreWindow = (Window)topLevel;
						mScoreWindow.Showing = true;
						break;
				}
			}

			mMainFrame = mMainWindow.SelectSingleElement<IGuiFrame>("MainFrame");
			mClothingButtonPrototype = mMainFrame.SelectSingleElement<PushButton>("ButtonPrototype");

			// Initialize the clothing stack slots
			uint clothingStacks = (uint)(mMainFrame.InternalSize.x / mClothingButtonPrototype.ExternalSize.x);
			for (uint i = 0; i < clothingStacks; ++i )
			{
				mActiveClothes.Add(new List<Pair<ClothingItem, PushButton>>());
			}

			mMainFrame.RemoveChildWidget(mClothingButtonPrototype);

			mWaveLabel = mScoreWindow.SelectSingleElement<Label>("**/WaveLabel");
			mWaveString = mWaveLabel.Text;
			mNextWaveButton = mScoreWindow.SelectSingleElement<Button>("**/NextWaveButton");
			mNextWaveButton.Enabled = false;
			mLevelLabel = mScoreWindow.SelectSingleElement<Label>("**/LevelLabel");
			mExperienceMeter = mScoreWindow.SelectSingleElement<ProgressIndicator>("**/ExperienceMeter");
			mExperienceLabel = mScoreWindow.SelectSingleElement<Label>("**/ExperienceLabel");
			mProgressStyle = GetNamedStyle("Progress");
			mProgressCompleteStyle = GetNamedStyle("ProgressComplete");
			mEnergyLabel = mScoreWindow.SelectSingleElement<Label>("**/EnergyLabel");
			mEnergyTimerLabel = mScoreWindow.SelectSingleElement<Label>("**/EnergyTimerLabel");
			mEnergyMeter = mScoreWindow.SelectSingleElement<ProgressIndicator>("**/EnergyMeter");

			mTasks.Add(mScheduler.StartCoroutine(UpdateEnergyDisplay()));
		}
		
		public void EnergyRefilled()
		{
			mLastKnownEnergy = mMaxEnergy;
			UpdateEnergyMeter();
		}

		public void SetEnergy(float energy, float maxEnergy, DateTime rechargeStart)
		{
			mLastKnownEnergy = energy;
			mMaxEnergy = maxEnergy;
			mLastEnergyTime = rechargeStart;

			UpdateEnergyMeter();
		}

		public void LeveledUp()
		{
			mMaxEnergy += Energy.MAX_ENERGY_INCREASE_PER_LEVEL;
			mLastKnownEnergy = mMaxEnergy;
			mLastEnergyTime = DateTime.UtcNow;

			UpdateEnergyMeter();
		}

		/// <summary>
		/// Approximates the current energy that has recharged and updates the display
		/// </summary>
		private IEnumerator<IYieldInstruction> UpdateEnergyDisplay()
		{
			while(true)
			{
				if( mLastEnergyTime != null )
				{
					UpdateEnergyMeter();
				}
				yield return new YieldForSeconds(ENERGY_UPDATE_RATE);
			}
		}

		private void UpdateEnergyMeter()
		{
			int currentEnergy = (int)Energy.CalculateCurrentEnergy(mMaxEnergy, mLastKnownEnergy, (DateTime)mLastEnergyTime);
			mEnergyLabel.Text = currentEnergy + "/" + mMaxEnergy.ToString("f0");
			float progress = (currentEnergy / mMaxEnergy);
			mEnergyMeter.SetProgress(progress);
			if (progress < 1.0) 
			{
				TimeSpan ts = Energy.GetTimeToNextRecharge(mMaxEnergy, mLastKnownEnergy, (DateTime)mLastEnergyTime);
				mEnergyTimerLabel.Text = String.Format("More in {0}:{1:00}", ts.Minutes, ts.Seconds);
			}
			else
			{
				mEnergyTimerLabel.Text = "Energy Full";
			}
		}

		public bool OccludesScreenPoint(Vector2 screenPoint)
		{
			return mMainWindow.Manager.OccludingTopLevels(screenPoint).Contains(mMainWindow);
		}

		public void ClearActiveClothes()
		{
			foreach (List<Pair<ClothingItem, PushButton>> itemStack in mActiveClothes)
			{
				foreach(Pair<ClothingItem, PushButton> item in itemStack)
				{
					item.First.Dispose();
				}
				itemStack.Clear();
			}

			LayoutStacks();
		}

		/// <summary>
		/// Copies the FashionGameClothingItem provided and adds the copy into the GUI
		/// </summary>
		public void AddNewClothingItem(ClothingItem clothingPrototype)
		{
			ClothingItem newClothing = new ClothingItem(clothingPrototype);
			if (clothingPrototype.NeedsFixin)
			{
				newClothing.MakeNeedFixin();
			}

			PutItemInGui(newClothing);
		}

		/// <summary>
		/// Works like 'AddNewClothingItem' except it doesn't create a new copy of the texture
		/// </summary>
		public void PutItemInGui(ClothingItem newClothing)
		{
			PushButton newPrototype = (PushButton)mClothingButtonPrototype.Clone();

			IGuiStyle style = GetNamedStyle(newClothing.StyleName);

			newPrototype.Image = newClothing.NormalTexture;
			newPrototype.Style = style;

			bool placed = false;
			Pair<ClothingItem, PushButton> newPair = new Pair<ClothingItem, PushButton>(newClothing, newPrototype);
			foreach(List<Pair<ClothingItem, PushButton>> stack in mActiveClothes )
			{
				if( stack.Count == 0 || stack[0].First.ItemId == newClothing.ItemId )
				{
					stack.Add(newPair);

					newPrototype.AddOnPressedAction
					(
						delegate()
						{
							// TODO: change how this filters so that momentary mouse presses work (like tapping a touchpad)
							if (mMouseUp && IsWithin(Time.frameCount, mMouseDownFrame, 2))
							{
								mMouseUp = false;
								mInput.ClothingSelected(RemoveClothingItemFromGui(newClothing.ItemId));
							}
						}
					);

					placed = true;
					break;
				}
			}

			if( placed == false )
			{
				throw new Exception("Unable to place ClothingItem(" + newClothing.ItemId + ") in the GUI. (Are there more pieces of clothing in this level than fit in the clothing frame?)");
			}
			
			mClothingButtonPrototypes.Add(newPair);
			LayoutStacks();
		}

		private static readonly Vector2 STACK_OFFSET = new Vector2(5.0f, 5.0f);
		private static readonly int MAX_STACK_DISPLAY = 4;

		/// <summary>
		/// Call this anytime the clothing in mActiveClothes changes to update the GUI
		/// </summary>
		private void LayoutStacks()
		{
			mMainFrame.ClearChildWidgets();

			int tallestStack = 0;
			foreach( List<Pair<ClothingItem, PushButton>> stack in mActiveClothes )
			{
				if( stack.Count > tallestStack )
				{
					tallestStack = stack.Count;
				}
			}

			if( tallestStack >= MAX_STACK_DISPLAY )
			{
				tallestStack = MAX_STACK_DISPLAY;
			}

			float buttonWidth = mClothingButtonPrototype.ExternalSize.x;
			
			// This ordering is a little awkward because the items need to be added
			//  to the frame in a particular order to keep from having z ordering issues
			for(int i = tallestStack - 1; i >= 0; --i)
			{
				Vector2 basePosition = new Vector2(0.0f, 0.0f);
				foreach( List<Pair<ClothingItem, PushButton>> stack in mActiveClothes )
				{
					if( stack.Count > i )
					{
						Vector2 buttonPosition = STACK_OFFSET * (float)i;
						buttonPosition += basePosition;

						IWidget clothingWidget = stack[i].Second;

						if( i == 0 )
						{
							clothingWidget = new Image(clothingWidget.Name + "(Inactive)", clothingWidget.Style, stack[i].Second.Image);
						}

						mMainFrame.AddChildWidget(stack[i].Second, new FixedPosition(buttonPosition));
					}
					basePosition.x += buttonWidth;
				}
			}
		}

		private ClothingItem RemoveClothingItemFromGui(ItemId clothingItemId)
		{
			foreach (List<Pair<ClothingItem, PushButton>> stack in mActiveClothes)
			{
				if (stack.Count > 0 && stack[0].First.ItemId == clothingItemId)
				{
					ClothingItem item = stack[0].First;
					stack.RemoveAt(0);
					LayoutStacks();
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// Removes the clothing from the GUI. If it isn't in the GUI, searches the TailorStations in the Level and then the current Selection.
		/// </summary>
		public void RemoveClothingItem(ItemId clothingItemId)
		{
			// Check the GUI for the clothing
			bool foundItem = RemoveClothingItemFromGui(clothingItemId) != null;
			
			if( !foundItem )
			{
				// Check the level's tailor stations for the clothing
				FashionLevel level = GameFacade.Instance.RetrieveMediator<FashionLevel>();
				foreach( TailorStation tailorStation in level.TailorStations )
				{
					if (tailorStation.CurrentClothing != null && tailorStation.CurrentClothing.ItemId == clothingItemId)
					{
						tailorStation.Clear();
						foundItem = true;
						break;
					}
				}
			}

			if (!foundItem)
			{
				// Check the selection for the item
				foundItem = mInput.Selection.RemoveClothing(clothingItemId);
			}

			if( !foundItem )
			{
				throw new Exception("Cannot remove a clothing item (" + clothingItemId + ") that does not exist in the GUI, the Selection or in a Tailor Station.");
			}
		}

		private static bool IsWithin(int x, int y, int epsilon)
		{
			return (x - epsilon) <= y && (x + epsilon) >= y;
		}

		/// <summary>
		/// Called by FashionGameInput when the user stops pressing mouse button 0
		/// </summary>
		public void MouseUp()
		{
			mMouseUp = true;
		}

		private int mMouseDownFrame = 0;
		public void MouseDown()
		{
			mMouseDownFrame = Time.frameCount;
		}

		public void SetWave(int currentWave, int totalWaves)
		{
			mWaveLabel.Text = String.Format(mWaveString, currentWave.ToString(), totalWaves.ToString());
		}

		public void EnableNextWave(bool enabled)
		{
			mNextWaveButton.Enabled = enabled;
		}

		public void SetNextWaveAction(Hangout.Shared.Action action)
		{
			mNextWaveButton.ClearOnPressedActions();
			mNextWaveButton.AddOnPressedAction(action);
		}

		public void SetLevel(string levelName)
		{
			mLevelLabel.Text = levelName;
		}
	
		public void SetExperience(uint currentXP, uint neededXP, bool levelUp)
		{
			float progress = (currentXP / (float)neededXP);
			// Update the text label
			if (levelUp)
			{
				mExperienceLabel.Text = "Level Up!";
				mExperienceMeter.Style = mProgressCompleteStyle;
				mExperienceMeter.SetProgress(1.0f);
			}
			else
			{
				mExperienceLabel.Text = currentXP.ToString() + "/" + neededXP.ToString();
				mExperienceMeter.Style = mProgressStyle;
				mExperienceMeter.SetProgress(progress);
			}
		}

		public void SpawnFloatingWindow(Vector3 startingWorldPoint, uint value, string message, Color color)
		{
			IGuiStyle labelStyle = new GuiStyle(Manager.GetDefaultStyle(typeof(Label)), "FloatingText");
			labelStyle.DefaultTextAnchor = GuiAnchor.BottomRight;
			labelStyle.Normal.TextColor = color;
			labelStyle.DefaultFont = (Font)Resources.Load("Fonts/HelveticaNeueCondensedBold");

			IGuiFrame mainFrame = new GuiFrame("MainFrame", new MainFrameSizePosition());
			Label scoreLabel = new Label("ScoreLabel", new ExpandText(), labelStyle, message + "\n+" + value.ToString());
			scoreLabel.DropShadowEnabled = true;

			mainFrame.AddChildWidget(scoreLabel, new HorizontalAutoLayout());

			// TODO: Hard coded values
			Window floatingWindow = new Window
			(
				"ScoreFloatingWindow", 
				new FixedSize(96.0f, 64.0f),
				Manager, 
				mainFrame, 
				Manager.GetDefaultStyle(typeof(GuiElement)) // invisible window 
			);

			Camera cam = GameFacade.Instance.RetrieveMediator<FashionCameraMediator>().Camera; 

			// TODO: Hard coded values
			Manager.SetTopLevelPosition
			(
				floatingWindow,
				new FloatFromPosition
				(
					mScheduler,
					cam,
					startingWorldPoint,
					75.0f,
					1.5f,
					delegate()
					{
						floatingWindow.Close();
					}
				)
			);
		}


		#region IMediator Members

		public string MediatorName
		{
			get { return this.GetType().Name; }
		}

		public IList<string> ListNotificationInterests()
		{
			return new List<string>();
		}

		public void HandleNotification(INotification notification)
		{
		}

		public void OnRegister()
		{
		}

		public void OnRemove()
		{
			Dispose();
		}

		#endregion
	}
}
