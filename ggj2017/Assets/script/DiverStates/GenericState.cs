using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericState : StateMachineBehaviour {

	public Diver.state state = Diver.state.None;
	public bool _matchTargetPosition = false;
	public bool _matchTargetAngle = false;
	public float _matchMin = 0f;
	public float _matchMax = 1f;
	
	public Diver.angles turnAngle = Diver.angles.Idle;
	public Diver.gangles twistAngle = Diver.gangles._0;
	public SnapManager.SnapType _snapPointType = SnapManager.SnapType.None;

	public bool _doTwist = false;

	public bool _surfaceSnapModifier = false;
	public bool _surfaceSnapValue = false;

	Quaternion _enterRotation;
	Vector3 _enterPosition;
	Vector3 _snapPosition;
	bool _snapX = false, _snapY = false;
	float _enterTime;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		_enterRotation = animator.rootRotation;
		_enterPosition = animator.rootPosition;
		_snapPosition = animator.rootPosition;
		_snapX = false;
		_snapY = false;
		_enterTime = Time.time;
		if ( _matchTargetPosition ){
			SetPositionToSnap();
		}
		
		if (  (state != Diver.state.None && !Diver.get.SetState(state))){
			return;
		}
		if ( _doTwist )
		{
			Diver.get.Twist();
		}
		
		if ( _surfaceSnapModifier ){
			Diver.get.GetComponent<Env.SurfaceSnap>().SetActive(_surfaceSnapValue);
		}
		// if ( _surfaceAngleSnapModifier ){
			// if ( _surfaceAngleSnapDataModifier ){
				// Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngle(Diver.get._angles[(int)_surfaceAngleSnapDataValue]);
			// }
		// }
	}
	
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller){
		float normalizedTime_ = Mathf.Clamp(stateInfo.loop ? (Time.time - _enterTime) * 3.0f : (stateInfo.normalizedTime - _matchMin) / (_matchMax - _matchMin), 0f, 1f);
		
		Quaternion targetRotation_ = animator.rootRotation;
		if ( _matchTargetAngle && normalizedTime_ > 0 ){
			Quaternion snappedRotation_ = Quaternion.Euler(0, Diver.get.IsTwist() ? 180+(int)twistAngle : (int)twistAngle, Diver.get._angles[(int)turnAngle]);
			
			targetRotation_ = Quaternion.Lerp(_enterRotation, snappedRotation_, normalizedTime_);
		}

		Vector3 targetPosition_ = animator.rootPosition;
		if ( _matchTargetPosition && normalizedTime_ > 0 ){
			Vector3 alteredPosition_ = Vector3.Lerp(_enterPosition, _snapPosition, normalizedTime_);
			if ( _snapX ){
				targetPosition_.x = alteredPosition_.x;	
			}
			if ( _snapY ){
				targetPosition_.y = alteredPosition_.y;	
			}
		}

		if ( !_matchTargetPosition && !_matchTargetAngle ){
			animator.ApplyBuiltinRootMotion();
		} else {
			animator.transform.position = targetPosition_;
			animator.transform.rotation = targetRotation_;
		}
	}

	void SetPositionToSnap(){
		SnapTrigger snap_ = SnapManager.get.GetSnapTrigger(_snapPointType);
		if ( snap_ == null ){
			Debug.LogWarning(this.ToString() + "." + state.ToString() + ": Snap not around.");
			return;
		}

		SnapPoint point_ = snap_.GetSnap(_snapPointType);
		if ( point_ == null || !(point_.snapX || point_.snapY) ){
			Debug.LogWarning(this.ToString() + "." + state.ToString() + ": Snap point type " + _snapPointType + " not present.");
			return;
		}

		_snapPosition = (point_.snapObject != null ? point_.snapObject.transform.position : snap_.transform.position) + point_.offset;
		_snapX = point_.snapX;
		_snapY = point_.snapY;
	}
}


