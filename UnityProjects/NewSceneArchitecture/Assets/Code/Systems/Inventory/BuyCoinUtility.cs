/**  --------------------------------------------------------  *
 *   InventoryMediator.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/14/2009
 *	 
 *   --------------------------------------------------------  *
 */

using PureMVC.Patterns;
using PureMVC.Interfaces;

using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.Messages;

namespace Hangout.Client
{	
	public static class BuyCoinUtility
	{
		/// <summary>
		/// Pops up conformation GUI before navigating the browser to the buy cash page.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="onOk">The string param is the Javascript call result</param>
		/// <param name="onCancel"></param>
		public static void GoToBuyCashPage(string title, string message, Action<string> onOk, Hangout.Shared.Action onCancel)
		{
			ConfigManagerClient configManager = GameFacade.Instance.RetrieveProxy<ConfigManagerClient>();
			if (configManager.GetBool("purchase_page_enabled", true))
			{
				// Popup confirm
				List<object> args = new List<object>();
				args.Add(title);
				args.Add(message);
				Hangout.Shared.Action okcb = delegate()
				{
					HandleBuyCashOk(onOk);
				};
				args.Add(okcb);
				args.Add(onCancel);
				args.Add(Translation.NEED_CASH_BUTTON);
				GameFacade.Instance.SendNotification(GameFacade.SHOW_CONFIRM, args);
				GameFacade.Instance.SendNotification(GameFacade.GET_CASH_GUI_OPENED);
			}
			else
			{
				// Popup coming soon dialog
				List<object> args = new List<object>();
				args.Add(title);
				args.Add(Translation.NEED_CASH_TEXT_COMING_SOON);

				GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);
				GameFacade.Instance.SendNotification(GameFacade.GET_CASH_GUI_OPENED);
			}

		}

		private static void HandleBuyCashOk(Action<string> onOk)
		{
			Console.WriteLine("HandleBuyCashOk");

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			string urlParams = "p=" + inventoryProxy.SecurePaymentInfo;
			JSDispatcher jsd = new JSDispatcher();
			jsd.RequestCashStore(urlParams, onOk);
			GameFacade.Instance.SendNotification(GameFacade.GET_CASH_GUI_CLOSED);
		}
	}
}
