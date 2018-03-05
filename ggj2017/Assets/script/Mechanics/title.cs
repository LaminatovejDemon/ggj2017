using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class title : MonoBehaviour {

	public string [] _randomTexts;
	public title _followUp;

	TextMesh _ver1;
	Material _ver2;

	public enum state{
		Hidden,
		ToBeDisplayed,
		FadeIn,
		ToBeHidden,
		FadeOut,
	};

	float _stateTimeStamp = -1;
	state _state = state.Hidden;
	Vector3 _initialPosition = Vector3.zero;

	public void Reset(){
		_state = state.Hidden;
		_initialPosition = Vector3.zero;
		_stateTimeStamp = -1;
		if (_followUp != null) {
			_followUp.Reset ();
		}
		SetAlpha(0);
	}

	public void SetTitle(string title){
		GetComponent<TextMesh> ().text = title;
	}

	void SetAlpha(float alpha){
		if (_ver1 == null && _ver2 == null) {
			_ver1 = GetComponent<TextMesh> ();
			if (_ver1 == null) {
				_ver2 = GetComponent<MeshRenderer> ().material;
			}
		}

		Color bak_;

		if (_ver1 != null) {
			bak_ = _ver1.color;
			bak_.a = alpha;
			_ver1.color = bak_;	
		} else if ( _ver2 != null ){
			bak_ = _ver2.GetColor ("_TintColor");
			bak_.a = alpha;
			_ver2.SetColor("_TintColor", bak_);
		}
	}

	Color GetColor(){
		if (_ver1 == null && _ver2 == null) {
			_ver1 = GetComponent<TextMesh> ();
			if (_ver1 == null) {
				_ver2 = GetComponent<MeshRenderer> ().material;
			}
		}

		if (_ver1 != null) {
			return _ver1.color;
		} else if ( _ver2 != null ){
			return _ver2.GetColor("_TintColor");
		}
		return Color.red;
	}

	public state GetState(){
		return _state;
	}
			
	void Update () {

		if (_initialPosition == Vector3.zero) {
			_initialPosition = transform.localPosition;
		}
		if (_state != state.Hidden && _state != state.ToBeDisplayed) {
			Vector3 back_ = _initialPosition;
			back_.y = Mathf.Sin (Time.time * 0.2f) * 0.25f + _initialPosition.y;
			transform.localPosition = back_;
		}


		switch (_state) {
		case state.Hidden:
			SetAlpha(0);
			break;
		case state.ToBeDisplayed:

			if (_randomTexts.Length > 0) {
				GetComponent<TextMesh> ().text = _randomTexts [Random.Range (0, _randomTexts.Length-1)];
			}
			SetAlpha(0);
			if (Time.time - _stateTimeStamp > 2.0f) {
				SetState (state.FadeIn);
				if (_followUp != null) {
					_followUp.SetState (state.ToBeDisplayed);
				}
			}
			break;
		case state.FadeIn:
			if (Time.time - _stateTimeStamp > 2.0f) {
				SetAlpha(1);
//				SetState (state.ToBeHidden);
			} else {
				SetAlpha(0.5f * (Time.time - _stateTimeStamp));
			}
			break;
		case state.ToBeHidden:
			if (Time.time - _stateTimeStamp > 1.0f) {
				SetState (state.FadeOut);
				if (_followUp != null) {
					_followUp.SetState (state.ToBeHidden);
				}
			}
			break;
		case state.FadeOut:
			if (Time.time - _stateTimeStamp > 1.0f) {
				SetAlpha(0);
				SetState (state.Hidden);
			} else {
				SetAlpha(1.0f - ( (Time.time - _stateTimeStamp)));
			}
			break;
		}

	}

	public void SetState(state target){
		if ( target == _state || (target == state.ToBeDisplayed && _state == state.FadeIn) ){
			return;
		}
		if (target == state.ToBeHidden && _state == state.Hidden) {
			return;
		}
		gameObject.SetActive(true);
		_state = target;
		_stateTimeStamp = Time.time;
	}
}
