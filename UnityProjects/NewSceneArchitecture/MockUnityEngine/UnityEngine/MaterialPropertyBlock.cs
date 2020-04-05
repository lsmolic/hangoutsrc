using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class MaterialPropertyBlock {
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

	public void InitBlock( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void InitBlock()" )){
			m_functionCallCounts.Add( "Void InitBlock()", 0 );
		}
		m_functionCallCounts["Void InitBlock()"]++;
			
	}

	public void DestroyBlock( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void DestroyBlock()" )){
			m_functionCallCounts.Add( "Void DestroyBlock()", 0 );
		}
		m_functionCallCounts["Void DestroyBlock()"]++;
			
	}

	public void AddFloat( System.String name, System.Single value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddFloat(System.String, Single)" )){
			m_functionCallCounts.Add( "Void AddFloat(System.String, Single)", 0 );
		}
		m_functionCallCounts["Void AddFloat(System.String, Single)"]++;
			
	}

	public void AddFloat( System.Int32 nameID, System.Single value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddFloat(Int32, Single)" )){
			m_functionCallCounts.Add( "Void AddFloat(Int32, Single)", 0 );
		}
		m_functionCallCounts["Void AddFloat(Int32, Single)"]++;
			
	}

	public void AddVector( System.String name, UnityEngine.Vector4 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddVector(System.String, Vector4)" )){
			m_functionCallCounts.Add( "Void AddVector(System.String, Vector4)", 0 );
		}
		m_functionCallCounts["Void AddVector(System.String, Vector4)"]++;
			
	}

	public void AddVector( System.Int32 nameID, UnityEngine.Vector4 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddVector(Int32, Vector4)" )){
			m_functionCallCounts.Add( "Void AddVector(Int32, Vector4)", 0 );
		}
		m_functionCallCounts["Void AddVector(Int32, Vector4)"]++;
			
	}

	public void AddColor( System.String name, UnityEngine.Color value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddColor(System.String, Color)" )){
			m_functionCallCounts.Add( "Void AddColor(System.String, Color)", 0 );
		}
		m_functionCallCounts["Void AddColor(System.String, Color)"]++;
			
	}

	public void AddColor( System.Int32 nameID, UnityEngine.Color value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddColor(Int32, Color)" )){
			m_functionCallCounts.Add( "Void AddColor(Int32, Color)", 0 );
		}
		m_functionCallCounts["Void AddColor(Int32, Color)"]++;
			
	}

	public void AddMatrix( System.String name, UnityEngine.Matrix4x4 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddMatrix(System.String, Matrix4x4)" )){
			m_functionCallCounts.Add( "Void AddMatrix(System.String, Matrix4x4)", 0 );
		}
		m_functionCallCounts["Void AddMatrix(System.String, Matrix4x4)"]++;
			
	}

	public void AddMatrix( System.Int32 nameID, UnityEngine.Matrix4x4 value ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddMatrix(Int32, Matrix4x4)" )){
			m_functionCallCounts.Add( "Void AddMatrix(Int32, Matrix4x4)", 0 );
		}
		m_functionCallCounts["Void AddMatrix(Int32, Matrix4x4)"]++;
			
	}

	public void Clear( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Clear()" )){
			m_functionCallCounts.Add( "Void Clear()", 0 );
		}
		m_functionCallCounts["Void Clear()"]++;
			
	}

	public MaterialPropertyBlock( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}
}
}
