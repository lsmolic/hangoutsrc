using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class DrawGizmo : System.Attribute {
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

	public DrawGizmo( UnityEditor.GizmoType gizmo ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(GizmoType)" )){
			m_functionCallCounts.Add( "Void .ctor(GizmoType)", 0 );
		}
		m_functionCallCounts["Void .ctor(GizmoType)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public DrawGizmo( UnityEditor.GizmoType gizmo, System.Type drawnGizmoType ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(GizmoType, Type)" )){
			m_functionCallCounts.Add( "Void .ctor(GizmoType, Type)", 0 );
		}
		m_functionCallCounts["Void .ctor(GizmoType, Type)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}
}
}
