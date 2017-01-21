using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxigen : MonoBehaviour {

	public TextMesh _oxigenTemplate;
	TextMesh _oxigen;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		InitialiseOxigen ();
		UpdateOxigen ();
	}

	void InitialiseOxigen () {
		if (_oxigen != null) {
			return;
		}

		_oxigen = new TextMesh ();
		_oxigen= GameObject.Instantiate (_oxigenTemplate);
		Vector3 v3Pos = new Vector3(1.2f, 1.1f, 10.0f);
		_oxigen.transform.parent = Camera.main.transform;
		_oxigen.transform.position = Camera.main.ViewportToWorldPoint(v3Pos);
		_oxigen.transform.position += Vector3.forward * 5.0f;
	}

	void UpdateOxigen () {

	}
}
