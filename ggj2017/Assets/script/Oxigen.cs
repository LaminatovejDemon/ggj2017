using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxigen : MonoBehaviour {

	public float _maxOxigenAmount;
	public TextMesh _oxigenTemplate;
	TextMesh _oxigen;
	float _oxigenAmount;
	float _secondsByLayerReward = 2f;//0.25f;
	float _defaultOxigenAmount = 15f;

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
		Vector3 v3Pos = new Vector3(1.2f, 1.1f, 10.0f);
		_oxigen.transform.parent = Camera.main.transform;
		_oxigen.transform.position = Camera.main.ViewportToWorldPoint(v3Pos);
		_oxigen.transform.position += Vector3.forward * 5.0f;
	}

	void UpdateOxigen () {
		if (diver.instance._state == diver.state.Diving) {
			if (_oxigenAmount <= 0.0) {
				diver.instance.Death ();
			} else {
				_oxigenAmount -= 1.0f * Time.deltaTime;
				_oxigen.text = _oxigenAmount.ToString ("0.0") + " sec";
			}
		}
	}

	public void Notify(taskManager.action what, float maxDepth){
//		Debug.Log ("Oxigen Notified: " + what.ToString () + "with depth: " + maxDepth);
		switch (what) {
		case taskManager.action.treasureDiveSuccess:
//			_oxigenAmount = _oxigenAmount + REWARD
		case taskManager.action.diveSuccess:
			_maxOxigenAmount += Mathf.Floor (maxDepth / 10) * _secondsByLayerReward;
			_oxigen.text = _maxOxigenAmount + " sec";
			break;
		case taskManager.action.death:
			_oxigenAmount = _defaultOxigenAmount;
			break;
		default:
			break;
		}
	}
}
