using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSurface : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if ( !Diver.get.SetState(Diver.state.Surface) ){
			return;
		}

		Diver.get.GetComponent<Water.SurfaceSnap>().SetActive(true);					
	}
}
