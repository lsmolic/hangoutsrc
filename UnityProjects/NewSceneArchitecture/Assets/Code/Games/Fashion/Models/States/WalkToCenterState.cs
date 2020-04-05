/**  --------------------------------------------------------  *
 *   WalkToCenterState.cs  
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
	public class WalkToCenterState : State, IFashionModelState
	{
		private readonly FashionModel mModel;
		public FashionModel Model
		{
			get { return mModel; }
		}


		public int CollisionCheckLayer
		{
			get { return (1 << FashionMinigame.MODEL_LAYER) | (1 << FashionMinigame.STATION_LAYER); }
		}

		private readonly FashionLevel mLevel;
		
		public WalkToCenterState(FashionModel model, FashionLevel level)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			mModel = model;


			if (level == null)
			{
				throw new ArgumentNullException("level");
			}
			mLevel = level;			
		}

		public bool Clickable
		{
			get { return true; }
		}

		public override void EnterState()
		{
			Vector3 middlePathPoint = LinearAlgebra.NearestPointOnLineSegment(mLevel.Start.First, mLevel.End.First, mModel.UnityGameObject.transform.position);
			mModel.WalkTo
			(
				new Pair<Vector3>
				(
					Vector3.Lerp
					(
						middlePathPoint,
						mModel.UnityGameObject.transform.position,
						Mathf.Lerp(0.0f, 0.5f, UnityEngine.Random.value)
					),
					(mLevel.Start.First - mLevel.End.First).normalized
				),
				delegate()
				{
					mModel.WalkToEndTarget();
				}
			);
		}

		public override void ExitState()
		{

		}
	}
}
