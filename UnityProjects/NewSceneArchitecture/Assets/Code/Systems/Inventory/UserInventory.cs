/**  --------------------------------------------------------  *
 *   UserInventory.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/25/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;


namespace Hangout.Client
{
	public class UserInventory
	{
		private List<AvatarClothingObject> mAvatarClothingObjects = new List<AvatarClothingObject>();
		public List<AvatarClothingObject> AvatarClothingObjects
		{
			get {return mAvatarClothingObjects;}
		}
		
		public UserInventory()
		{
			GetInventoryFromDb();
		}
		
		private void GetInventoryFromDb()
		{
			//We'd get the inventory from the DB here, obviously.
		}
		
		public void AddAvatarClothingObject(AvatarClothingObject clothingObject)
		{
			if (clothingObject == null)
			{
				throw new ArgumentNullException("clothingObject ");
			}
			mAvatarClothingObjects.Add(clothingObject);
		}
		
		public void RemoveAvatarClothingObject(AvatarClothingObject clothingObjectToRemove)
		{
			if ( !mAvatarClothingObjects.Remove(clothingObjectToRemove) )
			{
				throw new ArgumentException( "Avatar Clothing Object trying to be removed does not exist in the user's inventory.");
			}
		}
	}
}
