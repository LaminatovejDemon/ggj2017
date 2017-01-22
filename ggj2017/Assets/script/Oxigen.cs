using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxigen : MonoBehaviour {

	public GameObject _deathCurtain;
	public title _deathTitle;

	public float _maxOxigenAmount;

	float _oxigenAmount;
	float _baseSecondsReward = 0.25f;
	float _defaultOxigenAmount = 25f;
	float _deathTicker = -1;
	float _restartTimeStamp = -1;
	float _visibleOxygen = 0f;

	public float GetOxygenLeft(){
		return _visibleOxygen;
	}
		
	// Update is called once per frame
	void Update () {
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
		_visibleOxygen = 0f;
		_deathTicker = -1;
		_deathTitle.Reset ();
		_restartTimeStamp = Time.time;
		_maxOxigenAmount = _defaultOxigenAmount;
		_oxigenAmount = _defaultOxigenAmount;
		Camera.main.GetComponent<oxygenManager> ().Reset ();
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

	void Start () {
		_maxOxigenAmount = _defaultOxigenAmount;
		_oxigenAmount = _defaultOxigenAmount;
	}

	void UpdateOxigen () {
		if (diver.instance.GetState() == diver.state.Diving) {
			if (-diver.instance.transform.position.y * 0.5f > _oxigenAmount && _oxigenAmount > 10) {
				Camera.main.GetComponent<audioManager> ().Notify (taskManager.action.danger);
				Camera.main.GetComponent<oxygenManager> ().DoubleXP ();
			}
			if (_oxigenAmount <= 0.0) {
				diver.instance.Death ();
				_deathTicker = Time.time;
			} else {
				_oxigenAmount -= 1.0f * Time.deltaTime;
			}
		}

		if (_oxigenAmount != _visibleOxygen) {
			float addition_ = (_oxigenAmount - _visibleOxygen) * 0.8f * Time.deltaTime;
			if ((_visibleOxygen < _oxigenAmount && _visibleOxygen + addition_ > _oxigenAmount) ||
			    (_visibleOxygen > _oxigenAmount && _visibleOxygen + addition_ < _oxigenAmount)) {
				_visibleOxygen = _oxigenAmount;
			} else {
				_visibleOxygen = _visibleOxygen + addition_;
			}
		}
	}

	float CalculateReward(float depth, bool force = false){
		
		float reward_ = (!force && depth < 20.0f) ? 0 : Mathf.Pow(depth, 1.5f) * 0.001f;
		if (Camera.main.GetComponent<oxygenManager> ().IsDoubleXP ()) {
			reward_ *= 2.0f;
		}
		Debug.Log("Reward is " + reward_);
		return reward_;
	}

	public void Notify(taskManager.action what, float maxDepth = 0){

		float reward_ = 0;
//		Debug.Log ("Oxigen Notified: " + what.ToString () + "with depth: " + maxDepth);
		switch (what) {
		case taskManager.action.treasureDiveSuccess:
//			_maxOxigenAmount += maxDepth / 10) * _baseSecondsReward;
			reward_ = CalculateReward (maxDepth, true) * 10.0f;
			_maxOxigenAmount += reward_;
			Camera.main.GetComponent<oxygenManager> ().GotReward (reward_);
			_oxigenAmount = _maxOxigenAmount;
			break;
		case taskManager.action.diveSuccess:
			reward_ = CalculateReward(maxDepth);
			Camera.main.GetComponent<oxygenManager> ().GotReward(reward_);
			_maxOxigenAmount += reward_;
			_oxigenAmount = _maxOxigenAmount;
			break;
		default:
			break;
		}
	}
}
