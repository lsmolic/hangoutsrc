using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public class GUIText : UnityEngine.GUIElement {
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

	public GUIText( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public System.Single lineSpacing {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_lineSpacing" )){
				m_functionCallCounts.Add( "get_lineSpacing", 0 );
			}
			m_functionCallCounts["get_lineSpacing"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_lineSpacing" )){
				m_functionCallCounts.Add( "set_lineSpacing", 0 );
			}
			m_functionCallCounts["set_lineSpacing"]++;
			
		}
	}

	public UnityEngine.TextAlignment alignment {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_alignment" )){
				m_functionCallCounts.Add( "get_alignment", 0 );
			}
			m_functionCallCounts["get_alignment"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_alignment" )){
				m_functionCallCounts.Add( "set_alignment", 0 );
			}
			m_functionCallCounts["set_alignment"]++;
			
		}
	}

	public UnityEngine.Vector2 pixelOffset {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_pixelOffset" )){
				m_functionCallCounts.Add( "get_pixelOffset", 0 );
			}
			m_functionCallCounts["get_pixelOffset"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_pixelOffset" )){
				m_functionCallCounts.Add( "set_pixelOffset", 0 );
			}
			m_functionCallCounts["set_pixelOffset"]++;
			
		}
	}

	public System.Single tabSize {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_tabSize" )){
				m_functionCallCounts.Add( "get_tabSize", 0 );
			}
			m_functionCallCounts["get_tabSize"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_tabSize" )){
				m_functionCallCounts.Add( "set_tabSize", 0 );
			}
			m_functionCallCounts["set_tabSize"]++;
			
		}
	}

	public UnityEngine.Font font {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_font" )){
				m_functionCallCounts.Add( "get_font", 0 );
			}
			m_functionCallCounts["get_font"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_font" )){
				m_functionCallCounts.Add( "set_font", 0 );
			}
			m_functionCallCounts["set_font"]++;
			
		}
	}

	public UnityEngine.TextAnchor anchor {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_anchor" )){
				m_functionCallCounts.Add( "get_anchor", 0 );
			}
			m_functionCallCounts["get_anchor"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_anchor" )){
				m_functionCallCounts.Add( "set_anchor", 0 );
			}
			m_functionCallCounts["set_anchor"]++;
			
		}
	}

	public System.String text {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_text" )){
				m_functionCallCounts.Add( "get_text", 0 );
			}
			m_functionCallCounts["get_text"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_text" )){
				m_functionCallCounts.Add( "set_text", 0 );
			}
			m_functionCallCounts["set_text"]++;
			
		}
	}

	public UnityEngine.Material material {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_material" )){
				m_functionCallCounts.Add( "get_material", 0 );
			}
			m_functionCallCounts["get_material"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_material" )){
				m_functionCallCounts.Add( "set_material", 0 );
			}
			m_functionCallCounts["set_material"]++;
			
		}
	}
}
}
