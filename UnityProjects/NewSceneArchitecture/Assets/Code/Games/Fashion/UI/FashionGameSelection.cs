/**  --------------------------------------------------------  *
 *   FashionGameSelection.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/10/2009
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
	public class FashionGameSelection
	{
		// TODO: Hard coded value
		private const int MAX_CLOTHING_STACK = 4;

		private readonly List<ClothingItem> mClothes = new List<ClothingItem>();
		private FashionModel mModel = null;

		public IEnumerable<ClothingItem> Clothes
		{
			get { return mClothes; }
		}

		public FashionModel Model
		{
			get { return mModel; }
		}

		public ClothingItem TopClothing
		{
			get 
			{
				ClothingItem result = null;
				if( mClothes.Count != 0 )
				{
					result = mClothes[0];
				}
				return result;
			}
		}

		public bool NeedsFixin
		{
			get
			{
				bool result = false;
				foreach( ClothingItem item in mClothes )
				{
					if( item.NeedsFixin )
					{
						result = true;
						break;
					}
				}
				return result;
			}
		}

		public bool RemoveClothing(ClothingItem item)
		{
			bool wasRemoved = false;
			List<ClothingItem> tempList = new List<ClothingItem>(mClothes);
			foreach (ClothingItem selectedItem in tempList)
			{
				if (selectedItem == item)
				{
					wasRemoved = true;
					mClothes.Remove(item);
					item.Hide();
					break;
				}
			}

			return wasRemoved;
		}

		public bool RemoveClothing(ItemId itemId)
		{
			bool wasRemoved = false;
			List<ClothingItem> tempList = new List<ClothingItem>(mClothes);
			foreach (ClothingItem item in tempList)
			{
				if(item.ItemId == itemId)
				{
					wasRemoved = true;
					mClothes.Remove(item);
					item.Hide();
					break;
				}
			}
			
			return wasRemoved;
		}

		private static bool mStackedClothing = false;
		public void Select(ClothingItem clothes)
		{
			if(clothes == null)
			{
				throw new ArgumentNullException("clothes");
			}

			clothes.Show();
			mClothes.Add(clothes);
			if( !mStackedClothing && mClothes.Count > 1 )
			{
				mStackedClothing = true;

				EventLogger.Log(LogGlobals.CATEGORY_FASHION_MINIGAME, LogGlobals.GAMEPLAY_BEHAVIOR, "FirstTime",
					LogGlobals.STACKED_CLOTHING);
			}

			if (mClothes.Count > MAX_CLOTHING_STACK)
			{
				mClothes[0].Hide();
				GameFacade.Instance.RetrieveMediator<FashionGameGui>().PutItemInGui(mClothes[0]); // Recycle the discarded clothes
				mClothes.RemoveAt(0);
			}
		}

		public void Select(FashionModel model)
		{
			if (model != null)
			{
				ClearModel();
				mModel = model;
				mModel.Select(delegate() { mModel = null; });
			}
		}

		public bool Select(ModelStation station)
		{
			bool modelSent = false;
			if (mModel != null)
			{
				modelSent = true;
				mModel.GoToStation(station);
				ClearSelection();
			}
			return modelSent;
		}

		// TODO: Hard coded values (Offset for each selected clothing item in the stack)
		private static readonly Vector3 CLOTHING_OFFSET = new Vector3(-0.005f, -0.005f, -0.005f);

		public void UpdateClothingPosition(Vector3 position)
		{
			Vector3 currentOffset = Vector3.zero;
			foreach(ClothingItem clothing in mClothes)
			{
				if(clothing.UnityGameObject == null)
				{
					continue;
				}

				clothing.UnityGameObject.transform.position = position;
				clothing.UnityGameObject.transform.position += currentOffset;
				currentOffset += CLOTHING_OFFSET;
			}
		}

		public void ClearSelection()
		{
			ClearModel();
			ClearClothes();
		}

		public void ClearModel()
		{
			if (mModel != null)
			{
				mModel.Unselect();
			}
		}

		public void ClearClothes()
		{
			foreach( ClothingItem clothing in mClothes )
			{
				GameFacade.Instance.RetrieveMediator<FashionGameGui>().PutItemInGui(clothing); // Recycle the discarded clothes
				clothing.Hide();
			}
			mClothes.Clear();
		}
	}
}
