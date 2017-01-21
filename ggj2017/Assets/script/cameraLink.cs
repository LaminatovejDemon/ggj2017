﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLink : MonoBehaviour {

	public GameObject _target;
	public float _hardness = 0.9f;
	Vector3 _offset;
	Vector3 _targetValue;

	// Use this for initialization
	void Start () {
		_offset = this.transform.position - _target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		_targetValue = _target.transform.position + _offset;
	
		this.transform.position += (_targetValue - this.transform.position) * _hardness;

	}
}