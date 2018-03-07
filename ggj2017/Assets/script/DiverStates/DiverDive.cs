using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverDive : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if (!Diver.get.SetState(Diver.state.Diving) ){
			return;
		}

		Diver.get.NotifyManager (TaskManager.action.diveStarted);
		TaskManager.get._title.SetState(title.state.ToBeHidden);
		OxygenManager.get.DismissRewawrd ();
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		
	}
}
