using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public class DirectionMarker : BaseManager<DirectionMarker> {

	public Water.Surface _groundReference;
	Vector3 _uiVector;
	Vector3 _directionVector;
	Vector3 _tangentVector;
	// Vector3 _diffVector;
	bool _active = true;
	bool _hover = false;
		
	bool _mouseDown = false;
	Vector3 _lastControllerPosition = Vector3.zero;

	public bool IsCursorAboveGround(){
		UpdateViewportPosition();
		float difference_ = _groundReference.GetSurfaceZ(Diver.get.transform.position).y - RenderCamera.get.GetComponent<Camera>().ViewportToWorldPoint(_lastControllerPosition).y;
		return difference_ < 0;
	}


	void SetUIVector(){
		_uiVector = -RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint(Diver.get.transform.position) 
					+ RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint(transform.position);
		_uiVector.z = 0;
		_uiVector.Normalize ();
	}
	public float GetDirectionDot(){
		SetUIVector();		
		_directionVector = Diver.get.transform.rotation * Vector3.left;
		_directionVector.Normalize ();

		return Vector3.Dot (_uiVector, _directionVector);
	}

	public float GetTangentDot(){
		SetUIVector();
		_tangentVector = Diver.get.transform.rotation * Vector3.down;
		_tangentVector.Normalize ();

		return Vector3.Dot (_uiVector, _tangentVector);
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
		Gizmos.DrawLine (origin_, origin_ + _uiVector * 20.0f);

		Gizmos.color = Color.red;
		Gizmos.DrawLine (origin_, origin_ + _directionVector * 10.0f);
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine (origin_, origin_ + _tangentVector * 5.0f);
		

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position + Vector3.left, transform.position + Vector3.right );
		Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.down );
		Gizmos.DrawLine(transform.position + Vector3.forward, transform.position + Vector3.back );
	}	

	bool UpdateViewportPosition(){
		
		#if UNITY_EDITOR
		if ( Input.GetMouseButtonDown(0) ){
			_mouseDown = true;
		}
		if ( Input.GetMouseButtonUp(0) ){
			_mouseDown = false;
		}

		if ( !_mouseDown ){
			return false;
		}

		_lastControllerPosition = Input.mousePosition;

		#else
		if (Input.touchCount == 0) {
			return false;
		}

		for (int i = 0; i < Input.touchCount; ++i) {
			position += (Input.GetTouch (i).position);
		}

		position /= Input.touchCount;
		#endif

		_lastControllerPosition += Vector3.forward * (RenderCamera.get.transform.position - Diver.get.transform.position).magnitude;
		_lastControllerPosition = MainCamera.get.GetComponent<Camera>().ScreenToViewportPoint(_lastControllerPosition);

		return true;
	}	

	void Update () {
		transform.eulerAngles += Vector3.forward * Time.deltaTime * 60.0f;

		if (! UpdateViewportPosition() ){
			return;
		}

		Set();

		Vector3 diverViewport_ = RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint (Diver.get.transform.position);		
		Vector3 world_ = RenderCamera.get.GetComponent<Camera>().ViewportToWorldPoint(_lastControllerPosition);

		transform.position = world_;

		Vector2 viewport2d_ = (Vector2)_lastControllerPosition;
		Vector2 diverViewport2d_ = (Vector2)diverViewport_;

		if ((viewport2d_ - diverViewport2d_).magnitude < 0.05f) {
			Hover (true);	
		} else {
			float rawdirectionAngle_ = DirectionMarker.get.GetTangentDot();
			Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod() + ": " + rawdirectionAngle_);
	
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
