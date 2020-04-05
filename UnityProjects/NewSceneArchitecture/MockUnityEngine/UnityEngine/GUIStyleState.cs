using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
[UnityEngine.SerializePrivateVariables]
[System.SerializableAttribute]
public class GUIStyleState {
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

	public GUIStyleState( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public UnityEngine.Color textColor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_textColor" )){
				m_functionCallCounts.Add( "get_textColor", 0 );
			}
			m_functionCallCounts["get_textColor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_textColor" )){
				m_functionCallCounts.Add( "set_textColor", 0 );
			}
			m_functionCallCounts["set_textColor"]++;
			
		}
	}

	public UnityEngine.Texture2D background {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_background" )){
				m_functionCallCounts.Add( "get_background", 0 );
			}
			m_functionCallCounts["get_background"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_background" )){
				m_functionCallCounts.Add( "set_background", 0 );
			}
			m_functionCallCounts["set_background"]++;
			
		}
	}
}
}