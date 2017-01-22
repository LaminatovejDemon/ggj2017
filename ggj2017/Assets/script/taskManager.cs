using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taskManager : MonoBehaviour {

	public title _title;
	public title _tutorial1;
	public title _tutorial2;
	bool _tutorial1Happened = false;
	bool _tutorial2Happened = false;
	public task _taskTemplate;
	task _taskInstance = null;
	public GameObject _taskMarkerTemplate;
	GameObject [] _taskMarkerInstance;
	int progress = 0;


	public void Reset(){
		_title.Reset ();
		_tutorial1.Reset ();
		_tutorial2.Reset ();
		progress = 0;
		Destroy (_taskInstance);
		_treasureStatus = treasureStatus.none;
		PrepareTask ();
	}

	public float[] _progressDepth;

	treasureStatus _treasureStatus = treasureStatus.none;

	enum treasureStatus{
		none,
		hidden,
		found,
	};

	public enum action{
		idle,
		diveStarted,
		screenTurned,
		diveSuccess,
		treasureDiveSuccess,
		treasureFound,
		restart,
	};

	public void Notify(action what, float maxDepth = 0){
		switch (what) {
		case action.idle:
			_tutorial1.SetState (title.state.ToBeDisplayed);
			_tutorial1Happened = true;
			break;
		case action.diveStarted:
			if (_tutorial1Happened) {
				_tutorial1Happened = false;
				_tutorial1.SetState (title.state.ToBeHidden);
				_tutorial2.SetState (title.state.ToBeDisplayed);
				_tutorial2Happened = true;
			}

			break;
		case action.screenTurned:
			
			break;
		case action.diveSuccess:
			if (_tutorial2Happened) {
				_tutorial2.SetState (title.state.ToBeHidden);
				_tutorial2Happened = false;
			}
			if (_taskMarkerInstance == null) {
				_title.SetState (title.state.ToBeDisplayed);
				PrepareTask ();
			}
			break;
		case action.treasureDiveSuccess:
			progress = Mathf.Min (progress + 1, _progressDepth.Length - 1);
			Debug.Log (progress);
			PrepareTask ();
			break;
		case action.treasureFound:
			_treasureStatus = treasureStatus.found;
			break;
		case action.restart:
			Reset ();
			diver.instance.Restart ();
			break;
		}

	}

	void PrepareTask(){
		if (_treasureStatus == treasureStatus.hidden) {
			return;
		}
		if (_taskInstance != null) {
			Destroy (_taskInstance.gameObject);
		}
		if (_taskMarkerInstance == null) {
			_taskMarkerInstance = new GameObject[2];
			for (int i = 0; i < _taskMarkerInstance.Length; ++i) {
				_taskMarkerInstance [i] = GameObject.Instantiate (_taskMarkerTemplate);
			}
		}
		_treasureStatus = treasureStatus.hidden;
		_taskInstance = GameObject.Instantiate (_taskTemplate);
		_taskInstance.Setup (_progressDepth [progress]);
	}		

	void Update(){
		if (_taskMarkerInstance == null) {
			return;
		}


		Vector3 direction_ = _treasureStatus == treasureStatus.found ? Vector3.up * 2.0f :_taskInstance.transform.position - diver.instance.transform.position;
		direction_.z = 0;

		if (direction_.magnitude > 3.0f) {
			direction_ = direction_.normalized * 3.0f;
		}


		Quaternion turn_ = Quaternion.LookRotation (direction_);
		for (int i = 0; i < _taskMarkerInstance.Length; ++i) {
			_taskMarkerInstance[i].transform.position = diver.instance.transform.position + Vector3.back * (1.0f) + (turn_ * Vector3.forward * (direction_.magnitude - (0.3f * i )));
		}

	}
}
