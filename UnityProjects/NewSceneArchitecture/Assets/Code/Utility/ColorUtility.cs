/**  --------------------------------------------------------  *
 *   ColorUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/09/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hangout.Client
{
	public class ColorUtility
	{
		private static Regex mHexToColorRx;

		static ColorUtility()
		{
			mHexToColorRx = new Regex(@"(?<1>[A-Fa-f0-9][A-Fa-f0-9])(?<2>[A-Fa-f0-9][A-Fa-f0-9])(?<3>[A-Fa-f0-9][A-Fa-f0-9])", RegexOptions.Compiled);
		}

		/** 
		 * Takes a string in the form of FFFFFF or #FFFFFF and builds a UnityEngine.Color
		 * /throws FormatException
		 */
		public static Color HexToColor(string hexColor)
		{
			Match m = mHexToColorRx.Match(hexColor);
			if (m.Success)
			{
				float componentScale = 1.0f / 255.0f;
				return new Color(int.Parse(m.Groups[1].ToString(), System.Globalization.NumberStyles.HexNumber) * componentScale,
								  int.Parse(m.Groups[2].ToString(), System.Globalization.NumberStyles.HexNumber) * componentScale,
								  int.Parse(m.Groups[3].ToString(), System.Globalization.NumberStyles.HexNumber) * componentScale,
								  1.0f);
			}

			throw new FormatException("Unable to convert hex string, " + hexColor + ", to a UnityEngine.Color");
		}

		public static string ColorToHex(Color color)
		{
			string result = "#";
			result += ((int)(color.r * 255.0f)).ToString("X2");
			result += ((int)(color.g * 255.0f)).ToString("X2");
			result += ((int)(color.b * 255.0f)).ToString("X2");
			return result;
		}
	}
}