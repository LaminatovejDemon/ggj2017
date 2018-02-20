using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class task : MonoBehaviour {

	public void Setup(float depth){
		transform.position = Diver.instance.transform.position + Vector3.left * Random.Range (-30.0f, 30.0f) + Vector3.down * depth;
	}

	void Update(){
		Vector3 distance_ = transform.position - Diver.instance.transform.position;
		distance_.z = 0;

		if ((distance_).magnitude < 2.0f) {
			Diver.instance.GetTreasure ();
			GetComponent<ParticleSystem> ().Stop ();
		}
	}
}
