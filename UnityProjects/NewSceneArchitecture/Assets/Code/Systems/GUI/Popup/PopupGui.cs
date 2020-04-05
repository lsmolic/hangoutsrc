using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
    public class PopupGui : GuiController
    {
        // Popup styles
        public const string STYLE_OK = "styleOk";
        public const string STYLE_OK_CANCEL = "styleOkCancel";

        private readonly Window mMainWindow;

        private readonly Button mOkButton;
        public Button OkButton
        {
            get { return mOkButton; }
        }

        private readonly Button mCancelButton;
        public Button CancelButton
        { 
            get { return mCancelButton; } 
        }

        private readonly Label mTitle;
        public Label Title
        {
            get { return mTitle; }
        }

        private readonly Label mDescription;
        public Label Description 
        {   
            get { return mDescription; }
        }

        private const string mResourcePath = "resources://GUI/Popup/Popup.gui";

        public PopupGui(IGuiManager guiManager) : base (guiManager, mResourcePath)
        {
            mMainWindow = (Window)this.MainGui;
            mMainWindow.OnShowing(OnShowingCallback);
            mTitle = mMainWindow.SelectSingleElement<Label>("MainFrame/Title");
			mDescription = mMainWindow.SelectSingleElement<Label>("MainFrame/Description");
            mOkButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Ok");
			mCancelButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Cancel");
            mMainWindow.InFront = true;
            Init();
        }
        
        private void OnShowingCallback(bool showing)
        {
			if (showing)
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_POPUP_APPEAR_A);
			}
			else
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_CLOSE);
			}
        }

        /// <summary>
        /// Show popup window
        /// </summary>
        public void Show()
        {
            mMainWindow.Showing = true;
        }

        public void Hide()
        {
            mMainWindow.Showing = false;
            Init();
        }

        private void Init()
        {
            mOkButton.ClearOnPressedActions();
            mOkButton.AddOnPressedAction(Hide);
            mCancelButton.ClearOnPressedActions();
            mCancelButton.AddOnPressedAction(Hide);
            mTitle.Text = "";
            mDescription.Text = "";
            mOkButton.Text = Translation.OK;
            mCancelButton.Text = Translation.CANCEL;
        }
    }
}
