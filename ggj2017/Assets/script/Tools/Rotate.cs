using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	void Update () {
		transform.Rotate(Vector3.up, Time.deltaTime * 300.0f);		
	}
}
