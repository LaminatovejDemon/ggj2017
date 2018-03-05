using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverHover : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if ( !Diver.get.SetState(Diver.state.Hovering) ) {
			return;
		}
		
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if ( Diver.get.IsAboveSurface() ){
			Diver.get.TryState(Diver.state.Surface);
			DirectionMarker.get.Reset ();
			Diver.get.TestTreasure();
		}
	}
}
