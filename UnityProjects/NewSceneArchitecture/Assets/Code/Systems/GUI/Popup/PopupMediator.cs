using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using UnityEngine;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
	/// <summary>
	/// Generic popup dialog
	/// Use like this:
	///     List<object> args = new List<object>();
	///     args.Add("TestTitle");
	///     args.Add("Do you want to continue?");
	///     Hangout.Shared.Action okcb = delegate { Debug.Log("pressed ok"); };
	///     Hangout.Shared.Action cancelcb = delegate { Debug.Log("pressed cancel"); };
	///     args.Add(okcb);
	///     args.Add(cancelcb);
	///     args.Add("Ok button text"); // optional. "Ok" is default
	///     args.Add("Cancel button text"); // optional. "Cancel" is default
	///     GameFacade.Instance.SendNotification(GameFacade.SHOW_CONFIRM, args);
	/// </summary>
	public class PopupMediator : Mediator
	{
		private PopupGui mPopupGui = null;

		public PopupMediator()
			: base()
		{
		}

		public override IList<string> ListNotificationInterests()
		{
			return new string[] 
            {
                GameFacade.SHOW_CONFIRM,
                GameFacade.SHOW_DIALOG,
                GameFacade.HIDE_POPUP
            };
		}

		public override void HandleNotification(INotification notification)
		{
			List<object> args = (List<object>)notification.Body;
			switch (notification.Name)
			{
				case GameFacade.SHOW_CONFIRM:
					ShowConfirm(args);
					break;
				case GameFacade.SHOW_DIALOG:
					ShowDialog(args);
					break;
				case GameFacade.HIDE_POPUP:
					HidePopupGui();
					break;
			}
		}

		/// <summary>
		/// Show confirmation popup that has an ok and cancel button
		/// </summary>
		/// <param name="args">
		/// args[0] = title
		/// args[1] = description
		/// args[2] = ok callback action
		/// args[3] = cancel callback action
		/// args[4] = ok text (optional)
		/// args[5] = cancel text (optional)
		/// </param>
		private void ShowConfirm(List<object> args)
		{
			ShowPopupGui();
			SetTitleAndDescription(args);
			HandleOptionalArgs(args);

			mPopupGui.OkButton.Showing = true;
			mPopupGui.CancelButton.Showing = true;
		}


		/// <summary>
		/// Show dialog popup that has an ok button
		/// </summary>
		/// <param name="args">
		/// args[0] = title
		/// args[1] = description
		/// args[2] = ok handler action
		/// args[3] = ok text (optional)
		/// </param>
		private void ShowDialog(List<object> args)
		{
			ShowPopupGui();
			SetTitleAndDescription(args);

			if (args.Count > 2)
			{
				Hangout.Shared.Action okCallbackAction = (Hangout.Shared.Action)args[2];
				mPopupGui.OkButton.AddOnPressedAction(okCallbackAction);
			}

			if (args.Count > 3)
			{
				string okText = (string)args[3];
				mPopupGui.OkButton.Text = okText;
			}

			mPopupGui.OkButton.Showing = true;
			mPopupGui.CancelButton.Showing = false;
		}

		private void SetTitleAndDescription(List<object> args)
		{
			try
			{
				string title = (string)args[0];
				mPopupGui.Title.Text = title;

				string description = (string)args[1];
				mPopupGui.Description.Text = description;
			}
			catch (Exception e)
			{
				throw new Exception("Error parsing required arguments in ShowConfirm: " + e.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args">
		/// args[2] = ok callback action
		/// args[3] = cancel callback action
		/// args[4] = ok text
		/// args[5] = cancel text
		/// </param>
		private void HandleOptionalArgs(List<object> args)
		{
			if (args.Count > 2)
			{
				Hangout.Shared.Action okCallbackAction = (Hangout.Shared.Action)args[2];
				mPopupGui.OkButton.AddOnPressedAction(okCallbackAction);
			}
			if (args.Count > 3)
			{
				Hangout.Shared.Action cancelCallbackAction = (Hangout.Shared.Action)args[3];
				mPopupGui.CancelButton.AddOnPressedAction(cancelCallbackAction);
			}
			if (args.Count > 4)
			{
				string okText = (string)args[4];
				mPopupGui.OkButton.Text = okText;
			}

			if (args.Count > 5)
			{
				string cancelText = (string)args[5];
				mPopupGui.CancelButton.Text = cancelText;
			}
		}

		private void ShowPopupGui()
		{
			if (mPopupGui == null)
			{
				IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
				mPopupGui = new PopupGui(guiManager);
			}
			mPopupGui.Show();
		}

		private void HidePopupGui()
		{
			if (mPopupGui != null)
			{
				mPopupGui.Hide();
			}
		}
	}
}
