using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Event {
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

	public UnityEngine.EventType GetTypeForControl( System.Int32 controlID ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "EventType GetTypeForControl(Int32)" )){
			m_functionCallCounts.Add( "EventType GetTypeForControl(Int32)", 0 );
		}
		m_functionCallCounts["EventType GetTypeForControl(Int32)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void Use( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Use()" )){
			m_functionCallCounts.Add( "Void Use()", 0 );
		}
		m_functionCallCounts["Void Use()"]++;
			
	}

	public static UnityEngine.Event KeyboardEvent( System.String key ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
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

	public override System.Boolean Equals( System.Object obj ){
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

	public Event( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public Event( UnityEngine.Event other ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(Event)" )){
			m_functionCallCounts.Add( "Void .ctor(Event)", 0 );
		}
		m_functionCallCounts["Void .ctor(Event)"]++;
			
	}

	public System.Boolean numeric {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_numeric" )){
				m_functionCallCounts.Add( "get_numeric", 0 );
			}
			m_functionCallCounts["get_numeric"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_numeric" )){
				m_functionCallCounts.Add( "set_numeric", 0 );
			}
			m_functionCallCounts["set_numeric"]++;
			
		}
	}

	public System.Boolean alt {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_alt" )){
				m_functionCallCounts.Add( "get_alt", 0 );
			}
			m_functionCallCounts["get_alt"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_alt" )){
				m_functionCallCounts.Add( "set_alt", 0 );
			}
			m_functionCallCounts["set_alt"]++;
			
		}
	}

	public System.IntPtr camera {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_camera" )){
				m_functionCallCounts.Add( "get_camera", 0 );
			}
			m_functionCallCounts["get_camera"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_camera" )){
				m_functionCallCounts.Add( "set_camera", 0 );
			}
			m_functionCallCounts["set_camera"]++;
			
		}
	}

	public UnityEngine.Vector2 mousePosition {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_mousePosition" )){
				m_functionCallCounts.Add( "get_mousePosition", 0 );
			}
			m_functionCallCounts["get_mousePosition"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_mousePosition" )){
				m_functionCallCounts.Add( "set_mousePosition", 0 );
			}
			m_functionCallCounts["set_mousePosition"]++;
			
		}
	}

	public EventType type {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_type" )){
				m_functionCallCounts.Add( "get_type", 0 );
			}
			m_functionCallCounts["get_type"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_type" )){
				m_functionCallCounts.Add( "set_type", 0 );
			}
			m_functionCallCounts["set_type"]++;
			
		}
	}

	public System.Boolean capsLock {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_capsLock" )){
				m_functionCallCounts.Add( "get_capsLock", 0 );
			}
			m_functionCallCounts["get_capsLock"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_capsLock" )){
				m_functionCallCounts.Add( "set_capsLock", 0 );
			}
			m_functionCallCounts["set_capsLock"]++;
			
		}
	}

	public System.Boolean command {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_command" )){
				m_functionCallCounts.Add( "get_command", 0 );
			}
			m_functionCallCounts["get_command"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_command" )){
				m_functionCallCounts.Add( "set_command", 0 );
			}
			m_functionCallCounts["set_command"]++;
			
		}
	}

	public System.Int32 clickCount {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_clickCount" )){
				m_functionCallCounts.Add( "get_clickCount", 0 );
			}
			m_functionCallCounts["get_clickCount"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_clickCount" )){
				m_functionCallCounts.Add( "set_clickCount", 0 );
			}
			m_functionCallCounts["set_clickCount"]++;
			
		}
	}

	public System.Int32 button {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_button" )){
				m_functionCallCounts.Add( "get_button", 0 );
			}
			m_functionCallCounts["get_button"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_button" )){
				m_functionCallCounts.Add( "set_button", 0 );
			}
			m_functionCallCounts["set_button"]++;
			
		}
	}

	public System.Boolean isMouse {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isMouse" )){
				m_functionCallCounts.Add( "get_isMouse", 0 );
			}
			m_functionCallCounts["get_isMouse"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean isKey {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isKey" )){
				m_functionCallCounts.Add( "get_isKey", 0 );
			}
			m_functionCallCounts["get_isKey"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Char character {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_character" )){
				m_functionCallCounts.Add( "get_character", 0 );
			}
			m_functionCallCounts["get_character"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_character" )){
				m_functionCallCounts.Add( "set_character", 0 );
			}
			m_functionCallCounts["set_character"]++;
			
		}
	}

	public static UnityEngine.Event current {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public UnityEngine.Ray mouseRay {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_mouseRay" )){
				m_functionCallCounts.Add( "get_mouseRay", 0 );
			}
			m_functionCallCounts["get_mouseRay"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_mouseRay" )){
				m_functionCallCounts.Add( "set_mouseRay", 0 );
			}
			m_functionCallCounts["set_mouseRay"]++;
			
		}
	}

	public System.Boolean functionKey {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_functionKey" )){
				m_functionCallCounts.Add( "get_functionKey", 0 );
			}
			m_functionCallCounts["get_functionKey"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String commandName {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_commandName" )){
				m_functionCallCounts.Add( "get_commandName", 0 );
			}
			m_functionCallCounts["get_commandName"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_commandName" )){
				m_functionCallCounts.Add( "set_commandName", 0 );
			}
			m_functionCallCounts["set_commandName"]++;
			
		}
	}

	public UnityEngine.KeyCode keyCode {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_keyCode" )){
				m_functionCallCounts.Add( "get_keyCode", 0 );
			}
			m_functionCallCounts["get_keyCode"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_keyCode" )){
				m_functionCallCounts.Add( "set_keyCode", 0 );
			}
			m_functionCallCounts["set_keyCode"]++;
			
		}
	}

	public System.Boolean shift {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_shift" )){
				m_functionCallCounts.Add( "get_shift", 0 );
			}
			m_functionCallCounts["get_shift"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_shift" )){
				m_functionCallCounts.Add( "set_shift", 0 );
			}
			m_functionCallCounts["set_shift"]++;
			
		}
	}

	public System.Single pressure {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_pressure" )){
				m_functionCallCounts.Add( "get_pressure", 0 );
			}
			m_functionCallCounts["get_pressure"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_pressure" )){
				m_functionCallCounts.Add( "set_pressure", 0 );
			}
			m_functionCallCounts["set_pressure"]++;
			
		}
	}

	public UnityEngine.EventType rawType {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_rawType" )){
				m_functionCallCounts.Add( "get_rawType", 0 );
			}
			m_functionCallCounts["get_rawType"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean control {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_control" )){
				m_functionCallCounts.Add( "get_control", 0 );
			}
			m_functionCallCounts["get_control"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_control" )){
				m_functionCallCounts.Add( "set_control", 0 );
			}
			m_functionCallCounts["set_control"]++;
			
		}
	}

	public UnityEngine.Vector2 delta {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_delta" )){
				m_functionCallCounts.Add( "get_delta", 0 );
			}
			m_functionCallCounts["get_delta"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_delta" )){
				m_functionCallCounts.Add( "set_delta", 0 );
			}
			m_functionCallCounts["set_delta"]++;
			
		}
	}
}
}
