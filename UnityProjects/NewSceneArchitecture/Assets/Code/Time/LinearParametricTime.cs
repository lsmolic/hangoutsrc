/*
	Created by Vilas Tewari on 2009-07-20.
	
	LinearParametricTime Default Output:
	
			__ 1
		  /
	0 __/
*/

using UnityEngine;
using System.Collections;

public class LinearParametricTime : ParametricTime {
	
	/*
		Constructors
	*/
	public LinearParametricTime() : base(){}
	public LinearParametricTime( float period ) : base( period ){}
	public LinearParametricTime( float period, ParametricAnimation.Modifier pModifier ) : base( period, pModifier ){}

	
	public override float CalculateParameter() {
		if ( IsDone ) {
			return 1;
		} else {
			return ElapsedTime / Period;
		}
	}
}
