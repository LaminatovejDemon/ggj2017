using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Water{
public class Surface : ListenerHandler<SurfacePlane> {

	public int _surfaceHeight = 20;
	public int _surfaceWidth = 20;
	public int _depth = 10;
	public bool _animatedBottom = false;

	public float _textureScale = 0.1f;
	public float _resolutionX = 1.0f;
	public  float _resolutionY = 1.0f;

	public float _waveHeight = 1.0f;
	public float _waveSpeed = 1.0f;

	SurfaceBed _surface = null;
	
	
	public float _borderExtentionX;

	float[] _heights = null;

	protected override void OnNewListener(SurfacePlane source)
	{
		StoreIfSurface(source as SurfaceBed);
	}

	void StoreIfSurface(SurfaceBed target){
		if ( target == null || _surface != null ){
			return;
		}
		if ( target.IsAnimated() ){
			_surface = target;
		}
	}
	
	public int GetTileCountX()
	{
		return (int)(_surfaceWidth / _resolutionX + 0.99f);
	}
	public int GetMetresY(){
		return (int)(_surfaceHeight);
	}

	public int GetMetresX(){
		return (int)(_surfaceWidth);
	}

	public int GetTileCountY(){
		return (int)(_surfaceHeight / _resolutionY + 0.99f);
	}

	delegate void AnimateEdge();
	
	void AnimateListeners(){
		MethodInfo animateEdge = typeof(SurfacePlane).GetMethod("AnimateEdge");
		OpenDelegate d = (OpenDelegate) System.Delegate.CreateDelegate(typeof(OpenDelegate), null, animateEdge);
		NotifyListeners(d);
	}

	void InitiateSurface(){
		if (_heights != null) {
			return;
		}

		_heights = new float[GetTileCountY() * GetTileCountX() * 4];
	}
		
	float CalculateGridZ(int x, int y){
		float value_ = 0;
		float locX_ = (x + transform.position.x) * _resolutionX;
		float locY_ = (y + transform.position.z) * _resolutionY;

		float time_ = Time.time * _waveSpeed;

		int wave1_, from1_, to1_;
		wave1_ = (int)(locX_ * 2.0f + locY_ * 1.73f + time_ * 20.0f) % 100; 
		from1_ = 5;
		to1_ = 25;
		if (wave1_ > from1_ && wave1_ < to1_) {
			value_ += 0.2f  
			 * (1.0f + Mathf.Sin(Mathf.PI * 2.0f * (-0.25f + (float)(wave1_-from1_)/(float)(to1_-from1_))));
		}

		int wave2_, from2_, to2_;
		wave2_ = (int)(locY_ * 2.0f + time_ * 20.0f) % 100; 
		from2_ = 2;
		to2_ = 19;
		if (wave2_ > from2_ && wave2_ < to2_) {
			value_ += 0.2f  
				* (1.0f + Mathf.Sin(Mathf.PI * 2.0f * (-0.25f + (float)(wave2_-from2_)/(float)(to2_-from2_))));
		}

		value_ += Mathf.Sin (locX_ * 0.15f + locY_ * 0.25f + time_ * 0.8f) * 0.3f;
		
		value_ += Mathf.Sin(time_ * 1.4f + locX_ * 0.05f + locY_ * 0.1f) * 0.7f;
		return  value_ * _waveHeight;
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
		
		if ( _surface == null ){
			Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().ToString() + ": No surface found, height map is not available." );
			return Vector3.zero;
		}

		return _surface.GetGridPosition(x, y, offset);
	}

	void AnimateSurface(){
 
		int index_ = 0;

		for (int i = 0; i < _heights.Length; ++i) {
			_heights [i] = -10.0f;
		}
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

		AnimateListeners ();
	}
	
	// Update is called once per frame
	void Update () {
		InitiateSurface ();	
		AnimateSurface ();
	}
}
}
