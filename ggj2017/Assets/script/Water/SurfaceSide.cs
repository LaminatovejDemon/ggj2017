using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to display the animated sides attached to the existing surface.
/// 
/// <param name="surface">reference to the existing surface</param>
/// <param name="_textureScale">scale of the applied texture with planar mapping</param>
/// <param name="_depth">height of the object</param>
/// </summary>

namespace Water{
public class SurfaceSide : SurfaceEdge {


	void Start(){
		if (transform.localPosition.z >= _surface._surfaceHeight) {
			Vector3 bak_ = transform.localPosition;
			bak_.z = _surface._surfaceHeight;
			transform.localPosition = bak_;
		}

		_surface.RegisterAnimationListener (this);
	}

	protected override int GetLength(){
		return _surface._surfaceHeight;
	}
		
	public override void AnimateEdge () {
		InitialiseEdge ();
		int referenceX_ = (int)((transform.localPosition.x + _surface._surfaceWidth * 0.5f) / _surface._resolutionX);
		int clamp_ = (int)((_surface._surfaceWidth) / _surface._resolutionX) -1;
		int width_ = Mathf.Clamp (referenceX_, 0, clamp_);
		int vertexIndex_ = width_ == clamp_ ? 0 : 1;
		int lastIndex_ = width_ == clamp_ ? 3 : 2;
			
		int index_ = 0; 
		int max_ = _surface._surfaceHeight;

		for (int i = 0; i < max_; i++) {
			Vector3 bak_ = _vertices [index_];
			bool last_ = i == max_-1;

			bak_.y = (_animatedBottom ? _surface.GetGridZ(width_, last_ ? i: i+1, last_ ? lastIndex_ : vertexIndex_) : 0) - _depth;
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = (_animatedBottom ? _surface.GetGridZ(width_, i, vertexIndex_) : 0 ) - _depth;
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = _surface.GetGridZ(width_, i, vertexIndex_);
			_vertices [index_++] = bak_;

			bak_ = _vertices [index_];
			bak_.y = _surface.GetGridZ(width_, last_ ? i: i+1,  last_ ? lastIndex_ : vertexIndex_);
			_vertices [index_++] = bak_;
		}

		GetComponent<MeshFilter> ().mesh.vertices = _vertices;
	}

	protected override void CalculateUV(int index){
		_uvs [index * 4 + 0] = new Vector2(_vertices [index * 4 + 0].z * _textureScale, _vertices [index * 4 + 0].y * _textureScale * 2);
		_uvs [index * 4 + 1] = new Vector2(_vertices [index * 4 + 1].z * _textureScale, _vertices [index * 4 + 1].y * _textureScale * 2);
		_uvs [index * 4 + 2] = new Vector2(_vertices [index * 4 + 2].z * _textureScale, _vertices [index * 4 + 2].y * _textureScale * 2);
		_uvs [index * 4 + 3] = new Vector2(_vertices [index * 4 + 3].z * _textureScale, _vertices [index * 4 + 3].y * _textureScale * 2);
	}

	protected override void SetVertexChunk(int index){
		
		float yBottom_ = index / _surfaceLength -_depth * 0.5f - transform.localPosition.y - 0.1f;
		float yTop_ = index / _surfaceLength + 0.1f - transform.localPosition.y - 0.1f;
		float xOffset_ = _surface._resolutionY * _surfaceLength * 0.5f;

		if ( index == _surfaceLength-1 )
				_vertices [index*4+0] = new Vector3 (0, yBottom_, xOffset_ );
			else
				_vertices [index*4+0] = new Vector3 (0, yBottom_, ((index%_surfaceLength - (_surfaceLength) * 0.5f)+1f) * _surface._resolutionY);

			if ( index == 0 )
				_vertices [index*4+1] = new Vector3 (0, yBottom_, -xOffset_);
			else
				_vertices [index*4+1] = new Vector3 (0, yBottom_, ((index%_surfaceLength - (_surfaceLength) * 0.5f)-0f) * _surface._resolutionY);

			if ( index == 0 )
				_vertices [index*4+2] = new Vector3 (0, yTop_, -xOffset_);
			else
				_vertices [index*4+2] = new Vector3 (0, yTop_, ((index%_surfaceLength - (_surfaceLength) * 0.5f)-0f) * _surface._resolutionY);

			if ( index == _surfaceLength-1 )
				_vertices [index*4+3] = new Vector3 (0, yTop_, xOffset_);
			else
				_vertices [index*4+3] = new Vector3 (0, yTop_, ((index%_surfaceLength - (_surfaceLength) * 0.5f)+1f) * _surface._resolutionY);
	}
}
}
