using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Resources {
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

	public static UnityEngine.Object Load(System.String path) {
		throw new Exception("Unit tests should *not* be trying to do any real IO.");
	}

	public static UnityEngine.Object Load( System.String path, System.Type type ){
		throw new Exception("Unit tests should *not* be trying to do any real IO.");
	}

	public static UnityEngine.Object[] LoadAll(System.String path, System.Type type) {
		throw new Exception("Unit tests should *not* be trying to do any real IO.");
	}

	public static UnityEngine.Object[] LoadAll(System.String path) {
		throw new Exception("Unit tests should *not* be trying to do any real IO.");
	}

	public static UnityEngine.Object GetBuiltinResource(System.Type type, System.String path, UnityEngine.HideFlags flags) {
		throw new Exception("Unit tests should *not* be trying to do any real IO.");
	}

	public static UnityEngine.Object LoadAssetAtPath(System.String assetPath, System.Type type) {
		throw new Exception("Unit tests should *not* be trying to do any real IO.");
	}

	public Resources( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}
}
}
