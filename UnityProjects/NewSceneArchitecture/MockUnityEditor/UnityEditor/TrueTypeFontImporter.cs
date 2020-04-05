using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class TrueTypeFontImporter : UnityEditor.AssetImporter {
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

	public TrueTypeFontImporter( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEditor.FontTextureCase fontTextureCase {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_fontTextureCase" )){
				m_functionCallCounts.Add( "get_fontTextureCase", 0 );
			}
			m_functionCallCounts["get_fontTextureCase"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_fontTextureCase" )){
				m_functionCallCounts.Add( "set_fontTextureCase", 0 );
			}
			m_functionCallCounts["set_fontTextureCase"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEditor.FontRenderMode fontRenderMode {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_fontRenderMode" )){
				m_functionCallCounts.Add( "get_fontRenderMode", 0 );
			}
			m_functionCallCounts["get_fontRenderMode"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_fontRenderMode" )){
				m_functionCallCounts.Add( "set_fontRenderMode", 0 );
			}
			m_functionCallCounts["set_fontRenderMode"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Int32 fontSize {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_fontSize" )){
				m_functionCallCounts.Add( "get_fontSize", 0 );
			}
			m_functionCallCounts["get_fontSize"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_fontSize" )){
				m_functionCallCounts.Add( "set_fontSize", 0 );
			}
			m_functionCallCounts["set_fontSize"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
