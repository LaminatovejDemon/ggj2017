using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSurfaceLazyStop : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (!Diver.get.SetState(Diver.state.SurfaceSwimLazyStop) ){
			return;
		}

		Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(false);
	}


}
