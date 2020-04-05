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

public abstract class ParametricTime : System.Object {

	private float m_startTime;
	private float m_period;
	private ParametricAnimation.Modifier m_modifier;
	
	/*
		Properties
	*/
	public float P {
		get { return m_modifier( CalculateParameter() ); }
	}
	public float InverseP {
		get { return 1 - P; }
	}
	public float ElapsedTime {
		get { return Time.time - m_startTime; }
	}
	public float StartTime {
		get { return m_startTime; }
	}
	public float Period {
		get { return m_period; }
	}
	public bool IsDone {
		get { return ElapsedTime > Period; }
	}
	
	/*
		Constructor
	*/
	public ParametricTime() {
		m_period = 1f;
		m_modifier = ParametricAnimation.Default;
		Reset();
	}
	public ParametricTime( float period ) {
		m_period = period;
		m_modifier = ParametricAnimation.Default;
		Reset();
	}
	public ParametricTime( float period, ParametricAnimation.Modifier pModifier ) {
		m_period = period;
		m_modifier = pModifier;
		Reset();
	}
	
	/*
		Reset Parametric Time
	*/
	public void Reset() {
		m_startTime = Time.time;
	}
	
	/*
		Heart of Parametric Time
	*/
	public abstract float CalculateParameter();
}
