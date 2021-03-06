using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class EditorStyles {
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

	public EditorStyles( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.GUIStyle popup {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle objectField {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle textField {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle structHeadingLabel {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle colorField {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle label {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.Font boldFont {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle objectFieldThumb {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle toggle {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.Font standardFont {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle toggleGroup {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle numberField {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle foldout {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.GUIStyle layerMaskField {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}
}
}
