/**  --------------------------------------------------------  *
 *   ModelInactiveState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/26/2009
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
	public class ModelInactiveState : State, IFashionModelState
	{
		private readonly FashionModel mModel;
		public FashionModel Model
		{
			get { return mModel; }
		}

		public bool Clickable
		{
			get { return false; }
		}

		public int CollisionCheckLayer
		{
			get { return 0; }
		}

		public ModelInactiveState(FashionModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			mModel = model;
		}

		public override void EnterState()
		{
			mModel.DisplayObject.SetActiveRecursively(false);
		}

		public override void ExitState()
		{
			mModel.DisplayObject.SetActiveRecursively(true);
		}
	}
}
