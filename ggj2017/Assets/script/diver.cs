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


	void Update () {
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
			return;
		}

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
		if (Input.GetKey (KeyCode.S)) {
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
