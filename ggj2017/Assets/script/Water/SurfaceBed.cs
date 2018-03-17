using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Helper class to display the animated front or the top and bottom visuals attached to the existing surface.
/// 
/// </summary>

namespace Water{
public class SurfaceBed : SurfacePlane {

	float _quadHeight, _quadWidth;
	int _width = 0;
	bool _simplfiedVersion = false;

	public bool IsAnimated(){
		Initialise();
		return !_simplfiedVersion;
	}

	protected sealed override void Construct(){
		_simplfiedVersion = (!_animatedBottom && _relativePosition > 0);	
		_width = _surface.GetTileCountX();
		_quadHeight = _simplfiedVersion ? _surface.GetTileCountY() : 1;
		_quadWidth = _simplfiedVersion ? _surface.GetTileCountX() : 1;
		UpdateRelativePosition();	
	}

	protected sealed override int GetSize(){
		return _simplfiedVersion ? 1 : _surface.GetTileCountX() * _surface.GetTileCountY();
	}

	void UpdateRelativePosition(){
		Vector3 position_ = _surface.transform.position;
		position_.y -= _surface._depth * (_relativePosition);
		transform.position = position_;
	}

	public Vector3 GetGridPosition(int x, int y, int offset = 0){
		Initialise();
		
		if ( x < 0 || y < 0 || x >= _surface.GetTileCountX() || y >= _surface.GetTileCountY() ) {
			x = Mathf.Clamp(x, 0, _surface.GetTileCountX() -1);
			y = Mathf.Clamp(y, 0, _surface.GetTileCountY() - 1);
		}
		
		int index_ = ((y * _surface.GetTileCountX()) + x) * 4 + offset;
		if ( index_ < 0 || index_ > _vertices.Length ){
			Debug.LogWarning(this.ToString() + "." + MethodBase.GetCurrentMethod() + ": index out offset range.");
		}
		return _vertices[index_]; 
	}

	protected sealed override void SetVertexChunk(int index){
		
		float yl_ = index / _width * _surface._resolutionY;
		float yr_ = (index / _width + _quadHeight) * _surface._resolutionY;
		float xl_ = (index%_width  - _width * 0.5f) * _surface._resolutionX;
		float xr_ = (index%_width - _width * 0.5f + _quadWidth) * _surface._resolutionX;

		for ( int i = 0; i < 4; ++i){
			_normals[index*4+i] = Vector3.up;
		}
		

		if ( index%_width == _width -1 )
			_vertices [index*4+0] = new Vector3 (_borderDistance, 0, yl_);				
		else
			_vertices [index*4+0] = new Vector3 (xr_, 0, yl_);				

		if ( index%_width == 0 )
			_vertices [index*4+1] = new Vector3 (-_borderDistance, 0, yl_);
		else 
			_vertices [index*4+1] = new Vector3 (xl_, 0, yl_);

		if (index%_width == 0 )
			_vertices [index*4+2] = new Vector3 (-_borderDistance, 0, yr_);			
		else
			_vertices [index*4+2] = new Vector3 (xl_, 0, yr_);

		if ( index%_width == _width-1 )
			_vertices [index*4+3] = new Vector3 (_borderDistance, 0, yr_) ;
		else
			_vertices [index*4+3] = new Vector3 (xr_, 0, yr_);

		

	}

	protected sealed override void CalculateUV(int index){

		float xShift_ = 0;
		float yShift_ = 0;

		for ( int i = 0; i < 4; ++i ){
			_uvs [index * 4 + i] = new Vector2((_vertices [index * 4 + i].x + xShift_), (_vertices [index * 4 + i].z + yShift_));
		}

	}

	void AnimateWaves(int x, int y){
		int index_ = (y * (int)_width + x) * 4;
		Vector3 bak_;

		bak_ = _vertices [index_];
		bak_.y = _surface.GetGridZ(x, y-1);
		_vertices[index_] = bak_;

		bak_ = _vertices [index_+1];
		bak_.y = _surface.GetGridZ(x-1, y-1); 
		_vertices[index_+1] = bak_;

		bak_ = _vertices [index_+2];
		bak_.y = _surface.GetGridZ(x-1, y); 
		_vertices[index_+2] = bak_;

		bak_ = _vertices [index_+3];
		bak_.y = _surface.GetGridZ (x, y); // original calls Calculate
		_vertices[index_+3] = bak_;
	}

	public sealed override void AnimateEdge () {
		Initialise();

		if ( !_animatedBottom && _relativePosition != 0 ){
			return;
		}

		for ( int i = 0; i < _width; ++i ){
			for ( int j = 0; j < _surface.GetTileCountY(); ++j ){
				
				// CalculateUV(j * (int)_width + i);
				AnimateWaves(i,j);				
			}
		}
		GetComponent<MeshFilter> ().mesh.uv = _uvs;
		GetComponent<MeshFilter> ().mesh.vertices = _vertices;
		// GetComponent<MeshFilter> ().mesh.RecalculateNormals();
		// GetComponent<MeshFilter> ().mesh.RecalculateTangents();

	}
}
}
