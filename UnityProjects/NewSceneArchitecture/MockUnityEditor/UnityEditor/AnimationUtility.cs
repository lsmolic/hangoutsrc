using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class AnimationUtility {
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

	public static UnityEditor.AnimationClipCurveData[] GetAnimatableProperties( UnityEngine.GameObject go ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Boolean GetFloatValue( UnityEngine.GameObject root, System.String relativePath, System.Type type, System.String propertyName, out System.Single data ){
		data = default(System.Single);
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEditor.AnimationClipCurveData[] GetAllCurves( UnityEngine.AnimationClip clip, System.Boolean includeCurveData ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEditor.AnimationClipCurveData[] GetAllCurves( UnityEngine.AnimationClip clip ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.AnimationCurve GetEditorCurve( UnityEngine.AnimationClip clip, System.String relativePath, System.Type type, System.String propertyName ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void SetEditorCurve( UnityEngine.AnimationClip clip, System.String relativePath, System.Type type, System.String propertyName, UnityEngine.AnimationCurve curve ){
	}

	public static UnityEngine.AnimationEvent[] GetAnimationEvents( UnityEngine.AnimationClip clip ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void SetAnimationEvents( UnityEngine.AnimationClip clip, UnityEngine.AnimationEvent[] events ){
	}

	public AnimationUtility( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}
}
}
