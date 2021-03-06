using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
[UnityEngine.SerializePrivateVariables]
[System.SerializableAttribute]
public class GUISettings {
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

	public GUISettings( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public UnityEngine.Color cursorColor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_cursorColor" )){
				m_functionCallCounts.Add( "get_cursorColor", 0 );
			}
			m_functionCallCounts["get_cursorColor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_cursorColor" )){
				m_functionCallCounts.Add( "set_cursorColor", 0 );
			}
			m_functionCallCounts["set_cursorColor"]++;
			
		}
	}

	public System.Single cursorFlashSpeed {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_cursorFlashSpeed" )){
				m_functionCallCounts.Add( "get_cursorFlashSpeed", 0 );
			}
			m_functionCallCounts["get_cursorFlashSpeed"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_cursorFlashSpeed" )){
				m_functionCallCounts.Add( "set_cursorFlashSpeed", 0 );
			}
			m_functionCallCounts["set_cursorFlashSpeed"]++;
			
		}
	}

	public System.Boolean tripleClickSelectsLine {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_tripleClickSelectsLine" )){
				m_functionCallCounts.Add( "get_tripleClickSelectsLine", 0 );
			}
			m_functionCallCounts["get_tripleClickSelectsLine"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_tripleClickSelectsLine" )){
				m_functionCallCounts.Add( "set_tripleClickSelectsLine", 0 );
			}
			m_functionCallCounts["set_tripleClickSelectsLine"]++;
			
		}
	}

	public System.Boolean doubleClickSelectsWord {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_doubleClickSelectsWord" )){
				m_functionCallCounts.Add( "get_doubleClickSelectsWord", 0 );
			}
			m_functionCallCounts["get_doubleClickSelectsWord"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_doubleClickSelectsWord" )){
				m_functionCallCounts.Add( "set_doubleClickSelectsWord", 0 );
			}
			m_functionCallCounts["set_doubleClickSelectsWord"]++;
			
		}
	}

	public UnityEngine.Color selectionColor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_selectionColor" )){
				m_functionCallCounts.Add( "get_selectionColor", 0 );
			}
			m_functionCallCounts["get_selectionColor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_selectionColor" )){
				m_functionCallCounts.Add( "set_selectionColor", 0 );
			}
			m_functionCallCounts["set_selectionColor"]++;
			
		}
	}
}
}
