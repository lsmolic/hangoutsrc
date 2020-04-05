/*
   Created by Vilas Tewari on 2009-08-08.

	A PixelFilter sets the hue and saturation of the source pixel to the given
	values. Value is applied as a power
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class WhitePreservingColorFilter : PixelFilter
	{
		private HSVA mHSVA;
		
		/*
			Properies
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
		
		public WhitePreservingColorFilter() : base()
		{
			mHSVA = new HSVA(0, 1f, 1f, 1f);
		}
		
		/* Filter a pixel based on some parameters and return a color value */
		public override HColor FilterPixel(HColor source)
		{
			HSVA pixel = HueSaturationValue.ColorToHSV(source);

			/* Set the source pixel Hue */
			pixel.h = mHSVA.h;

			/*
				Assume the source pixel has saturation 1 and subtract inverse of saturation from it
				This subtractive process will not saturate white pixels like highlites
			*/
			pixel.s = Mathf.Clamp01(pixel.s - (1 - mHSVA.s));

			/*
				Use the value as an exponent to darken pixels
				This method does not darken white pixels ( to maintain highlights )
			*/
			pixel.v = Mathf.Clamp01(Mathf.Pow(pixel.v, 1 + (1 - (mHSVA.v)) * 2.5f));

			/* Apply alpha as a multiplier */
			pixel.a *= mHSVA.a;

			return HueSaturationValue.HSVToHColor(pixel);
		}
	}
}