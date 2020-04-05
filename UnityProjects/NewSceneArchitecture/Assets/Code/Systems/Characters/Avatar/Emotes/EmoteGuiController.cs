using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Client.Gui;

using Hangout.Shared;

using UnityEngine;

namespace Hangout.Client
{
	public class EmoteGuiController : GuiController
	{
		// TWEAKS FOR GUI
		private int mMenuButtonPadding = 3;
		private int mMenuBorderPadding = 5;
		private int mInterMenuPadding = 4;
		
		private int mIconFrameColumns = 5;
		private int mIconButtonPadding = 3;
		
		
		private const string EMOTE_MENU_GUI_PATH = "resources://GUI/EmoteMenu/EmoteMenu.gui";
		
		private AnimationProxy mAnimationProxy;
		private ClientAssetRepository mClientAssetRepository;
		
		private bool mShowing = false;
		public bool Showing
		{			
			get { return mShowing; }
		}
		
		private IReceipt mCheckIfClickedInWindowInputTask;
		
		private GuiFrame mMainMenuFrame;
		private GridView mTextMenuGridView;
		private GridView mIconMenuGridView;
		
		private Window mMainMenuWindow;
		private const string MAIN_MENU_WINDOW_NAME = "MainMenuWindow";
		private Window mIconMenuWindow;
		private const string ICON_MENU_WINDOW_NAME = "IconMenuWindow";
		private Window mTextMenuWindow;
		private const string TEXT_MENU_WINDOW_NAME = "TextMenuWindow";
		// This window remembers which window was last showing to preserve basic gui state.
		private Window mLastWindowShowing;
		private string mLastWindowShowingToUpdate;
		
		private Button mMainMenuMoodButton;
		private Button mMainMenuIconButton;
		private Button mMainMenuEmoteButton;
		private Button mActiveMenuButton;
		private Button mActiveMoodButton;
		private Button mIconMenuButton;

		private MoodAnimation mCurrentMood = (MoodAnimation)(-1);
		
		public EmoteGuiController(IGuiManager guiManager)
		 : base (guiManager, EMOTE_MENU_GUI_PATH)
		{
			foreach (Window window in this.AllGuis)
			{
				if (window.Name == MAIN_MENU_WINDOW_NAME)
				{
					mMainMenuWindow = window;
				}
				else if (window.Name == ICON_MENU_WINDOW_NAME)
				{
					mIconMenuWindow = window;
				}
				else if (window.Name == TEXT_MENU_WINDOW_NAME)
				{
					mTextMenuWindow = window;
				}
			}
			mClientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			
			// MainMenu Window Gui.
			mMainMenuFrame = mMainMenuWindow.SelectSingleElement<GuiFrame>("MainFrame/MainMenu");
			
			mMainMenuIconButton = mMainMenuFrame.SelectSingleElement<Button>("IconMenuButton");
			mMainMenuIconButton.AddOnPressedAction(ShowIconMenu);
			
			mMainMenuMoodButton = mMainMenuFrame.SelectSingleElement<Button>("MoodMenuButton");
			mMainMenuMoodButton.AddOnPressedAction(ShowMoodMenu);

			mMainMenuEmoteButton = mMainMenuFrame.SelectSingleElement<Button>("EmoteMenuButton");
			mMainMenuEmoteButton.AddOnPressedAction(ShowEmoteMenu);			

			// Text Menu Window Gui
			mActiveMenuButton = mTextMenuWindow.SelectSingleElement<Button>("MainFrame/ActiveMenuButton");
			mTextMenuWindow.MainFrame.Key.RemoveChildWidget((IWidget)mActiveMenuButton);
			mActiveMoodButton = mTextMenuWindow.SelectSingleElement<Button>("MainFrame/ActiveMoodButton");
			mTextMenuWindow.MainFrame.Key.RemoveChildWidget((IWidget)mActiveMoodButton);

			mTextMenuGridView = mTextMenuWindow.SelectSingleElement<GridView>("MainFrame/TextMenu");
			
			// Icon Menu Window Gui
			mIconMenuGridView = mIconMenuWindow.SelectSingleElement<GridView>("MainFrame/IconMenu");

			mIconMenuButton = mIconMenuWindow.SelectSingleElement<Button>("MainFrame/IconMenuButton");
			mIconMenuWindow.MainFrame.Key.RemoveChildWidget((IWidget)mIconMenuButton);
			
			// Init this to be the main menu window so it doesn't break the show function.
			mLastWindowShowing = mMainMenuWindow;
			
			Hide();
		}
		
