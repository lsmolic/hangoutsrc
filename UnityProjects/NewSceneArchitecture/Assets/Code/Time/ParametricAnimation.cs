/*
	Created by Vilas Tewari on 2009-07-20.
   
	ParametricTime is the base class for all objects that
	return a parameter P such that 0 <= p <= 1, based on elapsed time and a period.
	
	ParametricTime is used mostly for animation.
	
	A ParamtericTime Modifier can bias the output of a ParametricTime Object
	eg. a modifier can ease in, out, bounce etc.. the parameter P
*/

using UnityEngine;
using System.Collections;

public static class ParametricAnimation {
	
	/*
		A function that can modify the output Parameter P
	*/
	public delegate float Modifier( float p );

	public static float ToRange( float p, float newMin, float newMax ) {
		return ( ( newMax - newMin ) * p ) + newMin;
	}
	
	public static float NormalizeRange( float p, float oldMin, float oldMax ) {
		return ( p - oldMin )/( oldMax - oldMin );
	}
	
	/*
		PARAMETER MODIFIERS
	*/
	public static float Default( float p ){
		return p;
	}
	
	public static float EaseInAndOut( float p ){
		return ( - Mathf.Cos( p * Mathf.PI ) + 1) / 2f;
	}
	
	public static float EaseIn( float p ){
		return Mathf.Pow( p, 3 );
	}
	
	public static float EaseOut( float p ){
		return 1 - Mathf.Abs( Mathf.Pow( p-1, 3 ) );
	}
	
	public static float Bounce( float p ) {
        return Mathf.Abs( Mathf.Sin(6.28f * (p + 1f) * (p + 1f)) * (1f - p) );
    }

	public static float Sin( float p ) {
		// Convert 0 -> 1 to 0 -> 2PI
        return ( Mathf.Sin( p * 2 * Mathf.PI ) + 1 ) / 2;
    }

	public static float ElasticEaseOut( float p ) {
        return NormalizeRange( Sin( ToRange( EaseOut(p), 0.2f, 0.8f )), Sin(0.8f), Sin(0.2f));
    }
}
