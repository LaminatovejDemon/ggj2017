using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taskManager : MonoBehaviour {

	public enum action{
		diveSuccess,
		treasureDiveSuccess,
		death,
	};

	public void Notify(action what, float maxDepth){
		Debug.Log ("Task manager notification " + what.ToString () + ", max depth" + maxDepth);
	}
		
	// Update is called once per frame
	void Update () {
		
	}
}
