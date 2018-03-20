using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


namespace Env{
public class Surface : ListenerHandler<Listener> {

	public int _surfaceHeight = 20;
	public int _surfaceWidth = 20;
	public int _depth = 10;
	
	public float _resolutionX = 1.0f;
	public  float _resolutionY = 1.0f;

	SurfaceBed _mainSurface = null;
	public Vector2Int _borderExtention;
	protected float[] _heights = null;

	public int GetTileCountX()
	{
		return (int)(_surfaceWidth / _resolutionX + 0.99f);
	}
	public float GetMetresY(){
		return _surfaceHeight;
	}

	public float GetMetresX(){
		return _surfaceWidth;
	}

	public int GetTileCountY(){
		return (int)(_surfaceHeight / _resolutionY);
	}

	delegate void AnimateEdge();
	
	

	void StoreMainSurface(SurfaceBed target){
		if ( target == null || target._mainSurface == false)
		{
			return;
		}

		_mainSurface = target;
	}

	protected override void OnNewListener(Listener source)
	{
		StoreMainSurface(source as SurfaceBed);
	}

	protected void ShiftSurface(int offset){
		int w_ = GetTileCountX();
		int h_ = GetTileCountY();
		int indexFrom_, indexTo_;

		if ( offset > 0 ){
			for ( int x = w_-offset-1; x > 0; --x ){
				for ( int y = 0; y < h_; ++y ){
					indexFrom_ = (y * w_ + x) * 4;
					indexTo_ = (y * w_ + x+offset) * 4;
					_heights[indexTo_] = _heights[indexFrom_];  
					_heights[indexTo_+1] = _heights[indexFrom_+1];
					_heights[indexTo_+2] = _heights[indexFrom_+2];
					_heights[indexTo_+3] = _heights[indexFrom_+3];
				}
			}
		} else {
			for ( int x = -offset; x < w_; ++x ){
				for ( int y = 0; y < h_; ++y ){
					indexFrom_ = (y * w_ + x) * 4;
					indexTo_ = (y * w_ + x+offset) * 4;
					_heights[indexTo_] = _heights[indexFrom_];  
					_heights[indexTo_+1] = _heights[indexFrom_+1];
					_heights[indexTo_+2] = _heights[indexFrom_+2];
					_heights[indexTo_+3] = _heights[indexFrom_+3];
				}
			}
		}
	}

	protected void InitializeSurface(){
		if (_heights != null) {
			return;
		}

		_heights = new float[GetTileCountY() * GetTileCountX() * 4];
		UpdateSurface();
	}

	protected void UpdateSurface(int length){
		if ( length > 0 ){
			UpdateSurface(GetTileCountX()-length, 0);
		} else {
			UpdateSurface(0, GetTileCountX()+length-1);
		}
	}

	protected void UpdateSurface(int trimLeft = 0, int trimRight = 0){
 
		int index_ = 0;

		int w_ = GetTileCountX();
		int h_ = GetTileCountY();

		for ( int x = trimLeft; x < w_-trimRight; ++x ){
			for ( int y = 0; y < h_; ++y ){
				index_ = (y * w_ + x) * 4;

				_heights[index_] = GetGridZ(x, y-1);
				_heights[index_+1] = GetGridZ(x-1, y-1);
				_heights[index_+2] = GetGridZ(x-1, y);
				_heights[index_+3] = CalculateGridZ (x, y);
			}
		}
	}
		
	protected virtual float CalculateGridZ(int x, int y ){
		return 0;
	}

	public float GetGridZ(int x, int y, int vertexOffset = 3){
		if ( x < 0 || y < 0 ){
			return CalculateGridZ(x,y);
		}
		
		int index_ = ((y * GetTileCountX()) + x) * 4 + vertexOffset;
		if ( index_ >= _heights.Length ){
			Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().ToString() + " out of bounds ");
		}
		return _heights[index_]; 
	}

	public Vector3 GetSurfaceZ(Vector3 position){
		InitializeSurface ();

		Vector3 _alteredVector = position + Vector3.right * ((GetTileCountX() * 0.5f) * _resolutionX); 

		_alteredVector.x -= transform.position.x;
		_alteredVector.z -= transform.position.z;
		_alteredVector = GetGridPosition((int)(_alteredVector.x / _resolutionX), (int)(_alteredVector.z * _resolutionY));

		_alteredVector.x += transform.position.x;
		_alteredVector.z += transform.position.z;
		_alteredVector.y += transform.position.y;

		return _alteredVector;
	}
		
	Vector3 GetGridPosition(int x, int y, int offset = 0){
		
		if ( _mainSurface == null ){
			Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().ToString() + ": No surface found, height map is not available." );
			return Vector3.zero;
		}

		return _mainSurface.GetGridPosition(x, y, offset);
	}
}
}
