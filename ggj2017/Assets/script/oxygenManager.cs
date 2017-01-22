using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oxygenManager : MonoBehaviour {

	public TextMesh _meterTemplate;
	TextMesh[] _meters;


	Vector3 GetBase(){
		Vector3 ret_ = Camera.main.transform.position + Vector3.forward * 0.2f + Vector3.right * (Camera.main.aspect * 4.0f);
		ret_.y = (int)(Camera.main.transform.position.y / 10.0f) * 10.0f;
		ret_ += -Vector3.up * 10.0f;
		return ret_;
	}

	void InitialiseMeters(){
		if (_meters != null) {
			return;
		}

		_meters = new TextMesh[11];
		Vector3 base_ = GetBase ();

		for (int i = 0; i < _meters.Length; ++i) {
			_meters [i] = GameObject.Instantiate (_meterTemplate);
			_meters [i].text = "";
		}
	}

	void UpdateMeters(){
		float oxygenLeft_ = Camera.main.GetComponent<Oxigen> ().GetOxygenLeft ();
		int oxygenBase_ = (int)(oxygenLeft_);
		float oxygenRest_ = oxygenLeft_ - oxygenBase_;

		for (int i = 0; i < _meters.Length; ++i) {
			_meters [i].transform.position = Camera.main.transform.position + Vector3.forward * 10.0f + Vector3.right * (Camera.main.aspect * 4.0f)
				+ Vector3.down * (oxygenRest_ * 1.0f + 7) + Vector3.down * (i) + Vector3.up * (_meters.Length );
			int value_ = oxygenBase_ + 1 - i;
			_meters [i].text = value_ < 0 ? "" : value_ + "s";
		}
	}

	// Update is called once per frame
	void Update () {
		InitialiseMeters ();	
		UpdateMeters ();
	}
}
