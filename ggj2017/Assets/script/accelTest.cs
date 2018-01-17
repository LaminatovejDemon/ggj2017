using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accelTest : MonoBehaviour {

	void Update () {
		
		for (int i = 0; i < Input.touchCount; ++i) {
			GetComponent<TextMesh> ().text = Camera.main.ScreenToViewportPoint (Input.GetTouch (i).position).x.ToString ();
		}

	}
}
