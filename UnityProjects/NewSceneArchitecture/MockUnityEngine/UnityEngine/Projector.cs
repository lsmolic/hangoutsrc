using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Projector : UnityEngine.Behaviour {
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

	public Projector( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Single orthographicSize {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_orthographicSize" )){
				m_functionCallCounts.Add( "get_orthographicSize", 0 );
			}
			m_functionCallCounts["get_orthographicSize"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_orthographicSize" )){
				m_functionCallCounts.Add( "set_orthographicSize", 0 );
			}
			m_functionCallCounts["set_orthographicSize"]++;
			
		}
	}

	public System.Single aspectRatio {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_aspectRatio" )){
				m_functionCallCounts.Add( "get_aspectRatio", 0 );
			}
			m_functionCallCounts["get_aspectRatio"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_aspectRatio" )){
				m_functionCallCounts.Add( "set_aspectRatio", 0 );
			}
			m_functionCallCounts["set_aspectRatio"]++;
			
		}
	}

	public System.Single nearClipPlane {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_nearClipPlane" )){
				m_functionCallCounts.Add( "get_nearClipPlane", 0 );
			}
			m_functionCallCounts["get_nearClipPlane"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_nearClipPlane" )){
				m_functionCallCounts.Add( "set_nearClipPlane", 0 );
			}
			m_functionCallCounts["set_nearClipPlane"]++;
			
		}
	}

	public System.Boolean isOrthoGraphic {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isOrthoGraphic" )){
				m_functionCallCounts.Add( "get_isOrthoGraphic", 0 );
			}
			m_functionCallCounts["get_isOrthoGraphic"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_isOrthoGraphic" )){
				m_functionCallCounts.Add( "set_isOrthoGraphic", 0 );
			}
			m_functionCallCounts["set_isOrthoGraphic"]++;
			
		}
	}

	public System.Single fieldOfView {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_fieldOfView" )){
				m_functionCallCounts.Add( "get_fieldOfView", 0 );
			}
			m_functionCallCounts["get_fieldOfView"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_fieldOfView" )){
				m_functionCallCounts.Add( "set_fieldOfView", 0 );
			}
			m_functionCallCounts["set_fieldOfView"]++;
			
		}
	}

	public System.Int32 ignoreLayers {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_ignoreLayers" )){
				m_functionCallCounts.Add( "get_ignoreLayers", 0 );
			}
			m_functionCallCounts["get_ignoreLayers"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_ignoreLayers" )){
				m_functionCallCounts.Add( "set_ignoreLayers", 0 );
			}
			m_functionCallCounts["set_ignoreLayers"]++;
			
		}
	}

	public System.Boolean orthographic {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_orthographic" )){
				m_functionCallCounts.Add( "get_orthographic", 0 );
			}
			m_functionCallCounts["get_orthographic"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_orthographic" )){
				m_functionCallCounts.Add( "set_orthographic", 0 );
			}
			m_functionCallCounts["set_orthographic"]++;
			
		}
	}

	public System.Single orthoGraphicSize {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_orthoGraphicSize" )){
				m_functionCallCounts.Add( "get_orthoGraphicSize", 0 );
			}
			m_functionCallCounts["get_orthoGraphicSize"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_orthoGraphicSize" )){
				m_functionCallCounts.Add( "set_orthoGraphicSize", 0 );
			}
			m_functionCallCounts["set_orthoGraphicSize"]++;
			
		}
	}

	public System.Single farClipPlane {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_farClipPlane" )){
				m_functionCallCounts.Add( "get_farClipPlane", 0 );
			}
			m_functionCallCounts["get_farClipPlane"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_farClipPlane" )){
				m_functionCallCounts.Add( "set_farClipPlane", 0 );
			}
			m_functionCallCounts["set_farClipPlane"]++;
			
		}
	}

	public UnityEngine.Material material {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_material" )){
				m_functionCallCounts.Add( "get_material", 0 );
			}
			m_functionCallCounts["get_material"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_material" )){
				m_functionCallCounts.Add( "set_material", 0 );
			}
			m_functionCallCounts["set_material"]++;
			
		}
	}
}
}