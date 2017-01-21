using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class title : MonoBehaviour {

	public title _subtitle;

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

	void Start(){
		SetState (state.ToBeDisplayed);	
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
			GetComponent<TextMesh> ().color = new Color (1, 1, 1, 0);
			if (Time.time - _stateTimeStamp > 2.0f) {
				SetState (state.FadeIn);
				if ( _subtitle != null ){
					_subtitle.SetState (state.ToBeDisplayed);
				}
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
					if ( _subtitle != null ){
						_subtitle.SetState (state.ToBeHidden);
					}
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
		_state = target;
		_stateTimeStamp = Time.time;
	}
}
