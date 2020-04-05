using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct BoneWeight {
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

	public static System.Boolean operator ==( UnityEngine.BoneWeight lhs, UnityEngine.BoneWeight rhs ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Boolean operator !=( UnityEngine.BoneWeight lhs, UnityEngine.BoneWeight rhs ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Int32 boneIndex0 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_boneIndex0" )){
				m_functionCallCounts.Add( "get_boneIndex0", 0 );
			}
			m_functionCallCounts["get_boneIndex0"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_boneIndex0" )){
				m_functionCallCounts.Add( "set_boneIndex0", 0 );
			}
			m_functionCallCounts["set_boneIndex0"]++;
			
		}
	}

	public System.Int32 boneIndex1 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_boneIndex1" )){
				m_functionCallCounts.Add( "get_boneIndex1", 0 );
			}
			m_functionCallCounts["get_boneIndex1"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_boneIndex1" )){
				m_functionCallCounts.Add( "set_boneIndex1", 0 );
			}
			m_functionCallCounts["set_boneIndex1"]++;
			
		}
	}

	public System.Int32 boneIndex2 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_boneIndex2" )){
				m_functionCallCounts.Add( "get_boneIndex2", 0 );
			}
			m_functionCallCounts["get_boneIndex2"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_boneIndex2" )){
				m_functionCallCounts.Add( "set_boneIndex2", 0 );
			}
			m_functionCallCounts["set_boneIndex2"]++;
			
		}
	}

	public System.Int32 boneIndex3 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_boneIndex3" )){
				m_functionCallCounts.Add( "get_boneIndex3", 0 );
			}
			m_functionCallCounts["get_boneIndex3"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_boneIndex3" )){
				m_functionCallCounts.Add( "set_boneIndex3", 0 );
			}
			m_functionCallCounts["set_boneIndex3"]++;
			
		}
	}

	public System.Single weight0 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_weight0" )){
				m_functionCallCounts.Add( "get_weight0", 0 );
			}
			m_functionCallCounts["get_weight0"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_weight0" )){
				m_functionCallCounts.Add( "set_weight0", 0 );
			}
			m_functionCallCounts["set_weight0"]++;
			
		}
	}

	public System.Single weight1 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_weight1" )){
				m_functionCallCounts.Add( "get_weight1", 0 );
			}
			m_functionCallCounts["get_weight1"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_weight1" )){
				m_functionCallCounts.Add( "set_weight1", 0 );
			}
			m_functionCallCounts["set_weight1"]++;
			
		}
	}

	public System.Single weight2 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_weight2" )){
				m_functionCallCounts.Add( "get_weight2", 0 );
			}
			m_functionCallCounts["get_weight2"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_weight2" )){
				m_functionCallCounts.Add( "set_weight2", 0 );
			}
			m_functionCallCounts["set_weight2"]++;
			
		}
	}

	public System.Single weight3 {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_weight3" )){
				m_functionCallCounts.Add( "get_weight3", 0 );
			}
			m_functionCallCounts["get_weight3"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_weight3" )){
				m_functionCallCounts.Add( "set_weight3", 0 );
			}
			m_functionCallCounts["set_weight3"]++;
			
		}
	}
}
}