using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class GUILayoutOption {
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

	public GUILayoutOption( UnityEngine.Type type, System.Object value ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Type, Object)" )){
			m_functionCallCounts.Add( "Void .ctor(Type, Object)", 0 );
		}
		m_functionCallCounts["Void .ctor(Type, Object)"]++;
			
	}
}
}
