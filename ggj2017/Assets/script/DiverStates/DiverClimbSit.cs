using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverClimbSit : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		
		Diver.get.GetComponent<Water.SurfaceSnap>().SetActive(false);
		Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(false);
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		 Quaternion targetRotation_ = Quaternion.Euler(0, Diver.get.IsTwist() ? -180 : 0, Diver.get._surfaceIdleSnapAngle);
		 Snap snap_ = SnapManager.get.GetSnap();
		 Vector3 position_ = snap_ != null ? snap_.transform.position + snap_.offset : animator.targetPosition;
		 animator.MatchTarget(position_, targetRotation_, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0.5f, 1f);
	}
}
