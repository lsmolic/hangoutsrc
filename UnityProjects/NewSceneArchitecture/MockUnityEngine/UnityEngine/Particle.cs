using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct Particle {
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

	public System.Single energy {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_energy" )){
				m_functionCallCounts.Add( "get_energy", 0 );
			}
			m_functionCallCounts["get_energy"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_energy" )){
				m_functionCallCounts.Add( "set_energy", 0 );
			}
			m_functionCallCounts["set_energy"]++;
			
		}
	}

	public UnityEngine.Color color {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_color" )){
				m_functionCallCounts.Add( "get_color", 0 );
			}
			m_functionCallCounts["get_color"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_color" )){
				m_functionCallCounts.Add( "set_color", 0 );
			}
			m_functionCallCounts["set_color"]++;
			
		}
	}

	public System.Single size {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_size" )){
				m_functionCallCounts.Add( "get_size", 0 );
			}
			m_functionCallCounts["get_size"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_size" )){
				m_functionCallCounts.Add( "set_size", 0 );
			}
			m_functionCallCounts["set_size"]++;
			
		}
	}

	public UnityEngine.Vector3 velocity {
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
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_velocity" )){
				m_functionCallCounts.Add( "set_velocity", 0 );
			}
			m_functionCallCounts["set_velocity"]++;
			
		}
	}

	public UnityEngine.Vector3 position {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_position" )){
				m_functionCallCounts.Add( "get_position", 0 );
			}
			m_functionCallCounts["get_position"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_position" )){
				m_functionCallCounts.Add( "set_position", 0 );
			}
			m_functionCallCounts["set_position"]++;
			
		}
	}
}
}