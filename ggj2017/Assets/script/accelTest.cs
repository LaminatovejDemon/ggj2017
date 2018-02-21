using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class accelTest : MonoBehaviour {

	void Update () {
		
		for (int i = 0; i < Input.touchCount; ++i) {
			GetComponent<TextMesh> ().text = MainCamera.get.GetComponent<Camera>().ScreenToViewportPoint (Input.GetTouch (i).position).x.ToString ();
		}

	}
}
