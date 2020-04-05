/*
   HSVA.cs
   Assets
   
   Created by Vilas Tewari on 2009-10-16.
   
	A value type for Hue Saturation Value
*/


using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public struct HSVA
	{
		public float h, s, v, a;

		public HSVA( float hue, float saturation, float value, float alpha )
		{
			h = hue;
			s = saturation;
			v = value;
			a = alpha;
		}
	}
}