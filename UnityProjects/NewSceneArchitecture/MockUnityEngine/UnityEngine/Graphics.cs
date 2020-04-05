using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class Graphics {
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

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera, System.Int32 submeshIndex ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera, System.Int32 submeshIndex, UnityEngine.MaterialPropertyBlock properties ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera, System.Int32 submeshIndex ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, UnityEngine.Material material, System.Int32 layer ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera, System.Int32 submeshIndex, UnityEngine.MaterialPropertyBlock properties ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera, System.Int32 submeshIndex, UnityEngine.MaterialPropertyBlock properties, System.Boolean castShadows, System.Boolean receiveShadows ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, UnityEngine.Material material, System.Int32 layer, UnityEngine.Camera camera, System.Int32 submeshIndex, UnityEngine.MaterialPropertyBlock properties, System.Boolean castShadows, System.Boolean receiveShadows ){
	}

	public static void DrawMeshNow( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation ){
	}

	public static void DrawMeshNow( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Int32 materialIndex ){
	}

	public static void DrawMeshNow( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix ){
	}

	public static void DrawMeshNow( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, System.Int32 materialIndex ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, System.Int32 materialIndex ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix ){
	}

	public static void DrawMesh( UnityEngine.Mesh mesh, UnityEngine.Matrix4x4 matrix, System.Int32 materialIndex ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, UnityEngine.Material mat ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder, UnityEngine.Material mat ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, UnityEngine.Rect sourceRect, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, UnityEngine.Rect sourceRect, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder, UnityEngine.Material mat ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, UnityEngine.Rect sourceRect, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder, UnityEngine.Color color, UnityEngine.Material mat ){
	}

	public static void DrawTexture( UnityEngine.Rect screenRect, UnityEngine.Texture texture, UnityEngine.Rect sourceRect, System.Int32 leftBorder, System.Int32 rightBorder, System.Int32 topBorder, System.Int32 bottomBorder, UnityEngine.Color color ){
	}

	public static void SetupVertexLights( UnityEngine.Light[] lights ){
	}

	public Graphics( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public static System.Boolean supportsVertexProgram {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static System.String deviceName {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static System.String deviceVersion {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static System.String deviceVendor {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
