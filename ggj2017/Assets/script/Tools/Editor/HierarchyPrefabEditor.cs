using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HierarchyPrefab))]
public class HierarchyPrefabEditor : BaseEditor<HierarchyPrefab> {

	UnityEngine.Transform[] hierarchy_ = null;
	List<string> hierarchyNames_ = null;
	string[] hierarchyNamesArray_ = null;

	protected override void Initialise(){
		if ( instance._prefabs == null ){
			instance._prefabs = new List<HierarchyPrefab.PathPrefab>();
		}
		if ( hierarchy_ == null ){
			hierarchy_ = instance.transform.GetComponentsInChildren<Transform>();
			if ( hierarchyNames_ == null ){
				hierarchyNames_ = new List<string>();
			}
			hierarchyNames_.Clear();
			for ( int i = 0; i < hierarchy_.Length; ++i ){
				hierarchyNames_.Add(hierarchy_[i].name);
			}
			hierarchyNames_.Add("- None -");
			hierarchyNamesArray_ = hierarchyNames_.ToArray();
		}
	}

	int FindIndex(string name){
		return hierarchyNames_.IndexOf(name);
	}

	public override void OnInspector(){
		for ( int i = 0 ;i< instance._prefabs.Count; ++i ){
			EditorGUILayout.BeginHorizontal();
			int index_ = FindIndex(instance._prefabs[i].node);
			if (index_ == -1){
				index_ = hierarchyNames_.Count-1;
			}

			instance._prefabs[i].prefab = EditorGUILayout.ObjectField(instance._prefabs[i].prefab, typeof(GameObject), false) as GameObject;
			instance._prefabs[i].node = hierarchyNames_[EditorGUILayout.Popup(index_, hierarchyNamesArray_)];
			if ( GUILayout.Button("X") ){
				instance._prefabs.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.BeginHorizontal();
		if ( GUILayout.Button("Instantiate")){
			instance.Instantiate();
		}
		GUILayout.FlexibleSpace();
		if ( GUILayout.Button("+") ){
			instance._prefabs.Add(new HierarchyPrefab.PathPrefab());
		}
		EditorGUILayout.EndHorizontal();
	}
}
