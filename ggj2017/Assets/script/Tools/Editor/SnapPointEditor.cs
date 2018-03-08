using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Snap))]
public class SnapPointEditor : BaseEditor<Snap> {

	protected override void Initialise(){
		if ( instance._snapPoints == null ){
			instance._snapPoints = new List<SnapPoint>();
		}
	}

	public override void OnInspector(){
		for ( int i = 0; i < instance._snapPoints.Count; ++i ){
			EditorGUILayout.BeginHorizontal();
			instance._snapPoints[i].snapType = (SnapManager.SnapType)EditorGUILayout.EnumPopup("Type", instance._snapPoints[i].snapType);
			if ( GUILayout.Button("-", GUILayout.MaxWidth(20)) ){
				instance._snapPoints.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
			
			instance._snapPoints[i].offset = EditorGUILayout.Vector2Field("Relative Offset", instance._snapPoints[i].offset);
			instance._snapPoints[i].lookAt = EditorGUILayout.Vector2Field("Relative LookAt", instance._snapPoints[i].lookAt);
			
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Player is");
			EditorGUILayout.LabelField("Facing", GUILayout.MaxWidth(40));
			instance._snapPoints[i].facing = EditorGUILayout.Toggle(instance._snapPoints[i].facing, GUILayout.MaxWidth(20));
			instance._snapPoints[i].backwards = EditorGUILayout.ToggleLeft("Backwards", instance._snapPoints[i].backwards);
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Accessible from");
			EditorGUILayout.LabelField("Left", GUILayout.MaxWidth(40));
			instance._snapPoints[i].fromLeft = EditorGUILayout.Toggle(instance._snapPoints[i].fromLeft, GUILayout.MaxWidth(20));
			instance._snapPoints[i].fromRight = EditorGUILayout.ToggleLeft("Right", instance._snapPoints[i].fromRight);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
		}

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if ( GUILayout.Button("+") ){
			instance._snapPoints.Add(new SnapPoint());
		}
		EditorGUILayout.EndHorizontal();
	}
}
