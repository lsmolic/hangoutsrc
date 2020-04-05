using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
[System.SerializableAttribute]
public class UnassignedReferenceException : System.SystemException {
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

	public UnassignedReferenceException( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public UnassignedReferenceException( System.String message ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(String)" )){
			m_functionCallCounts.Add( "Void .ctor(String)", 0 );
		}
		m_functionCallCounts["Void .ctor(String)"]++;
			
	}

	public UnassignedReferenceException( System.String message, System.Exception innerException ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(String, Exception)" )){
			m_functionCallCounts.Add( "Void .ctor(String, Exception)", 0 );
		}
		m_functionCallCounts["Void .ctor(String, Exception)"]++;
			
	}
}
}
