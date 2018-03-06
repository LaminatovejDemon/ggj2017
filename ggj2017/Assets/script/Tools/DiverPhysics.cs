using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DiverPhysics : MonoBehaviour {

	float _currentCollisionDot = -1;
	Collision2D _lastCollision = null;

	void Update(){
		if ( _lastCollision == null) {
			return;
		}
		Vector2 diff_ = _lastCollision.contacts[0].point - (Vector2)(Diver.get.transform.position);
		if ( diff_.magnitude > 2.0f ){
			_lastCollision = null;
			_currentCollisionDot = -1;
			return;
		}
		_currentCollisionDot = DirectionMarker.get.GetCollisionUIDot(_lastCollision);
	}

	public void OnTriggerEnter2D(Collider2D source){
		SnapManager.get.ActivateSnap(source.gameObject.GetComponent<Snap>());
	}

	public void OnTriggerExit2D(Collider2D source){
		SnapManager.get.DisableSnap(source.gameObject.GetComponent<Snap>());
	}

	public bool IsSteepCollision(){
		return  _currentCollisionDot > 0.6f;
	}

	public void Reset(){
		_currentCollisionDot = -1;
	}

	public void OnCollisionEnter2D(Collision2D source){
		_lastCollision = source;
		Update();
		Diver.get.InCollision();
	}

	public void OnCollisionStay2D(Collision2D source){
		transform.parent = null;
		Diver.get.transform.position = transform.position;
		transform.parent = Diver.get.transform;
	}	
}
