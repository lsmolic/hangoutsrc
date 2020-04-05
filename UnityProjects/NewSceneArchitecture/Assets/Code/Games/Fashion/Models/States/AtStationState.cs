/**  --------------------------------------------------------  *
 *   AtStationState.cs  
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
	public class AtStationState : State, IFashionModelState
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
			get { return 0; }
		}

		public AtStationState(FashionModel model, ModelStation station)
		{
			if( model == null )
			{
				throw new ArgumentNullException("model");
			}
			mModel = model;

			if( station == null )
			{
				throw new ArgumentNullException("station");
			}
			mStation = station;
		}

		public override void EnterState()
		{
			mModel.PlaySittingIdleAnimation();
			mModel.Unselect();
			mStation.ModelArrived(Model);
		}

		public override void ExitState()
		{

		}
	}
}
