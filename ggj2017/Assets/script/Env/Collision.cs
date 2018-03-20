using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Env{
public class Collision : MonoBehaviour, Listener {
	public Bedrock _surface;
	Vector2[] points_;

	float previousOffsetX_ = 0;

	void Initialise(){
		if ( points_ != null ){
			return;
		}

		_surface.RegisterListener(this);
		points_ = new Vector2[_surface._surfaceWidth];
		
		previousOffsetX_ = transform.position.x;
		Recalculate(_surface._surfaceWidth);
	}

	void Update(){
		Initialise();
	}

	void Recalculate(){
		int delta_ = (int)(transform.position.x - previousOffsetX_);
		Recalculate(delta_);
	}

	void Recalculate(int delta_){
		int length_ = _surface._surfaceWidth;

		Shift(-delta_);
		if ( delta_ > 0 ){	
			Calculate (length_-delta_, length_);
		} else {
			Calculate(0, -delta_);
		}

		GetComponent<EdgeCollider2D>().points = points_;
		previousOffsetX_ = transform.position.x;
	}

	void Shift(int offset){
		int length_ = _surface._surfaceWidth;
		if ( offset > 0 ){
			for ( int i = length_-offset-1; i > 0; --i ){
				points_[i+offset].y = points_[i].y;
			}
		} else {
			for ( int i = -offset; i < length_; ++i ){
				points_[i+offset].y = points_[i].y;
			}
		}
	}

	void Calculate(int from, int to){
		for ( int i = from; i < to; ++i){
			points_[i] = Calculate(i);
		}
	}

	Vector2 Calculate(float posX){
		return new Vector2(posX-_surface._surfaceWidth*0.5f, _surface.GetGridZ((int)(posX), 5));
	}

	public void OnUpdate(){
		Recalculate();
	}

}
}
