using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class CustomEditor : System.Attribute 
{
	// Mock data:
	private Dictionary<string, int> m_functionCallCounts;
	public Dictionary<string, int> FunctionCallCounts 
    {
		get 
        { 
			if(m_functionCallCounts == null) 
            {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			return m_functionCallCounts;
		}
	}

	public CustomEditor( System.Type inspectedType )
    {
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Type)" ))
        {
			m_functionCallCounts.Add( "Void .ctor(Type)", 0 );
		}
		m_functionCallCounts["Void .ctor(Type)"]++;
	}
}
}
