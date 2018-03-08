using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericState : StateMachineBehaviour {

	public Diver.state state = Diver.state.None;
	public bool _matchTarget = false;
	public float _matchMin = 0f;
	public float _matchMax = 1f;
	public Diver.angles twistAngle;

	public bool _snapPointPosition = false;
	public SnapManager.SnapType _snapPointType = SnapManager.SnapType.None;

	public bool _surfaceSnapModifier = false;
	public bool _surfaceSnapValue = false;
	public bool _surfaceAngleSnapModifier = false;
	public bool _surfaceAngleSnapValue = false;
	public bool _surfaceAngleSnapDataModifier = false;
	public Diver.angles _surfaceAngleSnapDataValue;
	

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (  (state != Diver.state.None && !Diver.get.SetState(state))){
			return;
		}
		
		if ( _surfaceSnapModifier ){
			Diver.get.GetComponent<Water.SurfaceSnap>().SetActive(_surfaceSnapValue);
		}
		if ( _surfaceAngleSnapModifier ){
			Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(_surfaceAngleSnapValue);
			if ( _surfaceAngleSnapDataModifier ){
				Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngle(Diver.get._angles[(int)_surfaceAngleSnapDataValue]);
			}
		}
	}
	
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if ( _matchTarget ){
			Quaternion targetRotation_ = Quaternion.Euler(0, Diver.get.IsTwist() ? 180 : 0, Diver.get._angles[(int)twistAngle]);
			Vector3 position_ = animator.targetPosition;
			if (_snapPointPosition ){
				SetPositionToSnap(ref position_);
			}
			animator.MatchTarget(position_, targetRotation_, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), _matchMin, _matchMax);
		}
	}

	void SetPositionToSnap(ref Vector3 position){
		Snap snap_ = SnapManager.get.GetSnap();
		if ( snap_ == null ){
			Debug.LogWarning(this.ToString() + "." + state.ToString() + ": Snap not around.");
			return;
		}

		SnapPoint point_ = snap_.GetSnap(_snapPointType);
		if ( point_ == null ){
			Debug.LogWarning(this.ToString() + "." + state.ToString() + ": Snap point type " + _snapPointType + " not present.");
			return;
		}
		
		position = snap_.transform.position + point_.offset;
	}
}


