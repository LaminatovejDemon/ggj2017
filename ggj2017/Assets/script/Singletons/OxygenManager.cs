using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenManager : BaseManager<OxygenManager> {
	
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
		_deathCurtain.SetActive(true);
		_baseOxygenAmount = baseOxygenAmuount;
		_visibleOxygen = 0f;
		_deathTicker = -1;
		_deathTitle.Reset ();
		_restartTimeStamp = Time.time;
		_maxOxygenAmount = _baseOxygenAmount;
		_oxigenAmount = _baseOxygenAmount;
		
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
			_rewardTitle.SetTitle ("("+ TaskManager.get.progress +") Maximum oxygen capacity raised by  " + value.ToString ("0.00") + "s");
		} else {
			_rewardTitle.SetTitle ("("+ TaskManager.get.progress +") Dangerous dive strenghtened you by " + (value * 0.5f).ToString ("0.00") + "s X2");
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
		float oxygenLeft_ = GetOxygenLeft();
		int oxygenBase_ = (int)(oxygenLeft_);
		float oxygenRest_ = oxygenLeft_ - oxygenBase_;

		for (int i = 0; i < _meters.Length; ++i) {
			_meters [i].transform.position = RenderCamera.get.transform.position - Vector3.forward * 0.2f + Vector3.right * (RenderCamera.get.GetComponent<Camera>().aspect * 4.0f)
				+ Vector3.down * (oxygenRest_ * 1.0f + 7) + Vector3.down * (i) + Vector3.up * ( RenderCamera.get.GetComponent<Camera>().orthographicSize * 2 + 4f);
			int value_ = oxygenBase_ + 1 - i;
			_meters [i].text = value_ < 0 ? "" : value_ + "s";
		}
	}

	// Update is called once per frame
	void Update () {
		InitialiseMeters ();	
		UpdateMeters ();
		UpdateOxigen ();
		UpdateDeath ();
		UpdateRestart ();
	}

// OXYGEN PART
	public GameObject _deathCurtain;
	public title _deathTitle;
	public float baseOxygenAmuount;
	float _maxOxygenAmount;
	float _baseOxygenAmount;
	public float _oxigenAmount;
	// float _baseSecondsReward = 0.25f;
	
	float _deathTicker = -1;
	float _restartTimeStamp = -1;
	float _visibleOxygen = 0f;

	public float GetOxygenLeft(){
		return _visibleOxygen;
	}

	void UpdateRestart(){
		if (_restartTimeStamp == -1) {
			return;
		}

		float duration_ = 0.6f;

		Color bak_ = _deathCurtain.GetComponent<MeshRenderer> ().material.GetColor ("_TintColor");
		bak_.a = 1.0f - ((Time.time - _restartTimeStamp) /  duration_);
		_deathCurtain.GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", bak_);


		if (Time.time - _restartTimeStamp > duration_) {
			_restartTimeStamp = -1;
		}
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
				TaskManager.get.Notify (TaskManager.action.restart);
				Reset ();	
			}
		}
		else if ( Time.time - _deathTicker > 1.5f ) {
			if (_deathTitle.GetState () != title.state.ToBeDisplayed) {
				_deathTitle.SetState (title.state.ToBeDisplayed);
			}
		}

	}

	void Start () {
		_maxOxygenAmount = _baseOxygenAmount;
		_oxigenAmount = _baseOxygenAmount;
	}

	void UpdateOxigen () {
		if (Diver.get.GetState() == Diver.state.Diving) {
			if (-Diver.get.GetPosition().y * 0.7f > _oxigenAmount && _oxigenAmount > 13) {
				AudioManager.get.Notify (TaskManager.action.danger);
				DoubleXP ();
			}
			if (_oxigenAmount <= 0.0) {
				Diver.get.Death ();
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
		if (IsDoubleXP ()) {
			reward_ *= 2.0f;
		}
		Debug.Log("Reward is " + reward_);
		return reward_;
	}

	public void Notify(TaskManager.action what, float maxDepth = 0){

		float reward_ = 0;
//		Debug.Log ("Oxigen Notified: " + what.ToString () + "with depth: " + maxDepth);
		switch (what) {
		case TaskManager.action.treasureDiveSuccess:
//			_maxOxigenAmount += maxDepth / 10) * _baseSecondsReward;
			reward_ = CalculateReward (maxDepth, true) * 10.0f;
			_maxOxygenAmount += reward_;
			GotReward (reward_);
			_oxigenAmount = _maxOxygenAmount;
			break;
		case TaskManager.action.diveSuccess:
			reward_ = CalculateReward(maxDepth);
			GotReward(reward_);
			_maxOxygenAmount += reward_;
			_oxigenAmount = _maxOxygenAmount;
			break;
		default:
			break;
		}
	}
}
