//#define TOUCH_CONTROL
#define DIRECTION_CONTROL

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Diver : BaseManager<Diver> {
	public ParticleSystem _bubbles;
	public Vector3 _defaultPosition;
	public Quaternion _defaultRotation;
	public float _InitialDelay;

	float _directionAngleInterpolated = 0.5f;

	public enum state{
		None,
		Diving,
		Surface,
		Hovering,
		SurfaceLeft,
		Dying,

	};

	public Surface _surface;
	float _swimAngle = 0.5f;
	// Vector3 _defaultVector = Vector3.one;
	// int SurfaceState;
	// bool _keyUpDown = false;
	// bool _keyDownDown = false;
	// float _accelThreshold = 0.05f;
	state _state = state.None;

	public void Restart(){
		GetComponent<Animator> ().SetTrigger ("Restart");
		GetComponent<Animator> ().ResetTrigger ("Surface");
		GetComponent<Animator> ().ResetTrigger ("Dive");
		transform.position = _defaultPosition;
		transform.rotation = _defaultRotation;
		RenderCamera.get.GetComponent<CameraLink>().Reset ();
		SetState(state.Surface);
		
#if DIRECTION_CONTROL
		DirectionMarker.instance.Reset();
		_directionAngleInterpolated = 0.5f;
#endif
		_bubbles.Stop ();
		_InitialDelay = Time.time;
		_maxDepth = 0;
		_swimAngle = 0.5f;
		GetComponent<Animator> ().speed = 1.0f;
		AudioManager.get.Reset ();
	}

	public void Death () {
		if ( _state == state.Dying ){
			return;
		}
		
		SetState(state.Dying);
		
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
	}

	void Update () {
		
		switch (_state) {
		case state.Dying:
			return;
		case state.None:
			NotifyManager (TaskManager.action.idle);
			SetState(state.Surface);
#if DIRECTION_CONTROL
			DirectionMarker.instance.Reset ();
#endif
			break;
		case state.Surface:
			RenderCamera.get.GetComponent<CameraLink>()._hardness = 0.05f;
			HandleIdle ();
			break;
		case state.Diving:
			RenderCamera.get.GetComponent<CameraLink>()._hardness = 0.5f;
			HandleSwim ();
			break;
		}
		
		if ( TaskManager.get._title.GetState() == title.state.Hidden && GetState() == state.Surface && _InitialDelay != -1 && Time.time - _InitialDelay > 10.0f ) {
			NotifyManager (TaskManager.action.idle);
			_InitialDelay = -1;
		}

		if (transform.position.y > -5.0f && !_bubbles.isStopped) {
			_bubbles.Stop();
		} else if (transform.position.y < -5.0f & _bubbles.isStopped) {
			_bubbles.Play ();
		}
					
	}

	void SetState(state target){
		bool success = false;
		switch ( target ){
			case state.Dying:
				TaskManager.get.Reset ();
				GetComponent<Animator> ().speed = 0.15f;
				success = true;
			break;
			case state.Surface:
				GetComponent<SurfaceSnap>().enabled = true;
				success = true;
			break;
			case state.SurfaceLeft:
				GetComponent<Animator> ().SetTrigger ("SurfaceLeft");
				success = true;
			break;
			case state.Diving:
				if ( _state == state.Surface ){
					NotifyManager (TaskManager.action.diveStarted);
					TaskManager.get._title.SetState(title.state.ToBeHidden);
					GetComponent<Animator> ().SetTrigger ("Dive");
					OxygenManager.get.DismissRewawrd ();
					GetComponent<SurfaceSnap>().enabled = false;
					success = true;
				} else if ( _state == state.Hovering ){
					GetComponent<Animator> ().ResetTrigger ("Hover");
					GetComponent<Animator> ().SetTrigger ("Unhover");
					success = true;
				}
			break;
			case state.Hovering:
				GetComponent<Animator> ().ResetTrigger ("Unhover");
				GetComponent<Animator> ().SetTrigger ("Hover");
				success = true;
			break;
			default:
				Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().Name + ": Undefined parameter" );
			break;
		}

		if ( success ){
			_state = target;
		}
		else {
			Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().Name + ": transition " + _state + " → " + target + " failed.");
		}
	}

	public void Swim(){

		if (_state == state.Surface && DirectionMarker.instance.IsAboveGround ()) {
			SetState(state.SurfaceLeft);	
			return;
		}

		if (_state == state.Surface) {
			SetState(state.Diving);
			return;
		}
	}

	public void Hover(bool enable){
		if (_state == state.SurfaceLeft && enable) {
			GetComponent<Animator> ().SetTrigger ("Hover");
			SetState(state.Surface);
		}
		else if (_state == state.Diving && enable) {
			
			SetState(state.Hovering);
			
		} else if (_state == state.Hovering && !enable) {
			SetState(state.Diving);
		}
	}

	void HandleSwim(){	
		if ((_surface.GetSurfaceZ (transform.position) + Vector3.down * 0.3f).y < transform.position.y) {
			if (_maxDepth == 0) {
				return;
			}
			GetComponent<Animator> ().ResetTrigger ("Dive");
			GetComponent<Animator> ().SetTrigger ("Surface");
			GetComponent<Animator> ().ResetTrigger ("Hover");
			_InitialDelay = Time.time;

			NotifyManager (_treasure ? TaskManager.action.treasureDiveSuccess : TaskManager.action.diveSuccess);
			_treasure = false;
			SetState(state.Surface);
#if DIRECTION_CONTROL
			DirectionMarker.instance.Reset ();
#endif
			_maxDepth = 0;
			return;
		}

		_maxDepth = Mathf.Max (_maxDepth, -transform.position.y);

		HandleTouch ();
	}

	void HandleTouch(){
#if DIRECTION_CONTROL

		float directionAngle_ = (DirectionMarker.instance.GetDiffAngle() + 1) * 0.5f;

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
		if (!_keyDownDown && MainCamera.get.GetComponent<Camera>().ScreenToViewportPoint (Input.GetTouch (i).position).x > 0.5f) {
		_keyDownDown = true;
		} else if (!_keyUpDown && MainCamera.get.GetComponent<Camera>().ScreenToViewportPoint (Input.GetTouch (i).position).x < 0.5f) {
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



	void HandleIdle(){
#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.S)) {
#elif !DIRECTION_CONTROL
		if ( Input.touchCount > 0 ){
#else 
		if ( false ){
#endif 
			GetComponent<SurfaceSnap>().enabled = false;
			Swim();
		}

	/*	if (_defaultVector == Vector3.one) {
			_defaultVector = transform.eulerAngles;
		}

		if (transform.eulerAngles != _defaultVector) {
			transform.eulerAngles = _defaultVector;
		}

		Vector3 posBak_ = transform.position;
		posBak_.y = (_surface.GetSurfaceZ(transform.position) + Vector3.down * 0.3f).y;
		transform.position += (posBak_ - transform.position) * Time.deltaTime * 10.0f;*/
	}

	public float _maxDepth = 0;

	public void NotifyManager(TaskManager.action action){
		TaskManager.get.Notify(action, _maxDepth);
		OxygenManager.get.Notify(action, _maxDepth);
		AudioManager.get.Notify(action, _maxDepth);
	}

	public bool _treasure = false;
	public void GetTreasure(){
		if ( _treasure ){
			return;
		}
				
		NotifyManager (TaskManager.action.treasureFound);
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
