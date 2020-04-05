using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
[UnityEngine.SerializePrivateVariables]
[System.SerializableAttribute]
public class GUIStyle {
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

	public void Draw( UnityEngine.Rect position, System.Boolean isHover, System.Boolean isActive, System.Boolean on, System.Boolean hasKeyboardFocus ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Draw(Rect, Boolean, Boolean, Boolean, Boolean)" )){
			m_functionCallCounts.Add( "Void Draw(Rect, Boolean, Boolean, Boolean, Boolean)", 0 );
		}
		m_functionCallCounts["Void Draw(Rect, Boolean, Boolean, Boolean, Boolean)"]++;
			
	}

	public void Draw( UnityEngine.Rect position, System.String text, System.Boolean isHover, System.Boolean isActive, System.Boolean on, System.Boolean hasKeyboardFocus ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Draw(Rect, System.String, Boolean, Boolean, Boolean, Boolean)" )){
			m_functionCallCounts.Add( "Void Draw(Rect, System.String, Boolean, Boolean, Boolean, Boolean)", 0 );
		}
		m_functionCallCounts["Void Draw(Rect, System.String, Boolean, Boolean, Boolean, Boolean)"]++;
			
	}

	public void Draw( UnityEngine.Rect position, UnityEngine.Texture image, System.Boolean isHover, System.Boolean isActive, System.Boolean on, System.Boolean hasKeyboardFocus ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Draw(Rect, UnityEngine.Texture, Boolean, Boolean, Boolean, Boolean)" )){
			m_functionCallCounts.Add( "Void Draw(Rect, UnityEngine.Texture, Boolean, Boolean, Boolean, Boolean)", 0 );
		}
		m_functionCallCounts["Void Draw(Rect, UnityEngine.Texture, Boolean, Boolean, Boolean, Boolean)"]++;
			
	}

	public void Draw( UnityEngine.Rect position, UnityEngine.GUIContent content, System.Boolean isHover, System.Boolean isActive, System.Boolean on, System.Boolean hasKeyboardFocus ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Draw(Rect, UnityEngine.GUIContent, Boolean, Boolean, Boolean, Boolean)" )){
			m_functionCallCounts.Add( "Void Draw(Rect, UnityEngine.GUIContent, Boolean, Boolean, Boolean, Boolean)", 0 );
		}
		m_functionCallCounts["Void Draw(Rect, UnityEngine.GUIContent, Boolean, Boolean, Boolean, Boolean)"]++;
			
	}

	public void Draw( UnityEngine.Rect position, UnityEngine.GUIContent content, System.Int32 controlID ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Draw(Rect, UnityEngine.GUIContent, Int32)" )){
			m_functionCallCounts.Add( "Void Draw(Rect, UnityEngine.GUIContent, Int32)", 0 );
		}
		m_functionCallCounts["Void Draw(Rect, UnityEngine.GUIContent, Int32)"]++;
			
	}

	public void Draw( UnityEngine.Rect position, UnityEngine.GUIContent content, System.Int32 controlID, System.Boolean on ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Draw(Rect, UnityEngine.GUIContent, Int32, Boolean)" )){
			m_functionCallCounts.Add( "Void Draw(Rect, UnityEngine.GUIContent, Int32, Boolean)", 0 );
		}
		m_functionCallCounts["Void Draw(Rect, UnityEngine.GUIContent, Int32, Boolean)"]++;
			
	}

	public void DrawCursor( UnityEngine.Rect position, UnityEngine.GUIContent content, System.Int32 controlID, System.Int32 Character ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void DrawCursor(Rect, UnityEngine.GUIContent, Int32, Int32)" )){
			m_functionCallCounts.Add( "Void DrawCursor(Rect, UnityEngine.GUIContent, Int32, Int32)", 0 );
		}
		m_functionCallCounts["Void DrawCursor(Rect, UnityEngine.GUIContent, Int32, Int32)"]++;
			
	}

	public void DrawWithTextSelection( UnityEngine.Rect position, UnityEngine.GUIContent content, System.Int32 controlID, System.Int32 firstSelectedCharacter, System.Int32 lastSelectedCharacter ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void DrawWithTextSelection(Rect, UnityEngine.GUIContent, Int32, Int32, Int32)" )){
			m_functionCallCounts.Add( "Void DrawWithTextSelection(Rect, UnityEngine.GUIContent, Int32, Int32, Int32)", 0 );
		}
		m_functionCallCounts["Void DrawWithTextSelection(Rect, UnityEngine.GUIContent, Int32, Int32, Int32)"]++;
			
	}

	public UnityEngine.Vector2 GetCursorPixelPosition( UnityEngine.Rect position, UnityEngine.GUIContent content, System.Int32 cursorStringIndex ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector2 GetCursorPixelPosition(Rect, UnityEngine.GUIContent, Int32)" )){
			m_functionCallCounts.Add( "Vector2 GetCursorPixelPosition(Rect, UnityEngine.GUIContent, Int32)", 0 );
		}
		m_functionCallCounts["Vector2 GetCursorPixelPosition(Rect, UnityEngine.GUIContent, Int32)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void Internal_GetCursorPixelPosition( System.IntPtr target, UnityEngine.Rect position, System.String text, UnityEngine.Texture image, System.Int32 cursorStringIndex, out UnityEngine.Vector2 ret ){
		ret = default(UnityEngine.Vector2);
	}

	public System.Int32 GetCursorStringIndex( UnityEngine.Rect position, UnityEngine.GUIContent content, UnityEngine.Vector2 cursorPixelPosition ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Int32 GetCursorStringIndex(Rect, UnityEngine.GUIContent, Vector2)" )){
			m_functionCallCounts.Add( "Int32 GetCursorStringIndex(Rect, UnityEngine.GUIContent, Vector2)", 0 );
		}
		m_functionCallCounts["Int32 GetCursorStringIndex(Rect, UnityEngine.GUIContent, Vector2)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static System.Int32 Internal_GetCursorStringIndex( System.IntPtr target, UnityEngine.Rect position, System.String text, UnityEngine.Texture image, UnityEngine.Vector2 cursorPixelPosition ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public UnityEngine.Vector2 CalcSize( UnityEngine.GUIContent content ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector2 CalcSize(UnityEngine.GUIContent)" )){
			m_functionCallCounts.Add( "Vector2 CalcSize(UnityEngine.GUIContent)", 0 );
		}
		m_functionCallCounts["Vector2 CalcSize(UnityEngine.GUIContent)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public static void Internal_CalcSize( System.IntPtr target, System.String text, UnityEngine.Texture image, out UnityEngine.Vector2 ret ){
		ret = default(UnityEngine.Vector2);
	}

	public UnityEngine.Vector2 CalcScreenSize( UnityEngine.Vector2 contentSize ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Vector2 CalcScreenSize(Vector2)" )){
			m_functionCallCounts.Add( "Vector2 CalcScreenSize(Vector2)", 0 );
		}
		m_functionCallCounts["Vector2 CalcScreenSize(Vector2)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Single CalcHeight( UnityEngine.GUIContent content, System.Single width ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Single CalcHeight(UnityEngine.GUIContent, Single)" )){
			m_functionCallCounts.Add( "Single CalcHeight(UnityEngine.GUIContent, Single)", 0 );
		}
		m_functionCallCounts["Single CalcHeight(UnityEngine.GUIContent, Single)"]++;
			
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void CalcMinMaxWidth( UnityEngine.GUIContent content, out System.Single minWidth, out System.Single maxWidth ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void CalcMinMaxWidth(UnityEngine.GUIContent, Single ByRef, Single ByRef)" )){
			m_functionCallCounts.Add( "Void CalcMinMaxWidth(UnityEngine.GUIContent, Single ByRef, Single ByRef)", 0 );
		}
		m_functionCallCounts["Void CalcMinMaxWidth(UnityEngine.GUIContent, Single ByRef, Single ByRef)"]++;
			
		minWidth = default(System.Single);
		maxWidth = default(System.Single);
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

	public static implicit operator UnityEngine.GUIStyle( System.String str ){
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public GUIStyle( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			
	}

	public GUIStyle( UnityEngine.GUIStyle other ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor(GUIStyle)" )){
			m_functionCallCounts.Add( "Void .ctor(GUIStyle)", 0 );
		}
		m_functionCallCounts["Void .ctor(GUIStyle)"]++;
			
	}

	public System.Boolean stretchHeight {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_stretchHeight" )){
				m_functionCallCounts.Add( "get_stretchHeight", 0 );
			}
			m_functionCallCounts["get_stretchHeight"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_stretchHeight" )){
				m_functionCallCounts.Add( "set_stretchHeight", 0 );
			}
			m_functionCallCounts["set_stretchHeight"]++;
			
		}
	}

	public UnityEngine.RectOffset margin {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_margin" )){
				m_functionCallCounts.Add( "get_margin", 0 );
			}
			m_functionCallCounts["get_margin"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_margin" )){
				m_functionCallCounts.Add( "set_margin", 0 );
			}
			m_functionCallCounts["set_margin"]++;
			
		}
	}

	public UnityEngine.RectOffset border {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_border" )){
				m_functionCallCounts.Add( "get_border", 0 );
			}
			m_functionCallCounts["get_border"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_border" )){
				m_functionCallCounts.Add( "set_border", 0 );
			}
			m_functionCallCounts["set_border"]++;
			
		}
	}

	public System.Single fixedWidth {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_fixedWidth" )){
				m_functionCallCounts.Add( "get_fixedWidth", 0 );
			}
			m_functionCallCounts["get_fixedWidth"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_fixedWidth" )){
				m_functionCallCounts.Add( "set_fixedWidth", 0 );
			}
			m_functionCallCounts["set_fixedWidth"]++;
			
		}
	}

	public UnityEngine.TextClipping clipping {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_clipping" )){
				m_functionCallCounts.Add( "get_clipping", 0 );
			}
			m_functionCallCounts["get_clipping"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_clipping" )){
				m_functionCallCounts.Add( "set_clipping", 0 );
			}
			m_functionCallCounts["set_clipping"]++;
			
		}
	}

	public UnityEngine.RectOffset overflow {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_overflow" )){
				m_functionCallCounts.Add( "get_overflow", 0 );
			}
			m_functionCallCounts["get_overflow"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_overflow" )){
				m_functionCallCounts.Add( "set_overflow", 0 );
			}
			m_functionCallCounts["set_overflow"]++;
			
		}
	}

	public System.Boolean wordWrap {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_wordWrap" )){
				m_functionCallCounts.Add( "get_wordWrap", 0 );
			}
			m_functionCallCounts["get_wordWrap"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_wordWrap" )){
				m_functionCallCounts.Add( "set_wordWrap", 0 );
			}
			m_functionCallCounts["set_wordWrap"]++;
			
		}
	}

	public System.Single fixedHeight {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_fixedHeight" )){
				m_functionCallCounts.Add( "get_fixedHeight", 0 );
			}
			m_functionCallCounts["get_fixedHeight"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_fixedHeight" )){
				m_functionCallCounts.Add( "set_fixedHeight", 0 );
			}
			m_functionCallCounts["set_fixedHeight"]++;
			
		}
	}

	public UnityEngine.GUIStyleState onNormal {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_onNormal" )){
				m_functionCallCounts.Add( "get_onNormal", 0 );
			}
			m_functionCallCounts["get_onNormal"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_onNormal" )){
				m_functionCallCounts.Add( "set_onNormal", 0 );
			}
			m_functionCallCounts["set_onNormal"]++;
			
		}
	}

	public System.String name {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_name" )){
				m_functionCallCounts.Add( "get_name", 0 );
			}
			m_functionCallCounts["get_name"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_name" )){
				m_functionCallCounts.Add( "set_name", 0 );
			}
			m_functionCallCounts["set_name"]++;
			
		}
	}

	public UnityEngine.GUIStyleState normal {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_normal" )){
				m_functionCallCounts.Add( "get_normal", 0 );
			}
			m_functionCallCounts["get_normal"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_normal" )){
				m_functionCallCounts.Add( "set_normal", 0 );
			}
			m_functionCallCounts["set_normal"]++;
			
		}
	}

	public UnityEngine.RectOffset padding {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_padding" )){
				m_functionCallCounts.Add( "get_padding", 0 );
			}
			m_functionCallCounts["get_padding"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_padding" )){
				m_functionCallCounts.Add( "set_padding", 0 );
			}
			m_functionCallCounts["set_padding"]++;
			
		}
	}

	public UnityEngine.GUIStyleState hover {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_hover" )){
				m_functionCallCounts.Add( "get_hover", 0 );
			}
			m_functionCallCounts["get_hover"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_hover" )){
				m_functionCallCounts.Add( "set_hover", 0 );
			}
			m_functionCallCounts["set_hover"]++;
			
		}
	}

	public System.Single lineHeight {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_lineHeight" )){
				m_functionCallCounts.Add( "get_lineHeight", 0 );
			}
			m_functionCallCounts["get_lineHeight"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.GUIStyleState onFocused {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_onFocused" )){
				m_functionCallCounts.Add( "get_onFocused", 0 );
			}
			m_functionCallCounts["get_onFocused"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_onFocused" )){
				m_functionCallCounts.Add( "set_onFocused", 0 );
			}
			m_functionCallCounts["set_onFocused"]++;
			
		}
	}

	public UnityEngine.Vector2 clipOffset {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_clipOffset" )){
				m_functionCallCounts.Add( "get_clipOffset", 0 );
			}
			m_functionCallCounts["get_clipOffset"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_clipOffset" )){
				m_functionCallCounts.Add( "set_clipOffset", 0 );
			}
			m_functionCallCounts["set_clipOffset"]++;
			
		}
	}

	public UnityEngine.GUIStyleState focused {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_focused" )){
				m_functionCallCounts.Add( "get_focused", 0 );
			}
			m_functionCallCounts["get_focused"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_focused" )){
				m_functionCallCounts.Add( "set_focused", 0 );
			}
			m_functionCallCounts["set_focused"]++;
			
		}
	}

	public UnityEngine.TextAnchor alignment {
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

	public System.Boolean stretchWidth {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_stretchWidth" )){
				m_functionCallCounts.Add( "get_stretchWidth", 0 );
			}
			m_functionCallCounts["get_stretchWidth"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_stretchWidth" )){
				m_functionCallCounts.Add( "set_stretchWidth", 0 );
			}
			m_functionCallCounts["set_stretchWidth"]++;
			
		}
	}

	public System.Boolean isHeightDependantOnWidth {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isHeightDependantOnWidth" )){
				m_functionCallCounts.Add( "get_isHeightDependantOnWidth", 0 );
			}
			m_functionCallCounts["get_isHeightDependantOnWidth"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.GUIStyleState onHover {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_onHover" )){
				m_functionCallCounts.Add( "get_onHover", 0 );
			}
			m_functionCallCounts["get_onHover"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_onHover" )){
				m_functionCallCounts.Add( "set_onHover", 0 );
			}
			m_functionCallCounts["set_onHover"]++;
			
		}
	}

	public static UnityEngine.GUIStyle none {
		get {
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.ImagePosition imagePosition {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_imagePosition" )){
				m_functionCallCounts.Add( "get_imagePosition", 0 );
			}
			m_functionCallCounts["get_imagePosition"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_imagePosition" )){
				m_functionCallCounts.Add( "set_imagePosition", 0 );
			}
			m_functionCallCounts["set_imagePosition"]++;
			
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

	public UnityEngine.GUIStyleState onActive {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_onActive" )){
				m_functionCallCounts.Add( "get_onActive", 0 );
			}
			m_functionCallCounts["get_onActive"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_onActive" )){
				m_functionCallCounts.Add( "set_onActive", 0 );
			}
			m_functionCallCounts["set_onActive"]++;
			
		}
	}

	public UnityEngine.GUIStyleState active {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_active" )){
				m_functionCallCounts.Add( "get_active", 0 );
			}
			m_functionCallCounts["get_active"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_active" )){
				m_functionCallCounts.Add( "set_active", 0 );
			}
			m_functionCallCounts["set_active"]++;
			
		}
	}

	public UnityEngine.Vector2 contentOffset {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_contentOffset" )){
				m_functionCallCounts.Add( "get_contentOffset", 0 );
			}
			m_functionCallCounts["get_contentOffset"]++;
			
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_contentOffset" )){
				m_functionCallCounts.Add( "set_contentOffset", 0 );
			}
			m_functionCallCounts["set_contentOffset"]++;
			
		}
	}
}
}
