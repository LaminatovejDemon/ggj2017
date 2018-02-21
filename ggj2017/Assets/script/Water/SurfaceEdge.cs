using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
public abstract class SurfaceEdge : MonoBehaviour {

	public Surface _surface = null;
	public float _textureScale = 1.0f;
	public float _depth = 1.0f;
	public bool _animatedBottom = false;

	protected Vector3[] _vertices = null;
	int[] _indices = null;
	protected Vector2[] _uvs = null;

	protected int _surfaceLength = 0;
	protected float _borderExtensionX = 0;
	protected float _borderDistance = 0;

	public abstract void AnimateEdge();
	protected abstract void SetVertexChunk(int index);
	protected abstract int GetLength();
	protected abstract void CalculateUV(int index);

	protected void InitialiseEdge () {
		if (_vertices != null) {
			return;
		}

		_surfaceLength = GetLength();
		_borderExtensionX = _surface._borderExtentionX;

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		_vertices = new Vector3[_surfaceLength * 4];
		_indices = new int[_surfaceLength * 6];
		_uvs = new Vector2[_surfaceLength * 4];
		_borderDistance = (_borderExtensionX + _surfaceLength * 0.5f) * _surface._resolutionX;

		for (int i = 0; i < _surfaceLength; i++) {
			SetVertexChunk(i);

			//topside
			_indices [i*6] = i*4;
			_indices [i*6 + 1] = i*4+1;
			_indices [i*6 + 2] = i*4+2;
			_indices [i*6 + 3] = i*4+2;
			_indices [i*6 + 4] = i*4+3;
			_indices [i*6 + 5] = i*4;
			
			CalculateUV(i);
		}

		mesh.vertices = _vertices;
		mesh.triangles = _indices;
		mesh.uv = _uvs;
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
}