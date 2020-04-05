using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class GUILayoutGroup : UnityEngine.GUILayoutEntry {
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

	public override void ApplyOptions( UnityEngine.GUILayoutOption[] options ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void ApplyOptions(UnityEngine.GUILayoutOption[])" )){
			m_functionCallCounts.Add( "Void ApplyOptions(UnityEngine.GUILayoutOption[])", 0 );
		}
		m_functionCallCounts["Void ApplyOptions(UnityEngine.GUILayoutOption[])"]++;
			
	}

	public void ResetCursor( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void ResetCursor()" )){
			m_functionCallCounts.Add( "Void ResetCursor()", 0 );
		}
		m_functionCallCounts["Void ResetCursor()"]++;
			
	}

	public UnityEngine.GUILayoutEntry GetNext( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.GUILayoutEntry GetNext()" )){
			m_functionCallCounts.Add( "UnityEngine.GUILayoutEntry GetNext()", 0 );
		}
		m_functionCallCounts["UnityEngine.GUILayoutEntry GetNext()"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Rect GetLast( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Rect GetLast()" )){
			m_functionCallCounts.Add( "Rect GetLast()", 0 );
		}
		m_functionCallCounts["Rect GetLast()"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void Add( UnityEngine.GUILayoutEntry e ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Add(UnityEngine.GUILayoutEntry)" )){
			m_functionCallCounts.Add( "Void Add(UnityEngine.GUILayoutEntry)", 0 );
		}
		m_functionCallCounts["Void Add(UnityEngine.GUILayoutEntry)"]++;
			
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

	public override System.String ToString( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "System.String ToString()" )){
			m_functionCallCounts.Add( "System.String ToString()", 0 );
		}
		m_functionCallCounts["System.String ToString()"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public GUILayoutGroup( ) 
		: base(0.0f, 0.0f, 0.0f ,0.0f, null) {
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public GUILayoutGroup(UnityEngine.GUIStyle _style, UnityEngine.GUILayoutOption[] options)
		: base(0.0f, 0.0f, 0.0f, 0.0f, null) {
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(GUIStyle, GUILayoutOption[])" )){
			m_functionCallCounts.Add( "Void .ctor(GUIStyle, GUILayoutOption[])", 0 );
		}
		m_functionCallCounts["Void .ctor(GUIStyle, GUILayoutOption[])"]++;
			
	}

	public override UnityEngine.RectOffset margin {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_margin" )){
				m_functionCallCounts.Add( "get_margin", 0 );
			}
			m_functionCallCounts["get_margin"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
