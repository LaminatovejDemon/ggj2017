using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diver : MonoBehaviour {


	public ParticleSystem _bubbles;
	public Vector3 _defaultPosition;
	public Quaternion _defaultRotation;
	public float _InitialDelay;

	public static diver instance{
		get {
			return _instance;
		}
		private set {
			_instance = value;
		}
	}

	private static diver _instance;

	public enum state{
		None,
		Diving,
		Floating,
		Dying,
	};

	void RegisterSingleton(){
		if (_instance != null) {
			Debug.LogWarning ("You PORK");
		} 
		_defaultPosition = transform.position;
		_defaultRotation = transform.rotation;
		_instance = this;
	}

	public surface _surface;
	float _swimAngle = 0.5f;
	Vector3 _defaultVector = Vector3.one;
	int SurfaceState;
	bool _keyUpDown = false;
	bool _keyDownDown = false;
	public title _title;
	public GameObject _accelerometerUIMesh;
	float _accelThreshold = 0.17f;
	state _state = state.None;

	public void Restart(){
		GetComponent<Animator> ().SetTrigger ("Restart");
		GetComponent<Animator> ().ResetTrigger ("Surface");
		GetComponent<Animator> ().ResetTrigger ("Dive");
		transform.position = _defaultPosition;
		transform.rotation = _defaultRotation;
		Camera.main.GetComponent<cameraLink> ().Reset ();
		_state = state.Floating;
		_bubbles.Stop ();
		_InitialDelay = Time.time;
		_maxDepth = 0;
		_swimAngle = 0.5f;
		GetComponent<Animator> ().speed = 1.0f;
	}

	public void Death () {
		if ( _state == state.Dying ){
			return;
		}
		GetComponent<Animator> ().speed = 0.15f;
		_state = state.Dying;
	}

	public state GetState(){
		return _state;
	}

	void UpdateAccelerometer(){
		Vector3 backup_ = _accelerometerUIMesh.transform.localScale;
		backup_.x += (Mathf.Abs(Input.acceleration.x) - backup_.x) * 3f * Time.deltaTime;
		backup_.y += ((Mathf.Abs (Input.acceleration.x) > _accelThreshold ? 0.17f : 0.07f) - backup_.y) * 3f * Time.deltaTime;

		_accelerometerUIMesh.transform.localScale = backup_;
		backup_ = _accelerometerUIMesh.transform.localPosition;
		backup_.x += (Input.acceleration.x - backup_.x) * 3f * Time.deltaTime;
		_accelerometerUIMesh.transform.localPosition = backup_;
	}

	void Start(){
		_bubbles.Stop ();
		RegisterSingleton ();
	}

	void Update () {
		
		switch (_state) {
		case state.Dying:
			return;
		case state.None:
			NotifyManager (taskManager.action.idle);
			_state = state.Floating;
			break;
		}

		if ( _title.GetState() == title.state.Hidden && _state == state.Floating && _InitialDelay != -1 && Time.time - _InitialDelay > 10.0f ) {
			NotifyManager (taskManager.action.idle);
			_InitialDelay = -1;
		}

		if (transform.position.y > -15.0f && !_bubbles.isStopped) {
			_bubbles.Stop();
		} else if (transform.position.y < -15.0f & _bubbles.isStopped) {
			_bubbles.Play ();
		}
		
		UpdateAccelerometer ();
			
		if (GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			HandleIdle ();
		} else if (GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("diverSwimTree")) {
			HandleSwim ();
		}
	}


	void HandleSwim(){	
		if ((_surface.GetActualPosition (transform.position) + Vector3.down * 0.3f).y < transform.position.y) {
			if (_maxDepth == 0) {
				return;
			}
			GetComponent<Animator> ().ResetTrigger ("Dive");
			GetComponent<Animator> ().SetTrigger ("Surface");
			_InitialDelay = Time.time;

			NotifyManager (_treasure ? taskManager.action.treasureDiveSuccess : taskManager.action.diveSuccess);
			_treasure = false;
			_state = state.Floating;
			_maxDepth = 0;
			return;
		}

		_maxDepth = Mathf.Max (_maxDepth, -transform.position.y);

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
			NotifyManager (taskManager.action.screenTurned);
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
			NotifyManager(taskManager.action.diveStarted);
			_title.SetState(title.state.ToBeHidden);
			GetComponent<Animator> ().SetTrigger ("Dive");
			_state = state.Diving;
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


	public float _maxDepth = 0;

	void NotifyManager(taskManager.action action){
		Camera.main.GetComponent<taskManager>().Notify(action, _maxDepth);
		Camera.main.GetComponent<Oxigen>().Notify(action, _maxDepth);
	}

	public bool _treasure = false;
	public void GetTreasure(){
		NotifyManager (taskManager.action.treasureFound);
		_treasure = true;
	}

}
