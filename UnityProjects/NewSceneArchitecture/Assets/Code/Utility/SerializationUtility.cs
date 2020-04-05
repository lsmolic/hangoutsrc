/**  --------------------------------------------------------  *
 *   FashionGameProxy.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/21/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Hangout.Client
{
	public static class SerializationUtility
	{
		public static Vector3 ToVector3(string vector3Str)
		{
			string[] vector3StrSplit = vector3Str.Split();
			if( vector3StrSplit.Length != 3 )
			{
				throw new FormatException("Unable to parse string (" + vector3Str + ") to a Vector3"); 
			}

			return new Vector3
			(
				float.Parse(vector3StrSplit[0]),
				float.Parse(vector3StrSplit[1]),
				float.Parse(vector3StrSplit[2])
			);
		}

		public static string ToString(Vector3 vector3)
		{
			return String.Format("{0} {1} {2}", vector3.x.ToString("R"), vector3.y.ToString("R"), vector3.z.ToString("R"));
		}
	}
}