		private void ShowEmoteMenu()
		{
			mAnimationProxy = GameFacade.Instance.RetrieveProxy<AnimationProxy>();
			List<IWidget> enabledMenuButtons = new List<IWidget>();
			List<IWidget> disabledMenuButtons = new List<IWidget>();
			foreach (KeyValuePair<RigAnimationName, bool> kvp in mAnimationProxy.PlayableEmoteLookUpTable)
			{
				Button button = (Button)mActiveMenuButton.Clone();
				if (kvp.Value)
				{
					button.Enable();
					enabledMenuButtons.Add(button);
				}
				else
				{
					button.Disable();
					disabledMenuButtons.Add(button);
				}
				
				button.Text = TranslateEmoteNameToDisplayName(kvp.Key.ToString());
				string emoteName = kvp.Key.ToString();
				button.AddOnPressedAction(delegate()
				{
					GameFacade.Instance.SendNotification(GameFacade.PLAY_EMOTE, emoteName);
					EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.EVENT_EMOTE_CLICKED, "Emote", emoteName);
					Hide();
				});
			}
			List<IWidget> menuButtons = enabledMenuButtons;
			menuButtons.AddRange(disabledMenuButtons);
			ResizeTextMenuWindow(mTextMenuWindow, mActiveMenuButton, menuButtons.Count);
			RepositionWindow(mTextMenuWindow, mMainMenuWindow);
			mTextMenuGridView.SetPositionsWithBorderPadding(menuButtons, menuButtons.Count, 1, 0, mMenuBorderPadding, mMenuBorderPadding, mMenuButtonPadding);
			mTextMenuWindow.Showing = true;
			mIconMenuWindow.Showing = false;
			mLastWindowShowing = mTextMenuWindow;
			mLastWindowShowingToUpdate = "emote";
		}
		
		private void ShowIconMenu()
		{
			mAnimationProxy = GameFacade.Instance.RetrieveProxy<AnimationProxy>();
			List<IWidget> enabledMenuButtons = new List<IWidget>();
			List<IWidget> disabledMenuButtons = new List<IWidget>();
			foreach (KeyValuePair<string, bool> kvp in mAnimationProxy.PlayableIconLookUpTable)
			{
				Button button;
				button = (Button)mIconMenuButton.Clone();
				if (kvp.Value)
				{
					button.Enable();
					enabledMenuButtons.Add(button);
				}
				else
				{
					button.Disable();
					disabledMenuButtons.Add(button);
				}
				string emoticonPath = kvp.Key;
				mClientAssetRepository.LoadAssetFromPath<ImageAsset>(emoticonPath, delegate(ImageAsset asset)
				{
					button.Image = asset.Texture2D;
					button.AddOnPressedAction(delegate()
					{
						object[] args = { emoticonPath };
						GameFacade.Instance.SendNotification(GameFacade.SEND_EMOTICON, args);
						EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.EVENT_EMOTICON_CLICKED, "Emoticon", emoticonPath);
						Hide();
					});
				});
			}
			List<IWidget> menuButtons = enabledMenuButtons;
			menuButtons.AddRange(disabledMenuButtons);
			
