/**  --------------------------------------------------------  *
 *   FashionModelInfo.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;

namespace Hangout.Client.FashionGame
{
	public class FashionModelInfo
	{
		private readonly float mNeedFixinChance;
		public float NeedFixinChance
		{
			get { return mNeedFixinChance; }
		}

		private readonly string mName;
		public string Name
		{
			get { return mName; }
		}
		private readonly float mSpeed;
		public float Speed
		{
			get { return mSpeed; }
		}

		private readonly Range<int> mClothingNeeds;
		public Range<int> ClothingNeeds
		{
			get { return mClothingNeeds; }
		}

		private readonly Range<int> mStationNeeds;
		public Range<int> StationNeeds
		{
			get { return mStationNeeds; }
		}

		public FashionModelInfo(XmlNode modelNode)
		{
			XmlNode nameAttrib = modelNode.SelectSingleNode("@name");
			if (nameAttrib == null)
			{
				throw new Exception("Unable to find the name attribute in node (" + modelNode.Name + ")");
			}
			mName = nameAttrib.InnerText;

			XmlNode speedValueAttrib = modelNode.SelectSingleNode("Speed/@value");
			if (speedValueAttrib == null)
			{
				throw new Exception("Unable to find the Speed node's value attribute under node (" + modelNode.Name + ")");
			}

			XmlNode clothingMinAttrib = modelNode.SelectSingleNode("ClothingNeeds/@min");
			if (clothingMinAttrib == null)
			{
				throw new Exception("Unable to find the ClothingNeeds node's min attribute under node (" + modelNode.Name + ")");
			}

			XmlNode clothingMaxAttrib = modelNode.SelectSingleNode("ClothingNeeds/@max");
			if (clothingMaxAttrib == null)
			{
				throw new Exception("Unable to find the ClothingNeeds node's max attribute under node (" + modelNode.Name + ")");
			}

			XmlNode fixinChanceAttrib = modelNode.SelectSingleNode("ClothingNeeds/@needFixinChance");
			if (clothingMaxAttrib != null)
			{
				mNeedFixinChance = float.Parse(fixinChanceAttrib.InnerText);
			}
			else
			{
				mNeedFixinChance = 0.0f;
			}

			XmlNode stationMinAttrib = modelNode.SelectSingleNode("StationNeeds/@min");
			if (stationMinAttrib == null)
			{
				throw new Exception("Unable to find the StationNeeds node's min attribute under node (" + modelNode.Name + ")");
			}

			XmlNode stationMaxAttrib = modelNode.SelectSingleNode("StationNeeds/@max");
			if (stationMaxAttrib == null)
			{
				throw new Exception("Unable to find the StationNeeds node's max attribute under node (" + modelNode.Name + ")");
			}

			if (!float.TryParse(speedValueAttrib.InnerText, out mSpeed))
			{
				throw new FormatException("Unable to parse the Speed value node's value (" + speedValueAttrib.InnerText + ") to a number.");
			}

			int min;
			int max;
			if (int.TryParse(clothingMinAttrib.InnerText, out min) && int.TryParse(clothingMaxAttrib.InnerText, out max))
			{
				mClothingNeeds = new Range<int>(min, max);
			}
			else
			{
				throw new FormatException("The values for both ClothingNeeds min (" + clothingMinAttrib.InnerText + ") and max (" + clothingMaxAttrib.InnerText + ") must parse to integers");
			}

			if (int.TryParse(stationMinAttrib.InnerText, out min) && int.TryParse(stationMaxAttrib.InnerText, out max))
			{
				mStationNeeds = new Range<int>(min, max);
			}
			else
			{
				throw new FormatException("The values for both StationNeeds min (" + stationMinAttrib.InnerText + ") and max (" + stationMaxAttrib.InnerText + ") must parse to integers");
			}
		}
	}
}
