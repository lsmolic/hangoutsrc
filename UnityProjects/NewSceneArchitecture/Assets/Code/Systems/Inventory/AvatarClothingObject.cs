/**  --------------------------------------------------------  *
 *   AvatarClothingObject.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/26/2009
 *	 
 *   --------------------------------------------------------  *
 */



namespace Hangout.Client
{
	public class AvatarClothingObject
	{
		private string mName;
		public string Name
		{
			set {mName = value;}
			get {return mName;}
		}
		
		public AvatarClothingObject(string name)
		{
			mName = name;
		}
		
		public AvatarClothingObject(BagItem bagItem)
		{
			mName = bagItem.ItemName;
		}
	}
}
