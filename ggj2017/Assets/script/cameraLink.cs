using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLink : MonoBehaviour {

	public GameObject _target;
	public float _hardness = 0.9f;
	Vector3 _offset;
	Vector3 _targetValue;
	public bool _yAxis = true;

	public void Link(GameObject target){
		_target = target;
		_offset = this.transform.position - _target.transform.position;
	}

	public void Reset(){
		if ( _target == null ){
			return;
		}

		this.transform.position = _target.transform.position + _offset;
	}

	// Use this for initialization
	void Start () {
		if ( _target == null ){
			return;
		}
		
		_offset = this.transform.position - _target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if ( _target == null ){
			return;
		}
		
		_targetValue = _target.transform.position + _offset;
		if (!_yAxis) {
			_targetValue.y = this.transform.position.y;
		}

		this.transform.position += (_targetValue - this.transform.position) * _hardness;

	}
}
