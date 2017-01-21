using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxigen : MonoBehaviour {



	public float _maxOxigenAmount;
	public TextMesh _oxigenTemplate;
	TextMesh _oxigen;
	float _oxigenAmount;
	float _baseSecondsReward = 0.25f;
	float _defaultOxigenAmount = 25f;

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
			
		_maxOxigenAmount = _defaultOxigenAmount;
		_oxigenAmount = _defaultOxigenAmount;
		_oxigen = new TextMesh ();
		_oxigen= GameObject.Instantiate (_oxigenTemplate);
		_oxigen.transform.GetChild (0).gameObject.SetActive (false);
		Vector3 localPos_ = _oxigen.transform.localPosition;
		_oxigen.transform.parent = Camera.main.transform;
		_oxigen.transform.localRotation = Quaternion.identity;
		_oxigen.transform.localPosition = localPos_;
	}

	void UpdateOxigen () {
		///TEST


//		if (value > 0.0f) {


//			transform.GetComponentInChildren<MeshRenderer>().material.SetFloat ("_AlphaCutoff", value);
//		}
		/// TEST END

		if (diver.instance._state == diver.state.Diving) {
			if (_oxigenAmount <= 0.0) {
				diver.instance.Death ();
			} else {
				_oxigenAmount -= 1.0f * Time.deltaTime;
				_oxigen.text = _oxigenAmount.ToString ("0.0") + " sec";

				_oxigen.transform.GetChild (0).gameObject.SetActive (true);
				float value = Mathf.Max(0.25f, ((_maxOxigenAmount - _oxigenAmount) / _maxOxigenAmount));
				Debug.Log ("value: "+value);
				Debug.Log ("_maxOxigenAmount: "+_maxOxigenAmount);
				Debug.Log ("_oxigenAmount: "+_oxigenAmount);
				_oxigen.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetFloat ("_Cutoff", value);
			}
		}
	}

	public void Notify(taskManager.action what, float maxDepth){
//		Debug.Log ("Oxigen Notified: " + what.ToString () + "with depth: " + maxDepth);
		switch (what) {
		case taskManager.action.treasureDiveSuccess:
			//_maxOxigenAmount += REWARD
		case taskManager.action.diveSuccess:
			_maxOxigenAmount += Mathf.Floor (maxDepth / 10) * _baseSecondsReward;
			_oxigenAmount = _maxOxigenAmount;
			_oxigen.text = _maxOxigenAmount + " sec";
			break;
		case taskManager.action.death:
			_oxigenAmount = _defaultOxigenAmount;
			break;
		default:
			break;
		}

		_oxigen.transform.GetChild (0).gameObject.SetActive (false);
	}
}
