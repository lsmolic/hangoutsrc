/**  --------------------------------------------------------  *
 *   WalkToEndGoal.cs  
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
	public class WalkToEndGoal : State, IFashionModelState
	{
		private readonly FashionModel mModel;
		public FashionModel Model
		{
			get { return mModel; }
		}

		private readonly FashionLevel mLevel;

		public bool Clickable
		{
			get { return !mModel.Ready; }
		}


		public int CollisionCheckLayer
		{
			get { return (1 << FashionMinigame.MODEL_LAYER) | (1 << FashionMinigame.STATION_LAYER); }
		}

		public WalkToEndGoal(FashionModel model, FashionLevel level)
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

		public override void EnterState()
		{
			mModel.SetupAnimationClips();

			// Walk to the nearest point on the end finish line
			Vector3 start2End = (mLevel.End.First - mLevel.Start.First).normalized;

			Vector3 start2EndCrossUp = Vector3.Cross(start2End, Vector3.up).normalized;

			Vector3 endLineStart = (start2EndCrossUp * 0.5f * mLevel.EndWidth) + mLevel.End.First;
			Vector3 endLineEnd = (start2EndCrossUp * -0.5f * mLevel.EndWidth) + mLevel.End.First;

			mModel.WalkTo
			(
				new Pair<Vector3>
				(
					LinearAlgebra.NearestPointOnLineSegment(endLineStart, endLineEnd, mModel.UnityGameObject.transform.position),
					start2End
				),
				mModel.TargetReached
			);
		}

		public override void ExitState()
		{
			
		}
	}		
}
