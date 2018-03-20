using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Env{
public class Water : Surface {
	public bool _animatedBottom = false;

	public float _waveHeight = 1.0f;
	public float _waveSpeed = 1.0f;

	void Update () {
		InitializeSurface ();	
		UpdateSurface ();
		UpdateListeners();
	}

	protected override float CalculateGridZ(int x, int y){
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
}
}
