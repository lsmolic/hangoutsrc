using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class MonoScript : UnityEngine.TextAsset {
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

	public System.Type GetClass( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "System.Type GetClass()" )){
			m_functionCallCounts.Add( "System.Type GetClass()", 0 );
		}
		m_functionCallCounts["System.Type GetClass()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public MonoScript( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}
}
}
