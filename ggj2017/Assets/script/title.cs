using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class title : MonoBehaviour {

	public string [] _randomTexts;

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
		GetComponent<TextMesh> ().color = new Color (1, 1, 1, 0);
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
			GetComponent<TextMesh> ().color = new Color (1, 1, 1, 0);
			break;
		case state.ToBeDisplayed:

			if (_randomTexts.Length > 0) {
				GetComponent<TextMesh> ().text = _randomTexts [Random.Range (0, _randomTexts.Length-1)];
			}
			GetComponent<TextMesh> ().color = new Color (1, 1, 1, 0);
			if (Time.time - _stateTimeStamp > 2.0f) {
				SetState (state.FadeIn);
			}
			break;
		case state.FadeIn:
			if (Time.time - _stateTimeStamp > 2.0f) {
				GetComponent<TextMesh> ().color = new Color (1, 1, 1, 1);
//				SetState (state.ToBeHidden);
			} else {
				GetComponent<TextMesh> ().color = new Color (1, 1, 1, 0.5f * (Time.time - _stateTimeStamp));
			}
			break;
		case state.ToBeHidden:
			if (Time.time - _stateTimeStamp > 1.0f) {
				SetState (state.FadeOut);
			}
			break;
		case state.FadeOut:
			if (Time.time - _stateTimeStamp > 1.0f) {
				GetComponent<TextMesh> ().color = new Color (1, 1, 1, 0);
				SetState (state.Hidden);
			} else {
				GetComponent<TextMesh> ().color = new Color (1, 1, 1, 1.0f - ( (Time.time - _stateTimeStamp)));
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
		_state = target;
		_stateTimeStamp = Time.time;
	}
}
