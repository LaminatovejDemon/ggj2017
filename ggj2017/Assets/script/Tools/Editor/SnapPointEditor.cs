using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapTrigger))]
public class SnapPointEditor : BaseEditor<SnapTrigger> {

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
			
			instance._snapPoints[i].snapObject = EditorGUILayout.ObjectField("Snap Object", instance._snapPoints[i].snapObject, typeof(GameObject)) as GameObject;
			instance._snapPoints[i].offset = EditorGUILayout.Vector2Field("Relative Offset", instance._snapPoints[i].offset);
			EditorGUILayout.BeginHorizontal();
			instance._snapPoints[i].snapX = EditorGUILayout.Toggle("Snap Axis", instance._snapPoints[i].snapX);
			EditorGUILayout.LabelField("X", GUILayout.MaxWidth(40));
			instance._snapPoints[i].snapY = EditorGUILayout.Toggle(instance._snapPoints[i].snapY, GUILayout.MaxWidth(12));
			EditorGUILayout.LabelField("Y");
			EditorGUILayout.EndHorizontal();		

			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			instance._snapPoints[i].fromLeft = EditorGUILayout.Toggle("Accessible from", instance._snapPoints[i].fromLeft);
			EditorGUILayout.LabelField("Left", GUILayout.MaxWidth(40));
			instance._snapPoints[i].fromRight = EditorGUILayout.Toggle(instance._snapPoints[i].fromRight, GUILayout.MaxWidth(12));
			EditorGUILayout.LabelField("Right");
			EditorGUILayout.EndHorizontal();	
			
			EditorGUILayout.BeginHorizontal();
			instance._snapPoints[i].facing = EditorGUILayout.Toggle("Player is ", instance._snapPoints[i].facing);
			EditorGUILayout.LabelField("Facing", GUILayout.MaxWidth(40));
			instance._snapPoints[i].backwards = EditorGUILayout.Toggle(instance._snapPoints[i].backwards, GUILayout.MaxWidth(12));
			EditorGUILayout.LabelField("Backwards");
			EditorGUILayout.EndHorizontal();
			instance._snapPoints[i].facingOffsetX = EditorGUILayout.FloatField("Facing Horizontal Offset", instance._snapPoints[i].facingOffsetX);
		
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
