using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Diver : BaseManager<Diver> {
	public enum state{
		None,
		Diving,
		Surface,
		Hovering,
		SurfaceStart,
		SurfaceSwim,
		SurfaceTwist,
		SurfaceSwimTwist,
		SurfaceSwimLazyStop,
		Dying,
		Sit,
		Flip,
	};

	public Water.Surface _surface;
	public DiverPhysics _physics;
	public ParticleSystem _bubbles;
	public Vector3 _defaultPosition;
	public float _surfaceIdleSnapAngle = 270f;
	public float _surfaceSwimmingAngle = 113.6f;
	public float _lazyThreshold = 5.0f;

	float _lazyTimestamp = 0;

	state _state = state.None;
	bool _stateChanging = false;
	float _directionAngleInterpolated = 0.5f;
	float _surfaceIgnoreThresholdDegrees = 160;
	public bool _twistedDiver = false;

	public bool IsTwist(){
		return _twistedDiver;
	}
	public void Twist(){
		_twistedDiver = !_twistedDiver;
	}

	public Vector3 GetPosition(){
		return _physics.transform.position;
	}

	public void InCollision(){
		if ( _state == state.SurfaceSwim || _state == state.SurfaceSwimLazyStop ){
			TryState(state.Surface);	
		}
		else if ( _state == state.Diving && _physics.IsSteepCollision() ){
			TryState(state.Hovering);	
		}		
	}

	public void Restart(){
		GetComponent<Animator> ().Rebind();
		ApplyDefaultPosition();
		MainCamera.get.gameObject.SetActive(true);
		GetComponent<Water.SurfaceSnap>().SetSnapAngle(_surfaceIdleSnapAngle);
		GetComponent<Water.SurfaceSnap>().Reset();
		NotifyManager (TaskManager.action.idle);
		DirectionMarker.get.Reset();
		_directionAngleInterpolated = 0.5f;
		_maxDepth = 0;
		_physics.Reset();
		GetComponent<Animator> ().speed = 1.0f;
		OxygenManager.get.Reset();
		AudioManager.get.Reset ();

		RenderCamera.get.GetComponent<PositionLink>().Reset ();
	}

	public void Death () {
		if ( _state == state.Dying ){
			return;
		}
		_bubbles.Stop();
		TryState(state.Dying);	
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
		_bubbles.Play();
		StoreDefaultPosition();
		Restart();
	}
	
	void Update() {		
		switch (_state) {
		case state.Dying:
			return;
		case state.SurfaceSwim:
			UpdateSurfaceSwim();
			break;
		case state.Diving:
			UpdateDive();
			break;
		case state.SurfaceSwimTwist:
			UpdateSurfaceSwimTwist();
			break;
		case state.SurfaceTwist:
			UpdateSurfaceTwist();
			break;
		}
	
		Restrict2D();				
	}

	void Restrict2D(){
		 Vector3 pos_ = transform.position;
		 pos_.z = 0;
		 transform.position = pos_;
	}


	bool TwistTest(){
		float dot_ = DirectionMarker.get.GetGlobalUIDot();
		return dot_ < -0.2f;
	}

	public bool SetState(state target){
		if ( _state == target ){
			return false;
		}

		Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": " + _state + " → " + target);
		_state = target;
		_stateChanging = false;
		return true;
	}

	public void StoreDefaultPosition(){
		_defaultPosition = new Vector3 (transform.position.x, transform.position.y, 0);
	}

	public void ApplyDefaultPosition(){
		transform.position = _defaultPosition;
	}

	public void TryState(state target){
		bool success = false;
		string successTrigger = "";
		
		if ( _stateChanging ){
			return;
		}

		if ( _state != target ){
			switch ( target ){
				case state.Dying:
					TaskManager.get.Reset ();
					GetComponent<Animator> ().speed = 0.15f;
					success = true;
				break;

				case state.Sit:
					Twist();
					successTrigger = "ClimbSit";
				break;

				case state.Surface:
					DirectionMarker.get._directionArrow.SetActive(false);
					successTrigger = "Surface";	
				break;

				case state.SurfaceSwimTwist:
					Twist();
					successTrigger = "SurfaceTwist";
				break;

				case state.SurfaceTwist:
					Twist();
					successTrigger = "SurfaceTwist";
				break;

				case state.SurfaceSwim:	
					DirectionMarker.get._directionArrow.SetActive(true);
					Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(false);
					Diver.get.GetComponent<Water.SurfaceSnap>().SetActive(true);
					RenderCamera.get.GetComponent<PositionLink>().SetActive(true);
					RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(0);
					RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.5f;
					successTrigger = "SurfaceSwim";
				break;

				case state.SurfaceSwimLazyStop:
					
					successTrigger = "SurfaceLazy";
				break;

				case state.Diving:
					DirectionMarker.get._directionArrow.SetActive(true);
					if ( _state == state.Hovering ){
						successTrigger = _physics.IsSteepCollision() ? "Flip" : "Unhover";
					} else {
						
						RenderCamera.get.GetComponent<PositionLink>().SetActive(true);
						RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(0);
						RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.5f;
						successTrigger = "Dive";						
					} 
				break;

				case state.Flip:
					DirectionMarker.get._directionArrow.SetActive(true);
					successTrigger = "Flip";
				break;
				case state.Hovering:
					successTrigger = "Hover";
				break;
				default:
					Debug.LogWarning(this.ToString() + ", " + MethodBase.GetCurrentMethod().Name + ": Undefined parameter" );
				break;
			}
		}

		if ( successTrigger != "" ){
			// New system, state behaviour will change the state itself
			Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": " + _state + " ... " + target );
			GetComponent<Animator>().SetTrigger(successTrigger);
			_stateChanging = true;
		}
		else if ( success ){
			// Old system setting the state instantly
			SetState(target);
		}
		else {
			Debug.LogWarning(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": transition " + _state + " → " + target + " FAILED");
		}
	}

	public void DoSwim(){
		_lazyTimestamp = Time.time;

		switch ( _state ){
			case state.Sit:
				TryState(state.Surface);
			break;

			case state.SurfaceSwimLazyStop:
				if ( Mathf.Abs(DirectionMarker.get.GetUIAngle()) > _surfaceIgnoreThresholdDegrees ){
					TryState(state.Surface);
				} else {
					TryState(state.SurfaceSwim);
				}
				
				break;
			case state.Surface:
				if ( SnapManager.get.IsSnap() ){
					TryState(state.Sit);
					break;
				}
				
				if ( TwistTest() ){
					TryState(state.SurfaceTwist);
					break;
				}

				if ( _physics.IsSteepCollision() ){		
					break;
				}

				if ( !DirectionMarker.get.IsCursorAboveGround() ){
					TryState(state.Diving );
					break;
				}
				
				if ( Mathf.Abs(DirectionMarker.get.GetUIAngle()) <= _surfaceIgnoreThresholdDegrees ){
					TryState(state.SurfaceSwim);
				}
				break;

			case state.Hovering:
				if ( !_physics.IsSteepCollision () ){
					if ( FlipTest() ){
						TryState( state.Flip );
					} else {
						TryState( state.Diving );
					}		
				}
				break;

			case state.SurfaceSwim:
				if ( Mathf.Abs(DirectionMarker.get.GetUIAngle()) > _surfaceIgnoreThresholdDegrees  ){
					TryState(state.Surface);
				}else if ( (DirectionMarker.get.GetUIAngle() > 0 == IsTwist()) ){
					TryState(state.SurfaceSwimTwist);
				} else if ( !DirectionMarker.get.IsCursorAboveGround() ){
					TryState(state.Diving);
				}
				break;
		}
	}

	public void DoHover(){
		switch (_state ){
			case state.SurfaceSwim:
				TryState(state.Surface);
			break;
			case state.Diving:
				TryState(state.Hovering);
			break;	
		}
	}
	public bool IsAboveSurface(){
		return (_surface.GetSurfaceZ (transform.position) + Vector3.down * 0.5f).y < transform.position.y;
	}

	public bool TestSurfaceSwimAngle(float reference){
		return (reference <= _surfaceIgnoreThresholdDegrees && reference > 90 && !IsTwist()) || (reference > -_surfaceIgnoreThresholdDegrees && reference < -90 && IsTwist());
	}

	public void TestTreasure(){
		if (_maxDepth != 0) {
			NotifyManager (_treasure ? TaskManager.action.treasureDiveSuccess : TaskManager.action.diveSuccess);
			_treasure = false;
	
			_maxDepth = 0;	
		}
	}

	public void UpdateSurfaceSwimTwist(){
		if ( !DirectionMarker.get.IsCursorAboveGround() ){
			TryState(state.Diving);
		} else {
			TryState(state.SurfaceSwim);
		}
	}

	public void UpdateSurfaceTwist(){
		if ( !DirectionMarker.get.IsCursorAboveGround() ){
			TryState(state.Diving);
		} else if ( !_physics.IsSteepCollision() ) {
			TryState(state.SurfaceSwim);
		} else {
			TryState(state.Surface);
		}
	}

	public void UpdateSurfaceSwim(){
		if ( LazyTest() ){
			TryState(state.SurfaceSwimLazyStop);
		} else if (_physics.IsSteepCollision()){
			TryState(state.Surface);
		}
	}

	public void UpdateDive(){
		
		if (IsAboveSurface() ) {
			if ( TestSurfaceSwimAngle(DirectionMarker.get.GetDiverAngle()) ){			
				TryState(Diver.state.SurfaceSwim);
			} else {
				TryState(Diver.state.Surface);
				DirectionMarker.get.Reset ();
			}

			TestTreasure();
			return;

		} else {
			float tangentDot_ = (DirectionMarker.get.GetTangentUIDot() + 1) * 0.5f;
			Water.SurfaceSnap.Interpolate360(ref _directionAngleInterpolated, tangentDot_, 3f);
			GetComponent<Animator> ().SetFloat ("SwimDirection", _directionAngleInterpolated );

			_maxDepth = Mathf.Max (_maxDepth, -transform.position.y);
		}

		if ( LazyTest() ){
			TryState(state.Hovering);
		}

		if ( FlipTest() ){
			Diver.get.TryState(Diver.state.Flip);
		} 
	}

	bool LazyTest(){
		return Time.time - _lazyTimestamp > _lazyThreshold;
	}

	public bool HoverTest(){
		Vector3 diff_ = (DirectionMarker.get.transform.position - GetPosition());
		diff_.z = 0;
		return diff_.magnitude < 1.0f;
	}
	public bool FlipTest(){
		return DirectionMarker.get.GetDirectionUIDot() < -0.75f && !HoverTest();
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
