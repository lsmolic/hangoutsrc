using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class AnimationState : UnityEngine.RefCounted {
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

	public void AddMixingTransform( UnityEngine.Transform mix, System.Boolean recursive ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddMixingTransform(UnityEngine.Transform, Boolean)" )){
			m_functionCallCounts.Add( "Void AddMixingTransform(UnityEngine.Transform, Boolean)", 0 );
		}
		m_functionCallCounts["Void AddMixingTransform(UnityEngine.Transform, Boolean)"]++;
			
	}

	public void AddMixingTransform( UnityEngine.Transform mix ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddMixingTransform(UnityEngine.Transform)" )){
			m_functionCallCounts.Add( "Void AddMixingTransform(UnityEngine.Transform)", 0 );
		}
		m_functionCallCounts["Void AddMixingTransform(UnityEngine.Transform)"]++;
			
	}

	public AnimationState( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
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

	public System.Int32 layer {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_layer" )){
				m_functionCallCounts.Add( "get_layer", 0 );
			}
			m_functionCallCounts["get_layer"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_layer" )){
				m_functionCallCounts.Add( "set_layer", 0 );
			}
			m_functionCallCounts["set_layer"]++;
			
		}
	}

	public UnityEngine.AnimationClip clip {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_clip" )){
				m_functionCallCounts.Add( "get_clip", 0 );
			}
			m_functionCallCounts["get_clip"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.WrapMode wrapMode {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_wrapMode" )){
				m_functionCallCounts.Add( "get_wrapMode", 0 );
			}
			m_functionCallCounts["get_wrapMode"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_wrapMode" )){
				m_functionCallCounts.Add( "set_wrapMode", 0 );
			}
			m_functionCallCounts["set_wrapMode"]++;
			
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
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_name" )){
				m_functionCallCounts.Add( "set_name", 0 );
			}
			m_functionCallCounts["set_name"]++;
			
		}
	}

	public System.Single weight {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_weight" )){
				m_functionCallCounts.Add( "get_weight", 0 );
			}
			m_functionCallCounts["get_weight"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_weight" )){
				m_functionCallCounts.Add( "set_weight", 0 );
			}
			m_functionCallCounts["set_weight"]++;
			
		}
	}

	public UnityEngine.AnimationBlendMode blendMode {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_blendMode" )){
				m_functionCallCounts.Add( "get_blendMode", 0 );
			}
			m_functionCallCounts["get_blendMode"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_blendMode" )){
				m_functionCallCounts.Add( "set_blendMode", 0 );
			}
			m_functionCallCounts["set_blendMode"]++;
			
		}
	}

	public System.Single normalizedSpeed {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_normalizedSpeed" )){
				m_functionCallCounts.Add( "get_normalizedSpeed", 0 );
			}
			m_functionCallCounts["get_normalizedSpeed"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_normalizedSpeed" )){
				m_functionCallCounts.Add( "set_normalizedSpeed", 0 );
			}
			m_functionCallCounts["set_normalizedSpeed"]++;
			
		}
	}

	public System.Single length {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_length" )){
				m_functionCallCounts.Add( "get_length", 0 );
			}
			m_functionCallCounts["get_length"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Single normalizedTime {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_normalizedTime" )){
				m_functionCallCounts.Add( "get_normalizedTime", 0 );
			}
			m_functionCallCounts["get_normalizedTime"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_normalizedTime" )){
				m_functionCallCounts.Add( "set_normalizedTime", 0 );
			}
			m_functionCallCounts["set_normalizedTime"]++;
			
		}
	}

	public System.Boolean enabled {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_enabled" )){
				m_functionCallCounts.Add( "get_enabled", 0 );
			}
			m_functionCallCounts["get_enabled"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_enabled" )){
				m_functionCallCounts.Add( "set_enabled", 0 );
			}
			m_functionCallCounts["set_enabled"]++;
			
		}
	}

	public System.Single speed {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_speed" )){
				m_functionCallCounts.Add( "get_speed", 0 );
			}
			m_functionCallCounts["get_speed"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_speed" )){
				m_functionCallCounts.Add( "set_speed", 0 );
			}
			m_functionCallCounts["set_speed"]++;
			
		}
	}
}
}