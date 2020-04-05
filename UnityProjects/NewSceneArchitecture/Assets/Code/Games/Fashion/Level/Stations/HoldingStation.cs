/**  --------------------------------------------------------  *
 *   HoldingStation.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/21/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using UnityEngine;
using PureMVC.Interfaces;

using Hangout.Shared;

namespace Hangout.Client.FashionGame
{
	public class HoldingStation : ModelStation
	{
		public HoldingStation(Pair<Vector3> location, string label, float time, Vector3 guiOffset, GameObject displayObject)
			: base(location, label, null, time, guiOffset, displayObject)
		{
		}

		public override bool RequiresWorker
		{
			get { return false; }
		}

		private static bool mUsedHoldingStation = false;

		public override void ModelArrived(FashionModel model)
		{
			base.ModelArrived(model);

			if (!mUsedHoldingStation )
			{
				mUsedHoldingStation = true;

				EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.GAMEPLAY_BEHAVIOR, "FirstTime",
					LogGlobals.HOLDING_STATION);
			}
		}
	}
}
