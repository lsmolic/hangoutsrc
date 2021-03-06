using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class AnimationClip : UnityEngine.Object {
	// Mock data:
	private Dictionary<string, int> m_functionCallCounts;
	public Dictionary<string, int> FunctionCallCounts {
		get { 
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			return m_functionCallCounts;
		}
	}

	public void SetCurve( System.String relativePath, System.Type type, System.String propertyName, UnityEngine.AnimationCurve curve ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetCurve(System.String, System.Type, System.String, UnityEngine.AnimationCurve)" )){
			m_functionCallCounts.Add( "Void SetCurve(System.String, System.Type, System.String, UnityEngine.AnimationCurve)", 0 );
		}
		m_functionCallCounts["Void SetCurve(System.String, System.Type, System.String, UnityEngine.AnimationCurve)"]++;
			
	}

	public void EnsureQuaternionContinuity( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void EnsureQuaternionContinuity()" )){
			m_functionCallCounts.Add( "Void EnsureQuaternionContinuity()", 0 );
		}
		m_functionCallCounts["Void EnsureQuaternionContinuity()"]++;
			
	}

	public void ClearCurves( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void ClearCurves()" )){
			m_functionCallCounts.Add( "Void ClearCurves()", 0 );
		}
		m_functionCallCounts["Void ClearCurves()"]++;
			
	}

	public void AddEvent( UnityEngine.AnimationEvent theEvent ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddEvent(UnityEngine.AnimationEvent)" )){
			m_functionCallCounts.Add( "Void AddEvent(UnityEngine.AnimationEvent)", 0 );
		}
		m_functionCallCounts["Void AddEvent(UnityEngine.AnimationEvent)"]++;
			
	}

	public AnimationClip( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Single frameRate {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_frameRate" )){
				m_functionCallCounts.Add( "get_frameRate", 0 );
			}
			m_functionCallCounts["get_frameRate"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Single length {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_length" )){
				m_functionCallCounts.Add( "get_length", 0 );
			}
			m_functionCallCounts["get_length"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
