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
	public interface IFashionModelState : IState
	{
		/// <summary>
		/// Should the model be clickable in this state?
		/// </summary>
		bool Clickable { get; }

		/// <summary>
		/// The model this state is describing
		/// </summary>
		FashionModel Model { get; }

		int CollisionCheckLayer { get; }
	}
}
