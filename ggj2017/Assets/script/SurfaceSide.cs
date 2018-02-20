using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to display the animated front or the back visual attached to the existing surface.
/// 
/// <param name="surface">reference to the existing surface</param>
/// <param name="_textureScale">scale of the applied texture with planar mapping</param>
/// <param name="_depth">height of the object</param>
/// </summary>
public class SurfaceSide : MonoBehaviour {
	public Surface _surface;
	public float _textureScale = 1.0f;
	public float _depth = 1.0f;

	Vector3[] _vertices = null;
	int[] _indices = null;
	Vector2[] _uvs = null;

	void Start(){
		if (transform.localPosition.z >= _surface._surfaceHeight) {
			Vector3 bak_ = transform.localPosition;
			bak_.z = _surface._surfaceHeight;
			transform.localPosition = bak_;
		}

		_surface.RegisterAnimationListener (this);
	}

	void InitialiseEdge () {
		if (_vertices != null) {
			return;
		}

		int surfaceWidth_ = _surface.GetSurfaceWidth();
		float borderExtensionX_ = _surface._borderExtentionX;

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		_vertices = new Vector3[surfaceWidth_ * 4];
		_indices = new int[surfaceWidth_ * 6];
		_uvs = new Vector2[surfaceWidth_ * 4];
		float borderDistance_ = (borderExtensionX_ + surfaceWidth_ * 0.5f) * _surface._resolutionX;

		for (int i = 0; i < surfaceWidth_; i++) {
			if ( i == surfaceWidth_-1 )
				_vertices [i*4+0] = new Vector3 (borderDistance_-_surface._resolutionX+_surface._resolutionX, i / surfaceWidth_ -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [i*4+0] = new Vector3 (((i%surfaceWidth_ - (surfaceWidth_) * 0.5f)+1f) * _surface._resolutionX, i / surfaceWidth_ -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);

			if ( i == 0 )
				_vertices [i*4+1] = new Vector3 (-borderDistance_-_surface._resolutionX+_surface._resolutionX, i / surfaceWidth_ -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [i*4+1] = new Vector3 (((i%surfaceWidth_ - (surfaceWidth_) * 0.5f)-0f) * _surface._resolutionX, i / surfaceWidth_ -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);

			if ( i == 0 )
				_vertices [i*4+2] = new Vector3 (-borderDistance_-_surface._resolutionX+_surface._resolutionX, i / surfaceWidth_ + 0.1f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [i*4+2] = new Vector3 (((i%surfaceWidth_ - (surfaceWidth_) * 0.5f)-0f) * _surface._resolutionX, i / surfaceWidth_ + 0.1f - transform.localPosition.y - 0.1f, 0);

			if ( i == surfaceWidth_-1 )
				_vertices [i*4+3] = new Vector3 (borderDistance_-_surface._resolutionX+_surface._resolutionX, i / surfaceWidth_ + 0.1f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [i*4+3] = new Vector3 (((i%surfaceWidth_ - (surfaceWidth_) * 0.5f)+1f) * _surface._resolutionX, i / surfaceWidth_ + 0.1f - transform.localPosition.y - 0.1f, 0);

			//topside
			_indices [i*6] = i*4;
			_indices [i*6 + 1] = i*4+1;
			_indices [i*6 + 2] = i*4+2;
			_indices [i*6 + 3] = i*4+2;
			_indices [i*6 + 4] = i*4+3;
			_indices [i*6 + 5] = i*4;

			_uvs [i * 4 + 0] = new Vector2(_vertices [i * 4 + 0].x * _textureScale, _vertices [i * 4 + 0].y * _textureScale);
			_uvs [i * 4 + 1] = new Vector2(_vertices [i * 4 + 1].x * _textureScale, _vertices [i * 4 + 1].y * _textureScale);
			_uvs [i * 4 + 2] = new Vector2(_vertices [i * 4 + 2].x * _textureScale, _vertices [i * 4 + 2].y * _textureScale);
			_uvs [i * 4 + 3] = new Vector2(_vertices [i * 4 + 3].x * _textureScale, _vertices [i * 4 + 3].y * _textureScale);

		}

		mesh.vertices = _vertices;
		mesh.triangles = _indices;
		mesh.uv = _uvs;
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;

	}
		
	public void AnimateEdge () {
		InitialiseEdge ();
		int height_ = Mathf.Clamp ((int)transform.localPosition.z, 0, _surface._surfaceHeight - 1);
		int vertexIndex_ = height_ == _surface._surfaceHeight-1 ? 2 : 1;
		int lastIndex_ = height_ == _surface._surfaceHeight-1 ? 3 : 0;
			
		int index_ = 0; 
		int max_ = _surface.GetSurfaceWidth();

		for (int i = 0; i < max_; i++) {
			Vector3 bak_ = _vertices [index_];
			bool last_ = i == max_-1;

			bak_.y = _surface.GetGridZ(last_ ? i: i+1, height_, last_ ? lastIndex_ : vertexIndex_) - _depth;
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = _surface.GetGridZ(i, height_, vertexIndex_) - _depth;
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = _surface.GetGridZ(i, height_, vertexIndex_);
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = _surface.GetGridZ(last_ ? i: i+1, height_, last_ ? lastIndex_ : vertexIndex_);
			_vertices [index_++] = bak_;
		}

		GetComponent<MeshFilter> ().mesh.vertices = _vertices;
	}
}
