using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
[System.SerializableAttribute]
public class ModelImporterClipAnimation {
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

	public override System.Boolean Equals( System.Object o ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean Equals(System.Object)" )){
			m_functionCallCounts.Add( "Boolean Equals(System.Object)", 0 );
		}
		m_functionCallCounts["Boolean Equals(System.Object)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public override System.Int32 GetHashCode( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Int32 GetHashCode()" )){
			m_functionCallCounts.Add( "Int32 GetHashCode()", 0 );
		}
		m_functionCallCounts["Int32 GetHashCode()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public ModelImporterClipAnimation( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Int32 lastFrame {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_lastFrame" )){
				m_functionCallCounts.Add( "get_lastFrame", 0 );
			}
			m_functionCallCounts["get_lastFrame"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_lastFrame" )){
				m_functionCallCounts.Add( "set_lastFrame", 0 );
			}
			m_functionCallCounts["set_lastFrame"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String name {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_name" )){
				m_functionCallCounts.Add( "get_name", 0 );
			}
			m_functionCallCounts["get_name"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_name" )){
				m_functionCallCounts.Add( "set_name", 0 );
			}
			m_functionCallCounts["set_name"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean loop {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_loop" )){
				m_functionCallCounts.Add( "get_loop", 0 );
			}
			m_functionCallCounts["get_loop"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_loop" )){
				m_functionCallCounts.Add( "set_loop", 0 );
			}
			m_functionCallCounts["set_loop"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Int32 firstFrame {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_firstFrame" )){
				m_functionCallCounts.Add( "get_firstFrame", 0 );
			}
			m_functionCallCounts["get_firstFrame"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_firstFrame" )){
				m_functionCallCounts.Add( "set_firstFrame", 0 );
			}
			m_functionCallCounts["set_firstFrame"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
