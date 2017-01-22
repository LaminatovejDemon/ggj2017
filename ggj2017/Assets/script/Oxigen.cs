using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxigen : MonoBehaviour {

	public GameObject _deathCurtain;
	public title _deathTitle;

	public float _maxOxigenAmount;
	public TextMesh _oxigenTemplate;
	TextMesh _oxigen;
	float _oxigenAmount;
	float _baseSecondsReward = 0.25f;
	float _defaultOxigenAmount = 25f;
	float _deathTicker = -1;
	float _restartTimeStamp = -1;
		
	// Update is called once per frame
	void Update () {
		InitialiseOxigen ();
		UpdateOxigen ();
		UpdateDeath ();
		UpdateRestart ();
	}

	void UpdateRestart(){
		if (_restartTimeStamp == -1) {
			return;
		}

		Color bak_ = _deathCurtain.GetComponent<MeshRenderer> ().material.GetColor ("_TintColor");
		bak_.a = 1.0f - ((Time.time - _restartTimeStamp) * 0.5f);
		_deathCurtain.GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", bak_);


		if (Time.time - _restartTimeStamp > 2.0f) {
			_restartTimeStamp = -1;
		}
	}

	void Restart(){
		_baseSecondsReward = 0.25f;
		_defaultOxigenAmount = 25f;
		_deathTicker = -1;
		_deathTitle.Reset ();
		_restartTimeStamp = Time.time;
		_maxOxigenAmount = _defaultOxigenAmount;
		_oxigenAmount = _defaultOxigenAmount;
	}

	void UpdateDeath(){
		if (_deathTicker == -1) {
			return;
		}
		Color bak_ = _deathCurtain.GetComponent<MeshRenderer> ().material.GetColor ("_TintColor");
		bak_.a = Mathf.Pow ((Time.time - _deathTicker) * 0.2f, 2);
		_deathCurtain.GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", bak_);


		if ( Time.time - _deathTicker > 5.0f ){
			if (_deathTitle.GetState () == title.state.FadeIn) {
				_deathTitle.SetState (title.state.ToBeHidden);
			} else if (_deathTitle.GetState () == title.state.Hidden) {
				Camera.main.GetComponent<taskManager> ().Notify (taskManager.action.restart);
				Restart ();	
			}
		}
		else if ( Time.time - _deathTicker > 1.5f ) {
			if (_deathTitle.GetState () != title.state.ToBeDisplayed) {
				_deathTitle.SetState (title.state.ToBeDisplayed);
			}
		}

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
		if (diver.instance.GetState() == diver.state.Diving) {
			if (_oxigenAmount <= 0.0) {
				diver.instance.Death ();
				_deathTicker = Time.time;
			} else {
				_oxigenAmount -= 1.0f * Time.deltaTime;
				_oxigen.text = _oxigenAmount.ToString ("0.0");

				_oxigen.transform.GetChild (0).gameObject.SetActive (true);
				float value = Mathf.Max(0.25f, ((_maxOxigenAmount - _oxigenAmount) / _maxOxigenAmount));
				_oxigen.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetFloat ("_Cutoff", value);
			}
		}
	}

	public void Notify(taskManager.action what, float maxDepth = 0){
//		Debug.Log ("Oxigen Notified: " + what.ToString () + "with depth: " + maxDepth);
		switch (what) {
		case taskManager.action.treasureDiveSuccess:
			//_maxOxigenAmount += REWARD
		case taskManager.action.diveSuccess:
			_maxOxigenAmount += Mathf.Floor (maxDepth / 10) * _baseSecondsReward;
			_oxigenAmount = _maxOxigenAmount;
			_oxigen.text = _maxOxigenAmount.ToString();
			break;
		default:
			break;
		}

		_oxigen.transform.GetChild (0).gameObject.SetActive (false);
	}
}
