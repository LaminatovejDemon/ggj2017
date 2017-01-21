using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taskManager : MonoBehaviour {

	public task _taskTemplate;
	task _taskInstance = null;
	public GameObject _taskMarkerTemplate;
	GameObject _taskMarkerInstance = null;
	int progress = 0;


	public float[] _progressDepth;

	treasureStatus _treasureStatus = treasureStatus.none;

	enum treasureStatus{
		none,
		hidden,
		found,
	};

	public enum action{
		diveSuccess,
		treasureDiveSuccess,
		treasureFound,
		death,
	};

	public void Notify(action what, float maxDepth){
		switch (what) {
		case action.diveSuccess:
			if (_taskInstance == null) {
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
		}

	}

	void PrepareTask(){
		if (_taskInstance != null) {
			Destroy (_taskInstance.gameObject);
		}
		if (_taskMarkerInstance == null) {
			_taskMarkerInstance = GameObject.Instantiate (_taskMarkerTemplate);
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
		_taskMarkerInstance.transform.position = diver.instance.transform.position + Vector3.back * 1.0f + (turn_ * Vector3.forward * direction_.magnitude);

	}
}
