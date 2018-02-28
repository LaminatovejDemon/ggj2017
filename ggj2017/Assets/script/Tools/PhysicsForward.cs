using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsForward : MonoBehaviour {

	public Diver listener;
	bool InCollisionWorkaround = false;
	bool InCollisionUpdateWorkaround = false;
	Collision2D _lastSource = null;

	void Update(){
		if ( !InCollisionUpdateWorkaround && InCollisionWorkaround ){
			listener.OnCollisionExit2D(_lastSource);
			InCollisionWorkaround = false;	
		}
		InCollisionUpdateWorkaround = false;
	}

	public void OnCollisionStay2D(Collision2D source){
		InCollisionUpdateWorkaround = true;
		InCollisionWorkaround = true;
		_lastSource = source;
		listener.OnCollisionStay2D(source);
	}

	public void OnCollisionExit2D(Collision2D source){
		InCollisionWorkaround = false;
		listener.OnCollisionExit2D(source);
	}
}
