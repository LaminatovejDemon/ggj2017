using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSurface : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		if ( !Diver.get.SetState(Diver.state.Surface) ){
			return;
		}

		
		DirectionMarker.get._directionHolder.SetActive(false);
		
		Diver.get.StoreDefaultPosition();
		Diver.get.ApplyDefaultPosition();
		
		Diver.get.GetComponent<Water.SurfaceSnap>().SetActive(true);
		Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngleActive(true);
		Diver.get.GetComponent<Water.SurfaceSnap>().SetSnapAngle(Diver.get._surfaceIdleSnapAngle);

		RenderCamera.get.GetComponent<PositionLink>().SetActive(false);
		RenderCamera.get.GetComponent<PositionLink>().SetOffsetY(3.9f, true);
		RenderCamera.get.GetComponent<PositionLink>()._hardness = 0.05f;
						
	}
}
