using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Diver : BaseManager<Diver> {
	public ParticleSystem _bubbles;
	public Vector3 _defaultPosition;
	// public Quaternion _defaultRotation;
	public float _surfaceIdleSnapAngle = 270f;
	public float _surfaceSwimmingAngle = 113.6f;
	float _directionAngleInterpolated = 0.5f;

	float _surfaceIgnoreThresholdDegrees = 160;
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
		// Debug.Log("[" +Time.time + "]" + this.ToString() + "." + MethodBase.GetCurrentMethod() + ": " + clip.name);
		_currentClip = clip;
		if ( IsCurrentClip(clips.diverIdleUp) ){
			GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(true);
			GetComponent<Water.SurfaceSnap>().SetSnapAngle(_surfaceIdleSnapAngle);
		} else if ( IsCurrentClip(clips.diverSurfaceSwim) ){
			GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(true);
			GetComponent<Water.SurfaceSnap>().SetSnapAngle(_surfaceSwimmingAngle);
		} else if ( IsCurrentClip(clips.diverIdleTwist)) {
			 GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(true);
			 GetComponent<Water.SurfaceSnap>().SetSnapAngle(_surfaceSwimmingAngle);
		}
	}

	public bool IsCurrentClip(clips clip){
		return _currentClip != null && _currentClip.name == clip.ToString();
	}

	public AnimationClip GetCurrentClip(){
		return _currentClip;
	}
	
	public enum clips{
		diverIdleUp,
		diverHover,
		diverIdleTwist,
		diverSurfaceSwim,
		diverSurfaceSwimTwist,
		diverSwim,
		diverSwimDown,
		diverSwimUp,
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
	
	state _state = state.None;
	public Rigidbody2D _rigidBody;

	float _currentCollisionDot = -1;

	public Vector3 GetPosition(){
		return _rigidBody.transform.position;
	}

	public void Restart(){
		GetComponent<Animator> ().Rebind();
		transform.position = _defaultPosition;
		// transform.rotation = _defaultRotation;
		RenderCamera.get.GetComponent<PositionLink>().Reset ();
		GetComponent<Water.SurfaceSnap>().Reset();
		SetState(state.Surface);

		DirectionMarker.get.Reset();
		_directionAngleInterpolated = 0.5f;
		_bubbles.Stop ();
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
			// _defaultRotation = transform.rotation;
			NotifyManager (TaskManager.action.idle);
			Restart();
			DirectionMarker.get.Reset ();
			break;
		case state.Hovering:
			UpdateHover();
			break;
		case state.Diving:
			if ( IsCurrentClip(clips.diverSwim) ||
				IsCurrentClip(clips.diverSwimDown) ||
				IsCurrentClip(clips.diverSwimUp)){
				UpdateDive ();
			}
			break;
		}
		
		if (transform.position.y > -5.0f && !_bubbles.isStopped) {
			_bubbles.Stop();
		} else if (transform.position.y < -5.0f & _bubbles.isStopped) {
			_bubbles.Play ();
		}

		Restrict2D();				
	}

	void MatchAngle(float value){
		Animator anim_ = GetComponent<Animator>();
		Quaternion targetRotation_ = Quaternion.Euler(0, IsTwist() ? 180 : 0, value);
		anim_.MatchTarget(anim_.targetPosition, targetRotation_, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0f, 1f);
		
	}

	void Restrict2D(){
		 if ( IsCurrentClip(clips.diverIdleTwist)){
			MatchAngle(_surfaceIdleSnapAngle);
		 } else if ( IsCurrentClip(clips.diverSurfaceSwimTwist) ){
			MatchAngle(_surfaceSwimmingAngle);
		 }
		 Vector3 pos_ = transform.position;
		 pos_.z = 0;
		 transform.position = pos_;
	}

	public bool IsSteepCollision(){
		return  _currentCollisionDot > 0.6f;
	}

	public void OnCollisionStay2D(Collision2D source){
		_rigidBody.transform.parent = null;
		this.transform.position = _rigidBody.transform.position;
		_rigidBody.transform.parent = transform;

		_currentCollisionDot = DirectionMarker.get.GetCollisionUIDot(source);
		if ( _state == state.SurfaceSwim && !IsCurrentClip(clips.diverIdleTwist) /*&& IsSteepCollision()*/){

			SetState(state.Surface );	
		}
		else if ( _state != state.Collision && _state != state.Surface && IsSteepCollision() ){
			SetState(state.Collision);	
		}		
	}

	public void OnCollisionExit2D(Collision2D source){
		_currentCollisionDot = -1;
	}

	bool TwistTest(){
		float dot_ = DirectionMarker.get.GetGlobalUIDot();
		return dot_ < -0.2f;
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
					if ( _state == state.None || _state == state.Diving || _state == state.SurfaceSwim || _state == state.Hovering ){
						
						if (  _state == state.SurfaceSwim ){
							GetComponent<Animator> ().SetTrigger ("SurfaceStop");
						} else {	
							if ( !IsCurrentClip(clips.diverIdleUp) && _state != state.None ){
								GetComponent<Animator> ().SetTrigger ("Surface");
							} 
						}

						GetComponent<Water.SurfaceSnap>().SetActive(true);
						DirectionMarker.get._directionHolder.SetActive(false);
						
						_defaultPosition = new Vector3 (transform.position.x, transform.position.y, 0);
						transform.position = _defaultPosition;
						
						RenderCamera.get.GetComponent<PositionLink>().SetActive(false);
						RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(3.9f, true);
						RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.05f;
						
						success = true;
					}
				break;
				case state.SurfaceSwim:				
					GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(false);
					GetComponent<Water.SurfaceSnap>().SetActive(true);
					DirectionMarker.get._directionHolder.SetActive(true);
					
					if ( TwistTest() ){
						Twist();
						GetComponent<Animator>().SetTrigger("SurfaceTwist");
					}
					GetComponent<Animator>().SetTrigger("SurfaceSwim");					
					
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
					if ((_state == state.Surface && (IsCurrentClip(clips.diverIdleUp) || IsCurrentClip(clips.diverIdleTwist))) || 
					   	(_state == state.SurfaceSwim) ){
				
						GetComponent<Water.SurfaceSnap>().SetActive(false);
						DirectionMarker.get._directionHolder.SetActive(true);
						RenderCamera.get.GetComponent<PositionLink>().SetActive(true);
						RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(0);
						RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.5f;
						NotifyManager (TaskManager.action.diveStarted);
						TaskManager.get._title.SetState(title.state.ToBeHidden);
						
						if ( TwistTest() ){
							Twist();
							GetComponent<Animator>().SetTrigger("SurfaceTwist");
						}
						GetComponent<Animator>().SetTrigger("Dive");
						
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
					GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(false);
					GetComponent<Animator> ().SetTrigger ("Collision");
					success = true;
				break;
				default:
					Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().Name + ": Undefined parameter" );
				break;
			}
		}

		if ( success ){
			Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": " + _state + " → " + target);
			_state = target;
		}
		else {
			Debug.LogWarning(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": transition " + _state + " → " + target + " FAILED (clip: " + _currentClip.name + ")");
		}
	}

	public void DoSwim(){

		switch ( _state ){
			case state.Surface:
				if ( IsSteepCollision() ){
					break;
				}
				if ( DirectionMarker.get.IsCursorAboveGround() ){
					if ( Mathf.Abs(DirectionMarker.get.GetUIAngle()) <= _surfaceIgnoreThresholdDegrees ){
						SetState(state.SurfaceSwim);
					} 
				}else {
					SetState(state.Diving );
				}
			 	
				break;
			case state.Hovering:
			case state.Collision:
				if ( !IsSteepCollision () ){
					SetState(FlipTest() ? state.Flip : state.Diving);
				}
				break;
			case state.SurfaceSwim:
				if ( DirectionMarker.get.IsCursorAboveGround() ){
					if ( Mathf.Abs(DirectionMarker.get.GetUIAngle()) > _surfaceIgnoreThresholdDegrees ){
						SetState(state.Surface);
					}else if ( 	(DirectionMarker.get.GetUIAngle() > 0 == IsTwist()) ){
						Twist();
						GetComponent<Animator>().SetTrigger("SurfaceTwist");
					}
				} else {
					SetState(state.Diving);
				}
				break;
		}
	}

	public void DoHover(){
		switch (_state ){
			case state.SurfaceSwim:
				SetState(state.Surface);
			break;
			case state.Diving:
				SetState(state.Hovering);
			break;	
		}
	}


	bool IsAboveSurface(){
		return (_surface.GetSurfaceZ (transform.position) + Vector3.down * 0.5f).y < transform.position.y;
	}

	bool TestSurfaceSwimAngle(float reference){
		return (reference < _surfaceIgnoreThresholdDegrees && reference > 90 && !IsTwist()) || (reference > -_surfaceIgnoreThresholdDegrees && reference < -90 && IsTwist());
	}

	void TestTreasure(){
		if (_maxDepth != 0) {
			NotifyManager (_treasure ? TaskManager.action.treasureDiveSuccess : TaskManager.action.diveSuccess);
			_treasure = false;
	
			_maxDepth = 0;	
		}
	}

	void UpdateHover(){
		if ( IsAboveSurface() ){
			SetState(state.Surface);
			DirectionMarker.get.Reset ();
			TestTreasure();
		}
	}

	void UpdateDive(){
		if (IsAboveSurface() ) {
			if ( TestSurfaceSwimAngle(DirectionMarker.get.GetDiverAngle()) ){			
				SetState(state.SurfaceSwim);
			} else {
				SetState(state.Surface);
				DirectionMarker.get.Reset ();
			}

			TestTreasure();
			return;

		} else {
			float tangentDot_ = (DirectionMarker.get.GetTangentUIDot() + 1) * 0.5f;
			Water.SurfaceSnap.Interpolate(ref _directionAngleInterpolated, tangentDot_, 3f);
			GetComponent<Animator> ().SetFloat ("SwimDirection", _directionAngleInterpolated );

			_maxDepth = Mathf.Max (_maxDepth, -transform.position.y);
		}

		if ( FlipTest() ){
			SetState(state.Flip);
			return;	
		} 
	}

	bool FlipTest(){
		return DirectionMarker.get.GetDirectionUIDot() < -0.75f;
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

	
}
