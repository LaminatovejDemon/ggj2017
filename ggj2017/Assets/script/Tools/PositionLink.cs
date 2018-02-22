using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLink : MonoBehaviour {

	public GameObject _target;
	public float _hardness = 0.6f;
	Vector3 _offset;
	Vector3 _targetValue;
	public bool _yAxis = true;
	public bool _enabled = true;

	public void SetActive(bool state ){
		_enabled = state;
	}

	public void Link(GameObject target){
		_target = target;
		_offset = this.transform.position - _target.transform.position;
	}

	public void SetOffsetY(float value, bool absolute = false){
		_targetValue.y = absolute ? value : this.transform.position.y + value;
		_offset.y = _target == null ? 0 : (_targetValue - _target.transform.position).y;
	}

	public void Reset(){
		if ( _target == null ){
			return;
		}

		this.transform.position = _target.transform.position + _offset;
	}

	// Use this for initialization
	void Start () {	
		_targetValue = this.transform.position;
		_offset = _target == null ? Vector3.zero : this.transform.position - _target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( _enabled && _target != null ){
			_targetValue = _target.transform.position + _offset;
			if (!_yAxis) {
				_targetValue.y = this.transform.position.y;
			}
		}

		if ( _hardness >= 1 ){
			this.transform.position = _targetValue;
		}else{
			this.transform.position += (_targetValue - this.transform.position) * _hardness;
		}
			

	}
}
