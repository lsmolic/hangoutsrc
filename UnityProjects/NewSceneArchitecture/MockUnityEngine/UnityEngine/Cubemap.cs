using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Cubemap : UnityEngine.Texture {
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

	public void SetPixel( UnityEngine.CubemapFace face, System.Int32 x, System.Int32 y, UnityEngine.Color color ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetPixel(CubemapFace, Int32, Int32, Color)" )){
			m_functionCallCounts.Add( "Void SetPixel(CubemapFace, Int32, Int32, Color)", 0 );
		}
		m_functionCallCounts["Void SetPixel(CubemapFace, Int32, Int32, Color)"]++;
			
	}

	public UnityEngine.Color GetPixel( UnityEngine.CubemapFace face, System.Int32 x, System.Int32 y ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Color GetPixel(CubemapFace, Int32, Int32)" )){
			m_functionCallCounts.Add( "Color GetPixel(CubemapFace, Int32, Int32)", 0 );
		}
		m_functionCallCounts["Color GetPixel(CubemapFace, Int32, Int32)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Color[] GetPixels( UnityEngine.CubemapFace face, System.Int32 miplevel ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.Color[] GetPixels(CubemapFace, Int32)" )){
			m_functionCallCounts.Add( "UnityEngine.Color[] GetPixels(CubemapFace, Int32)", 0 );
		}
		m_functionCallCounts["UnityEngine.Color[] GetPixels(CubemapFace, Int32)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Color[] GetPixels( UnityEngine.CubemapFace face ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.Color[] GetPixels(CubemapFace)" )){
			m_functionCallCounts.Add( "UnityEngine.Color[] GetPixels(CubemapFace)", 0 );
		}
		m_functionCallCounts["UnityEngine.Color[] GetPixels(CubemapFace)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void SetPixels( UnityEngine.Color[] colors, UnityEngine.CubemapFace face, System.Int32 miplevel ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetPixels(UnityEngine.Color[], CubemapFace, Int32)" )){
			m_functionCallCounts.Add( "Void SetPixels(UnityEngine.Color[], CubemapFace, Int32)", 0 );
		}
		m_functionCallCounts["Void SetPixels(UnityEngine.Color[], CubemapFace, Int32)"]++;
			
	}

	public void SetPixels( UnityEngine.Color[] colors, UnityEngine.CubemapFace face ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetPixels(UnityEngine.Color[], CubemapFace)" )){
			m_functionCallCounts.Add( "Void SetPixels(UnityEngine.Color[], CubemapFace)", 0 );
		}
		m_functionCallCounts["Void SetPixels(UnityEngine.Color[], CubemapFace)"]++;
			
	}

	public void Apply( System.Boolean updateMipmaps ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Apply(Boolean)" )){
			m_functionCallCounts.Add( "Void Apply(Boolean)", 0 );
		}
		m_functionCallCounts["Void Apply(Boolean)"]++;
			
	}

	public void Apply( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Apply()" )){
			m_functionCallCounts.Add( "Void Apply()", 0 );
		}
		m_functionCallCounts["Void Apply()"]++;
			
	}

	public Cubemap( System.Int32 size, UnityEngine.TextureFormat format, System.Boolean mipmap ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Int32, TextureFormat, Boolean)" )){
			m_functionCallCounts.Add( "Void .ctor(Int32, TextureFormat, Boolean)", 0 );
		}
		m_functionCallCounts["Void .ctor(Int32, TextureFormat, Boolean)"]++;
			
	}

	public UnityEngine.TextureFormat format {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_format" )){
				m_functionCallCounts.Add( "get_format", 0 );
			}
			m_functionCallCounts["get_format"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
