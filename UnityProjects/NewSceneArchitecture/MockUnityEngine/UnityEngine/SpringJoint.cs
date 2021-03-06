using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class SpringJoint : UnityEngine.Joint {
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

	public SpringJoint( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Single damper {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_damper" )){
				m_functionCallCounts.Add( "get_damper", 0 );
			}
			m_functionCallCounts["get_damper"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_damper" )){
				m_functionCallCounts.Add( "set_damper", 0 );
			}
			m_functionCallCounts["set_damper"]++;
			
		}
	}

	public System.Single maxDistance {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_maxDistance" )){
				m_functionCallCounts.Add( "get_maxDistance", 0 );
			}
			m_functionCallCounts["get_maxDistance"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_maxDistance" )){
				m_functionCallCounts.Add( "set_maxDistance", 0 );
			}
			m_functionCallCounts["set_maxDistance"]++;
			
		}
	}

	public System.Single spring {
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

	public System.Single minDistance {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_minDistance" )){
				m_functionCallCounts.Add( "get_minDistance", 0 );
			}
			m_functionCallCounts["get_minDistance"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_minDistance" )){
				m_functionCallCounts.Add( "set_minDistance", 0 );
			}
			m_functionCallCounts["set_minDistance"]++;
			
		}
	}
}
}
