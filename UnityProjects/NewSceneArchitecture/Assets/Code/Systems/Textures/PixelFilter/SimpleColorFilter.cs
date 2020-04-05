/*
   Created by Vilas Tewari on 2009-08-08.

	A PixelFilter sets the hue and saturation of the source pixel to the given
	values. Value is applied as a powe
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public class SimpleColorFilter : PixelFilter
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
		
		public SimpleColorFilter() : base()
		{
			mHSVA = new HSVA( 0f, 1f, 1f, 1f );
		}
		
		/* Filter a pixel based on some parameters and return a color value */
		public override HColor FilterPixel(HColor source)
		{
			HSVA pixel = HueSaturationValue.ColorToHSV(source);

			/* Set the source pixel Hue */
			pixel.h = mHSVA.h;

			pixel.s = pixel.s * mHSVA.s;
			pixel.v = pixel.v * mHSVA.v;

			/* Apply alpha as a multiplier */
			pixel.a *= mHSVA.a;

			return HueSaturationValue.HSVToHColor(pixel);
		}
	}
}