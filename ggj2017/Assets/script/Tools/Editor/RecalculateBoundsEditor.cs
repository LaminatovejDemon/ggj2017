using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RecalculateBounds))]
public class RecalculateBoundaryEditor : BaseEditModeEditor<RecalculateBounds> {

	public override void OnInspector(){
		GUILayout.BeginHorizontal();
		if ( GUILayout.Button("Fetch Renderers")){
			instance.FetchRenderers();
		}

		if ( GUILayout.Button("Recalculate Bounds") ){
			instance.Recalculate();
		}
		GUILayout.FlexibleSpace();
		EditModeToggle();
		GUILayout.EndHorizontal();

		EditorGUILayout.LabelField(instance.GetRenderersCount() + " renderers found");
		
	}
}
