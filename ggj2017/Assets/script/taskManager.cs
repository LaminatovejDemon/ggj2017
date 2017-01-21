using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taskManager : MonoBehaviour {

	public task _taskTemplate;
	task _taskInstance;
	int progress = 0;

	public float[] _progressDepth;

	public enum action{
		diveSuccess,
		treasureDiveSuccess,
		death,
	};

	public void Notify(action what, float maxDepth){
		Debug.Log ("Task manager notification " + what.ToString () + ", max depth" + maxDepth);
		switch (what) {
		case action.diveSuccess:
			if (_taskInstance == null) {
				PrepareTask ();
			}
			break;
		case action.treasureDiveSuccess:
			progress = Mathf.Min (progress + 1, _progressDepth.Length-1);
			PrepareTask ();
			break;
		}
	}

	void PrepareTask(){
		_taskInstance = GameObject.Instantiate (_taskTemplate);
		_taskInstance.Setup (_progressDepth [0]);
	}		
}
