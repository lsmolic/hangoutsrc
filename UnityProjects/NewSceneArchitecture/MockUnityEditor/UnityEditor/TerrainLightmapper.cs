using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class TerrainLightmapper {
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

	public static UnityEngine.Quaternion[] SuperSampleShadowJitter( System.Single shadowAngle, System.Int32 shadowSamples ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector3[] SuperSampleShadowOffset( System.Single shadowSize, System.Int32 shadowSamples ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void UpdateTreeLightmapColor( UnityEngine.Texture2D shadowMap, UnityEngine.TerrainData terrain ){
	}

	public static void UpdateTreeColor( UnityEngine.Texture2D shadowMap, UnityEngine.TerrainData terrain ){
	}

	public static void Generate( UnityEngine.Vector3 terrainPosition, UnityEngine.TerrainData terrainData, UnityEngine.TerrainCollider collider, UnityEngine.Light[] lights, UnityEngine.Color ambient, System.Int32 shadowSamples, UnityEngine.Texture2D lightmap, UnityEngine.Texture2D shadowMap ){
	}

	public TerrainLightmapper( ){
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
