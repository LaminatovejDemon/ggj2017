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

namespace Water{
public class SurfaceFace : SurfaceEdge {

	void Start(){
		if (transform.localPosition.z >= _surface._surfaceHeight) {
			Vector3 bak_ = transform.localPosition;
			bak_.z = _surface._surfaceHeight;
			transform.localPosition = bak_;
		}

		_surface.RegisterAnimationListener (this);
	}

	protected override int GetLength(){
		return _surface.GetSurfaceWidth();
	}

	protected override void SetVertexChunk(int index){
		
		if ( index == _surfaceLength-1 )
				_vertices [index*4+0] = new Vector3 (_borderDistance-_surface._resolutionX+_surface._resolutionX, index / _surfaceLength -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [index*4+0] = new Vector3 (((index%_surfaceLength - (_surfaceLength) * 0.5f)+1f) * _surface._resolutionX, index / _surfaceLength -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);

			if ( index == 0 )
				_vertices [index*4+1] = new Vector3 (-_borderDistance-_surface._resolutionX+_surface._resolutionX, index / _surfaceLength -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [index*4+1] = new Vector3 (((index%_surfaceLength - (_surfaceLength) * 0.5f)-0f) * _surface._resolutionX, index / _surfaceLength -_depth * 0.5f - transform.localPosition.y - 0.1f, 0);

			if ( index == 0 )
				_vertices [index*4+2] = new Vector3 (-_borderDistance-_surface._resolutionX+_surface._resolutionX, index / _surfaceLength + 0.1f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [index*4+2] = new Vector3 (((index%_surfaceLength - (_surfaceLength) * 0.5f)-0f) * _surface._resolutionX, index / _surfaceLength + 0.1f - transform.localPosition.y - 0.1f, 0);

			if ( index == _surfaceLength-1 )
				_vertices [index*4+3] = new Vector3 (_borderDistance-_surface._resolutionX+_surface._resolutionX, index / _surfaceLength + 0.1f - transform.localPosition.y - 0.1f, 0);
			else
				_vertices [index*4+3] = new Vector3 (((index%_surfaceLength - (_surfaceLength) * 0.5f)+1f) * _surface._resolutionX, index / _surfaceLength + 0.1f - transform.localPosition.y - 0.1f, 0);
	}

	protected override void CalculateUV(int index){
		_uvs [index * 4 + 0] = new Vector2(_vertices [index * 4 + 0].x * _textureScale, _vertices [index * 4 + 0].y * _textureScale * 2);
		_uvs [index * 4 + 1] = new Vector2(_vertices [index * 4 + 1].x * _textureScale, _vertices [index * 4 + 1].y * _textureScale * 2);
		_uvs [index * 4 + 2] = new Vector2(_vertices [index * 4 + 2].x * _textureScale, _vertices [index * 4 + 2].y * _textureScale * 2);
		_uvs [index * 4 + 3] = new Vector2(_vertices [index * 4 + 3].x * _textureScale, _vertices [index * 4 + 3].y * _textureScale * 2);
	}

	public override void AnimateEdge () {
		InitialiseEdge ();
		int height_ = Mathf.Clamp ((int)transform.localPosition.z, 0, _surface._surfaceHeight - 1);
		int vertexIndex_ = height_ == _surface._surfaceHeight-1 ? 2 : 1;
		int lastIndex_ = height_ == _surface._surfaceHeight-1 ? 3 : 0;
			
		int index_ = 0; 
		int max_ = _surface.GetSurfaceWidth();
		Vector3 bak_;

		for (int i = 0; i < max_; i++) {
			bool last_ = i == max_-1;

			bak_ = _vertices [index_];
			 bak_.y = (_animatedBottom ? _surface.GetGridZ(last_ ? i: i+1, height_, last_ ? lastIndex_ : vertexIndex_) : 0) - _depth;
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = (_animatedBottom ? _surface.GetGridZ(i, height_, vertexIndex_) : 0 ) - _depth;
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
}
