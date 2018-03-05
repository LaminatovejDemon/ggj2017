using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSurfaceSwim : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (! Diver.get.SetState(Diver.state.SurfaceSwim) ){
			return;
		}

		Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(true);
		Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngle(Diver.get._surfaceSwimmingAngle);	
	}
}
