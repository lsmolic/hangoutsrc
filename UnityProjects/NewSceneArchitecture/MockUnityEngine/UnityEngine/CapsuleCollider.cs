using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class CapsuleCollider : UnityEngine.Collider {
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

	public CapsuleCollider( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Int32 direction {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_direction" )){
				m_functionCallCounts.Add( "get_direction", 0 );
			}
			m_functionCallCounts["get_direction"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_direction" )){
				m_functionCallCounts.Add( "set_direction", 0 );
			}
			m_functionCallCounts["set_direction"]++;
			
		}
	}

	public System.Single height {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_height" )){
				m_functionCallCounts.Add( "get_height", 0 );
			}
			m_functionCallCounts["get_height"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_height" )){
				m_functionCallCounts.Add( "set_height", 0 );
			}
			m_functionCallCounts["set_height"]++;
			
		}
	}

	public UnityEngine.Vector3 center {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_center" )){
				m_functionCallCounts.Add( "get_center", 0 );
			}
			m_functionCallCounts["get_center"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_center" )){
				m_functionCallCounts.Add( "set_center", 0 );
			}
			m_functionCallCounts["set_center"]++;
			
		}
	}

	public System.Single radius {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_radius" )){
				m_functionCallCounts.Add( "get_radius", 0 );
			}
			m_functionCallCounts["get_radius"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_radius" )){
				m_functionCallCounts.Add( "set_radius", 0 );
			}
			m_functionCallCounts["set_radius"]++;
			
		}
	}
}
}