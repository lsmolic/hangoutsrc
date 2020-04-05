/**  --------------------------------------------------------  *
 *   ColorUtilityTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/09/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Shared.UnitTest;

namespace Hangout.Client.UnitTest
{
	[TestFixture]
	public class ColorUtilityTest
	{
		[Test]
		public void ColorToHexVerification()
		{
			Color testColor = new Color(0.1f, 0.0f, 1.0f, 1.0f);
			Color testColor2 = new Color(1.0f, 0.5f, 0.33333f, 1.0f);
			Color testColor3 = Color.white;

			Color resultColor = ColorUtility.HexToColor(ColorUtility.ColorToHex(testColor));
			Color resultColor2 = ColorUtility.HexToColor(ColorUtility.ColorToHex(testColor2));
			Color resultColor3 = ColorUtility.HexToColor(ColorUtility.ColorToHex(testColor3));

			// The epsilon on these is because the color object internally converts to 0-255 integer and back to float.
			Assert.IsWithin(testColor.r, resultColor.r, 0.01f, "HexToColor and ColorToHex are not symmetrical");
			Assert.IsWithin(testColor.g, resultColor.g, 0.01f, "HexToColor and ColorToHex are not symmetrical");
			Assert.IsWithin(testColor.b, resultColor.b, 0.01f, "HexToColor and ColorToHex are not symmetrical");

			Assert.IsWithin(testColor2.r, resultColor2.r, 0.01f, "HexToColor and ColorToHex are not symmetrical");
			Assert.IsWithin(testColor2.g, resultColor2.g, 0.01f, "HexToColor and ColorToHex are not symmetrical");
			Assert.IsWithin(testColor2.b, resultColor2.b, 0.01f, "HexToColor and ColorToHex are not symmetrical");

			Assert.IsWithin(testColor3.r, resultColor3.r, 0.01f, "HexToColor and ColorToHex are not symmetrical");
			Assert.IsWithin(testColor3.g, resultColor3.g, 0.01f, "HexToColor and ColorToHex are not symmetrical");
			Assert.IsWithin(testColor3.b, resultColor3.b, 0.01f, "HexToColor and ColorToHex are not symmetrical");
		}

		[Test]
		public void ColorsAreParsedFromHex()
		{
			Color expectedColor = new Color(1.0f, 1.0f, 1.0f);
			Color testColor = ColorUtility.HexToColor("#FFFFFF");

			Assert.IsWithin(expectedColor.r, testColor.r, 0.01f);
			Assert.IsWithin(expectedColor.g, testColor.g, 0.01f);
			Assert.IsWithin(expectedColor.b, testColor.b, 0.01f);
		}
	}

}
