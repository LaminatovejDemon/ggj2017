﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class task : MonoBehaviour {

	public void Setup(float depth){
		transform.position = diver.instance.transform.position + Vector3.left * Random.Range (-5.0f, 5.0f) + Vector3.down * depth;
	}

	void Update(){
		Vector3 distance_ = transform.position - diver.instance.transform.position;
		distance_.z = 0;

		if ((distance_).magnitude < 1.0f) {
			diver.instance.GetTreasure ();
			GetComponent<ParticleSystem> ().Stop ();
		}
	}
}
