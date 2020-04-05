using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Shader : UnityEngine.Object {
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

	public static void ClearAll( ){
	}

	public static UnityEngine.Shader Find( System.String name ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void EnableKeyword( System.String keyword ){
	}

	public static void DisableKeyword( System.String keyword ){
	}

	public static void SetGlobalColor( System.String propertyName, UnityEngine.Color color ){
	}

	public static void SetGlobalVector( System.String propertyName, UnityEngine.Vector4 vec ){
	}

	public static void SetGlobalFloat( System.String propertyName, System.Single value ){
	}

	public static void SetGlobalTexture( System.String propertyName, UnityEngine.Texture tex ){
	}

	public static void SetGlobalMatrix( System.String propertyName, UnityEngine.Matrix4x4 mat ){
	}

	public static void SetGlobalTexGenMode( System.String propertyName, UnityEngine.TexGenMode mode ){
	}

	public static void SetGlobalTextureMatrixName( System.String propertyName, System.String matrixName ){
	}

	public void SetStaticFloat( System.String propertyName, System.Single value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetStaticFloat(System.String, Single)" )){
			m_functionCallCounts.Add( "Void SetStaticFloat(System.String, Single)", 0 );
		}
		m_functionCallCounts["Void SetStaticFloat(System.String, Single)"]++;
			
	}

	public System.Single GetStaticFloat( System.String propertyName ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Single GetStaticFloat(System.String)" )){
			m_functionCallCounts.Add( "Single GetStaticFloat(System.String)", 0 );
		}
		m_functionCallCounts["Single GetStaticFloat(System.String)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void SetStaticColor( System.String propertyName, UnityEngine.Color color ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetStaticColor(System.String, Color)" )){
			m_functionCallCounts.Add( "Void SetStaticColor(System.String, Color)", 0 );
		}
		m_functionCallCounts["Void SetStaticColor(System.String, Color)"]++;
			
	}

	public UnityEngine.Color GetStaticColor( System.String propertyName ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Color GetStaticColor(System.String)" )){
			m_functionCallCounts.Add( "Color GetStaticColor(System.String)", 0 );
		}
		m_functionCallCounts["Color GetStaticColor(System.String)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void SetStaticTexture( System.String propertyName, UnityEngine.Texture texture ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetStaticTexture(System.String, UnityEngine.Texture)" )){
			m_functionCallCounts.Add( "Void SetStaticTexture(System.String, UnityEngine.Texture)", 0 );
		}
		m_functionCallCounts["Void SetStaticTexture(System.String, UnityEngine.Texture)"]++;
			
	}

	public UnityEngine.Texture GetStaticTexture( System.String propertyName ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.Texture GetStaticTexture(System.String)" )){
			m_functionCallCounts.Add( "UnityEngine.Texture GetStaticTexture(System.String)", 0 );
		}
		m_functionCallCounts["UnityEngine.Texture GetStaticTexture(System.String)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Int32 PropertyToID( System.String name ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public Shader( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Boolean isSupported {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isSupported" )){
				m_functionCallCounts.Add( "get_isSupported", 0 );
			}
			m_functionCallCounts["get_isSupported"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static System.Int32 globalMaximumLOD {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public System.Int32 maximumLOD {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_maximumLOD" )){
				m_functionCallCounts.Add( "get_maximumLOD", 0 );
			}
			m_functionCallCounts["get_maximumLOD"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_maximumLOD" )){
				m_functionCallCounts.Add( "set_maximumLOD", 0 );
			}
			m_functionCallCounts["set_maximumLOD"]++;
			
		}
	}

	public System.Int32 renderQueue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_renderQueue" )){
				m_functionCallCounts.Add( "get_renderQueue", 0 );
			}
			m_functionCallCounts["get_renderQueue"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}