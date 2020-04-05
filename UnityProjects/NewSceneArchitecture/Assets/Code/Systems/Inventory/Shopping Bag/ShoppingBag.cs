/**  --------------------------------------------------------  *
 *   ShoppingBag.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/25/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class ShoppingBag
	{
		private List<BagItem> mItemsInBag = new List<BagItem>();
		public List<BagItem> ItemsInBag
		{
			get {return mItemsInBag;}
		}
		
		public ShoppingBag()
		{
			
		}
		
		public void AddItemToBag(BagItem addItem)
		{
			if (addItem == null)
			{
				throw new ArgumentNullException("addItem");
			}
			mItemsInBag.Add(addItem);
		}
		
		public void RemoveItemFromBag(BagItem removeItem)
		{
			if (removeItem == null)
			{
				throw new ArgumentNullException("removeItem");
			}
			if (!mItemsInBag.Remove(removeItem))
			{
				throw new InvalidOperationException("removeItem(" + removeItem.ItemName + ") Was not in the shopping bag.");
			}
		}
		
		public void EmptyShoppingBag()
		{
			mItemsInBag.Clear();
		}
	}
}
