using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UVRotate))]
public class UVRotateEditor : Editor{
	List<bool> _texFold;
	UVRotate instance;
	Shader _shader = null;
	void Initialise(){
		if ( instance == null ){
			instance = (UVRotate)target;
		}
		if ( _texFold == null ){
			_texFold = new List<bool>();
		}

		if ( _texFold.Count != instance._propertyCount) {
			_texFold.Clear();
			while ( _texFold.Count < instance._propertyCount){
				_texFold.Add(false);
			}
		}
		
		if ( _shader == null ){
			_shader = instance.GetComponent<MeshRenderer>().sharedMaterial.shader;
		}
		if (instance._propertyCount == 0 ){
			instance._propertyCount = ShaderUtil.GetPropertyCount(_shader);
		}
		instance.Initialise();
		if ( instance._propertyNames.Count != instance._propertyCount ){
			instance._propertyNames.Clear();
			
			for ( int i = 0; i < instance._propertyCount; ++i ){
				if ( ShaderUtil.GetPropertyType(_shader, i) == ShaderUtil.ShaderPropertyType.TexEnv ){
					instance._propertyNames.Add(ShaderUtil.GetPropertyName(_shader, i));
			
				}
			}
			instance._propertyCount = instance._propertyNames.Count;
		}
	}

	GUIStyle _toggleButtonNormal = null, _toggleButtonToggled = null;

	public override void OnInspectorGUI(){
		Initialise();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if ( GUILayout.Button(new GUIContent("R", "Rescan shader properties"), GUILayout.ExpandWidth(false) )){
			instance._propertyCount = 0;
			instance._propertyNames.Clear();
		}

		if ( _toggleButtonNormal == null ){
			_toggleButtonNormal = "Button";
			_toggleButtonToggled = new GUIStyle(_toggleButtonNormal);
			_toggleButtonToggled.normal.background = _toggleButtonToggled.active.background;
		}

		if ( GUILayout.Button(new GUIContent("E", "Run in edit mode"), (instance.runInEditMode ? _toggleButtonToggled : _toggleButtonNormal) )){
			instance.runInEditMode = !instance.runInEditMode;
		}
		if ( instance.runInEditMode ){
			EditorUtility.SetDirty(instance);
		}

		EditorGUILayout.EndHorizontal();
		

		if (_texFold.Count == 0){
			while ( _texFold.Count < instance._propertyCount ){
				_texFold.Add(true);
			}
		}

		for ( int i = 0; i < instance._propertyCount; ++i ){
			
			EditorGUILayout.BeginHorizontal();
			
			_texFold[i] = EditorGUILayout.Foldout(_texFold[i], instance._propertyNames[i]);
			if (!_texFold[i]){
				EditorGUILayout.LabelField("\t Offset: "+ instance._uvSettings[i].OffsetX + ", " + instance._uvSettings[i].OffsetY + "\tSpeed: " + instance._uvSettings[i].SpeedX + ", " + instance._uvSettings[i].SpeedY );
			}
			EditorGUILayout.EndHorizontal();
			if (_texFold[i]){
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Offset", GUILayout.MaxWidth(40));
				instance._uvSettings[i].OffsetX = EditorGUILayout.FloatField(instance._uvSettings[i].OffsetX, GUILayout.ExpandWidth(false));
				instance._uvSettings[i].OffsetY = EditorGUILayout.FloatField(instance._uvSettings[i].OffsetY, GUILayout.ExpandWidth(false));
				EditorGUILayout.LabelField("Speed", GUILayout.MaxWidth(40));
				instance._uvSettings[i].SpeedX = EditorGUILayout.FloatField(instance._uvSettings[i].SpeedX, GUILayout.ExpandWidth(false));
				instance._uvSettings[i].SpeedY = EditorGUILayout.FloatField(instance._uvSettings[i].SpeedY, GUILayout.ExpandWidth(false));	
			EditorGUILayout.EndHorizontal();
			}
		}
	}
}
