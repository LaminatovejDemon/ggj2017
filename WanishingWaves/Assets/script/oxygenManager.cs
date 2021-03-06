﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oxygenManager : MonoBehaviour {

	public title _rewardTitle;
	public TextMesh _meterTemplate;
	TextMesh[] _meters;
	bool _doubleXP = false;


	public bool IsDoubleXP(){
		return _doubleXP;
	}

	public void DoubleXP(){
		_doubleXP = true;
	}

	public void Reset(){
		_rewardTitle.Reset ();
		_doubleXP = false;
	}

	public void DismissRewawrd(){
		_rewardTitle.SetState (title.state.ToBeHidden);
	}

	public void GotReward(float value){
		
		if (value <= 0) {
			_doubleXP = false;
			return;
		}

		if (!_doubleXP) {
			_rewardTitle.SetTitle ("Maximum oxygen capacity raised by  " + value.ToString ("0.00") + "s");
		} else {
			_rewardTitle.SetTitle ("Dangerous dive strenghtened you by " + (value * 0.5f).ToString ("0.00") + "s X2");
		}
		_doubleXP = false;
			
		_rewardTitle.SetState (title.state.FadeIn);
	}

	void InitialiseMeters(){
		if (_meters != null) {
			return;
		}

		_meters = new TextMesh[11];

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
