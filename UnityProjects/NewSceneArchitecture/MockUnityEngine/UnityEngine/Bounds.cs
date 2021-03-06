using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct Bounds {
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

	public void SetMinMax( UnityEngine.Vector3 min, UnityEngine.Vector3 max ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetMinMax(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void SetMinMax(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void SetMinMax(Vector3, Vector3)"]++;
			
	}

	public void Encapsulate( UnityEngine.Vector3 point ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Encapsulate(Vector3)" )){
			m_functionCallCounts.Add( "Void Encapsulate(Vector3)", 0 );
		}
		m_functionCallCounts["Void Encapsulate(Vector3)"]++;
			
	}

	public void Encapsulate( UnityEngine.Bounds bounds ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Encapsulate(Bounds)" )){
			m_functionCallCounts.Add( "Void Encapsulate(Bounds)", 0 );
		}
		m_functionCallCounts["Void Encapsulate(Bounds)"]++;
			
	}

	public void Expand( System.Single amount ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Expand(Single)" )){
			m_functionCallCounts.Add( "Void Expand(Single)", 0 );
		}
		m_functionCallCounts["Void Expand(Single)"]++;
			
	}

	public void Expand( UnityEngine.Vector3 amount ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Expand(Vector3)" )){
			m_functionCallCounts.Add( "Void Expand(Vector3)", 0 );
		}
		m_functionCallCounts["Void Expand(Vector3)"]++;
			
	}

	public System.Boolean Contains( UnityEngine.Vector3 point ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean Contains(Vector3)" )){
			m_functionCallCounts.Add( "Boolean Contains(Vector3)", 0 );
		}
		m_functionCallCounts["Boolean Contains(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Single SqrDistance( UnityEngine.Vector3 point ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Single SqrDistance(Vector3)" )){
			m_functionCallCounts.Add( "Single SqrDistance(Vector3)", 0 );
		}
		m_functionCallCounts["Single SqrDistance(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean IntersectRay( UnityEngine.Ray ray ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean IntersectRay(Ray)" )){
			m_functionCallCounts.Add( "Boolean IntersectRay(Ray)", 0 );
		}
		m_functionCallCounts["Boolean IntersectRay(Ray)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean IntersectRay( UnityEngine.Ray ray, out System.Single distance ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean IntersectRay(Ray, Single ByRef)" )){
			m_functionCallCounts.Add( "Boolean IntersectRay(Ray, Single ByRef)", 0 );
		}
		m_functionCallCounts["Boolean IntersectRay(Ray, Single ByRef)"]++;
			
		distance = default(System.Single);
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

	public static System.Boolean operator ==( UnityEngine.Bounds lhs, UnityEngine.Bounds rhs ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Boolean operator !=( UnityEngine.Bounds lhs, UnityEngine.Bounds rhs ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public Bounds( UnityEngine.Vector3 center, UnityEngine.Vector3 size ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void .ctor(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void .ctor(Vector3, Vector3)"]++;
			
	}

	public UnityEngine.Vector3 size {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_size" )){
				m_functionCallCounts.Add( "get_size", 0 );
			}
			m_functionCallCounts["get_size"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_size" )){
				m_functionCallCounts.Add( "set_size", 0 );
			}
			m_functionCallCounts["set_size"]++;
			
		}
	}

	public UnityEngine.Vector3 min {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_min" )){
				m_functionCallCounts.Add( "get_min", 0 );
			}
			m_functionCallCounts["get_min"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_min" )){
				m_functionCallCounts.Add( "set_min", 0 );
			}
			m_functionCallCounts["set_min"]++;
			
		}
	}

	public UnityEngine.Vector3 max {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_max" )){
				m_functionCallCounts.Add( "get_max", 0 );
			}
			m_functionCallCounts["get_max"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_max" )){
				m_functionCallCounts.Add( "set_max", 0 );
			}
			m_functionCallCounts["set_max"]++;
			
		}
	}

	public UnityEngine.Vector3 extents {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_extents" )){
				m_functionCallCounts.Add( "get_extents", 0 );
			}
			m_functionCallCounts["get_extents"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_extents" )){
				m_functionCallCounts.Add( "set_extents", 0 );
			}
			m_functionCallCounts["set_extents"]++;
			
		}
	}

	public UnityEngine.Vector3 center {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_center" )){
				m_functionCallCounts.Add( "get_center", 0 );
			}
			m_functionCallCounts["get_center"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_center" )){
				m_functionCallCounts.Add( "set_center", 0 );
			}
			m_functionCallCounts["set_center"]++;
			
		}
	}
}
}
