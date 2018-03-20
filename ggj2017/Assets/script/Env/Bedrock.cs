using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Env{
public class Bedrock : Surface, Listener {
	

	public bool DEBUGRealtime = false;
	public AnimationCurve _shape;
	public AnimationCurve _lateralShape;
	public AnimationCurve _roughness;
	public float _shapeLength = 200;

	float previousOffsetX_ = 0;

	void Start(){
		GetComponent<PositionLink>().RegisterListener(this);
	}
	
	public void Update(){
		if ( DEBUGRealtime ){
			OnUpdate();
		}
	}

	public void OnUpdate(){
		int delta_ = (int)(transform.position.x - previousOffsetX_);
		
		InitializeSurface ();	

		ShiftSurface(-delta_);
		UpdateSurface(delta_);
		UpdateListeners ();
		previousOffsetX_ = transform.position.x;
	}

	protected override float CalculateGridZ(int x, int y){
		float refX_ = (transform.position.x + x * _resolutionX);
		float X_ = refX_ / (float)_shapeLength;

		float refY_ = y * _resolutionY;
		Random.InitState(((int)(refY_ * 2314.0f)));
		float random1_ = Random.value * 0.1f;
		Random.InitState(((int)(refY_ * refX_ * 1233.2f)));
		float random2_ = Random.value * _roughness.Evaluate(X_);

		float base_ = -_depth + _shape.Evaluate(X_) * _depth;
		float lateral_ = _lateralShape.Evaluate(refY_ / (float)_surfaceHeight) * 20;
		return Mathf.Min(2,base_ + lateral_ + random1_ + random2_);	
	}
}
}