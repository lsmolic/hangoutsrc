/*
   Created by Vilas Tewari on 2009-08-08.

	A PixelFilter that does not modify the source
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class ClearFilter : PixelFilter
	{
		public override Color FilterColor
		{
			get{ return Color.clear; }
			set{ }
		}
		
		/*
			Filter a pixel based on some parameters and return a color value
			This base class doesn't modify the pixel source
		*/
		public override HColor FilterPixel( HColor source )
		{
			return source;
		}
	}
}