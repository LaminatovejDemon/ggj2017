using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class task : MonoBehaviour {

	public void Setup(float depth){
		transform.position = Diver.get.GetPosition() + Vector3.left * Random.Range (-30.0f, 30.0f) + Vector3.down * depth;
	}

	void Update(){
		Vector3 distance_ = transform.position - Diver.get.GetPosition();
		distance_.z = 0;

		if ((distance_).magnitude < 2.0f) {
			Diver.get.GetTreasure ();
			GetComponent<ParticleSystem> ().Stop ();
		}
	}
}
