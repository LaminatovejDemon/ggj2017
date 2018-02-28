using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterManager : BaseManager<MeterManager> {

	public TextMesh _meterTemplate;
	float step_ = 1;
	TextMesh[] _meters;


	void InitialiseMeters(){
		if (_meters != null) {
			return;
		}

		_meters = new TextMesh[(int)((RenderCamera.get.GetComponent<Camera>().orthographicSize * 2) / step_) + 2];
		Vector3 base_ = Vector3.forward * 0.2f + Vector3.left * (MainCamera.get.GetComponent<Camera>().aspect * 4.9f);
		base_.y = (int)(MainCamera.get.transform.position.y / 10.0f) * 10.0f;
		base_ += -Vector3.up * 5.0f;

		for (int i = 0; i < _meters.Length; ++i) {
			_meters [i] = GameObject.Instantiate (_meterTemplate);
			Vector3 position_ =  base_ + Vector3.down * (i * 5.0f);
			_meters [i].transform.position = position_;
			_meters [i].text = "- " + (int)(-_meters [i].transform.position.y) + "m";
			_meters [i].transform.parent = MainCamera.get.transform;
		}
	}
		
	void UpdateMeters(){
		float depth_ = RenderCamera.get.transform.position.y -Diver.get._surface.GetSurfaceZ(Diver.get.GetPosition()).y;
		float depthMajor_ = (int)(depth_ / step_) * step_;
		float depthMinor_ = depth_ - depthMajor_;

		// float viewPortX_ = MainCamera.get.GetComponent<Camera>().WorldToViewportPoint (_meters [0].transform.position).x;

		for (int i = 0; i < _meters.Length; ++i) {
			Vector3 pos_ = _meters [i].transform.position;
			pos_.y = MainCamera.get.GetComponent<Camera>().ViewportToWorldPoint (Vector3.up * ( 0.5f - ((_meters.Length * 0.5f) - i) * 0.1f )).y * step_ - depthMinor_;
			_meters [i].transform.position = pos_;

			float currentDepth_ = -(int)(depthMajor_ + (i * step_) - (_meters.Length+2) * 0.5f );
			_meters [i].text = currentDepth_ > 0 ? currentDepth_ + "m" : "";
		}
	}

	// Update is called once per frame
	void Update () {
		InitialiseMeters ();	
		UpdateMeters ();
	}
}
