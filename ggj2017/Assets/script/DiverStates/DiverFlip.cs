using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverFlip : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (!Diver.get.SetState(Diver.state.Flip) ){
			return;
		}
	}
}
