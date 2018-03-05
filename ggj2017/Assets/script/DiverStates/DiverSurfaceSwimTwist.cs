using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSurfaceSwimTwist : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (!Diver.get.SetState(Diver.state.SurfaceSwimTwist) ){
			return;
		}
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		Quaternion targetRotation_ = Quaternion.Euler(0, Diver.get.IsTwist() ? 180 : 0, Diver.get._surfaceSwimmingAngle);
		animator.MatchTarget(animator.targetPosition, targetRotation_, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0f, 1f);
	}
}
