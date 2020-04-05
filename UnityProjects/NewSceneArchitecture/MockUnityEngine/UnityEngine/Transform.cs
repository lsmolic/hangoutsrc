using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Transform : UnityEngine.Component {
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

	public void Translate( UnityEngine.Vector3 translation ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Translate(Vector3)" )){
			m_functionCallCounts.Add( "Void Translate(Vector3)", 0 );
		}
		m_functionCallCounts["Void Translate(Vector3)"]++;
			
	}

	public void Translate( UnityEngine.Vector3 translation, UnityEngine.Space relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Translate(Vector3, Space)" )){
			m_functionCallCounts.Add( "Void Translate(Vector3, Space)", 0 );
		}
		m_functionCallCounts["Void Translate(Vector3, Space)"]++;
			
	}

	public void Translate( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Translate(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Void Translate(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Void Translate(Single, Single, Single)"]++;
			
	}

	public void Translate( System.Single x, System.Single y, System.Single z, UnityEngine.Space relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Translate(Single, Single, Single, Space)" )){
			m_functionCallCounts.Add( "Void Translate(Single, Single, Single, Space)", 0 );
		}
		m_functionCallCounts["Void Translate(Single, Single, Single, Space)"]++;
			
	}

	public void Translate( UnityEngine.Vector3 translation, UnityEngine.Transform relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Translate(Vector3, UnityEngine.Transform)" )){
			m_functionCallCounts.Add( "Void Translate(Vector3, UnityEngine.Transform)", 0 );
		}
		m_functionCallCounts["Void Translate(Vector3, UnityEngine.Transform)"]++;
			
	}

	public void Translate( System.Single x, System.Single y, System.Single z, UnityEngine.Transform relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Translate(Single, Single, Single, UnityEngine.Transform)" )){
			m_functionCallCounts.Add( "Void Translate(Single, Single, Single, UnityEngine.Transform)", 0 );
		}
		m_functionCallCounts["Void Translate(Single, Single, Single, UnityEngine.Transform)"]++;
			
	}

	public void Rotate( UnityEngine.Vector3 eulerAngles ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Rotate(Vector3)" )){
			m_functionCallCounts.Add( "Void Rotate(Vector3)", 0 );
		}
		m_functionCallCounts["Void Rotate(Vector3)"]++;
			
	}

	public void Rotate( UnityEngine.Vector3 eulerAngles, UnityEngine.Space relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Rotate(Vector3, Space)" )){
			m_functionCallCounts.Add( "Void Rotate(Vector3, Space)", 0 );
		}
		m_functionCallCounts["Void Rotate(Vector3, Space)"]++;
			
	}

	public void Rotate( System.Single xAngle, System.Single yAngle, System.Single zAngle ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Rotate(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Void Rotate(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Void Rotate(Single, Single, Single)"]++;
			
	}

	public void Rotate( System.Single xAngle, System.Single yAngle, System.Single zAngle, UnityEngine.Space relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Rotate(Single, Single, Single, Space)" )){
			m_functionCallCounts.Add( "Void Rotate(Single, Single, Single, Space)", 0 );
		}
		m_functionCallCounts["Void Rotate(Single, Single, Single, Space)"]++;
			
	}

	public void Rotate( UnityEngine.Vector3 axis, System.Single angle ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Rotate(Vector3, Single)" )){
			m_functionCallCounts.Add( "Void Rotate(Vector3, Single)", 0 );
		}
		m_functionCallCounts["Void Rotate(Vector3, Single)"]++;
			
	}

	public void Rotate( UnityEngine.Vector3 axis, System.Single angle, UnityEngine.Space relativeTo ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Rotate(Vector3, Single, Space)" )){
			m_functionCallCounts.Add( "Void Rotate(Vector3, Single, Space)", 0 );
		}
		m_functionCallCounts["Void Rotate(Vector3, Single, Space)"]++;
			
	}

	public void RotateAround( UnityEngine.Vector3 point, UnityEngine.Vector3 axis, System.Single angle ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void RotateAround(Vector3, Vector3, Single)" )){
			m_functionCallCounts.Add( "Void RotateAround(Vector3, Vector3, Single)", 0 );
		}
		m_functionCallCounts["Void RotateAround(Vector3, Vector3, Single)"]++;
			
	}

	public void LookAt( UnityEngine.Transform target ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void LookAt(UnityEngine.Transform)" )){
			m_functionCallCounts.Add( "Void LookAt(UnityEngine.Transform)", 0 );
		}
		m_functionCallCounts["Void LookAt(UnityEngine.Transform)"]++;
			
	}

	public void LookAt( UnityEngine.Transform target, UnityEngine.Vector3 worldUp ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void LookAt(UnityEngine.Transform, Vector3)" )){
			m_functionCallCounts.Add( "Void LookAt(UnityEngine.Transform, Vector3)", 0 );
		}
		m_functionCallCounts["Void LookAt(UnityEngine.Transform, Vector3)"]++;
			
	}

	public void LookAt( UnityEngine.Vector3 worldPosition, UnityEngine.Vector3 worldUp ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void LookAt(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void LookAt(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void LookAt(Vector3, Vector3)"]++;
			
	}

	public void LookAt( UnityEngine.Vector3 worldPosition ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void LookAt(Vector3)" )){
			m_functionCallCounts.Add( "Void LookAt(Vector3)", 0 );
		}
		m_functionCallCounts["Void LookAt(Vector3)"]++;
			
	}

	public UnityEngine.Vector3 TransformDirection( UnityEngine.Vector3 direction ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 TransformDirection(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 TransformDirection(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 TransformDirection(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 TransformDirection( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 TransformDirection(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Vector3 TransformDirection(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Vector3 TransformDirection(Single, Single, Single)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 InverseTransformDirection( UnityEngine.Vector3 direction ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 InverseTransformDirection(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 InverseTransformDirection(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 InverseTransformDirection(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 InverseTransformDirection( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 InverseTransformDirection(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Vector3 InverseTransformDirection(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Vector3 InverseTransformDirection(Single, Single, Single)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 TransformPoint( UnityEngine.Vector3 position ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 TransformPoint(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 TransformPoint(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 TransformPoint(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 TransformPoint( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 TransformPoint(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Vector3 TransformPoint(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Vector3 TransformPoint(Single, Single, Single)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 InverseTransformPoint( UnityEngine.Vector3 position ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 InverseTransformPoint(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 InverseTransformPoint(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 InverseTransformPoint(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 InverseTransformPoint( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 InverseTransformPoint(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Vector3 InverseTransformPoint(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Vector3 InverseTransformPoint(Single, Single, Single)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void DetachChildren( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void DetachChildren()" )){
			m_functionCallCounts.Add( "Void DetachChildren()", 0 );
		}
		m_functionCallCounts["Void DetachChildren()"]++;
			
	}

	public UnityEngine.Transform Find( System.String name ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.Transform Find(System.String)" )){
			m_functionCallCounts.Add( "UnityEngine.Transform Find(System.String)", 0 );
		}
		m_functionCallCounts["UnityEngine.Transform Find(System.String)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean IsChildOf( UnityEngine.Transform parent ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean IsChildOf(UnityEngine.Transform)" )){
			m_functionCallCounts.Add( "Boolean IsChildOf(UnityEngine.Transform)", 0 );
		}
		m_functionCallCounts["Boolean IsChildOf(UnityEngine.Transform)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Transform FindChild( System.String name ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.Transform FindChild(System.String)" )){
			m_functionCallCounts.Add( "UnityEngine.Transform FindChild(System.String)", 0 );
		}
		m_functionCallCounts["UnityEngine.Transform FindChild(System.String)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public virtual System.Collections.IEnumerator GetEnumerator( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "IEnumerator GetEnumerator()" )){
			m_functionCallCounts.Add( "IEnumerator GetEnumerator()", 0 );
		}
		m_functionCallCounts["IEnumerator GetEnumerator()"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void RotateAround( UnityEngine.Vector3 axis, System.Single angle ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void RotateAround(Vector3, Single)" )){
			m_functionCallCounts.Add( "Void RotateAround(Vector3, Single)", 0 );
		}
		m_functionCallCounts["Void RotateAround(Vector3, Single)"]++;
			
	}

	public void RotateAroundLocal( UnityEngine.Vector3 axis, System.Single angle ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void RotateAroundLocal(Vector3, Single)" )){
			m_functionCallCounts.Add( "Void RotateAroundLocal(Vector3, Single)", 0 );
		}
		m_functionCallCounts["Void RotateAroundLocal(Vector3, Single)"]++;
			
	}

	public UnityEngine.Transform GetChild( System.Int32 index ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEngine.Transform GetChild(Int32)" )){
			m_functionCallCounts.Add( "UnityEngine.Transform GetChild(Int32)", 0 );
		}
		m_functionCallCounts["UnityEngine.Transform GetChild(Int32)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Int32 GetChildCount( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Int32 GetChildCount()" )){
			m_functionCallCounts.Add( "Int32 GetChildCount()", 0 );
		}
		m_functionCallCounts["Int32 GetChildCount()"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 up {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_up" )){
				m_functionCallCounts.Add( "get_up", 0 );
			}
			m_functionCallCounts["get_up"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_up" )){
				m_functionCallCounts.Add( "set_up", 0 );
			}
			m_functionCallCounts["set_up"]++;
			
		}
	}

	public UnityEngine.Matrix4x4 worldToLocalMatrix {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_worldToLocalMatrix" )){
				m_functionCallCounts.Add( "get_worldToLocalMatrix", 0 );
			}
			m_functionCallCounts["get_worldToLocalMatrix"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Vector3 position {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_position" )){
				m_functionCallCounts.Add( "get_position", 0 );
			}
			m_functionCallCounts["get_position"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_position" )){
				m_functionCallCounts.Add( "set_position", 0 );
			}
			m_functionCallCounts["set_position"]++;
			
		}
	}

	public UnityEngine.Vector3 localEulerAngles {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_localEulerAngles" )){
				m_functionCallCounts.Add( "get_localEulerAngles", 0 );
			}
			m_functionCallCounts["get_localEulerAngles"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_localEulerAngles" )){
				m_functionCallCounts.Add( "set_localEulerAngles", 0 );
			}
			m_functionCallCounts["set_localEulerAngles"]++;
			
		}
	}

	public UnityEngine.Vector3 localScale {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_localScale" )){
				m_functionCallCounts.Add( "get_localScale", 0 );
			}
			m_functionCallCounts["get_localScale"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_localScale" )){
				m_functionCallCounts.Add( "set_localScale", 0 );
			}
			m_functionCallCounts["set_localScale"]++;
			
		}
	}

	public UnityEngine.Quaternion rotation {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_rotation" )){
				m_functionCallCounts.Add( "get_rotation", 0 );
			}
			m_functionCallCounts["get_rotation"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_rotation" )){
				m_functionCallCounts.Add( "set_rotation", 0 );
			}
			m_functionCallCounts["set_rotation"]++;
			
		}
	}

	public UnityEngine.Quaternion localRotation {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_localRotation" )){
				m_functionCallCounts.Add( "get_localRotation", 0 );
			}
			m_functionCallCounts["get_localRotation"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_localRotation" )){
				m_functionCallCounts.Add( "set_localRotation", 0 );
			}
			m_functionCallCounts["set_localRotation"]++;
			
		}
	}

	public UnityEngine.Matrix4x4 localToWorldMatrix {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_localToWorldMatrix" )){
				m_functionCallCounts.Add( "get_localToWorldMatrix", 0 );
			}
			m_functionCallCounts["get_localToWorldMatrix"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Vector3 lossyScale {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_lossyScale" )){
				m_functionCallCounts.Add( "get_lossyScale", 0 );
			}
			m_functionCallCounts["get_lossyScale"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Int32 childCount {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_childCount" )){
				m_functionCallCounts.Add( "get_childCount", 0 );
			}
			m_functionCallCounts["get_childCount"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Vector3 right {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_right" )){
				m_functionCallCounts.Add( "get_right", 0 );
			}
			m_functionCallCounts["get_right"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_right" )){
				m_functionCallCounts.Add( "set_right", 0 );
			}
			m_functionCallCounts["set_right"]++;
			
		}
	}

	public UnityEngine.Transform root {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_root" )){
				m_functionCallCounts.Add( "get_root", 0 );
			}
			m_functionCallCounts["get_root"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Vector3 forward {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_forward" )){
				m_functionCallCounts.Add( "get_forward", 0 );
			}
			m_functionCallCounts["get_forward"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_forward" )){
				m_functionCallCounts.Add( "set_forward", 0 );
			}
			m_functionCallCounts["set_forward"]++;
			
		}
	}

	public UnityEngine.Vector3 localPosition {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_localPosition" )){
				m_functionCallCounts.Add( "get_localPosition", 0 );
			}
			m_functionCallCounts["get_localPosition"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_localPosition" )){
				m_functionCallCounts.Add( "set_localPosition", 0 );
			}
			m_functionCallCounts["set_localPosition"]++;
			
		}
	}

	public UnityEngine.Transform parent {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_parent" )){
				m_functionCallCounts.Add( "get_parent", 0 );
			}
			m_functionCallCounts["get_parent"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_parent" )){
				m_functionCallCounts.Add( "set_parent", 0 );
			}
			m_functionCallCounts["set_parent"]++;
			
		}
	}

	public UnityEngine.Vector3 eulerAngles {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_eulerAngles" )){
				m_functionCallCounts.Add( "get_eulerAngles", 0 );
			}
			m_functionCallCounts["get_eulerAngles"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_eulerAngles" )){
				m_functionCallCounts.Add( "set_eulerAngles", 0 );
			}
			m_functionCallCounts["set_eulerAngles"]++;
			
		}
	}
}
}
