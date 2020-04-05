/**  --------------------------------------------------------  *
 *   WalkToStationState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/21/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class WalkToStationState : State, IFashionModelState
	{
		private readonly FashionModel mModel;
		public FashionModel Model
		{
			get { return mModel; }
		}
		private readonly ModelStation mStation;

		public bool Clickable
		{
			get { return false; }
		}

		public int CollisionCheckLayer
		{
			get { return 1 << FashionMinigame.MODEL_LAYER; }
		}

		public override string Name
		{
			get{ return "WalkTo" + mStation.Name + "Station"; }
		}
		private readonly FashionLevel mLevel;

		public WalkToStationState(FashionModel model, ModelStation station, FashionLevel level)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			mModel = model;

			if (station == null)
			{
				throw new ArgumentNullException("station");
			}
			mStation = station;

			if (level == null)
			{
				throw new ArgumentNullException("level");
			}
			mLevel = level;
		}

		public override void EnterState()
		{
			Pair<Vector3> stationTarget = new Pair<Vector3>
			(
				mStation.AvatarTargetPosition,
				mStation.FacingDirection
			);

			mModel.WalkTo(stationTarget, delegate() // Delegate that handles what to do when the WalkTarget is reached
			{
				mModel.ArrivedAtStation(mStation);
			});

			if( !(mStation is HoldingStation) && mModel.DistanceTo(mLevel.End.First) < mLevel.CloseSaveDistance )
			{
				GameFacade.Instance.SendNotification
				(
					FashionMinigame.EARNED_EXPERIENCE_NOTIFICATION,
					new ExperienceInfo
					(
						ExperienceType.CloseSave
					)
				);
			}
		}

		public override void ExitState()
		{
		}
	}
}
