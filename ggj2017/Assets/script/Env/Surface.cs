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

	protected void InitiateSurface(){
		if (_heights != null) {
			return;
		}

		_heights = new float[GetTileCountY() * GetTileCountX() * 4];
		UpdateSurface();
	}

	protected void UpdateSurface(){
 
		int index_ = 0;

		int w_ = GetTileCountX ();

		for ( int i = 0; i < w_; ++i ){
			for ( int j = 0; j < GetTileCountY(); ++j ){
				index_ = (j * w_ + i) * 4;

				_heights[index_] = GetGridZ(i, j-1);
				_heights[index_+1] = GetGridZ(i-1, j-1);
				_heights[index_+2] = GetGridZ(i-1, j);
				_heights[index_+3] = CalculateGridZ (i, j);
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
		InitiateSurface ();

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
