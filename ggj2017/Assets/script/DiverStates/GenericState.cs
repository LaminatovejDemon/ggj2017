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
	public bool _surfaceSnapModifier = false;
	public bool _surfaceSnapValue = false;
	public bool _surfaceAngleSnapModifier = false;
	public bool _surfaceAngleSnapValue = false;

	public bool _surfaceAngleSnapDataModifier = false;
	public Diver.angles _surfaceAngleSnapDataValue;
	

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (state != Diver.state.None && !Diver.get.SetState(state) ){
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
				Snap snap_ = SnapManager.get.GetSnap();
				position_ = snap_ != null ? snap_.transform.position + snap_.offset : animator.targetPosition;
			}
			animator.MatchTarget(position_, targetRotation_, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), _matchMin, _matchMax);
		}
	}
}
