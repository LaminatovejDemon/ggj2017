using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager<AudioManager> {

	public AudioSource[] introAudio;
	public AudioSource[] divingAudio;
	public AudioSource[] divingWarningAudio;
	public AudioSource[] sounds;

			
	public void Notify(TaskManager.action what, float maxDepth = 0){
		switch (what) {
		case TaskManager.action.danger:
			playAudio (playedBrand.warning);
			break;
		case TaskManager.action.treasureDiveSuccess:
		case TaskManager.action.diveSuccess:
			playAudio (playedBrand.intro);
			break;
		case TaskManager.action.diveStarted:
			playAudio (playedBrand.diving);
			break;
		case TaskManager.action.treasureFound:
			PlaySound ();
			break;
		default:
			break;
		}
	}

	void PlaySound(){
		Debug.Log ("playing sound");
		sounds [0].Play();
	}

	void Update(){
		if (_playedBrand == playedBrand.none && Time.time > 3.0f) {
			playAudio (playedBrand.intro);
		}

		if (_oldBrand != playedBrand.none) {
			float value_ = 0.5f - (Time.time - _oldTrackTimestamp);
			if (value_ <= 0) {
				GetBrand (_oldBrand) [_oldIndex].Stop ();	
				_oldBrand = playedBrand.none;
				_oldIndex = -1;
			} else {
				GetBrand (_oldBrand) [_oldIndex].volume = value_;
			}
		}
	}

	public void Reset(){
		if (_oldBrand != playedBrand.none) {
			GetBrand (_oldBrand) [_oldIndex].Stop ();
		}
		if ( _playedIndex != -1 ){
			GetBrand (_playedBrand) [_playedIndex].Stop ();
			_playedBrand = playedBrand.none;
			_playedIndex = -1;

			_oldBrand = playedBrand.none;
			_oldIndex = -1;
			_oldTrackTimestamp = -1;
		}
		playAudio (playedBrand.intro);
	}

	public enum playedBrand{
		none = -1,
		intro = 0,
		diving = 1,
		warning = 2, 
	};

	playedBrand _playedBrand = playedBrand.none;
	int _playedIndex = -1;
	playedBrand _oldBrand = playedBrand.none;
	int _oldIndex = -1;
	float _oldTrackTimestamp;

	void StopPreviousAudio(){
		if (_playedBrand == playedBrand.none || _playedIndex == -1) {
			return;
		}
		GetBrand (_playedBrand) [_playedIndex].Stop ();
/*		if (_oldBrand != playedBrand.none) {
			GetBrand (_oldBrand) [_oldIndex].Stop ();
		}
		_oldIndex = _playedIndex;
		_oldBrand = _playedBrand;
		_oldTrackTimestamp = Time.time;*/
	}

	AudioSource[] GetBrand(playedBrand brand){
		switch (_playedBrand) {
		case playedBrand.intro:
			return introAudio;
		case playedBrand.diving:
			return divingAudio;
		case playedBrand.warning:
			return divingWarningAudio;
		}
		return null;
	}

	void playAudio(playedBrand band){
		if (_playedBrand == band && GetBrand(_playedBrand)[_playedIndex].isPlaying) {
			return;
		}
		StopPreviousAudio ();
		_playedBrand = band;
		_playedIndex = Random.Range (0, GetBrand(_playedBrand).Length);
		GetBrand (_playedBrand) [_playedIndex].volume = 1.0f;
		GetBrand(_playedBrand)[_playedIndex].Play ();
	}



}
