using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class HingeJoint : UnityEngine.Joint {
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

	public HingeJoint( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Single angle {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_angle" )){
				m_functionCallCounts.Add( "get_angle", 0 );
			}
			m_functionCallCounts["get_angle"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean useLimits {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_useLimits" )){
				m_functionCallCounts.Add( "get_useLimits", 0 );
			}
			m_functionCallCounts["get_useLimits"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_useLimits" )){
				m_functionCallCounts.Add( "set_useLimits", 0 );
			}
			m_functionCallCounts["set_useLimits"]++;
			
		}
	}

	public System.Boolean useSpring {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_useSpring" )){
				m_functionCallCounts.Add( "get_useSpring", 0 );
			}
			m_functionCallCounts["get_useSpring"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_useSpring" )){
				m_functionCallCounts.Add( "set_useSpring", 0 );
			}
			m_functionCallCounts["set_useSpring"]++;
			
		}
	}

	public System.Boolean useMotor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_useMotor" )){
				m_functionCallCounts.Add( "get_useMotor", 0 );
			}
			m_functionCallCounts["get_useMotor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_useMotor" )){
				m_functionCallCounts.Add( "set_useMotor", 0 );
			}
			m_functionCallCounts["set_useMotor"]++;
			
		}
	}

	public UnityEngine.JointLimits limits {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_limits" )){
				m_functionCallCounts.Add( "get_limits", 0 );
			}
			m_functionCallCounts["get_limits"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_limits" )){
				m_functionCallCounts.Add( "set_limits", 0 );
			}
			m_functionCallCounts["set_limits"]++;
			
		}
	}

	public System.Single velocity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_velocity" )){
				m_functionCallCounts.Add( "get_velocity", 0 );
			}
			m_functionCallCounts["get_velocity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.JointSpring spring {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_spring" )){
				m_functionCallCounts.Add( "get_spring", 0 );
			}
			m_functionCallCounts["get_spring"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_spring" )){
				m_functionCallCounts.Add( "set_spring", 0 );
			}
			m_functionCallCounts["set_spring"]++;
			
		}
	}

	public UnityEngine.JointMotor motor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_motor" )){
				m_functionCallCounts.Add( "get_motor", 0 );
			}
			m_functionCallCounts["get_motor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_motor" )){
				m_functionCallCounts.Add( "set_motor", 0 );
			}
			m_functionCallCounts["set_motor"]++;
			
		}
	}
}
}
