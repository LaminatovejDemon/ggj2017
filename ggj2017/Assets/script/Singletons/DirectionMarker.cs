using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DirectionMarker : BaseManager<DirectionMarker> {

	public Camera _worldCamera;
	public Camera _UICamera;
	Vector3 _uiVector;
	Vector3 _diverVector;
	Vector3 _diffVector;
	bool _active = true;
	bool _hover = false;

	public static DirectionMarker instance{
		get {
			return _Instance;
		}
		private set {
			_Instance = value;
		}
	}

	static DirectionMarker _Instance;

	void Start(){
		if ( _Instance != null && _Instance != this ) {
			GameObject.Destroy (_Instance);
		}
		instance = this;
	}
		
	bool _mouseDown = false;


	public bool IsAboveGround(){
		return transform.position.y - Diver.get.transform.position.y > 0;
	}

	public float GetDiffAngle(){
		Diver.get.NotifyManager (TaskManager.action.screenTurned);
		_uiVector = -_worldCamera.WorldToViewportPoint(Diver.get.transform.position) + _UICamera.WorldToViewportPoint(transform.position);
		_uiVector.z = 0;
		_uiVector.Normalize ();
		_diverVector = Diver.get.transform.rotation * Vector3.down;
		_diverVector.Normalize ();

		_diffVector = _uiVector - _diverVector;

		return Vector3.Dot (_uiVector, _diverVector);
	}

	void Set(){
		if (_active) {
			return;
		}
		Diver.get.Swim ();
		GetComponent<Renderer> ().enabled = true;
		_active = true;
	}

	public void Reset(){
		if (!_active) {
			return;
		}
		_hover = false;
		_active = false;
		GetComponent<Renderer> ().enabled = false;
	}

	void OnDrawGizmos(){
		Vector3 origin = Diver.get.transform.position;
		
		if (!Diver.Exists()) {
			return;
		}

		Vector3 origin_ = Diver.get.transform.position;

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine (origin_, origin_ + _uiVector * 100.0f);

		Gizmos.color = Color.red;
		Gizmos.DrawLine (origin_, origin_ + _diverVector * 100.0f);
	}		

	void Update () {

		Vector2 averagePosition_ = Vector2.zero;
		transform.eulerAngles += Vector3.forward * Time.deltaTime * 60.0f;

		#if UNITY_EDITOR
		if ( Input.GetMouseButtonDown(0) ){
			_mouseDown = true;
		}
		if ( Input.GetMouseButtonUp(0) ){
			_mouseDown = false;
		}

		if ( !_mouseDown ){
			return;
		}

		Set();

		averagePosition_ = Input.mousePosition;

		#else
		if (Input.touchCount == 0) {
			return;
		}

		for (int i = 0; i < Input.touchCount; ++i) {
			averagePosition_ += (Input.GetTouch (i).position);
		}

		Set();

		averagePosition_ /= Input.touchCount;
		#endif

		Vector3 viewport_ = MainCamera.get.GetComponent<Camera>().ScreenToViewportPoint (averagePosition_) + Vector3.forward * (_UICamera.transform.position - Diver.get.transform.position).magnitude;
		Vector3 diverViewport_ = _worldCamera.WorldToViewportPoint (Diver.get.transform.position);

		Vector3 world_ = _UICamera.ViewportToWorldPoint(viewport_);

		transform.position = world_;

		Vector2 viewport2d_ = (Vector2)viewport_;
		Vector2 diverViewport2d_ = (Vector2)diverViewport_;

		if ((viewport2d_ - diverViewport2d_).magnitude < 0.05f) {
			Hover (true);	
		} else {
			Hover (false);
		}
	}

	void Hover( bool state){
		if ( _hover == state ){
			return;
		}
		Debug.Log(Time.time + ": " + state + ": ");

		_hover = state;
		Diver.get.Hover (state);
	}
}
