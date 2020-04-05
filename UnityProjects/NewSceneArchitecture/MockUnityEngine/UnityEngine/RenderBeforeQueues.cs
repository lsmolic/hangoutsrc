using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
[System.AttributeUsageAttribute(AttributeTargets.All)]
public class RenderBeforeQueues : System.Attribute {
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

	public RenderBeforeQueues( System.Int32[] args ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Int32[])" )){
			m_functionCallCounts.Add( "Void .ctor(Int32[])", 0 );
		}
		m_functionCallCounts["Void .ctor(Int32[])"]++;
			
	}
}
}
