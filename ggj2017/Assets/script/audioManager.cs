using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {

	public AudioSource introAudio;
	public AudioSource divingAudio;
	public AudioSource divingWarningAudio;
			
	public void Notify(taskManager.action what, float maxDepth = 0){
		switch (what) {
		case taskManager.action.danger:
			playDivingWarningAudio ();
			break;
		case taskManager.action.treasureDiveSuccess:
		case taskManager.action.diveSuccess:
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
		if (introAudio != null && !introAudio.isPlaying) {
			Debug.Log ("Playing Intro");
			introAudio.Play ();
			divingAudio.Stop ();
			divingWarningAudio.Stop ();
		}
	}

	void playDivingAudio () {
		if (divingAudio != null && !divingAudio.isPlaying) {
			Debug.Log ("Playing Diving");
			introAudio.Stop ();
			divingAudio.Play ();
			divingWarningAudio.Stop ();
		}
	}

	void playDivingWarningAudio () {
		if (divingWarningAudio != null && !divingWarningAudio.isPlaying) {
			Debug.Log ("Playing Warning");
			introAudio.Stop ();
			divingAudio.Stop ();
			divingWarningAudio.Play ();
		}
	}

/*	bool diverNeedsToGoBack () {
		return (diver.instance.GetState () == diver.state.Diving) && 
			((diver.instance.GetCurrentDepth () / Camera.main.GetComponent<Oxigen> ().GetOxygenLeft ()) < 2);
	}*/
}
