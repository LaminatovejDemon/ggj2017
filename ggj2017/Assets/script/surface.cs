using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour {

	public int _surfaceHeight = 50;
	public int _surfaceWidth = 100;
	public float _textureScale = 0.1f;
	public float _resolutionX = 1.0f;
	public float _borderExtentionX;

	Vector3[] _vertices = null;
	int[] _indices = null;
	Vector2[] _uvs = null;

	List<SurfaceSide> _animationListeners;

	public int GetSurfaceWidth()
	{
		return (int)(_surfaceWidth / _resolutionX);
	}

	public void RegisterAnimationListener(SurfaceSide source)
	{
		if (_animationListeners == null) {
			_animationListeners = new List<SurfaceSide> ();
		}

		if (_animationListeners.Contains (source)) {
			return;
		}

		_animationListeners.Add (source);
	}

	void AnimateListeners(){
		for (int i = 0; i < _animationListeners.Count; ++i) {
			_animationListeners[i].AnimateEdge();
		}
	}

	void InitiateSurface(){
		if (_vertices != null) {
			return;
		}


		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear();

		int w_ = GetSurfaceWidth();
		_vertices = new Vector3[_surfaceHeight * w_ * 4];
		_indices = new int[_surfaceHeight * w_ * 12];
		_uvs = new Vector2[_surfaceHeight * w_ * 4];
		float borderDistance_ = (_borderExtentionX + w_ * 0.5f) * _resolutionX;
	
		for (int i = 0; i < w_ * _surfaceHeight; i++) {

			if ( i%w_ == w_ -1 )
				_vertices [i*4+0] = new Vector3 (borderDistance_, 0, i / w_);				
			else
				_vertices [i*4+0] = new Vector3 (((i%w_)+1 - w_ * 0.5f) * _resolutionX, 0, i / w_);				

			if ( i%w_ == 0 )
				_vertices [i*4+1] = new Vector3 (-borderDistance_, 0, i / w_);
			else 
				_vertices [i*4+1] = new Vector3 ((i%w_  - w_ * 0.5f) * _resolutionX, 0, i / w_);

			if (i%w_ == 0 )
				_vertices [i*4+2] = new Vector3 (-borderDistance_, 0, i / w_ + 1);			
			else
				_vertices [i*4+2] = new Vector3 ((i%w_ - w_ * 0.5f) * _resolutionX, 0, i / w_ + 1);

			if ( i%w_ == w_-1 )
				_vertices [i*4+3] = new Vector3 (borderDistance_, 0, i / w_ + 1);
			else
				_vertices [i*4+3] = new Vector3 (((i%w_ - w_ * 0.5f)+1) * _resolutionX, 0, i / w_ + 1);

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

			_uvs [i * 4 + 0] = new Vector2(_vertices [i * 4 + 0].x * _textureScale, _vertices [i * 4 + 0].z * _textureScale);
			_uvs [i * 4 + 1] = new Vector2(_vertices [i * 4 + 1].x * _textureScale, _vertices [i * 4 + 1].z * _textureScale);
			_uvs [i * 4 + 2] = new Vector2(_vertices [i * 4 + 2].x * _textureScale, _vertices [i * 4 + 2].z * _textureScale);
			_uvs [i * 4 + 3] = new Vector2(_vertices [i * 4 + 3].x * _textureScale, _vertices [i * 4 + 3].z * _textureScale);
		}

		mesh.vertices = _vertices;
		mesh.triangles = _indices;
		mesh.uv = _uvs;
	}
		
	float CalculateGridZ(int x, int y){
		float value_ = 0;
		float locX_ = x + transform.position.x / _resolutionX;
		float locY_ = y + transform.position.z;

		int wave1_, from1_, to1_;
		wave1_ = (int)(locX_ * 2.0f + locY_ * 1.73f + Time.time * 20.0f) % 100; 
		from1_ = 5;
		to1_ = 25;
		if (wave1_ > from1_ && wave1_ < to1_) {
			value_ += 0.2f  
			 * (1.0f + Mathf.Sin(Mathf.PI * 2.0f * (-0.25f + (float)(wave1_-from1_)/(float)(to1_-from1_))));
		}

		int wave2_, from2_, to2_;
		wave2_ = (int)(locY_ * 2.0f + Time.time * 20.0f) % 100; 
		from2_ = 5;
		to2_ = 10;
		if (wave2_ > from2_ && wave2_ < to2_) {
			value_ += 0.4f  
				* (1.0f + Mathf.Sin(Mathf.PI * 2.0f * (-0.25f + (float)(wave2_-from2_)/(float)(to2_-from2_))));
		}

		value_ += Mathf.Sin (locX_ * 0.15f + locY_ * 0.25f + Time.time * 0.8f) * 0.3f;
		
		value_ += Mathf.Sin(Time.time * 1.4f + locX_ * 0.05f + locY_ * 0.1f) * 0.7f;
		return  value_;
	}

	public float GetGridZ(int x, int y, int vertexOffset = 3){
		if (x < 0 || y < 0) {
			return CalculateGridZ (x, y);
		}
			
		int index_ = ((y * GetSurfaceWidth()) + x) * 4 + vertexOffset;
		return _vertices[index_].y; 
	}

	public Vector3 GetSurfaceZ(Vector3 position){
		InitiateSurface ();

		Vector3 _alteredVector = position + Vector3.left * ((GetSurfaceWidth() * 0.5f) * _resolutionX); 

		_alteredVector.x -= transform.position.x;
		_alteredVector.z += transform.position.z;

		_alteredVector = GetGridPosition((int)(_alteredVector.x / _resolutionX), (int)(_alteredVector.z));

		_alteredVector.x += transform.position.x;
		_alteredVector.z -= transform.position.z;

		return _alteredVector;
	}
		
	Vector3 GetGridPosition(int x, int y, int offset_ = 0){
		int index_ = ((y * GetSurfaceWidth()) + x) * 4 + offset_;
		return _vertices[index_]; 
	}

	void AnimateSurface(){
		int index_ = 0;
		Vector3 bak_;

		for (int i = 0; i < _vertices.Length; ++i) {
			_vertices [i].y = -10.0f;
		}
		int w_ = GetSurfaceWidth ();

		for ( int i = 0; i < w_; ++i ){
			for ( int j = 0; j < _surfaceHeight; ++j ){
				index_ = (j * w_ + i) * 4;

				bak_ = _vertices [index_];
				bak_.y = GetGridZ(i, j-1);
				_vertices[index_] = bak_;

				bak_ = _vertices [index_+1];
				bak_.y = GetGridZ(i-1, j-1); 
				_vertices[index_+1] = bak_;

				bak_ = _vertices [index_+2];
				bak_.y = GetGridZ(i-1, j); 
				_vertices[index_+2] = bak_;

				bak_ = _vertices [index_+3];
				bak_.y = CalculateGridZ (i, j);
				_vertices[index_+3] = bak_;
			}
		}

		GetComponent<MeshFilter> ().mesh.vertices = _vertices;

		AnimateListeners ();
	}
	
	// Update is called once per frame
	void Update () {
		InitiateSurface ();	
		AnimateSurface ();
	}
}
