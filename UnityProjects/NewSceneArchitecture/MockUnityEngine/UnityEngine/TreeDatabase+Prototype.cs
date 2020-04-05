using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngineInternal {
public class Prototype {
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

	public void Cleanup( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Cleanup()" )){
			m_functionCallCounts.Add( "Void Cleanup()", 0 );
		}
		m_functionCallCounts["Void Cleanup()"]++;
			
	}

	public Prototype( UnityEngine.GameObject source, System.Single inBendFactor ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(GameObject, Single)" )){
			m_functionCallCounts.Add( "Void .ctor(GameObject, Single)", 0 );
		}
		m_functionCallCounts["Void .ctor(GameObject, Single)"]++;
			
	}
}
}
