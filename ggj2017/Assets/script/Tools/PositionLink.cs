﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLink : ListenerHandler<Env.Listener> {
	public GameObject _target;
	public float _hardness = 0.05f;
	public Vector3 _offset;
	Vector3 _targetValue;
	public bool _yAxis = true;
	public bool _ZAxis = true;
	public bool _enabled = true;
	public bool _digital = false;
	public float _digitalStep = 1.0f;

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
		_targetValue = this.transform.position;
	}

	void Awake () {	
		_targetValue = this.transform.position;
		_offset = _target == null ? Vector3.zero : this.transform.position - _target.transform.position;
	}
	
	void Update () {

		if ( _enabled && _target != null ){
			_targetValue = _target.transform.position + _offset;
			if (!_yAxis) {
				_targetValue.y = this.transform.position.y;
			}
			if (!_ZAxis) {
				_targetValue.z = this.transform.position.z;
			}
		}

		if ( _digital ){
			_targetValue = (Vector3)(Vector3Int.FloorToInt((_targetValue) / _digitalStep)) * _digitalStep;
		}

		if ( this.transform.position == _targetValue ){
			return;
		}

		if ( _hardness >= 1 ){
			this.transform.position = _targetValue;
		}else{
			this.transform.position += (_targetValue - this.transform.position) * _hardness;
		}

		UpdateListeners();
	}
}