			ResizeIconWindow(mIconMenuWindow, mIconMenuButton, menuButtons.Count);
			RepositionWindow(mIconMenuWindow, mMainMenuWindow);
			int rows = menuButtons.Count/mIconFrameColumns + 1;
			mIconMenuGridView.SetPositionsWithBorderPadding(menuButtons, rows, mIconFrameColumns, 0, mMenuBorderPadding, mMenuBorderPadding, mIconButtonPadding);
			mIconMenuWindow.Showing = true;
			mTextMenuWindow.Showing = false;
			mLastWindowShowing = mIconMenuWindow;
			mLastWindowShowingToUpdate = "icon";
		}
		
		private void ShowMoodMenu()
		{
			mAnimationProxy = GameFacade.Instance.RetrieveProxy<AnimationProxy>();
			List<IWidget> enabledMenuButtons = new List<IWidget>();
			List<IWidget> disabledMenuButtons = new List<IWidget>();

			foreach (KeyValuePair<MoodAnimation, bool> kvp in mAnimationProxy.PlayableMoodLookUpTable)
			{
				if (mCurrentMood == kvp.Key)
				{
					Button button = (Button)mActiveMoodButton.Clone(); 
					button.AddOnPressedAction(delegate()
					{
						Hide();
					});
					enabledMenuButtons.Add(button);
					button.Text = kvp.Key.ToString();
				}
				
				else
				{				
					Button button = (Button)mActiveMenuButton.Clone(); 
					if (kvp.Value)
					{
						button.Enable();
						enabledMenuButtons.Add(button);
					}
					else
					{
						button.Disable();
						disabledMenuButtons.Add(button);
					}
					button.Text = kvp.Key.ToString();
					string moodNameString = kvp.Key.ToString();
					button.AddOnPressedAction(delegate()
					{
						moodNameString = moodNameString.Split(' ')[0];
						MoodAnimation moodName = (MoodAnimation)Enum.Parse(typeof(MoodAnimation), moodNameString);
						
						List<AssetInfo> assetInfos = GameFacade.Instance.RetrieveProxy<AnimationProxy>().GetMoodAssetInfos(moodName);
						GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity.UpdateAssetsWithCallback(assetInfos, delegate(AvatarEntity avatarEntity)
						{
							GameFacade.Instance.RetrieveProxy<LocalAvatarProxy>().SaveDna();
						});
						EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.EVENT_MOOD_CLICKED, "Mood", moodNameString);
						mCurrentMood = moodName;
						EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.EVENT_MOOD_CLICKED, "Mood", moodNameString);
						Hide();
					});
				}
			}
			List<IWidget> menuButtons = enabledMenuButtons;
			menuButtons.AddRange(disabledMenuButtons);
			
			ResizeTextMenuWindow(mTextMenuWindow, mActiveMenuButton, menuButtons.Count);
			RepositionWindow(mTextMenuWindow, mMainMenuWindow);
			mTextMenuGridView.SetPositionsWithBorderPadding(menuButtons, menuButtons.Count, 1, 0, mMenuBorderPadding, mMenuBorderPadding, mMenuButtonPadding);
			mTextMenuWindow.Showing = true;
			mIconMenuWindow.Showing = false;
			mLastWindowShowing = mTextMenuWindow;
			mLastWindowShowingToUpdate = "mood";
		}
		
		private void ResizeIconWindow(Window iconWindow, Button iconButton, float elements)
		{
			float xSize = (iconButton.Size.x * mIconFrameColumns) + // Size of buttons added up
				((mIconFrameColumns - 1) * mIconButtonPadding) + // Size of padding in between buttons
				(2 * mMenuBorderPadding); // Add in size of top and bottom window padding

			int rows = (int)elements / mIconFrameColumns + 1;
			float ySize = (iconButton.Size.y * rows) + // Size of buttons added up
				((rows - 1) * mIconButtonPadding) + // Size of padding in between buttons
				(2 * mMenuBorderPadding); // Add in size of top and bottom window padding

			iconWindow.GuiSize = new FixedSize(new Vector2(xSize, ySize));
		}

		private void ResizeTextMenuWindow(Window window, Button button, float elements)
		{
			float xSize = button.Size.x + (2 * mMenuBorderPadding);
			float ySize = (elements * button.Size.y) + // Size of buttons added up
				((elements - 1) * mMenuButtonPadding) +  // Size of padding in between buttons
				(2 * mMenuBorderPadding); // Add in size of top and bottom window padding
			window.GuiSize = new FixedSize(new Vector2(xSize, ySize));
		}
		
		private void RepositionWindow(Window windowToReposition, Window windowToTheLeft)
		{
			Vector2 windowToTheLeftPosition = windowToTheLeft.Manager.GetChildPosition(windowToTheLeft);

			float maxXOfWindowToLeft = windowToTheLeft.Size.x + windowToTheLeftPosition.x;
			float maxYOfWindowToLeft = windowToTheLeft.Size.y + windowToTheLeftPosition.y;
			
			Vector2 bottomRightOfWindowToLeft = new Vector2(maxXOfWindowToLeft, maxYOfWindowToLeft);
			
			float xOfNewPosition = mInterMenuPadding + bottomRightOfWindowToLeft.x;
			float yOfNewPosition = bottomRightOfWindowToLeft.y - windowToReposition.Size.y;
			Vector2 newPosition = new Vector2(xOfNewPosition, yOfNewPosition);

			windowToReposition.Manager.UpdateChildPosition(windowToReposition, newPosition);		
		}
		
		private string TranslateEmoteNameToDisplayName(string nameToTranslate)
		{
			nameToTranslate = nameToTranslate.Remove(nameToTranslate.Length - 5);
			List<int> indiciesOfUpperCaseChars = new List<int>();
			for (int i = 1; i < nameToTranslate.Length; ++i)
			{
				if (Char.IsUpper(nameToTranslate[i]))
				{
					indiciesOfUpperCaseChars.Add(i);
				}
			}
			foreach (int i in indiciesOfUpperCaseChars)
			{
				nameToTranslate = nameToTranslate.Insert(i, " ");
			}
			return nameToTranslate;
		}
		
		private void Show()
		{
			mShowing = true;
			mMainMenuWindow.Showing = true;
			switch (mLastWindowShowingToUpdate)
			{
				case "mood":
					ShowMoodMenu();
					break;
				case "icon":
					ShowIconMenu();
					break;
				case "emote":
					ShowEmoteMenu();
					break;
			}
			mLastWindowShowing.Showing = true;
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitOneFrameToRegisterMouseUpEventSinceGuiButtonUpEventsHappenBeforeInputUpEvents());
		}
		
		private IEnumerator<IYieldInstruction> WaitOneFrameToRegisterMouseUpEventSinceGuiButtonUpEventsHappenBeforeInputUpEvents()
		{
			yield return new YieldUntilNextFrame();
			mCheckIfClickedInWindowInputTask = GameFacade.Instance.RetrieveMediator<InputManagerMediator>().RegisterForButtonUp(KeyCode.Mouse0, CheckIfClickedInWindow);
		}

		private void Hide()
		{
			mShowing = false;
			mMainMenuWindow.Showing = false;
			mTextMenuWindow.Showing = false;
			mIconMenuWindow.Showing = false;
			if (mCheckIfClickedInWindowInputTask != null)
			{
				mCheckIfClickedInWindowInputTask.Exit();
			}
		}
		
		private void CheckIfClickedInWindow()
		{
			InputManagerMediator inputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();
			Vector3 mousePosition = inputManager.MousePosition;
			if (IsMouseOverOccludingElement(mMainMenuWindow, mousePosition) || IsMouseOverOccludingElement(mIconMenuWindow, mousePosition) || IsMouseOverOccludingElement(mTextMenuWindow, mousePosition))
			{
				return;
			}
			else
			{
				GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(ClosedFromMouseUp());
			}
		}

		private bool IsMouseOverOccludingElement(GuiElement element, Vector2 mousePositionInScreenSpace)
		{
			if (!element.Showing)
			{
				return false;
			}
			else
			{
				Rect elementRect = new Rect(0.0f, 0.0f, element.Size.x, element.Size.y);
				return elementRect.Contains(element.GetWidgetPosition(mousePositionInScreenSpace));
			}
		}
		
		private IEnumerator<IYieldInstruction> ClosedFromMouseUp()
		{
			yield return new YieldUntilNextFrame();
			Hide();
		}

		public void ToggleOpen()
		{
			if (!mShowing)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}
}
