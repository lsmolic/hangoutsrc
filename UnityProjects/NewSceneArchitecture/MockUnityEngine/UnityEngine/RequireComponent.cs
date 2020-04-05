using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
[System.AttributeUsageAttribute(AttributeTargets.All)]
public class RequireComponent : System.Attribute {
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

	public RequireComponent( System.Type requiredComponent ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Type)" )){
			m_functionCallCounts.Add( "Void .ctor(Type)", 0 );
		}
		m_functionCallCounts["Void .ctor(Type)"]++;
			
	}

	public RequireComponent( System.Type requiredComponent, System.Type requiredComponent2 ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Type, Type)" )){
			m_functionCallCounts.Add( "Void .ctor(Type, Type)", 0 );
		}
		m_functionCallCounts["Void .ctor(Type, Type)"]++;
			
	}

	public RequireComponent( System.Type requiredComponent, System.Type requiredComponent2, System.Type requiredComponent3 ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Type, Type, Type)" )){
			m_functionCallCounts.Add( "Void .ctor(Type, Type, Type)", 0 );
		}
		m_functionCallCounts["Void .ctor(Type, Type, Type)"]++;
			
	}
}
}
