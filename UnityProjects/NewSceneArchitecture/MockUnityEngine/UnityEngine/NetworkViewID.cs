using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct NetworkViewID {
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
	}

	public override System.Boolean Equals( System.Object other ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean Equals(System.Object)" )){
			m_functionCallCounts.Add( "Boolean Equals(System.Object)", 0 );
		}
		m_functionCallCounts["Boolean Equals(System.Object)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
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

	public static System.Boolean operator ==( UnityEngine.NetworkViewID lhs, UnityEngine.NetworkViewID rhs ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Boolean operator !=( UnityEngine.NetworkViewID lhs, UnityEngine.NetworkViewID rhs ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.NetworkPlayer owner {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_owner" )){
				m_functionCallCounts.Add( "get_owner", 0 );
			}
			m_functionCallCounts["get_owner"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static UnityEngine.NetworkViewID unassigned {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean isMine {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isMine" )){
				m_functionCallCounts.Add( "get_isMine", 0 );
			}
			m_functionCallCounts["get_isMine"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
