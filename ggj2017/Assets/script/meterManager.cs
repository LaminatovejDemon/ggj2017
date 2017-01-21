using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meterManager : MonoBehaviour {

	public TextMesh _meterTemplate;
	TextMesh[] _meters;


	void InitialiseMeters(){
		if (_meters != null) {
			return;
		}

		_meters = new TextMesh[5];
		Vector3 base_ = Vector3.forward * 0.2f + Vector3.left * (Camera.main.aspect * 4.0f);
		base_.y = (int)(Camera.main.transform.position.y / 10.0f) * 10.0f;
		base_ += -Vector3.up * 5.0f;

		for (int i = 0; i < _meters.Length; ++i) {
			_meters [i] = GameObject.Instantiate (_meterTemplate);
			Vector3 position_ =  base_ + Vector3.down * (i * 5.0f);
			_meters [i].transform.position = position_;
			_meters [i].text = "- " + (int)(-_meters [i].transform.position.y) + "m";
		}
	}
		
	void UpdateMeters(){
		for (int i = 0; i < _meters.Length; ++i) {
			float viewportPoint_ = Camera.main.WorldToViewportPoint (_meters [i].transform.position).y;

			if (viewportPoint_ > 1.0f) {
				_meters [i].transform.position += Vector3.down * 25.0f;
				_meters [i].text = "- " + (int)(-_meters [i].transform.position.y) + "m";
			}

			if (viewportPoint_ < -0.65f && _meters[i].transform.position.y+25.0f < 2.0f) {
				_meters [i].transform.position -= Vector3.down * 25.0f;
				_meters [i].text = "- " + (int)(-_meters [i].transform.position.y) + "m";
			}

			Vector3 bak_ = _meters [i].transform.position;
			bak_.x = Camera.main.transform.position.x - (Camera.main.aspect * 4.0f);
			_meters [i].transform.position = bak_;
		}
	}

	// Update is called once per frame
	void Update () {
		InitialiseMeters ();	
		UpdateMeters ();
	}
}
