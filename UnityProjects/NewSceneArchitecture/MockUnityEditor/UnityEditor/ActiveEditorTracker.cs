using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
[System.SerializableAttribute]
public class ActiveEditorTracker {
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

	public System.Int32 GetVisible( System.Int32 index ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Int32 GetVisible(Int32)" )){
			m_functionCallCounts.Add( "Int32 GetVisible(Int32)", 0 );
		}
		m_functionCallCounts["Int32 GetVisible(Int32)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void SetVisible( System.Int32 index, System.Int32 visible ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetVisible(Int32, Int32)" )){
			m_functionCallCounts.Add( "Void SetVisible(Int32, Int32)", 0 );
		}
		m_functionCallCounts["Void SetVisible(Int32, Int32)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void ClearDirty( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void ClearDirty()" )){
			m_functionCallCounts.Add( "Void ClearDirty()", 0 );
		}
		m_functionCallCounts["Void ClearDirty()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void RebuildIfNecessary( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void RebuildIfNecessary()" )){
			m_functionCallCounts.Add( "Void RebuildIfNecessary()", 0 );
		}
		m_functionCallCounts["Void RebuildIfNecessary()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void VerifyModifiedMonoBehaviours( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void VerifyModifiedMonoBehaviours()" )){
			m_functionCallCounts.Add( "Void VerifyModifiedMonoBehaviours()", 0 );
		}
		m_functionCallCounts["Void VerifyModifiedMonoBehaviours()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEditor.Editor MakeCustomEditor( UnityEngine.Object obj ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Boolean HasCustomEditor( UnityEngine.Object obj ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public ActiveEditorTracker( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEditor.Editor[] activeEditors {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_activeEditors" )){
				m_functionCallCounts.Add( "get_activeEditors", 0 );
			}
			m_functionCallCounts["get_activeEditors"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static UnityEditor.ActiveEditorTracker sharedTracker {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean isLocked {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isLocked" )){
				m_functionCallCounts.Add( "get_isLocked", 0 );
			}
			m_functionCallCounts["get_isLocked"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_isLocked" )){
				m_functionCallCounts.Add( "set_isLocked", 0 );
			}
			m_functionCallCounts["set_isLocked"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean isDirty {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isDirty" )){
				m_functionCallCounts.Add( "get_isDirty", 0 );
			}
			m_functionCallCounts["get_isDirty"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean debugMode {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_debugMode" )){
				m_functionCallCounts.Add( "get_debugMode", 0 );
			}
			m_functionCallCounts["get_debugMode"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_debugMode" )){
				m_functionCallCounts.Add( "set_debugMode", 0 );
			}
			m_functionCallCounts["set_debugMode"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
