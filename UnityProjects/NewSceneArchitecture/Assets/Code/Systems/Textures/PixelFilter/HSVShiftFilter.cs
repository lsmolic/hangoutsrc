/*
   Created by Vilas Tewari on 2009-08-08.

	A PixelSource Filter that simply shifts the source pixel by the given Hue and Saturation
	Value and Alpha areused as multipliers
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class HSVShiftFilter : PixelFilter
	{
		private HSVA mHSVA;
		
		/*
			Properties
		*/
		public override Color FilterColor
		{
			set
			{
				mHSVA = HueSaturationValue.ColorToHSV( value );
				MarkAsDirty();
			}
			get { return HueSaturationValue.HSVToColor( mHSVA ); }
		}
		
		public HSVShiftFilter() : base()
		{
			mHSVA = new HSVA( 0, 0, 1, 1 );
		}
		
		/* Filter a pixel based on some parameters and return a color value */
		public override HColor FilterPixel(HColor source)
		{
			HSVA pixel = HueSaturationValue.ColorToHSV(source);
			pixel.h = (pixel.h + mHSVA.h) % 360;
			pixel.s = Mathf.Clamp01(pixel.s + mHSVA.s);
			pixel.v = Mathf.Clamp01(pixel.v * mHSVA.v);
			pixel.a = Mathf.Clamp01(pixel.a * mHSVA.a);

			return HueSaturationValue.HSVToHColor(pixel);
		}
	}
}