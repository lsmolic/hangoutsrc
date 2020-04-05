/**  --------------------------------------------------------  *
 *   FashionGameTranslation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client;
using Hangout.Client.Gui;

namespace Hangout.Client.FashionGame
{
	public static class FashionGameTranslation
	{
		public const string LEVEL_CLEAR = "Level Clear!";
		public const string PERFECT_LEVEL = "Perfect Level!";
		public const string WAVE = "Wave";

		public const string BUY = "Buy";
		public const string CANCEL = "Cancel";
		public const string BUY_ENERGY = "Buy Energy";
		public const string REFILL_ENERGY_MESSAGE = "Purchase one full energy refill for {0} Hangout Cash?";
		public const string SUCCESS = "Success";
		public const string PURCHASE_COMPLETE = "Purchase Complete";
		public const string PLAY_LEVEL = "Play Level";
		public const string NOT_ENOUGH_CASH_TITLE = "Not Enough Cash";
		public const string NOT_ENOUGH_CASH_MESSAGE = "You do not have enough Hangout Cash to purchase an energy refill.";
		public const string BUY_CASH = "Buy Cash";

		public const string GO_TO_SHOW = "Go to Fashion Show";
	}
}
