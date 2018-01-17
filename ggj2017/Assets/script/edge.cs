using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edge : MonoBehaviour {
	public surface _surface;
	public int _surfaceWidth = 150;
	public float _uvScale = 1.0f;
	public float _depth = 1.0f;

	Vector3[] _vertices = null;
	Vector3[] _verticesAnimation = null;
	int[] _indices = null;
	Vector2[] _uvs = null;

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		InitialiseEdge ();
		UpdateEdge ();
	}

	void InitialiseEdge () {
		if (_vertices != null) {
			return;
		}

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		_vertices = new Vector3[_surfaceWidth * 4];
		_verticesAnimation = new Vector3[_surfaceWidth * 4];
		_indices = new int[_surfaceWidth * 6];
		_uvs = new Vector2[_surfaceWidth * 4];

//		int y = -1;
//		sint i = 0;
		for (int i = 0; i < _surfaceWidth; i++) {
			_vertices [i*4+0] = new Vector3 ((i%_surfaceWidth)+1, i / _surfaceWidth -_depth * 0.5f - transform.position.y - 0.1f, 0);
			_vertices [i*4+1] = new Vector3 ((i%_surfaceWidth), i / _surfaceWidth -_depth * 0.5f - transform.position.y - 0.1f, 0);
			_vertices [i*4+2] = new Vector3 ((i%_surfaceWidth), i / _surfaceWidth + 0.1f - transform.position.y - 0.1f, 0);
			_vertices [i*4+3] = new Vector3 ((i%_surfaceWidth)+1, i / _surfaceWidth + 0.1f - transform.position.y - 0.1f, 0);
			_verticesAnimation [i * 4] = _vertices [i * 4];
			_verticesAnimation [i * 4+1] = _vertices [i * 4+1];
			_verticesAnimation [i * 4+2] = _vertices [i * 4+2];
			_verticesAnimation [i * 4+3] = _vertices [i * 4+3];

			//topside
			_indices [i*6] = i*4;
			_indices [i*6 + 1] = i*4+1;
			_indices [i*6 + 2] = i*4+2;
			_indices [i*6 + 3] = i*4+2;
			_indices [i*6 + 4] = i*4+3;
			_indices [i*6 + 5] = i*4;

			_uvs [i * 4 + 0] = new Vector2(_vertices [i * 4 + 0].x * _uvScale, _vertices [i * 4 + 0].y * _uvScale);
			_uvs [i * 4 + 1] = new Vector2(_vertices [i * 4 + 1].x * _uvScale, _vertices [i * 4 + 1].y * _uvScale);
			_uvs [i * 4 + 2] = new Vector2(_vertices [i * 4 + 2].x * _uvScale, _vertices [i * 4 + 2].y * _uvScale);
			_uvs [i * 4 + 3] = new Vector2(_vertices [i * 4 + 3].x * _uvScale, _vertices [i * 4 + 3].y * _uvScale);

		}

		mesh.vertices = _vertices;
		mesh.triangles = _indices;
		mesh.uv = _uvs;
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;

	}


	void UpdateEdge () {
		for (int i = 0; i < _vertices.Length; ++i) {
			_verticesAnimation[i].y = _surface.GetActualPosition(i, transform.position.z) + _vertices [i].y;
		}

		GetComponent<MeshFilter> ().mesh.vertices = _verticesAnimation;
	}
}
