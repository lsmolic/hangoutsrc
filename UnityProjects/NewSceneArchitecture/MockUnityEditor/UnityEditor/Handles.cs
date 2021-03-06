using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class Handles {
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

	public static UnityEngine.Vector3 PositionHandle( UnityEngine.Vector3 position ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector3 PositionHandle( UnityEngine.Vector3 position, ref UnityEngine.Quaternion rotation ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Quaternion RotationHandle( UnityEngine.Quaternion rotation, UnityEngine.Vector3 position ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector3 ScaleHandle( UnityEngine.Vector3 scale, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector3 Slider( UnityEngine.Vector3 position, UnityEngine.Vector3 direction ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector3 Slider( UnityEngine.Vector3 position, UnityEngine.Vector3 direction, System.Single size, UnityEditor.DrawCapFunction drawFunc, System.Single snap ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Quaternion FreeRotateHandle( UnityEngine.Quaternion rotation, UnityEngine.Vector3 position, System.Single size ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector3 FreeMoveHandle( UnityEngine.Vector3 position, ref UnityEngine.Quaternion rotation, System.Single size, UnityEditor.DrawCapFunction capFunc ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Single ScaleSlider( System.Single scale, UnityEngine.Vector3 position, UnityEngine.Vector3 direction, UnityEngine.Quaternion rotation, System.Single size, System.Single snap ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Single ScaleValueHandle( System.Single value, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size, UnityEditor.DrawCapFunction capFunc, System.Single snap ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Quaternion Disc( UnityEngine.Quaternion rotation, UnityEngine.Vector3 position, UnityEngine.Vector3 axis, System.Single size, System.Boolean cutoffPlane, System.Single snap ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Boolean Button( UnityEngine.Vector3 position, UnityEngine.Quaternion direction, System.Single size, System.Single pickSize, UnityEditor.DrawCapFunction capFunc ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void DrawCube( System.Int32 controlID, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
	}

	public static void DrawSphere( System.Int32 controlID, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
	}

	public static void DrawCone( System.Int32 controlID, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
	}

	public static void DrawCylinder( System.Int32 controlID, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
	}

	public static void DrawRectangle( System.Int32 controlID, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
	}

	public static void DrawArrow( System.Int32 controlID, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Single size ){
	}

	public static void DrawLine( UnityEngine.Vector3 p1, UnityEngine.Vector3 p2 ){
	}

	public static void DrawPolyLine( UnityEngine.Vector3[] points ){
	}

	public static void DrawWireDisc( UnityEngine.Vector3 center, UnityEngine.Vector3 normal, System.Single radius ){
	}

	public static void DrawWireArc( UnityEngine.Vector3 center, UnityEngine.Vector3 normal, UnityEngine.Vector3 from, System.Single angle, System.Single radius ){
	}

	public static void DrawSolidDisc( UnityEngine.Vector3 center, UnityEngine.Vector3 normal, System.Single radius ){
	}

	public static void DrawSolidArc( UnityEngine.Vector3 center, UnityEngine.Vector3 normal, UnityEngine.Vector3 from, System.Single angle, System.Single radius ){
	}

	public static void Label( UnityEngine.Vector3 position, System.String text ){
	}

	public static void Label( UnityEngine.Vector3 position, UnityEngine.Texture image ){
	}

	public static void Label( UnityEngine.Vector3 position, UnityEngine.GUIContent content ){
	}

	public static void Label( UnityEngine.Vector3 position, System.String text, UnityEngine.GUIStyle style ){
	}

	public static void Label( UnityEngine.Vector3 position, UnityEngine.GUIContent content, UnityEngine.GUIStyle style ){
	}

	public static void ClearCamera( UnityEngine.Rect position, UnityEngine.Camera camera ){
	}

	public static void DrawCamera( UnityEngine.Rect position, UnityEngine.Camera camera, System.Int32 renderMode ){
	}

	public static void SetCamera( UnityEngine.Camera camera ){
	}

	public static void SetCamera( UnityEngine.Rect position, UnityEngine.Camera camera ){
	}

	public static void BeginGUI( ){
	}

	public static void BeginGUI( UnityEngine.Rect position ){
	}

	public static void EndGUI( ){
	}

	public Handles( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Color color {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static System.Boolean lighting {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public UnityEngine.Camera currentCamera {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_currentCamera" )){
				m_functionCallCounts.Add( "get_currentCamera", 0 );
			}
			m_functionCallCounts["get_currentCamera"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_currentCamera" )){
				m_functionCallCounts.Add( "set_currentCamera", 0 );
			}
			m_functionCallCounts["set_currentCamera"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
