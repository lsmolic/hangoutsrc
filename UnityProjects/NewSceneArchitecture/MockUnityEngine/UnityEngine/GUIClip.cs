using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class GUIClip {
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

	public static void Push( UnityEngine.Rect screenRect ){
	}

	public static void Push( UnityEngine.Rect screenRect, UnityEngine.Vector2 scrollOffset ){
	}

	public static void Push( UnityEngine.Rect screenRect, UnityEngine.Vector2 scrollOffset, UnityEngine.Vector2 renderOffset, System.Boolean resetOffset ){
	}

	public static void Pop( ){
	}

	public static UnityEngine.Vector2 Unclip( UnityEngine.Vector2 pos ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Rect Unclip( UnityEngine.Rect rect ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Vector2 Clip( UnityEngine.Vector2 absolutePos ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static UnityEngine.Rect Clip( UnityEngine.Rect absoluteRect ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void Reapply( ){
	}

	public static void SetGUIClipRect( UnityEngine.Rect r ){
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

	public static UnityEngine.Rect visibleRect {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public static UnityEngine.Rect topmostRect {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
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
