using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Diver : BaseManager<Diver> {
	public ParticleSystem _bubbles;
	public Vector3 _defaultPosition;
	public Quaternion _defaultRotation;
	public float _obsoleteInitialDelay;

	float _directionAngleInterpolated = 0.5f;

	AnimationClip _currentClip = null;


	bool _twistedDiver = false;

	public bool IsTwist(){
		return _twistedDiver;
	}
	public void Twist(){
		_twistedDiver = !_twistedDiver;
	}

	void PrepareCallbacks(){
		RuntimeAnimatorController controller_ = GetComponent<Animator>().runtimeAnimatorController;
		AnimationClip[] clips_ = controller_.animationClips;
		
		for ( int i = 0; i < clips_.Length; ++i ){
			List<AnimationEvent> eventList_ = new List<AnimationEvent>(clips_[i].events);
			AnimationEvent match_ = eventList_.Find((e) => e.functionName == "OnClipEnded");
			if ( match_ != null ){
				Debug.LogWarning(this.ToString() + "." + MethodBase.GetCurrentMethod() + ": OnClipEnded callback already exists for " + clips_[i].name); 
			} else {
				AnimationEvent evt_ = new AnimationEvent();
				evt_.functionName = "OnClipStarted";
				evt_.objectReferenceParameter = clips_[i];
				evt_.time = 0;
				clips_[i].AddEvent(evt_);
			}
		}
	}

	public void OnClipStarted(AnimationClip clip){
		_currentClip = clip;
		if ( IsCurrentClip(clips.diverIdleUp) ){
			GetComponent<Water.SurfaceSnap>().SetSnapAngle(true);
		}
		// Debug.Log("[" +Time.time + "] " + this.ToString() + "." + MethodBase.GetCurrentMethod() + " called by " + clip.ToString());
	}

	public bool IsCurrentClip(clips clip){
		return _currentClip != null && _currentClip.name == clip.ToString();
	}

	public AnimationClip GetCurrentClip(){
		return _currentClip;
	}
	
	public enum clips{
		diverSubmerge,
		diverIdleUp,
		diverHover,
		diverIdleTwist
	}

	public enum state{
		None,
		Diving,
		Surface,
		Flip,
		Hovering,
		Collision,
		SurfaceSwim,
		Dying,
	};

	public Water.Surface _surface;
	// float _swimAngle = 0.5f;
	// Vector3 _defaultVector = Vector3.one;
	// int SurfaceState;
	// bool _keyUpDown = false;
	// bool _keyDownDown = false;
	// float _accelThreshold = 0.05f;
	
	state _state = state.None;
	public Rigidbody2D _rigidBody;

	float _currentCollisionDot = -1;

	public Vector3 GetPosition(){
		return _rigidBody.transform.position;
	}

	public void Restart(){
		GetComponent<Animator> ().Rebind();
		transform.position = _defaultPosition;
		transform.rotation = _defaultRotation;
		RenderCamera.get.GetComponent<PositionLink>().Reset ();
		SetState(state.Surface);

		DirectionMarker.get.Reset();
		_directionAngleInterpolated = 0.5f;
		_bubbles.Stop ();
		_obsoleteInitialDelay = Time.time;
		_maxDepth = 0;
		_currentCollisionDot = -1;
		// _swimAngle = 0.5f;
		GetComponent<Animator> ().speed = 1.0f;
		OxygenManager.get.Reset();
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
		PrepareCallbacks();
	}

	void Update () {
		
		switch (_state) {
		case state.Dying:
			return;
		case state.None:
			_defaultPosition = transform.position;
			_defaultRotation = transform.rotation;
			NotifyManager (TaskManager.action.idle);
			Restart();
			DirectionMarker.get.Reset ();
			break;
		case state.Diving:
			UpdateDive ();
			break;
		}
		
		if ( TaskManager.get._title.GetState() == title.state.Hidden && GetState() == state.Surface && _obsoleteInitialDelay != -1 && Time.time - _obsoleteInitialDelay > 10.0f ) {
			NotifyManager (TaskManager.action.idle);
			_obsoleteInitialDelay = -1;
		}

		if (transform.position.y > -5.0f && !_bubbles.isStopped) {
			_bubbles.Stop();
		} else if (transform.position.y < -5.0f & _bubbles.isStopped) {
			_bubbles.Play ();
		}
					
	}
	public bool IsSteepCollision(){
		return  _currentCollisionDot > 0.6f;
	}

	public void OnCollisionStay2D(Collision2D source){
		_rigidBody.transform.parent = null;
		this.transform.position = _rigidBody.transform.position;
		_rigidBody.transform.parent = transform;

		_currentCollisionDot = DirectionMarker.get.GetCollisionUIDot(source);
		if ( _state == state.SurfaceSwim && !IsCurrentClip(clips.diverIdleTwist) && IsSteepCollision()){
			SetState(state.Surface );	
		}
		else if ( _state != state.Collision && _state != state.Surface && IsSteepCollision() ){
			SetState(state.Collision);	
		}		
	}

	public void OnCollisionExit2D(Collision2D source){
		_currentCollisionDot = -1;
	}

	void TwistTest(string negativeTrigger, string positiveTrigger, string secondaryPositive ){
		float dot_ = DirectionMarker.get.GetTangentDot();
		if ( dot_ < -0.2f ){
			Twist();
			GetComponent<Animator> ().SetTrigger ( positiveTrigger );
			GetComponent<Animator> ().SetTrigger ( secondaryPositive );
		} else {
			GetComponent<Animator> ().SetTrigger (negativeTrigger);
		}
	}

	public void SetState(state target){
		bool success = false;

		if ( _state != target ){
			switch ( target ){
				case state.Dying:
					TaskManager.get.Reset ();
					GetComponent<Animator> ().speed = 0.15f;
					success = true;
				break;
				case state.Surface:
					if ( _state == state.None || _state == state.Diving || _state == state.SurfaceSwim ){
						
						if (  _state == state.SurfaceSwim ){
							GetComponent<Animator> ().SetTrigger ("SurfaceCollision");
						} else {
							GetComponent<Water.SurfaceSnap>().SetSnapAngle(true);		
							if ( !IsCurrentClip(clips.diverIdleUp) && _state != state.None ){
								GetComponent<Animator> ().SetTrigger ("Surface");
							} 
						}
						GetComponent<Water.SurfaceSnap>().SetActive(true);
						
						// 
						_defaultPosition = transform.position;
						
						RenderCamera.get.GetComponent<PositionLink>().SetActive(false);
						RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(3.9f, true);
						RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.05f;
						
						success = true;
					}
				break;
				case state.SurfaceSwim:
					GetComponent<Water.SurfaceSnap>().SetSnapAngle(false);

					TwistTest( "SurfaceSwim", "SurfaceTwist", "TwistSwim");
					
					RenderCamera.get.GetComponent<PositionLink>().SetActive(true);
					RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(0);
					RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.5f;
					success = true;
				break;
				case state.Flip:
					if ( _state == state.Diving || _state == state.Hovering || _state == state.Collision ){
						GetComponent<Animator>().SetTrigger("Flip");
						success = true;
					}
					break;
				case state.Diving:
					if ( _state == state.Surface && IsCurrentClip(clips.diverIdleUp) ){
						DirectionMarker.get._directionHolder.SetActive(true);
						RenderCamera.get.GetComponent<PositionLink>().SetActive(true);
						RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(0);
						RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.5f;
						NotifyManager (TaskManager.action.diveStarted);
						TaskManager.get._title.SetState(title.state.ToBeHidden);
						
						GetComponent<Water.SurfaceSnap>().SetActive(false);
						GameObject.Destroy(GetComponent<Rigidbody2D>());

						TwistTest("Dive", "SurfaceTwist", "TwistDive");
						
						OxygenManager.get.DismissRewawrd ();
						
						success = true;
					} else if ( IsCurrentClip(clips.diverHover) ){
						GetComponent<Animator> ().SetTrigger ("Unhover");
						success = true;
					} else if ( _state == state.Flip ){
						success = true;
					}
				break;
				case state.Hovering:
					GetComponent<Animator> ().SetTrigger ("Hover");
					success = true;
				break;
				case state.Collision:
					GetComponent<Animator> ().SetTrigger ("Collision");
					success = true;
				break;
				default:
					Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().Name + ": Undefined parameter" );
				break;
			}
		}

		if ( success ){
			_state = target;
		}
		else {
			Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().Name + ": transition " + _state + " → " + target + " failed.");
		}
	}

	public void DoSwim(){

		switch ( _state ){
			case state.Surface:
			 	SetState(DirectionMarker.get.IsCursorAboveGround () ? state.SurfaceSwim : state.Diving );
				break;
			case state.Hovering:
			case state.Collision:
				if ( !IsSteepCollision () ){
					SetState(FlipTest() ? state.Flip : state.Diving);
				}
				break;
		}

		if (_state == state.Surface) {
			SetState(state.Diving);
			return;
		}
	}

	public void DoHover(){
		switch (_state ){
			case state.Diving:
				SetState(state.Hovering);
			break;	
		}
	}


	bool IsAboveSurface(){
		return (_surface.GetSurfaceZ (transform.position) + Vector3.down * 0.3f).y < transform.position.y;
	}

	void UpdateDive(){	
		if (IsAboveSurface() && !IsCurrentClip(clips.diverSubmerge) && !IsCurrentClip(clips.diverIdleTwist)) {
			SetState(state.Surface);
			
			DirectionMarker.get.Reset ();
			if (_maxDepth != 0) {
				_obsoleteInitialDelay = Time.time;

				NotifyManager (_treasure ? TaskManager.action.treasureDiveSuccess : TaskManager.action.diveSuccess);
				_treasure = false;
				
				_maxDepth = 0;	
			}
			return;
		}

		_maxDepth = Mathf.Max (_maxDepth, -transform.position.y);

		HandleTouch ();
	}

	bool FlipTest(){
		return DirectionMarker.get.GetDirectionDot() < -0.75f;
	}

	void HandleTouch(){

		if ( FlipTest() && 
		!IsCurrentClip(clips.diverSubmerge) && 
		!IsCurrentClip(clips.diverIdleTwist) &&
		!IsCurrentClip(clips.diverIdleUp) ){
			SetState(state.Flip);
			return;	
		} 

		float tangentDot_ = (DirectionMarker.get.GetTangentDot() + 1) * 0.5f;

		Interpolate(ref _directionAngleInterpolated, tangentDot_, 0.1f);

		GetComponent<Animator> ().SetFloat ("SwimDirection", _directionAngleInterpolated );

		return;
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
