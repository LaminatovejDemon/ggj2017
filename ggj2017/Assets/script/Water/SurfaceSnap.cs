﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Component used to snap an object to the surface. 
///
/// <param name="_surface">reference to the existing surface</param>
/// <param name="_width">object width in the X axis, if greater than zero, object will be floating on two points</
/// <param name="_softness">logarithmic interpolation of the floating object to the surface</param>
/// <param name="_rockingAngle">perpendicular floating angle generated by Sin function</param>
/// </summary>

namespace Water {
public class SurfaceSnap : MonoBehaviour {

	public Surface _surface;
	public float _width = 0;
	public float _verticalOffset = 0;
	public float _roughness = 3.0f;
	public float _rockingAngle = 15.0f;

	public float _snapAngleValue = 0;
	public bool _snapAngleActive = false;

	Vector3 _leftDepth, _rightDepth;
	Vector3 _targetPosition;

	public bool _active;

	public void Reset(){
		Vector3 bak_ = transform.eulerAngles;
		bak_.z = _snapAngleValue;
		transform.eulerAngles = bak_;
	}

	public void SetSnapAngle(float value){
		_snapAngleValue = value;
	}

	public void SetSnapAngleActive(bool active){
		_snapAngleActive = active;
	}

	public void SetActive(bool state){
		if ( !state ){
			 Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ":" + state + " while playing " + Diver.get.GetCurrentClip() + " in " + Diver.get.GetState() );	
			
		} 
		_active = state;
	}
	void SetLineSnap(){
		Debug.Assert(_snapAngleActive == true, this.ToString() + "." + MethodBase.GetCurrentMethod() + ": snapAngle is not supported." );
		_targetPosition = transform.position;

		_leftDepth = _surface.GetSurfaceZ (transform.position + Vector3.left * _width * 0.5f);
		_rightDepth = _surface.GetSurfaceZ (transform.position + Vector3.right * _width * 0.5f);

		float angle_ = Mathf.Tan ((_rightDepth.y - _leftDepth.y) / (_rightDepth.x - _leftDepth.x));

		 Quaternion rotation_ = Quaternion.AngleAxis (angle_ * 180.0f / Mathf.PI, Vector3.forward);
		 Quaternion counterRotation_ = Quaternion.AngleAxis (Mathf.Sin (Time.time * 0.5f) * _rockingAngle, Vector3.left);
		 Quaternion offset_ = Quaternion.AngleAxis (_snapAngleValue * 180.0f / Mathf.PI, Vector3.forward);
		 transform.rotation =  rotation_ * counterRotation_ * offset_;

		Vector3 finalPosition_ = _targetPosition; 
		finalPosition_.y = (_leftDepth.y + _rightDepth.y) * 0.5f + _verticalOffset;
		_targetPosition = finalPosition_;
	}
	void SetPointSnap(){
		_targetPosition = transform.position;

		_leftDepth = _surface.GetSurfaceZ(transform.position);
		_rightDepth = _leftDepth + Vector3.up * 3.0f;
		
		if ( _snapAngleActive ) {
			Quaternion base_ = transform.rotation;
			Quaternion counterRotation_ = Quaternion.AngleAxis (Mathf.Sin (Time.time * 0.5f) * _rockingAngle, Vector3.left);
			transform.rotation = base_ * counterRotation_;
			
			Vector3 bak_ = transform.eulerAngles;
			Interpolate(ref bak_.z, _snapAngleValue, 60f);
			transform.eulerAngles = bak_;
			
		}
		_targetPosition.y = _leftDepth.y + _verticalOffset;
	}

	public static void Interpolate(ref float source, float target, float speed){
		if (target == source) {
			return;
		}

		float delta_ = target - source;

		if (delta_ > 0) {
			if (target < source + speed * Time.deltaTime) {
				source = target;
			} else {
				source += speed * Time.deltaTime;
			}
		} else {
			if (target > source - speed * Time.deltaTime) {
				source = target;
			} else {	
				source -= speed * Time.deltaTime;
			}
		}

		return;
	}

	void Update () {
		if ( !_active ){
			return;
		}

		if ( _width <= 0 ){
			SetPointSnap();
		} else {
			SetLineSnap();
		}
	
		transform.position += (_targetPosition - transform.position) * Mathf.Min(Time.deltaTime * _roughness, 1.0f);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawLine (_leftDepth, _rightDepth );
		Gizmos.DrawLine (_leftDepth + Vector3.up * 5.0f, _rightDepth + Vector3.up * 5.0f);
	}
}
}
