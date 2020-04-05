/**  --------------------------------------------------------  *
 *   FashionModelStateMachine.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/18/2009
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
	public class FashionModelStateMachine : StateMachine
	{
		private readonly WalkToEndGoal mWalkToEndGoalState;
		public WalkToEndGoal WalkToEndGoalState
		{
			get { return mWalkToEndGoalState; }
		}
		private readonly WalkToCenterState mWalkToCenterState;
		public WalkToCenterState WalkToCenterState
		{
			get { return mWalkToCenterState; }
		}

		private readonly ModelInactiveState mInactiveState;

		private readonly IDictionary<ModelStation, WalkToStationState> mWalkToStationStates = new Dictionary<ModelStation, WalkToStationState>();
		private readonly IDictionary<ModelStation, AtStationState> mAtStationStates = new Dictionary<ModelStation, AtStationState>();
		
		private readonly FashionModel mModel;

		/// <summary>
		/// Constructs a FashionModelStateMachine for the given model. Model must have needs before creating the state machine (so that the proper states for each station can be created)
		/// </summary>
		public FashionModelStateMachine(FashionModel model, FashionLevel level)
		{
			if( model == null )
			{
				throw new ArgumentNullException("model");
			}
			mModel = model;

			mInactiveState = new ModelInactiveState(model);
			
			mWalkToEndGoalState = new WalkToEndGoal(model, level);
			mInactiveState.AddTransition(mWalkToEndGoalState);

			mWalkToEndGoalState.AddTransition(mInactiveState);

			mWalkToCenterState = new WalkToCenterState(model, level);
			mWalkToCenterState.AddTransition(mInactiveState);

			List<ModelStation> possibleStations = new List<ModelStation>(model.RequiredStations);
			foreach (HoldingStation station in level.HoldingStations )
			{
				possibleStations.Add(station);
			}
			foreach (ModelStation station in possibleStations)
			{
				WalkToStationState walkToStation = new WalkToStationState(model, station, level);

				AtStationState atStation = new AtStationState(model, station);
				atStation.AddTransition(mWalkToCenterState);
				atStation.AddTransition(mInactiveState);
				
				// All the WalkToStation states can enter the corresponding AtStation state
				walkToStation.AddTransition(atStation);
				walkToStation.AddTransition(mInactiveState);

				mAtStationStates.Add(station, atStation);

				// If the model doesn't need to stay at this station, it could go back to walk to center
				walkToStation.AddTransition(mWalkToCenterState);

				mWalkToEndGoalState.AddTransition(walkToStation);

				// An avatar walking to center can be redirected to a station
				mWalkToCenterState.AddTransition(walkToStation);

				mWalkToStationStates.Add(station, walkToStation);
			}

			// Make it so that avatars can be interrupted from walking to 
			// one station by sending them to another station they need
			foreach(WalkToStationState stateA in mWalkToStationStates.Values)
			{
				foreach(WalkToStationState stateB in mWalkToStationStates.Values)
				{
					stateA.AddTransition(stateB);
				}
			}

			mWalkToCenterState.AddTransition(mWalkToEndGoalState);

			mModel.Clickable = mInactiveState.Clickable;
			this.EnterInitialState(mInactiveState);
		}

		/// <summary>
		/// Called when an avatar completes a station, this will remove that station's states
		/// </summary>
		public void RemoveStation(ModelStation station)
		{
			if( station is HoldingStation )
			{
				// Holding stations don't need to be removed
				return;
			}

			WalkToStationState thisStationWalkState = mWalkToStationStates[station];

			mAtStationStates.Remove(station);
			mWalkToStationStates.Remove(station);

			foreach(WalkToStationState walkState in mWalkToStationStates.Values)
			{
				walkState.RemoveTransition(thisStationWalkState);
			}

			mWalkToCenterState.RemoveTransition(thisStationWalkState);
			mWalkToEndGoalState.RemoveTransition(thisStationWalkState);
		}

		/// <summary>
		/// Returns the same object as CurrentState, just as IFashionModelState
		/// </summary>
		public IFashionModelState CurrentFashionState
		{
			// This is OK to not check for the cast here because the type is checked in TransitionToState
			get { return (IFashionModelState)this.CurrentState; }
		}

		/// <summary>
		/// Transition to the state that corresponds with 'Walk to the given station'
		/// </summary>
		/// <returns>True when the model needed this station, False otherwise</returns>
		public bool TransitionToWalkToStation(ModelStation station)
		{
			WalkToStationState state;
			bool result = false;
			if( mWalkToStationStates.TryGetValue(station, out state) )
			{
				this.TransitionToState(state);
				result = true;
			}
			else if( station is HoldingStation )
			{
				throw new Exception("Models should always have states for going to a HoldingStation until they're completed.");
			}
			return result;
		}

		/// <summary>
		/// Transition to the state that corresponds with 'stand at the given station'
		/// </summary>
		public void TransitionToStayAtStation(ModelStation station)
		{
			AtStationState state;
			if (mAtStationStates.TryGetValue(station, out state))
			{
				this.TransitionToState(state);
			}
			else
			{
				throw new ArgumentException("Station not needed by this Model", "station");
			}
		}

		public void TransitionToInactiveState()
		{
			this.TransitionToState(mInactiveState);
		}

		//private static uint mNextId = 0;
		//private uint mId = mNextId++;
		public override IState TransitionToState(string stateName)
		{
			//Debug.Log(mId + "> Leaving State: " + this.CurrentState.Name + ", Entering State: " + this.CurrentState.Name);
			IState state = base.TransitionToState(stateName);

			if (!(state is IFashionModelState))
			{
				throw new ArgumentException("Only IFashionModelStates are valid in the FashionModelStateMachine", "state");
			}

			IFashionModelState modelState = (IFashionModelState)state;
			mModel.Clickable = modelState.Clickable;

			return state;
		}
	}
}
