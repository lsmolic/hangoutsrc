/**  --------------------------------------------------------  *
 *   FashionModelNeeds.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/23/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client;
using Hangout.Client.Gui;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	/// <summary>
	/// All the Clothes and Stations that a single model requires
	/// </summary>
	public class FashionModelNeeds
	{
		private float mNeedFixinChance;
		public float NeedFixinChance
		{
			get { return mNeedFixinChance; }
			set { mNeedFixinChance = value; }
		}

		public int ClothingLeft
		{
			get { return mClothing.Count; }
		}

		private readonly List<ItemId> mClothing = new List<ItemId>();
		public ICollection<ItemId> Clothing
		{
			get { return mClothing; }
		}

		private readonly List<ModelStation> mStations = new List<ModelStation>();
		public ICollection<ModelStation> Stations
		{
			get { return mStations; }
		}

		public void Add(ItemId clothing)
		{
			mClothing.Add(clothing);
		}

		public void Add(ModelStation station)
		{
			mStations.Add(station);
		}

		public void Remove(ItemId clothing)
		{
			mClothing.Remove(clothing);
			CheckComplete();
		}

		public void Remove(ModelStation station)
		{
			mStations.Remove(station);
			CheckComplete();
		}

		private void CheckComplete()
		{
			if( mStations.Count + mClothing.Count == 0 && mOnComplete != null)
			{
				mOnComplete();
				mOnComplete = null;
			}
		}

		private Hangout.Shared.Action mOnComplete = null;
		public void AddOnCompleteAction(Hangout.Shared.Action onComplete)
		{
			mOnComplete += onComplete;
		}
	}
}
