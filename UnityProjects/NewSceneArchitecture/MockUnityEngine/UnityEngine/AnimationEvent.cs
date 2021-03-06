using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class AnimationEvent {
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

	public AnimationEvent( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.String data {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_data" )){
				m_functionCallCounts.Add( "get_data", 0 );
			}
			m_functionCallCounts["get_data"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_data" )){
				m_functionCallCounts.Add( "set_data", 0 );
			}
			m_functionCallCounts["set_data"]++;
			
		}
	}

	public System.Single time {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_time" )){
				m_functionCallCounts.Add( "get_time", 0 );
			}
			m_functionCallCounts["get_time"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_time" )){
				m_functionCallCounts.Add( "set_time", 0 );
			}
			m_functionCallCounts["set_time"]++;
			
		}
	}

	public UnityEngine.SendMessageOptions messageOptions {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_messageOptions" )){
				m_functionCallCounts.Add( "get_messageOptions", 0 );
			}
			m_functionCallCounts["get_messageOptions"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_messageOptions" )){
				m_functionCallCounts.Add( "set_messageOptions", 0 );
			}
			m_functionCallCounts["set_messageOptions"]++;
			
		}
	}

	public UnityEngine.AnimationState animationState {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_animationState" )){
				m_functionCallCounts.Add( "get_animationState", 0 );
			}
			m_functionCallCounts["get_animationState"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String functionName {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_functionName" )){
				m_functionCallCounts.Add( "get_functionName", 0 );
			}
			m_functionCallCounts["get_functionName"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_functionName" )){
				m_functionCallCounts.Add( "set_functionName", 0 );
			}
			m_functionCallCounts["set_functionName"]++;
			
		}
	}
}
}
