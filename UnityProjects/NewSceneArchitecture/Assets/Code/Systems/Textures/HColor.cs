/* Vilas T.
 * 10/28/09
 * Summary:  Custom color class that has a lower memory footprint than UnityEngine.Color.
 */

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public struct HColor
	{
		public byte r, g, b, a;
		
		public HColor( Color color )
		{
			r = (byte) (int)( color.r*255 );
			g = (byte) (int)( color.g*255 );
			b = (byte) (int)( color.b*255 );
			a = (byte) (int)( color.a*255 );
		}
		
		public HColor( byte rVal, byte gVal, byte bVal, byte aVal )
		{
			r = rVal;
			g = gVal;
			b = bVal;
			a = aVal;
		}
		
		public HColor( float rVal, float gVal, float bVal, float aVal )
		{
			r = (byte) (int)( rVal*255 );
			g = (byte) (int)( gVal*255 );
			b = (byte) (int)( bVal*255 );
			a = (byte) (int)( aVal*255 );
		}
		
		public static HColor Add( HColor c1, HColor c2 )
		{
			return new HColor( c1.r + c2.g, c1.b + c2.g, c1.b + c2.b, c1.a + c2.a );
		}
		
		public static HColor Multiply( HColor c1, HColor c2 )
		{
			return new HColor( (byte)((c1.r * c2.r)/255), (byte)((c1.g * c2.g)/255), (byte)((c1.b * c2.b)/255), (byte)((c1.a * c2.a)/255));
		}
		
		public static bool IsWhite( HColor c )
		{
			return ( c.r == 255 && c.g == 255 && c.b == 255 );
		}
		
		public static Color ToUnityColor( HColor c )
		{
			return new Color( c.r/255f, c.g/255f, c.b/255f, c.a/255f );
		}
	}
}