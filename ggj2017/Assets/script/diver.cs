//#define TOUCH_CONTROL
#define DIRECTION_CONTROL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diver : MonoBehaviour {

	public cameraLink _cameraLink;
	public audioManager _audioManager;
	public taskManager _taskManager;
	public oxygenManager _oxigenManager;
	public ParticleSystem _bubbles;
	public Vector3 _defaultPosition;
	public Quaternion _defaultRotation;
	public float _InitialDelay;

	float _directionAngleInterpolated = 0.5f;

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
		Hovering,
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
	float _accelThreshold = 0.05f;
	state _state = state.None;

	public void Restart(){
		GetComponent<Animator> ().SetTrigger ("Restart");
		GetComponent<Animator> ().ResetTrigger ("Surface");
		GetComponent<Animator> ().ResetTrigger ("Dive");
		transform.position = _defaultPosition;
		transform.rotation = _defaultRotation;
		_cameraLink.Reset ();
		_state = state.Floating;
#if DIRECTION_CONTROL
		directionMarker.instance.Reset();
		_directionAngleInterpolated = 0.5f;
#endif
		_bubbles.Stop ();
		_InitialDelay = Time.time;
		_maxDepth = 0;
		_swimAngle = 0.5f;
		GetComponent<Animator> ().speed = 1.0f;
		_audioManager.Reset ();
	}

	public void Death () {
		if ( _state == state.Dying ){
			return;
		}
		_taskManager.Reset ();
		GetComponent<Animator> ().speed = 0.15f;
		_state = state.Dying;
	}

	public state GetState(){
		return _state;
	}

	public float GetCurrentDepth(){
		return transform.position.y;
	}

	public float GetMaxDepth () {
		return _maxDepth;
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
#if DIRECTION_CONTROL
			directionMarker.instance.Reset ();
#endif
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
					
		if (GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Surface")) {
			HandleIdle ();
		} else if (GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("diverSwimTree")) {
			HandleSwim ();
		}
	}

	public void Hover(bool enable){
		if (_state == state.Diving && enable) {
			GetComponent<Animator> ().ResetTrigger ("Unhover");
			GetComponent<Animator> ().SetTrigger ("Hover");
			_state = state.Hovering;
		} else if (_state == state.Hovering && !enable) {
			_state = state.Diving;
			GetComponent<Animator> ().ResetTrigger ("Hover");
			GetComponent<Animator> ().SetTrigger ("Unhover");
		}
	}

	void HandleSwim(){	
		if ((_surface.GetActualPosition (transform.position) + Vector3.down * 0.3f).y < transform.position.y) {
			if (_maxDepth == 0) {
				return;
			}
			GetComponent<Animator> ().ResetTrigger ("Dive");
			GetComponent<Animator> ().SetTrigger ("Surface");
			GetComponent<Animator> ().ResetTrigger ("Hover");
			_InitialDelay = Time.time;

			NotifyManager (_treasure ? taskManager.action.treasureDiveSuccess : taskManager.action.diveSuccess);
			_treasure = false;
			_state = state.Floating;
#if DIRECTION_CONTROL
			directionMarker.instance.Reset ();
#endif
			_maxDepth = 0;
			return;
		}

		_maxDepth = Mathf.Max (_maxDepth, -transform.position.y);

		HandleTouch ();
	}

	void HandleTouch(){
#if DIRECTION_CONTROL

		float directionAngle_ = (directionMarker.instance.GetDiffAngle() + 1) * 0.5f;

		Interpolate(ref _directionAngleInterpolated, directionAngle_, 0.02f);

		GetComponent<Animator> ().SetFloat ("SwimDirection", _directionAngleInterpolated );

		return;

#elif UNITY_EDITOR
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
#elif TOUCH_CONTROL
		_keyDownDown = false;
		_keyUpDown = false;
		for (int i = 0; i < Input.touchCount; ++i) {
		if (!_keyDownDown && Camera.main.ScreenToViewportPoint (Input.GetTouch (i).position).x > 0.5f) {
		_keyDownDown = true;
		} else if (!_keyUpDown && Camera.main.ScreenToViewportPoint (Input.GetTouch (i).position).x < 0.5f) {
		_keyUpDown = true;
		}
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

#if TOUCH_CONTROL || (UNITY_EDITOR && !DIRECTION_CONTROL)
		if (_keyUpDown) {
			_swimAngle -= Time.deltaTime * 2.0f;
		}

		if (_keyDownDown) {
			_swimAngle += Time.deltaTime * 2.0f;
		}

		if (!(_keyDownDown || _keyUpDown)) {

			_swimAngle += (0.5f - _swimAngle) * Time.deltaTime * 3.0f; 
		} else {
			NotifyManager (taskManager.action.screenTurned);
			_swimAngle = Mathf.Clamp (_swimAngle, 0, 1);
		}

		GetComponent<Animator> ().SetFloat ("SwimDirection", _swimAngle);
#endif 

#if !DIRECTION_CONTROL
		GetComponent<Animator> ().SetFloat ("SwimDirection", -Input.acceleration.x + 0.5f);
#endif
	}

	public void InitiateDive(){
		if (_state == state.Diving) {
			return;
		}

		NotifyManager(taskManager.action.diveStarted);
		_title.SetState(title.state.ToBeHidden);
		GetComponent<Animator> ().SetTrigger ("Dive");
		_state = state.Diving;
		_oxigenManager.DismissRewawrd();
	}

	void HandleIdle(){
#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.S)) {
#elif !DIRECTION_CONTROL
		if ( Input.touchCount > 0 ){
#else 
		if ( false ){
#endif 
			InitiateDive();
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

	public void NotifyManager(taskManager.action action){
		_taskManager.Notify(action, _maxDepth);
		_oxigenManager._oxygen.Notify(action, _maxDepth);
		_audioManager.Notify(action, _maxDepth);
	}

	public bool _treasure = false;
	public void GetTreasure(){
		if ( _treasure ){
			return;
		}
				
		NotifyManager (taskManager.action.treasureFound);
		_treasure = true;
//		Camera.main.GetComponent<audioManager> ().Notify (taskManager.action.treasureFound);
	}

	void Interpolate(ref float source, float target, float speed){
		if (target == source) {
			return;
		}

		float delta_ = target - source;

		if (delta_ > 0) {
			if (target < source + speed) {
				source = target;
			} else {
				source += speed;
			}
		} else {
			if (target > source - speed) {
				source = target;
			} else {	
				source -= speed;
			}
		}

		return;
	}
}
