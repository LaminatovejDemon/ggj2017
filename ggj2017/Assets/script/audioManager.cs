using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {

	public AudioSource introAudio;
	public AudioSource divingAudio;
	public AudioSource divingWarningAudio;

	void Start () {
		divingAudio.enabled = true;
		divingWarningAudio.enabled = true;
	}

	void Update () {
		if (diverNeedsToGoBack ()) {
			playDivingWarningAudio ();
		}

		Camera.main.GetComponent<Oxigen>().GetOxygenLeft();
		diver.instance.GetCurrentDepth();

		//if diving -> start diving music
			//if DIVER SHOULD GO UP
	}

	public void Notify(taskManager.action what, float maxDepth = 0){
		switch (what) {
		case taskManager.action.idle:
			playIntroAudio ();
			break;
		case taskManager.action.diveStarted:
			playDivingAudio ();
			break;
		default:
			break;
		}
	}

	void playIntroAudio () {
		if (introAudio != null) {
			introAudio.Play ();
			divingAudio.Stop ();
			divingWarningAudio.Stop ();
		}
	}

	void playDivingAudio () {
		if (divingAudio != null) {
			introAudio.Stop ();
			divingAudio.Play ();
			divingWarningAudio.Stop ();
		}
	}

	void playDivingWarningAudio () {
		if (divingWarningAudio != null) {
			introAudio.Stop ();
			divingAudio.Stop ();
			divingWarningAudio.Play ();
		}
	}

	bool diverNeedsToGoBack () {
		return (diver.instance.GetState () == diver.state.Diving) && 
			((diver.instance.GetCurrentDepth () / Camera.main.GetComponent<Oxigen> ().GetOxygenLeft ()) < 2);
	}
}
