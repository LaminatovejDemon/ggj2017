using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class surface : MonoBehaviour {

	public int _surfaceHeight = 50;
	public int _surfaceWidth = 100;
	public float _uvScale = 0.1f;

	Vector3[] _vertices = null;
	int[] _indices = null;
	Vector2[] _uvs = null;


	void InitiateSurface(){
		if (_vertices != null) {
			return;
		}

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear();

		_vertices = new Vector3[_surfaceHeight * _surfaceWidth * 4];
		_indices = new int[_surfaceHeight * _surfaceWidth * 12];
		_uvs = new Vector2[_surfaceHeight * _surfaceWidth * 4];
	
		for (int i = 0; i < _surfaceWidth * _surfaceHeight; i++) {
			_vertices [i*4+0] = new Vector3 ((i%_surfaceWidth)+1, 0, i / _surfaceWidth);
			_vertices [i*4+1] = new Vector3 ((i%_surfaceWidth), 0, i / _surfaceWidth);
			_vertices [i*4+2] = new Vector3 ((i%_surfaceWidth), 0, i / _surfaceWidth + 1);
			_vertices [i*4+3] = new Vector3 ((i%_surfaceWidth)+1, 0, i / _surfaceWidth + 1);

			//topside
			_indices [i*12] = i*4;
			_indices [i*12 + 1] = i*4+1;
			_indices [i*12 + 2] = i*4+2;
			_indices [i*12 + 3] = i*4+2;
			_indices [i*12 + 4] = i*4+3;
			_indices [i*12 + 5] = i*4;
			//bottomside
			_indices [i*12 + 6] = i*4;
			_indices [i*12 + 7] = i*4+2;
			_indices [i*12 + 8] = i*4+1;
			_indices [i*12 + 9] = i*4+3;
			_indices [i*12 + 10] = i*4+2;
			_indices [i*12 + 11] = i*4;

			_uvs [i * 4 + 0] = new Vector2(_vertices [i * 4 + 0].x * _uvScale, _vertices [i * 4 + 0].z * _uvScale);
			_uvs [i * 4 + 1] = new Vector2(_vertices [i * 4 + 1].x * _uvScale, _vertices [i * 4 + 1].z * _uvScale);
			_uvs [i * 4 + 2] = new Vector2(_vertices [i * 4 + 2].x * _uvScale, _vertices [i * 4 + 2].z * _uvScale);
			_uvs [i * 4 + 3] = new Vector2(_vertices [i * 4 + 3].x * _uvScale, _vertices [i * 4 + 3].z * _uvScale);
		}

		mesh.vertices = _vertices;
		mesh.triangles = _indices;
		mesh.uv = _uvs;
	}

	public float GetActualPosition(int seed){
		InitiateSurface ();
		return _vertices [seed].y;
	}

	public float GetPositionInTime(int seed){
		return Mathf.Sin (Time.time + (_vertices [seed].x * 0.1f) + (_vertices [seed].z * 0.043f));
	}

	void SurfaceUpdate(){
		
		for (int i = 0; i < _vertices.Length; ++i) {
			Vector3 backup_ = _vertices [i];
			backup_.y = GetPositionInTime (i);
			_vertices [i] = backup_;
		}

		GetComponent<MeshFilter> ().mesh.vertices = _vertices;
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		InitiateSurface ();	
		SurfaceUpdate ();
	}
}
