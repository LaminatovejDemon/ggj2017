
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanktonManager : BaseManager<PlanktonManager> {

	public plankton [] planktonTemplates;
	plankton[] _planktonInstances;
	public int _planktonCount = 1;

	void InitialisePlankton(){
		if (_planktonInstances != null ) {
			return;
		}
		_planktonInstances = new plankton[_planktonCount];

		for (int i = 0; i < _planktonCount; ++i) {
			_planktonInstances [i] = GameObject.Instantiate (planktonTemplates [Random.Range (0, planktonTemplates.Length)]);
			_planktonInstances [i].Initialise ();
		}
	}		

	void Update(){
		InitialisePlankton ();
	}
}
