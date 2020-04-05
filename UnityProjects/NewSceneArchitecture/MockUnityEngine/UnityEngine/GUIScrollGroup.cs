using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class GUIScrollGroup : UnityEngine.GUILayoutGroup {
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

	public override void CalcWidth( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void CalcWidth()" )){
			m_functionCallCounts.Add( "Void CalcWidth()", 0 );
		}
		m_functionCallCounts["Void CalcWidth()"]++;
			
	}

	public override void SetHorizontal( System.Single x, System.Single width ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetHorizontal(Single, Single)" )){
			m_functionCallCounts.Add( "Void SetHorizontal(Single, Single)", 0 );
		}
		m_functionCallCounts["Void SetHorizontal(Single, Single)"]++;
			
	}

	public override void CalcHeight( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void CalcHeight()" )){
			m_functionCallCounts.Add( "Void CalcHeight()", 0 );
		}
		m_functionCallCounts["Void CalcHeight()"]++;
			
	}

	public override void SetVertical( System.Single y, System.Single height ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetVertical(Single, Single)" )){
			m_functionCallCounts.Add( "Void SetVertical(Single, Single)", 0 );
		}
		m_functionCallCounts["Void SetVertical(Single, Single)"]++;
			
	}

	public GUIScrollGroup( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}
}
}
