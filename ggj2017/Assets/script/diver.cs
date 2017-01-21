using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diver : MonoBehaviour {

	public surface _surface;
	float _swimAngle = 0.5f;
	Vector3 _defaultVector = Vector3.one;
	int SurfaceState;
	bool _keyUpDown = false;
	bool _keyDownDown = false;
	public title _title;
	public GameObject _accelerometerUIMesh;
	float _accelThreshold = 0.17f;

	void UpdateAccelerometer(){
		Vector3 backup_ = _accelerometerUIMesh.transform.localScale;
		backup_.x += (Mathf.Abs(Input.acceleration.x) - backup_.x) * 3f * Time.deltaTime;
		backup_.y += ((Mathf.Abs (Input.acceleration.x) > _accelThreshold ? 0.17f : 0.07f) - backup_.y) * 3f * Time.deltaTime;

		_accelerometerUIMesh.transform.localScale = backup_;
		backup_ = _accelerometerUIMesh.transform.localPosition;
		backup_.x += (Input.acceleration.x - backup_.x) * 3f * Time.deltaTime;
		_accelerometerUIMesh.transform.localPosition = backup_;
	}

	void Update () {
		UpdateAccelerometer ();
			
		if (GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			HandleIdle ();
		} else if (GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("diverSwimTree")) {
			HandleSwim ();
		}
	}


	void HandleSwim(){
		if ((_surface.GetActualPosition (transform.position) + Vector3.down * 0.3f).y < transform.position.y) {
			GetComponent<Animator> ().ResetTrigger ("Dive");
			GetComponent<Animator> ().SetTrigger ("Surface");
			_title.SetState (title.state.ToBeDisplayed);
			return;
		}

#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.A)) {
			_keyUpDown = true;
		} else if (Input.GetKeyUp (KeyCode.A)) {
			_keyUpDown = false;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			_keyDownDown = true;
		} else if (Input.GetKeyUp (KeyCode.D)) {
			_keyDownDown = false;
		}
#else
		if ( Input.acceleration.x > _accelThreshold ){
			_keyUpDown = true;
		} else if (Input.acceleration.x < -_accelThreshold){
			_keyDownDown = true;
		} else {
			_keyUpDown = false;
			_keyDownDown = false;
		}
#endif


		if (_keyUpDown) {
			_swimAngle += Time.deltaTime * 2.0f;
		}

		if (_keyDownDown) {
			_swimAngle -= Time.deltaTime * 2.0f;
		}

		if (!(_keyDownDown || _keyUpDown)) {
			_swimAngle += (0.5f - _swimAngle) * Time.deltaTime * 3.0f; 
		} else {
			_swimAngle = Mathf.Clamp (_swimAngle, 0, 1);
		}
			
		GetComponent<Animator> ().SetFloat ("SwimDirection", _swimAngle);
	}

	void HandleIdle(){
#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.S)) {
#else
		if ( Input.touchCount > 0 ){

#endif
			_title.SetState(title.state.ToBeHidden);
			GetComponent<Animator> ().SetTrigger ("Dive");
		}




		if (_defaultVector == Vector3.one) {
			_defaultVector = transform.eulerAngles;
		}

		if (transform.eulerAngles != _defaultVector) {
			transform.eulerAngles = _defaultVector;
		}


		Vector3 posBak_ = transform.position;
		posBak_.y = (_surface.GetActualPosition(transform.position) + Vector3.down * 0.3f).y;
		transform.position += (posBak_ - transform.position) * Time.deltaTime * 10.0f;
	}




}
