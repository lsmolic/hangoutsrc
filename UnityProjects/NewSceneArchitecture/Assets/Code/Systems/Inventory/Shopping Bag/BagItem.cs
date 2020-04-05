/**  --------------------------------------------------------  *
 *   BagItem.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/24/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class BagItem
	{
	
		private string mItemName;
		public string ItemName
		{
			get {return mItemName;}
		}
		
		private string mItemPrice = "";
		public string ItemPrice
		{
			get {return mItemPrice;}
		}		
		public BagItem(string itemName)
		{
			mItemName = itemName;
		}
		public BagItem(BagItem copy)
		{
			mItemName = copy.ItemName;
		}
	}
}
