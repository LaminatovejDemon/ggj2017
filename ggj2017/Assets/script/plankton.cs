using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plankton : MonoBehaviour {

	planktonManager _manager;
	float _initTimestamp;
	float _lifespan;
	Vector3 _velocity;
	float _maxAlpha;
	public bool _tintable;
	public bool _scalable;
	public int _lifespanMultiplier;
	public float _depthMultiplier = 1;

	public void Initialise(planktonManager manager){
		_manager = manager;

		Vector3 randomViewport_ = new Vector3 ((Random.value) * 1.0f, (Random.value) * 1.0f, Random.Range(10.0f - (7.0f * _depthMultiplier), 10.0f + (7.0f * _depthMultiplier)));
		GameObject renderCameraObject_ = GameObject.FindGameObjectWithTag ("RenderCamera");
		Camera renderCamera_ = renderCameraObject_.GetComponent<Camera>();
		transform.position = renderCamera_.ViewportToWorldPoint( randomViewport_);
		float _scale_ = Random.Range (0.02f, 0.8f);
		_initTimestamp = Time.time;
		_maxAlpha = Random.Range (0.005f, 0.03f) / _scale_;
		_velocity = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), 0) * 0.05f / _scale_;
		_lifespan = Random.Range (2.0f, 5.0f) * _lifespanMultiplier;
		while (transform.position.y > -3.0f) {
			Vector3 bak_ = transform.position;
			bak_.y -= 5.0f;
			transform.position = bak_;
		}
		if ( _scalable ){
				transform.localScale = Vector3.one * _scale_;
		}
	}

	// Update is called once per frame
	void Update () {

		if (Time.time - _initTimestamp > _lifespan) {
			Initialise (_manager);
		}

		if (!_tintable) {
			return;
		}
			
		transform.position += _velocity * Time.deltaTime;
		Color bak_ = GetComponent<Renderer> ().material.GetColor ("_TintColor");
		bak_.a = (0.5f - Mathf.Abs (0.5f - ((Time.time - _initTimestamp) / _lifespan))) * _maxAlpha * 2.0f;
		GetComponent<Renderer> ().material.SetColor("_TintColor", bak_);

	/*	Vector3 viewport_ = Camera.main.WorldToViewportPoint (transform.position);
		Debug.Log (viewport_);
		if (viewport_.x > 1.5f || viewport_.x < -1.5f || viewport_.y > 1.5f || viewport_.y < -1.5f) {
			Initialise (_manager);
		}*/
	}
}
