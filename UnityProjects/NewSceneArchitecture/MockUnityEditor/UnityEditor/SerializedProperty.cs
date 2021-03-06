using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEditor {
public class SerializedProperty {
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

	public UnityEditor.SerializedProperty Copy( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "UnityEditor.SerializedProperty Copy()" )){
			m_functionCallCounts.Add( "UnityEditor.SerializedProperty Copy()", 0 );
		}
		m_functionCallCounts["UnityEditor.SerializedProperty Copy()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void Reset( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void Reset()" )){
			m_functionCallCounts.Add( "Void Reset()", 0 );
		}
		m_functionCallCounts["Void Reset()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean Next( System.Boolean enterChildren ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean Next(Boolean)" )){
			m_functionCallCounts.Add( "Boolean Next(Boolean)", 0 );
		}
		m_functionCallCounts["Boolean Next(Boolean)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean NextVisible( System.Boolean enterChildren ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean NextVisible(Boolean)" )){
			m_functionCallCounts.Add( "Boolean NextVisible(Boolean)", 0 );
		}
		m_functionCallCounts["Boolean NextVisible(Boolean)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean DuplicateCommand( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean DuplicateCommand()" )){
			m_functionCallCounts.Add( "Boolean DuplicateCommand()", 0 );
		}
		m_functionCallCounts["Boolean DuplicateCommand()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean DeleteCommand( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean DeleteCommand()" )){
			m_functionCallCounts.Add( "Boolean DeleteCommand()", 0 );
		}
		m_functionCallCounts["Boolean DeleteCommand()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean ValidatePPtrValue( UnityEngine.Object obj ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Boolean ValidatePPtrValue(UnityEngine.Object)" )){
			m_functionCallCounts.Add( "Boolean ValidatePPtrValue(UnityEngine.Object)", 0 );
		}
		m_functionCallCounts["Boolean ValidatePPtrValue(UnityEngine.Object)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Int32 CountRemaining( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Int32 CountRemaining()" )){
			m_functionCallCounts.Add( "Int32 CountRemaining()", 0 );
		}
		m_functionCallCounts["Int32 CountRemaining()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Int32[] GetLayerMaskSelectedIndex( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "System.Int32[] GetLayerMaskSelectedIndex()" )){
			m_functionCallCounts.Add( "System.Int32[] GetLayerMaskSelectedIndex()", 0 );
		}
		m_functionCallCounts["System.Int32[] GetLayerMaskSelectedIndex()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.String[] GetLayerMaskNames( ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "System.String[] GetLayerMaskNames()" )){
			m_functionCallCounts.Add( "System.String[] GetLayerMaskNames()", 0 );
		}
		m_functionCallCounts["System.String[] GetLayerMaskNames()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public void ToggleLayerMaskAtIndex( System.Int32 index ){
		//Mock Data:
		if(m_functionCallCounts == null) {
			m_functionCallCounts = new Dictionary<string, int>();
		}
		if(!m_functionCallCounts.ContainsKey( "Void ToggleLayerMaskAtIndex(Int32)" )){
			m_functionCallCounts.Add( "Void ToggleLayerMaskAtIndex(Int32)", 0 );
		}
		m_functionCallCounts["Void ToggleLayerMaskAtIndex(Int32)"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public SerializedProperty( ){
		//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
		if(!m_functionCallCounts.ContainsKey( "Void .ctor()" )){
			m_functionCallCounts.Add( "Void .ctor()", 0 );
		}
		m_functionCallCounts["Void .ctor()"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
	}

	public System.Boolean hasChildren {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_hasChildren" )){
				m_functionCallCounts.Add( "get_hasChildren", 0 );
			}
			m_functionCallCounts["get_hasChildren"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Color colorValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_colorValue" )){
				m_functionCallCounts.Add( "get_colorValue", 0 );
			}
			m_functionCallCounts["get_colorValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_colorValue" )){
				m_functionCallCounts.Add( "set_colorValue", 0 );
			}
			m_functionCallCounts["set_colorValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Int32 depth {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_depth" )){
				m_functionCallCounts.Add( "get_depth", 0 );
			}
			m_functionCallCounts["get_depth"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String type {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_type" )){
				m_functionCallCounts.Add( "get_type", 0 );
			}
			m_functionCallCounts["get_type"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEditor.MetaFlags metaFlags {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_metaFlags" )){
				m_functionCallCounts.Add( "get_metaFlags", 0 );
			}
			m_functionCallCounts["get_metaFlags"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String tooltip {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_tooltip" )){
				m_functionCallCounts.Add( "get_tooltip", 0 );
			}
			m_functionCallCounts["get_tooltip"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEditor.SerializedPropertyType propertyType {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_propertyType" )){
				m_functionCallCounts.Add( "get_propertyType", 0 );
			}
			m_functionCallCounts["get_propertyType"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Single floatValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_floatValue" )){
				m_functionCallCounts.Add( "get_floatValue", 0 );
			}
			m_functionCallCounts["get_floatValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_floatValue" )){
				m_functionCallCounts.Add( "set_floatValue", 0 );
			}
			m_functionCallCounts["set_floatValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String mangledName {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_mangledName" )){
				m_functionCallCounts.Add( "get_mangledName", 0 );
			}
			m_functionCallCounts["get_mangledName"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
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
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Object pptrValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_pptrValue" )){
				m_functionCallCounts.Add( "get_pptrValue", 0 );
			}
			m_functionCallCounts["get_pptrValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_pptrValue" )){
				m_functionCallCounts.Add( "set_pptrValue", 0 );
			}
			m_functionCallCounts["set_pptrValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean boolValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_boolValue" )){
				m_functionCallCounts.Add( "get_boolValue", 0 );
			}
			m_functionCallCounts["get_boolValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_boolValue" )){
				m_functionCallCounts.Add( "set_boolValue", 0 );
			}
			m_functionCallCounts["set_boolValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Int32 enumValueIndex {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_enumValueIndex" )){
				m_functionCallCounts.Add( "get_enumValueIndex", 0 );
			}
			m_functionCallCounts["get_enumValueIndex"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_enumValueIndex" )){
				m_functionCallCounts.Add( "set_enumValueIndex", 0 );
			}
			m_functionCallCounts["set_enumValueIndex"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String enumStringValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_enumStringValue" )){
				m_functionCallCounts.Add( "get_enumStringValue", 0 );
			}
			m_functionCallCounts["get_enumStringValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_enumStringValue" )){
				m_functionCallCounts.Add( "set_enumStringValue", 0 );
			}
			m_functionCallCounts["set_enumStringValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Int32 intValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_intValue" )){
				m_functionCallCounts.Add( "get_intValue", 0 );
			}
			m_functionCallCounts["get_intValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_intValue" )){
				m_functionCallCounts.Add( "set_intValue", 0 );
			}
			m_functionCallCounts["set_intValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String pptrStringValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_pptrStringValue" )){
				m_functionCallCounts.Add( "get_pptrStringValue", 0 );
			}
			m_functionCallCounts["get_pptrStringValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Vector3 vector3Value {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_vector3Value" )){
				m_functionCallCounts.Add( "get_vector3Value", 0 );
			}
			m_functionCallCounts["get_vector3Value"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_vector3Value" )){
				m_functionCallCounts.Add( "set_vector3Value", 0 );
			}
			m_functionCallCounts["set_vector3Value"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String stringValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_stringValue" )){
				m_functionCallCounts.Add( "get_stringValue", 0 );
			}
			m_functionCallCounts["get_stringValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_stringValue" )){
				m_functionCallCounts.Add( "set_stringValue", 0 );
			}
			m_functionCallCounts["set_stringValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String propertyPath {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_propertyPath" )){
				m_functionCallCounts.Add( "get_propertyPath", 0 );
			}
			m_functionCallCounts["get_propertyPath"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean hasVisibleChildren {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_hasVisibleChildren" )){
				m_functionCallCounts.Add( "get_hasVisibleChildren", 0 );
			}
			m_functionCallCounts["get_hasVisibleChildren"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Rect rectValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_rectValue" )){
				m_functionCallCounts.Add( "get_rectValue", 0 );
			}
			m_functionCallCounts["get_rectValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_rectValue" )){
				m_functionCallCounts.Add( "set_rectValue", 0 );
			}
			m_functionCallCounts["set_rectValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean prefabOverride {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_prefabOverride" )){
				m_functionCallCounts.Add( "get_prefabOverride", 0 );
			}
			m_functionCallCounts["get_prefabOverride"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_prefabOverride" )){
				m_functionCallCounts.Add( "set_prefabOverride", 0 );
			}
			m_functionCallCounts["set_prefabOverride"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String layerMaskStringValue {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_layerMaskStringValue" )){
				m_functionCallCounts.Add( "get_layerMaskStringValue", 0 );
			}
			m_functionCallCounts["get_layerMaskStringValue"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEngine.Vector2 vector2Value {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_vector2Value" )){
				m_functionCallCounts.Add( "get_vector2Value", 0 );
			}
			m_functionCallCounts["get_vector2Value"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_vector2Value" )){
				m_functionCallCounts.Add( "set_vector2Value", 0 );
			}
			m_functionCallCounts["set_vector2Value"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean editable {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_editable" )){
				m_functionCallCounts.Add( "get_editable", 0 );
			}
			m_functionCallCounts["get_editable"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.String[] enumNames {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_enumNames" )){
				m_functionCallCounts.Add( "get_enumNames", 0 );
			}
			m_functionCallCounts["get_enumNames"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public UnityEditor.SerializedObject serializedObject {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_serializedObject" )){
				m_functionCallCounts.Add( "get_serializedObject", 0 );
			}
			m_functionCallCounts["get_serializedObject"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}

	public System.Boolean isExpanded {
		get {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "get_isExpanded" )){
				m_functionCallCounts.Add( "get_isExpanded", 0 );
			}
			m_functionCallCounts["get_isExpanded"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
		set {
			if(m_functionCallCounts == null) {
				m_functionCallCounts = new Dictionary<string, int>();
			}
			if(!m_functionCallCounts.ContainsKey( "set_isExpanded" )){
				m_functionCallCounts.Add( "set_isExpanded", 0 );
			}
			m_functionCallCounts["set_isExpanded"]++;
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}
	}
}
}
