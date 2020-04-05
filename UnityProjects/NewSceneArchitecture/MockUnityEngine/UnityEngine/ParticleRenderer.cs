using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class ParticleRenderer : UnityEngine.Renderer {
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

	public ParticleRenderer( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Single maxPartileSize {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_maxPartileSize" )){
				m_functionCallCounts.Add( "get_maxPartileSize", 0 );
			}
			m_functionCallCounts["get_maxPartileSize"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_maxPartileSize" )){
				m_functionCallCounts.Add( "set_maxPartileSize", 0 );
			}
			m_functionCallCounts["set_maxPartileSize"]++;
			
		}
	}

	public System.Single uvAnimationCycles {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_uvAnimationCycles" )){
				m_functionCallCounts.Add( "get_uvAnimationCycles", 0 );
			}
			m_functionCallCounts["get_uvAnimationCycles"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_uvAnimationCycles" )){
				m_functionCallCounts.Add( "set_uvAnimationCycles", 0 );
			}
			m_functionCallCounts["set_uvAnimationCycles"]++;
			
		}
	}

	public System.Single maxParticleSize {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_maxParticleSize" )){
				m_functionCallCounts.Add( "get_maxParticleSize", 0 );
			}
			m_functionCallCounts["get_maxParticleSize"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_maxParticleSize" )){
				m_functionCallCounts.Add( "set_maxParticleSize", 0 );
			}
			m_functionCallCounts["set_maxParticleSize"]++;
			
		}
	}

	public UnityEngine.ParticleRenderMode particleRenderMode {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_particleRenderMode" )){
				m_functionCallCounts.Add( "get_particleRenderMode", 0 );
			}
			m_functionCallCounts["get_particleRenderMode"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_particleRenderMode" )){
				m_functionCallCounts.Add( "set_particleRenderMode", 0 );
			}
			m_functionCallCounts["set_particleRenderMode"]++;
			
		}
	}

	public UnityEngine.AnimationCurve rotationCurve {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_rotationCurve" )){
				m_functionCallCounts.Add( "get_rotationCurve", 0 );
			}
			m_functionCallCounts["get_rotationCurve"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_rotationCurve" )){
				m_functionCallCounts.Add( "set_rotationCurve", 0 );
			}
			m_functionCallCounts["set_rotationCurve"]++;
			
		}
	}

	public System.Int32 animatedTextureCount {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_animatedTextureCount" )){
				m_functionCallCounts.Add( "get_animatedTextureCount", 0 );
			}
			m_functionCallCounts["get_animatedTextureCount"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_animatedTextureCount" )){
				m_functionCallCounts.Add( "set_animatedTextureCount", 0 );
			}
			m_functionCallCounts["set_animatedTextureCount"]++;
			
		}
	}

	public System.Int32 uvAnimationXTile {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_uvAnimationXTile" )){
				m_functionCallCounts.Add( "get_uvAnimationXTile", 0 );
			}
			m_functionCallCounts["get_uvAnimationXTile"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_uvAnimationXTile" )){
				m_functionCallCounts.Add( "set_uvAnimationXTile", 0 );
			}
			m_functionCallCounts["set_uvAnimationXTile"]++;
			
		}
	}

	public System.Int32 uvAnimationYTile {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_uvAnimationYTile" )){
				m_functionCallCounts.Add( "get_uvAnimationYTile", 0 );
			}
			m_functionCallCounts["get_uvAnimationYTile"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_uvAnimationYTile" )){
				m_functionCallCounts.Add( "set_uvAnimationYTile", 0 );
			}
			m_functionCallCounts["set_uvAnimationYTile"]++;
			
		}
	}

	public System.Single cameraVelocityScale {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_cameraVelocityScale" )){
				m_functionCallCounts.Add( "get_cameraVelocityScale", 0 );
			}
			m_functionCallCounts["get_cameraVelocityScale"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_cameraVelocityScale" )){
				m_functionCallCounts.Add( "set_cameraVelocityScale", 0 );
			}
			m_functionCallCounts["set_cameraVelocityScale"]++;
			
		}
	}

	public System.Single velocityScale {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_velocityScale" )){
				m_functionCallCounts.Add( "get_velocityScale", 0 );
			}
			m_functionCallCounts["get_velocityScale"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_velocityScale" )){
				m_functionCallCounts.Add( "set_velocityScale", 0 );
			}
			m_functionCallCounts["set_velocityScale"]++;
			
		}
	}

	public UnityEngine.Rect[] uvTiles {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_uvTiles" )){
				m_functionCallCounts.Add( "get_uvTiles", 0 );
			}
			m_functionCallCounts["get_uvTiles"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_uvTiles" )){
				m_functionCallCounts.Add( "set_uvTiles", 0 );
			}
			m_functionCallCounts["set_uvTiles"]++;
			
		}
	}

	public UnityEngine.AnimationCurve heightCurve {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_heightCurve" )){
				m_functionCallCounts.Add( "get_heightCurve", 0 );
			}
			m_functionCallCounts["get_heightCurve"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_heightCurve" )){
				m_functionCallCounts.Add( "set_heightCurve", 0 );
			}
			m_functionCallCounts["set_heightCurve"]++;
			
		}
	}

	public UnityEngine.AnimationCurve widthCurve {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_widthCurve" )){
				m_functionCallCounts.Add( "get_widthCurve", 0 );
			}
			m_functionCallCounts["get_widthCurve"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_widthCurve" )){
				m_functionCallCounts.Add( "set_widthCurve", 0 );
			}
			m_functionCallCounts["set_widthCurve"]++;
			
		}
	}

	public System.Single lengthScale {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_lengthScale" )){
				m_functionCallCounts.Add( "get_lengthScale", 0 );
			}
			m_functionCallCounts["get_lengthScale"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_lengthScale" )){
				m_functionCallCounts.Add( "set_lengthScale", 0 );
			}
			m_functionCallCounts["set_lengthScale"]++;
			
		}
	}
}
}