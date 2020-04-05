using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class BitStream {
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

	public void Serialize( ref System.Boolean value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Boolean ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Boolean ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Boolean ByRef)"]++;
			
	}

	public void Serialize( ref System.Char value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Char ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Char ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Char ByRef)"]++;
			
	}

	public void Serialize( ref System.Int16 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Int16 ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Int16 ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Int16 ByRef)"]++;
			
	}

	public void Serialize( ref System.Int32 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Int32 ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Int32 ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Int32 ByRef)"]++;
			
	}

	public void Serialize( ref System.Single value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Single ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Single ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Single ByRef)"]++;
			
	}

	public void Serialize( ref System.Single value, System.Single maxDelta ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Single ByRef, Single)" )){
			m_functionCallCounts.Add( "Void Serialize(Single ByRef, Single)", 0 );
		}
		m_functionCallCounts["Void Serialize(Single ByRef, Single)"]++;
			
	}

	public void Serialize( ref UnityEngine.Quaternion value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Quaternion ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Quaternion ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Quaternion ByRef)"]++;
			
	}

	public void Serialize( ref UnityEngine.Quaternion value, System.Single maxDelta ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Quaternion ByRef, Single)" )){
			m_functionCallCounts.Add( "Void Serialize(Quaternion ByRef, Single)", 0 );
		}
		m_functionCallCounts["Void Serialize(Quaternion ByRef, Single)"]++;
			
	}

	public void Serialize( ref UnityEngine.Vector3 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Vector3 ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(Vector3 ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(Vector3 ByRef)"]++;
			
	}

	public void Serialize( ref UnityEngine.Vector3 value, System.Single maxDelta ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(Vector3 ByRef, Single)" )){
			m_functionCallCounts.Add( "Void Serialize(Vector3 ByRef, Single)", 0 );
		}
		m_functionCallCounts["Void Serialize(Vector3 ByRef, Single)"]++;
			
	}

	public void Serialize( ref UnityEngine.NetworkPlayer value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(NetworkPlayer ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(NetworkPlayer ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(NetworkPlayer ByRef)"]++;
			
	}

	public void Serialize( ref UnityEngine.NetworkViewID viewID ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Serialize(NetworkViewID ByRef)" )){
			m_functionCallCounts.Add( "Void Serialize(NetworkViewID ByRef)", 0 );
		}
		m_functionCallCounts["Void Serialize(NetworkViewID ByRef)"]++;
			
	}

	public BitStream( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Boolean isWriting {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isWriting" )){
				m_functionCallCounts.Add( "get_isWriting", 0 );
			}
			m_functionCallCounts["get_isWriting"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean isReading {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isReading" )){
				m_functionCallCounts.Add( "get_isReading", 0 );
			}
			m_functionCallCounts["get_isReading"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
