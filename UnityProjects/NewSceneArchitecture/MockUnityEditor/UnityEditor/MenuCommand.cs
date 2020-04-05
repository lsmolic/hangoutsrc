using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class MenuCommand {
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

	public MenuCommand( UnityEngine.Object inContext, System.Int32 inUserData ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Object, Int32)" )){
			m_functionCallCounts.Add( "Void .ctor(Object, Int32)", 0 );
		}
		m_functionCallCounts["Void .ctor(Object, Int32)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public MenuCommand( UnityEngine.Object inContext ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Object)" )){
			m_functionCallCounts.Add( "Void .ctor(Object)", 0 );
		}
		m_functionCallCounts["Void .ctor(Object)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}
}
}