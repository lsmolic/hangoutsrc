using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct Plane {
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

	public void SetNormalAndPosition( UnityEngine.Vector3 inNormal, UnityEngine.Vector3 inPoint ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetNormalAndPosition(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void SetNormalAndPosition(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void SetNormalAndPosition(Vector3, Vector3)"]++;
			
	}

	public void Set3Points( UnityEngine.Vector3 a, UnityEngine.Vector3 b, UnityEngine.Vector3 c ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Set3Points(Vector3, Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void Set3Points(Vector3, Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void Set3Points(Vector3, Vector3, Vector3)"]++;
			
	}

	public System.Single GetDistanceToPoint( UnityEngine.Vector3 inPt ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Single GetDistanceToPoint(Vector3)" )){
			m_functionCallCounts.Add( "Single GetDistanceToPoint(Vector3)", 0 );
		}
		m_functionCallCounts["Single GetDistanceToPoint(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean GetSide( UnityEngine.Vector3 inPt ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean GetSide(Vector3)" )){
			m_functionCallCounts.Add( "Boolean GetSide(Vector3)", 0 );
		}
		m_functionCallCounts["Boolean GetSide(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean SameSide( UnityEngine.Vector3 inPt0, UnityEngine.Vector3 inPt1 ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean SameSide(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Boolean SameSide(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Boolean SameSide(Vector3, Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean Raycast( UnityEngine.Ray ray, out System.Single enter ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean Raycast(Ray, Single ByRef)" )){
			m_functionCallCounts.Add( "Boolean Raycast(Ray, Single ByRef)", 0 );
		}
		m_functionCallCounts["Boolean Raycast(Ray, Single ByRef)"]++;
			
		enter = default(System.Single);
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public Plane( UnityEngine.Vector3 inNormal, UnityEngine.Vector3 inPoint ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void .ctor(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void .ctor(Vector3, Vector3)"]++;
			
	}

	public Plane( UnityEngine.Vector3 inNormal, System.Single d ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Vector3, Single)" )){
			m_functionCallCounts.Add( "Void .ctor(Vector3, Single)", 0 );
		}
		m_functionCallCounts["Void .ctor(Vector3, Single)"]++;
			
	}

	public Plane( UnityEngine.Vector3 a, UnityEngine.Vector3 b, UnityEngine.Vector3 c ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Vector3, Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void .ctor(Vector3, Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void .ctor(Vector3, Vector3, Vector3)"]++;
			
	}

	public System.Single distance {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_distance" )){
				m_functionCallCounts.Add( "get_distance", 0 );
			}
			m_functionCallCounts["get_distance"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_distance" )){
				m_functionCallCounts.Add( "set_distance", 0 );
			}
			m_functionCallCounts["set_distance"]++;
			
		}
	}

	public UnityEngine.Vector3 normal {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_normal" )){
				m_functionCallCounts.Add( "get_normal", 0 );
			}
			m_functionCallCounts["get_normal"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_normal" )){
				m_functionCallCounts.Add( "set_normal", 0 );
			}
			m_functionCallCounts["set_normal"]++;
			
		}
	}
}
}
