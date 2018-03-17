using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyPrefab : MonoBehaviour {

	[System.Serializable]
	public class PathPrefab{
		public string node;
		public GameObject prefab;
	}

	public List<PathPrefab> _prefabs; 

	public void Instantiate(){
		if (_prefabs == null ){
			return;
		}
		
		for ( int i = 0; i < _prefabs.Count; ++i ){
			if ( _prefabs[i].prefab == null ){
				continue;
			}
			string name_ = _prefabs[i].node;
			Transform node_ = FindTransform(name_);
			if ( node_ == null ){
				continue;
			}
			if (FindTransform(_prefabs[i].prefab.name, node_) != null){
				continue;
			}
			GameObject instance_ = GameObject.Instantiate(_prefabs[i].prefab, node_);
			instance_.name = _prefabs[i].prefab.name;
			instance_.transform.localPosition = _prefabs[i].prefab.transform.localPosition;
			instance_.transform.localRotation = _prefabs[i].prefab.transform.localRotation;
			instance_.transform.localScale = _prefabs[i].prefab.transform.localScale;
		}
	}

	public Transform FindTransform(string name, Transform root_ = null){
		if ( root_ == null ){
			root_ = transform;
		}
	
		for (int i = 0; i < root_.childCount; ++i ){
			Transform next_ = root_.GetChild(i);
			if ( next_.name == name ){
				return next_;
			}

			Transform ret_ = FindTransform(name, next_);
			if ( ret_ != null)
			return ret_;
		}
		return null;
	}
}
