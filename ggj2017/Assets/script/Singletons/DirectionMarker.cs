using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public class DirectionMarker : BaseManager<DirectionMarker> {

	public Water.Surface _groundReference;
	Vector3 uiVector;
	Vector3 directionVector;
	Vector3 tangentVector;
	// Vector3 _diffVector;		
	bool _mouseDown = false;
	Vector3 _lastControllerPosition = Vector3.zero;

	public bool IsCursorAboveGround(){
		UpdateViewportPosition();
		float difference_ = _groundReference.GetSurfaceZ(Diver.get.transform.position).y - RenderCamera.get.GetComponent<Camera>().ViewportToWorldPoint(_lastControllerPosition).y;
		return difference_ < 0;
	}

	void SetUIVector(){
		uiVector = -RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint(Diver.get.transform.position) 
					+ RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint(transform.position);
		uiVector.z = 0;
		uiVector.Normalize ();
	}

	public float GetCollisionDot(Collision2D collision){
		Vector3 collision_ = collision.contacts[0].point;
		Vector3 diver_ = Diver.get.transform.position;
		collision_.z = diver_.z;
		Vector3 diverDifference_ = (collision_ - diver_).normalized;
		return Vector3.Dot(uiVector, diverDifference_);
	}

	public float GetDirectionDot(){	
		directionVector = Diver.get.transform.rotation * Vector3.left;
		directionVector.z = 0;
		directionVector.Normalize ();

		return Vector3.Dot (uiVector, directionVector);
	}

	public float GetTangentDot(){		
		tangentVector = Diver.get.transform.rotation * Vector3.down;
		tangentVector.z = 0;
		tangentVector.Normalize ();

		return Vector3.Dot (uiVector, tangentVector);
	}

	void Set(){
		Diver.get.DoSwim ();
		GetComponent<Renderer> ().enabled = true;
	}

	public void Reset(){
		GetComponent<Renderer> ().enabled = false;
	}

	void OnDrawGizmos(){
		Vector3 origin = Diver.get.transform.position;
		
		if (!Diver.Exists()) {
			return;
		}

		Vector3 origin_ = Diver.get.transform.position;

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine (origin_, origin_ + uiVector * 20.0f);

		Gizmos.color = Color.red;
		Gizmos.DrawLine (origin_, origin_ + directionVector * 10.0f);
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine (origin_, origin_ + tangentVector * 5.0f);
		
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
			_lastControllerPosition += (Input.GetTouch (i).position);
		}

		_lastControllerPosition /= Input.touchCount;
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

		// Vector3 diverViewport_ = RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint (Diver.get.transform.position);		
		Vector3 world_ = RenderCamera.get.GetComponent<Camera>().ViewportToWorldPoint(_lastControllerPosition);

		transform.position = world_;
		
		SetUIVector();	
		Set();
		
		// Vector2 viewport2d_ = (Vector2)_lastControllerPosition;
		// Vector2 diverViewport2d_ = (Vector2)diverViewport_;
	}
}
