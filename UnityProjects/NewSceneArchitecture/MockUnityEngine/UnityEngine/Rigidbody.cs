using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Rigidbody : UnityEngine.Component {
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

	public void SetDensity( System.Single density ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetDensity(Single)" )){
			m_functionCallCounts.Add( "Void SetDensity(Single)", 0 );
		}
		m_functionCallCounts["Void SetDensity(Single)"]++;
			
	}

	public void AddForce( UnityEngine.Vector3 force, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddForce(Vector3, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddForce(Vector3, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddForce(Vector3, ForceMode)"]++;
			
	}

	public void AddForce( UnityEngine.Vector3 force ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddForce(Vector3)" )){
			m_functionCallCounts.Add( "Void AddForce(Vector3)", 0 );
		}
		m_functionCallCounts["Void AddForce(Vector3)"]++;
			
	}

	public void AddForce( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddForce(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Void AddForce(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Void AddForce(Single, Single, Single)"]++;
			
	}

	public void AddForce( System.Single x, System.Single y, System.Single z, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddForce(Single, Single, Single, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddForce(Single, Single, Single, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddForce(Single, Single, Single, ForceMode)"]++;
			
	}

	public void AddRelativeForce( UnityEngine.Vector3 force, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeForce(Vector3, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddRelativeForce(Vector3, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddRelativeForce(Vector3, ForceMode)"]++;
			
	}

	public void AddRelativeForce( UnityEngine.Vector3 force ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeForce(Vector3)" )){
			m_functionCallCounts.Add( "Void AddRelativeForce(Vector3)", 0 );
		}
		m_functionCallCounts["Void AddRelativeForce(Vector3)"]++;
			
	}

	public void AddRelativeForce( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeForce(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Void AddRelativeForce(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Void AddRelativeForce(Single, Single, Single)"]++;
			
	}

	public void AddRelativeForce( System.Single x, System.Single y, System.Single z, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeForce(Single, Single, Single, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddRelativeForce(Single, Single, Single, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddRelativeForce(Single, Single, Single, ForceMode)"]++;
			
	}

	public void AddTorque( UnityEngine.Vector3 torque, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddTorque(Vector3, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddTorque(Vector3, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddTorque(Vector3, ForceMode)"]++;
			
	}

	public void AddTorque( UnityEngine.Vector3 torque ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddTorque(Vector3)" )){
			m_functionCallCounts.Add( "Void AddTorque(Vector3)", 0 );
		}
		m_functionCallCounts["Void AddTorque(Vector3)"]++;
			
	}

	public void AddTorque( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddTorque(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Void AddTorque(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Void AddTorque(Single, Single, Single)"]++;
			
	}

	public void AddTorque( System.Single x, System.Single y, System.Single z, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddTorque(Single, Single, Single, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddTorque(Single, Single, Single, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddTorque(Single, Single, Single, ForceMode)"]++;
			
	}

	public void AddRelativeTorque( UnityEngine.Vector3 torque, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeTorque(Vector3, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddRelativeTorque(Vector3, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddRelativeTorque(Vector3, ForceMode)"]++;
			
	}

	public void AddRelativeTorque( UnityEngine.Vector3 torque ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeTorque(Vector3)" )){
			m_functionCallCounts.Add( "Void AddRelativeTorque(Vector3)", 0 );
		}
		m_functionCallCounts["Void AddRelativeTorque(Vector3)"]++;
			
	}

	public void AddRelativeTorque( System.Single x, System.Single y, System.Single z ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeTorque(Single, Single, Single)" )){
			m_functionCallCounts.Add( "Void AddRelativeTorque(Single, Single, Single)", 0 );
		}
		m_functionCallCounts["Void AddRelativeTorque(Single, Single, Single)"]++;
			
	}

	public void AddRelativeTorque( System.Single x, System.Single y, System.Single z, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddRelativeTorque(Single, Single, Single, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddRelativeTorque(Single, Single, Single, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddRelativeTorque(Single, Single, Single, ForceMode)"]++;
			
	}

	public void AddForceAtPosition( UnityEngine.Vector3 force, UnityEngine.Vector3 position, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddForceAtPosition(Vector3, Vector3, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddForceAtPosition(Vector3, Vector3, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddForceAtPosition(Vector3, Vector3, ForceMode)"]++;
			
	}

	public void AddForceAtPosition( UnityEngine.Vector3 force, UnityEngine.Vector3 position ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddForceAtPosition(Vector3, Vector3)" )){
			m_functionCallCounts.Add( "Void AddForceAtPosition(Vector3, Vector3)", 0 );
		}
		m_functionCallCounts["Void AddForceAtPosition(Vector3, Vector3)"]++;
			
	}

	public void AddExplosionForce( System.Single explosionForce, UnityEngine.Vector3 explosionPosition, System.Single explosionRadius, System.Single upwardsModifier, UnityEngine.ForceMode mode ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddExplosionForce(Single, Vector3, Single, Single, ForceMode)" )){
			m_functionCallCounts.Add( "Void AddExplosionForce(Single, Vector3, Single, Single, ForceMode)", 0 );
		}
		m_functionCallCounts["Void AddExplosionForce(Single, Vector3, Single, Single, ForceMode)"]++;
			
	}

	public void AddExplosionForce( System.Single explosionForce, UnityEngine.Vector3 explosionPosition, System.Single explosionRadius, System.Single upwardsModifier ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddExplosionForce(Single, Vector3, Single, Single)" )){
			m_functionCallCounts.Add( "Void AddExplosionForce(Single, Vector3, Single, Single)", 0 );
		}
		m_functionCallCounts["Void AddExplosionForce(Single, Vector3, Single, Single)"]++;
			
	}

	public void AddExplosionForce( System.Single explosionForce, UnityEngine.Vector3 explosionPosition, System.Single explosionRadius ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void AddExplosionForce(Single, Vector3, Single)" )){
			m_functionCallCounts.Add( "Void AddExplosionForce(Single, Vector3, Single)", 0 );
		}
		m_functionCallCounts["Void AddExplosionForce(Single, Vector3, Single)"]++;
			
	}

	public UnityEngine.Vector3 ClosestPointOnBounds( UnityEngine.Vector3 position ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 ClosestPointOnBounds(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 ClosestPointOnBounds(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 ClosestPointOnBounds(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 GetRelativePointVelocity( UnityEngine.Vector3 relativePoint ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 GetRelativePointVelocity(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 GetRelativePointVelocity(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 GetRelativePointVelocity(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector3 GetPointVelocity( UnityEngine.Vector3 worldPoint ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector3 GetPointVelocity(Vector3)" )){
			m_functionCallCounts.Add( "Vector3 GetPointVelocity(Vector3)", 0 );
		}
		m_functionCallCounts["Vector3 GetPointVelocity(Vector3)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void MovePosition( UnityEngine.Vector3 position ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void MovePosition(Vector3)" )){
			m_functionCallCounts.Add( "Void MovePosition(Vector3)", 0 );
		}
		m_functionCallCounts["Void MovePosition(Vector3)"]++;
			
	}

	public void MoveRotation( UnityEngine.Quaternion rot ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void MoveRotation(Quaternion)" )){
			m_functionCallCounts.Add( "Void MoveRotation(Quaternion)", 0 );
		}
		m_functionCallCounts["Void MoveRotation(Quaternion)"]++;
			
	}

	public void Sleep( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Sleep()" )){
			m_functionCallCounts.Add( "Void Sleep()", 0 );
		}
		m_functionCallCounts["Void Sleep()"]++;
			
	}

	public System.Boolean IsSleeping( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean IsSleeping()" )){
			m_functionCallCounts.Add( "Boolean IsSleeping()", 0 );
		}
		m_functionCallCounts["Boolean IsSleeping()"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void WakeUp( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void WakeUp()" )){
			m_functionCallCounts.Add( "Void WakeUp()", 0 );
		}
		m_functionCallCounts["Void WakeUp()"]++;
			
	}

	public void SetMaxAngularVelocity( System.Single a ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void SetMaxAngularVelocity(Single)" )){
			m_functionCallCounts.Add( "Void SetMaxAngularVelocity(Single)", 0 );
		}
		m_functionCallCounts["Void SetMaxAngularVelocity(Single)"]++;
			
	}

	public Rigidbody( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Boolean freezeRotation {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_freezeRotation" )){
				m_functionCallCounts.Add( "get_freezeRotation", 0 );
			}
			m_functionCallCounts["get_freezeRotation"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_freezeRotation" )){
				m_functionCallCounts.Add( "set_freezeRotation", 0 );
			}
			m_functionCallCounts["set_freezeRotation"]++;
			
		}
	}

	public System.Boolean detectCollisions {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_detectCollisions" )){
				m_functionCallCounts.Add( "get_detectCollisions", 0 );
			}
			m_functionCallCounts["get_detectCollisions"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_detectCollisions" )){
				m_functionCallCounts.Add( "set_detectCollisions", 0 );
			}
			m_functionCallCounts["set_detectCollisions"]++;
			
		}
	}

	public UnityEngine.Quaternion inertiaTensorRotation {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_inertiaTensorRotation" )){
				m_functionCallCounts.Add( "get_inertiaTensorRotation", 0 );
			}
			m_functionCallCounts["get_inertiaTensorRotation"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_inertiaTensorRotation" )){
				m_functionCallCounts.Add( "set_inertiaTensorRotation", 0 );
			}
			m_functionCallCounts["set_inertiaTensorRotation"]++;
			
		}
	}

	public UnityEngine.Vector3 worldCenterOfMass {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_worldCenterOfMass" )){
				m_functionCallCounts.Add( "get_worldCenterOfMass", 0 );
			}
			m_functionCallCounts["get_worldCenterOfMass"]++;
			
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

	public System.Single drag {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_drag" )){
				m_functionCallCounts.Add( "get_drag", 0 );
			}
			m_functionCallCounts["get_drag"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_drag" )){
				m_functionCallCounts.Add( "set_drag", 0 );
			}
			m_functionCallCounts["set_drag"]++;
			
		}
	}

	public System.Single sleepAngularVelocity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_sleepAngularVelocity" )){
				m_functionCallCounts.Add( "get_sleepAngularVelocity", 0 );
			}
			m_functionCallCounts["get_sleepAngularVelocity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_sleepAngularVelocity" )){
				m_functionCallCounts.Add( "set_sleepAngularVelocity", 0 );
			}
			m_functionCallCounts["set_sleepAngularVelocity"]++;
			
		}
	}

	public UnityEngine.Vector3 angularVelocity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_angularVelocity" )){
				m_functionCallCounts.Add( "get_angularVelocity", 0 );
			}
			m_functionCallCounts["get_angularVelocity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_angularVelocity" )){
				m_functionCallCounts.Add( "set_angularVelocity", 0 );
			}
			m_functionCallCounts["set_angularVelocity"]++;
			
		}
	}

	public UnityEngine.RigidbodyInterpolation interpolation {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_interpolation" )){
				m_functionCallCounts.Add( "get_interpolation", 0 );
			}
			m_functionCallCounts["get_interpolation"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_interpolation" )){
				m_functionCallCounts.Add( "set_interpolation", 0 );
			}
			m_functionCallCounts["set_interpolation"]++;
			
		}
	}

	public System.Boolean isKinematic {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isKinematic" )){
				m_functionCallCounts.Add( "get_isKinematic", 0 );
			}
			m_functionCallCounts["get_isKinematic"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_isKinematic" )){
				m_functionCallCounts.Add( "set_isKinematic", 0 );
			}
			m_functionCallCounts["set_isKinematic"]++;
			
		}
	}

	public UnityEngine.Vector3 centerOfMass {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_centerOfMass" )){
				m_functionCallCounts.Add( "get_centerOfMass", 0 );
			}
			m_functionCallCounts["get_centerOfMass"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_centerOfMass" )){
				m_functionCallCounts.Add( "set_centerOfMass", 0 );
			}
			m_functionCallCounts["set_centerOfMass"]++;
			
		}
	}

	public System.Boolean useGravity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_useGravity" )){
				m_functionCallCounts.Add( "get_useGravity", 0 );
			}
			m_functionCallCounts["get_useGravity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_useGravity" )){
				m_functionCallCounts.Add( "set_useGravity", 0 );
			}
			m_functionCallCounts["set_useGravity"]++;
			
		}
	}

	public System.Boolean useConeFriction {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_useConeFriction" )){
				m_functionCallCounts.Add( "get_useConeFriction", 0 );
			}
			m_functionCallCounts["get_useConeFriction"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_useConeFriction" )){
				m_functionCallCounts.Add( "set_useConeFriction", 0 );
			}
			m_functionCallCounts["set_useConeFriction"]++;
			
		}
	}

	public System.Single mass {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_mass" )){
				m_functionCallCounts.Add( "get_mass", 0 );
			}
			m_functionCallCounts["get_mass"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_mass" )){
				m_functionCallCounts.Add( "set_mass", 0 );
			}
			m_functionCallCounts["set_mass"]++;
			
		}
	}

	public UnityEngine.Vector3 velocity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_velocity" )){
				m_functionCallCounts.Add( "get_velocity", 0 );
			}
			m_functionCallCounts["get_velocity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_velocity" )){
				m_functionCallCounts.Add( "set_velocity", 0 );
			}
			m_functionCallCounts["set_velocity"]++;
			
		}
	}

	public System.Single angularDrag {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_angularDrag" )){
				m_functionCallCounts.Add( "get_angularDrag", 0 );
			}
			m_functionCallCounts["get_angularDrag"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_angularDrag" )){
				m_functionCallCounts.Add( "set_angularDrag", 0 );
			}
			m_functionCallCounts["set_angularDrag"]++;
			
		}
	}

	public System.Single maxAngularVelocity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_maxAngularVelocity" )){
				m_functionCallCounts.Add( "get_maxAngularVelocity", 0 );
			}
			m_functionCallCounts["get_maxAngularVelocity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_maxAngularVelocity" )){
				m_functionCallCounts.Add( "set_maxAngularVelocity", 0 );
			}
			m_functionCallCounts["set_maxAngularVelocity"]++;
			
		}
	}

	public System.Single sleepVelocity {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_sleepVelocity" )){
				m_functionCallCounts.Add( "get_sleepVelocity", 0 );
			}
			m_functionCallCounts["get_sleepVelocity"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_sleepVelocity" )){
				m_functionCallCounts.Add( "set_sleepVelocity", 0 );
			}
			m_functionCallCounts["set_sleepVelocity"]++;
			
		}
	}

	public System.Int32 solverIterationCount {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_solverIterationCount" )){
				m_functionCallCounts.Add( "get_solverIterationCount", 0 );
			}
			m_functionCallCounts["get_solverIterationCount"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_solverIterationCount" )){
				m_functionCallCounts.Add( "set_solverIterationCount", 0 );
			}
			m_functionCallCounts["set_solverIterationCount"]++;
			
		}
	}

	public UnityEngine.Vector3 inertiaTensor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_inertiaTensor" )){
				m_functionCallCounts.Add( "get_inertiaTensor", 0 );
			}
			m_functionCallCounts["get_inertiaTensor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_inertiaTensor" )){
				m_functionCallCounts.Add( "set_inertiaTensor", 0 );
			}
			m_functionCallCounts["set_inertiaTensor"]++;
			
		}
	}
}
}
