using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Gizmos {
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

	public static void DrawRay( UnityEngine.Ray r ){
	}

	public static void DrawRay( UnityEngine.Vector3 from, UnityEngine.Vector3 direction ){
	}

	public static void DrawLine( UnityEngine.Vector3 from, UnityEngine.Vector3 to ){
	}

	public static void DrawWireSphere( UnityEngine.Vector3 center, System.Single radius ){
	}

	public static void DrawSphere( UnityEngine.Vector3 center, System.Single radius ){
	}

	public static void DrawWireCube( UnityEngine.Vector3 center, UnityEngine.Vector3 size ){
	}

	public static void DrawCube( UnityEngine.Vector3 center, UnityEngine.Vector3 size ){
	}

	public static void DrawIcon( UnityEngine.Vector3 center, System.String name ){
	}

	public static void DrawGUITexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture ){
	}

	public static void DrawGUITexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, UnityEngine.Material mat ){
	}

	public static void DrawGUITexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder, UnityEngine.Material mat ){
	}

	public static void DrawGUITexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder ){
	}

	public static void DrawFrustum( UnityEngine.Vector3 center, System.Single fov, System.Single maxRange, System.Single minRange, System.Single aspect ){
	}

	public Gizmos( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public static UnityEngine.Color color {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}

	public static UnityEngine.Matrix4x4 matrix {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}
}
}
