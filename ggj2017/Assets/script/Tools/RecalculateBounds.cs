using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecalculateBounds : MonoBehaviour {

	SkinnedMeshRenderer [] renderers_ = null;
	public void FetchRenderers(){
		if (renderers_ != null && renderers_.Length != 0){
			return;
		}
		renderers_ = GetComponentsInChildren<SkinnedMeshRenderer>();
	}

	public int GetRenderersCount(){
		return renderers_ == null ? 0 : renderers_.Length;
	}

	public void Recalculate(){
		FetchRenderers();
		for ( int i = 0; i < renderers_.Length; ++i ){
			renderers_[i].sharedMesh.RecalculateBounds();
			renderers_[i].sharedMesh.RecalculateNormals();
			renderers_[i].sharedMesh.RecalculateTangents();
			renderers_[i].updateWhenOffscreen = true;
		}
		Debug.Log(this.name + ": " + renderers_.Length + " skinned mesh renderes recalculated");
	}

	public void OnDrawGizmos(){
		if ( renderers_ == null ){
			return;
		}
		Gizmos.color = Color.red;
		for ( int i = 0; i < renderers_.Length; ++i ){
			Gizmos.DrawWireCube(renderers_[i].sharedMesh.bounds.center, renderers_[i].sharedMesh.bounds.size);
		}
	}

	void Update(){
		// Recalculate();
	}
}
