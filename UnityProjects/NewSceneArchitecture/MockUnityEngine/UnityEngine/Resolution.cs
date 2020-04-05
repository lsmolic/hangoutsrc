using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct Resolution {
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

	public System.Int32 refreshRate {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_refreshRate" )){
				m_functionCallCounts.Add( "get_refreshRate", 0 );
			}
			m_functionCallCounts["get_refreshRate"]++;

			return 60;
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_refreshRate" )){
				m_functionCallCounts.Add( "set_refreshRate", 0 );
			}
			m_functionCallCounts["set_refreshRate"]++;
			
		}
	}

	public System.Int32 height {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_height" )){
				m_functionCallCounts.Add( "get_height", 0 );
			}
			m_functionCallCounts["get_height"]++;

			return 640;
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

	public System.Int32 width {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_width" )){
				m_functionCallCounts.Add( "get_width", 0 );
			}
			m_functionCallCounts["get_width"]++;

			return 480;
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_width" )){
				m_functionCallCounts.Add( "set_width", 0 );
			}
			m_functionCallCounts["set_width"]++;
			
		}
	}
}
}
