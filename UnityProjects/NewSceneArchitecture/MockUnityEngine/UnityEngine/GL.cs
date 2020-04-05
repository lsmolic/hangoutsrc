using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class GL {
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

	public static void Vertex3( System.Single x, System.Single y, System.Single z ){
	}

	public static void Vertex( UnityEngine.Vector3 v ){
	}

	public static void Color( UnityEngine.Color c ){
	}

	public static void TexCoord( UnityEngine.Vector3 v ){
	}

	public static void TexCoord2( System.Single x, System.Single y ){
	}

	public static void TexCoord3( System.Single x, System.Single y, System.Single z ){
	}

	public static void MultiTexCoord2( System.Int32 unit, System.Single x, System.Single y ){
	}

	public static void MultiTexCoord3( System.Int32 unit, System.Single x, System.Single y, System.Single z ){
	}

	public static void MultiTexCoord( System.Int32 unit, UnityEngine.Vector3 v ){
	}

	public static void Begin( System.Int32 mode ){
	}

	public static void End( ){
	}

	public static void LoadOrtho( ){
	}

	public static void LoadPixelMatrix( ){
	}

	public static void LoadPixelMatrix( System.Single left, System.Single right, System.Single bottom, System.Single top ){
	}

	public static void Viewport( UnityEngine.Rect pixelRect ){
	}

	public static void LoadProjectionMatrix( UnityEngine.Matrix4x4 mat ){
	}

	public static void LoadIdentity( ){
	}

	public static void MultMatrix( UnityEngine.Matrix4x4 mat ){
	}

	public static void PushMatrix( ){
	}

	public static void PopMatrix( ){
	}

	public static void SetRevertBackfacing( System.Boolean revertBackFaces ){
	}

	public static void Clear( System.Boolean clearDepth, System.Boolean clearColor, UnityEngine.Color backgroundColor ){
	}

	public GL( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public static UnityEngine.Matrix4x4 modelview {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
		}
	}
}
}