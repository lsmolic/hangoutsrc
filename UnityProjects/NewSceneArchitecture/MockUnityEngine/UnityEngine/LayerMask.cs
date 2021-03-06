using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public struct LayerMask {
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

	private static readonly List<string> mLayers = new List<string>();

	public static System.String LayerToName( System.Int32 layer )
	{
		string result = "";
		if( mLayers.Count > layer )
		{
			result = mLayers[layer];
		}
		return result;
	}

	public static System.Int32 NameToLayer( System.String layerName )
	{
		int index = -1;
		if(!mLayers.Contains(layerName))
		{
			mLayers.Add(layerName);
			index = mLayers.Count - 1;
		}
		else
		{
			index = mLayers.FindIndex(delegate(string a)
			{
				return a == layerName;
			});
		}
		return index;
	}

	public static implicit operator System.Int32( UnityEngine.LayerMask mask ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static implicit operator UnityEngine.LayerMask( System.Int32 intVal ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Int32 value {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_value" )){
				m_functionCallCounts.Add( "get_value", 0 );
			}
			m_functionCallCounts["get_value"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_value" )){
				m_functionCallCounts.Add( "set_value", 0 );
			}
			m_functionCallCounts["set_value"]++;
			
		}
	}
}
}
