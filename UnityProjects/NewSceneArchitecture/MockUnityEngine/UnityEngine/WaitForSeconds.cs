using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class WaitForSeconds : UnityEngine.YieldInstruction {
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

	public WaitForSeconds( System.Single seconds ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Single)" )){
			m_functionCallCounts.Add( "Void .ctor(Single)", 0 );
		}
		m_functionCallCounts["Void .ctor(Single)"]++;
			
	}
}
}
