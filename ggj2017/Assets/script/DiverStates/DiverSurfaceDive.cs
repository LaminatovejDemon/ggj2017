using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSurfaceDive : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		// if (!Diver.get.SetState(Diver.state.Flip) ){
			// return;
		// }

		Diver.get.GetComponent<Water.SurfaceSnap>().SetActive(false);
	}
}

