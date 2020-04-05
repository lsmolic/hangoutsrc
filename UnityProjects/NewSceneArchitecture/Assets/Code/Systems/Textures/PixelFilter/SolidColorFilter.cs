/*
   Created by Vilas Tewari on 2009-08-08.

	A PixelSource Filter that uses the PixelSource alpha to return a solid color
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class SolidColorFilter : PixelFilter
	{
		private HColor mColor;
		
		/*
			Properies
		*/
		public override Color FilterColor
		{
			set
			{
				mColor = new HColor(value);
				MarkAsDirty();
			}
			get { return HColor.ToUnityColor(mColor); }
		}

		public SolidColorFilter()
			: base()
		{
			mColor = new HColor(1f, 1f, 1f, 1f);
		}
		
		/* Filter a pixel based on some parameters and return a color value */
		public override HColor FilterPixel(HColor source)
		{
			float alpha = Mathf.Floor(((source.a * mColor.a) / 65025f) * 255);
			return new HColor(mColor.r, mColor.g, mColor.b, (byte)alpha);
		}
	}
}