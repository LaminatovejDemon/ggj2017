using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Env{
public class Collision : MonoBehaviour, Listener {
	public Bedrock _surface;
	List<Vector2> points_;

	float previousOffsetX_ = 0;

	void Initialise(){
		if ( points_ != null ){
			return;
		}

		_surface.RegisterListener(this);
		points_ = new List<Vector2>();
		
		while (points_.Count < _surface._surfaceWidth){
			points_.Add(Vector2.zero);
		}

		previousOffsetX_ = transform.position.x;
	}

	void Update(){
		Initialise();
	}

	void Recalculate(){
		EdgeCollider2D collider_ = GetComponent<EdgeCollider2D>();
		
		float offset_ = _surface._surfaceWidth*0.5f;

		

		for ( int i = 0; i < _surface._surfaceWidth; ++i ){
			points_[i] = _surface.GetSurfaceZ(Vector3.right * (i-offset_) + transform.position) - transform.position + Vector3.left * 5;
		}

		collider_.points = points_.ToArray();
	}

	public void OnUpdate(){
		Recalculate();
	}

}
}
