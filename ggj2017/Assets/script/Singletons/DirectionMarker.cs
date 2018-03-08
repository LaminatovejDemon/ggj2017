using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public class DirectionMarker : BaseManager<DirectionMarker> {

	public GameObject _directionArrow;
	public Water.Surface _groundReference;
	Vector3 uiVector;
	Vector3 directionVector;
	Vector3 tangentVector;
	// Vector3 _diffVector;		
#if UNITY_EDITOR || UNITY_STANDALONE
	bool _mouseDown = false;
#endif
	Vector3 _lastControllerPosition = Vector3.zero;

	public bool IsCursorAboveGround(){
		UpdateViewportPosition();
		float difference_ = _groundReference.GetSurfaceZ(Diver.get.GetPosition()).y - RenderCamera.get.GetComponent<Camera>().ViewportToWorldPoint(_lastControllerPosition).y;
		return difference_ < 1.0f;
	}

	void SetUIVector(){
		uiVector = -RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint(Diver.get.GetPosition()) 
					+ RenderCamera.get.GetComponent<Camera>().WorldToViewportPoint(transform.position);
		uiVector.z = 0;
		uiVector.Normalize ();
	}

	public float GetCollisionUIDot(Collision2D collision){
		Vector3 collision_ = collision.contacts[0].point;
		Vector3 diver_ = Diver.get.GetPosition();
		collision_.z = diver_.z;
		Vector3 diverDifference_ = (collision_ - diver_).normalized;
		return Vector3.Dot(uiVector, diverDifference_);
	}

	public float GetDirectionUIDot(){	
		directionVector = Diver.get.transform.rotation * Vector3.left;
		directionVector.z = 0;
		directionVector.Normalize ();

		return Vector3.Dot (uiVector, directionVector);
	}

	public float GetTangentUIDot(){		
		tangentVector = Diver.get.transform.rotation * Vector3.down;
		tangentVector.z = 0;
		tangentVector.Normalize ();

		return Vector3.Dot (uiVector, tangentVector);
	}

	public float GetGlobalUIDot(){
		return Vector3.Dot(uiVector, Diver.get.IsTwist() ? Vector3.right : Vector3.left );
	}

	public float GetSnapUIDot(Snap source, Vector3 relativeOffset){
		Vector3 diverdiff_ = (Diver.get.GetPosition() - (source.transform.position + relativeOffset)).normalized;
		float dot_ = Vector3.Dot(uiVector, diverdiff_ );
		return dot_;
	}

	public float GetUIAngle(){
		Vector3 diff_ = Diver.get.GetPosition() - transform.position;
		
		return Mathf.Atan2(diff_.x, diff_.y) * 180.0f / Mathf.PI;
	}

	public float GetDiverAngle(){
		Vector3 diff_ = Diver.get.transform.rotation * Vector3.right;
		
		return Mathf.Atan2(diff_.x, diff_.y) * 180.0f / Mathf.PI;
	}

	public bool IsDiverFacingLeft(){
		return !Diver.get.IsTwist();
	}

	void Set(){
		if ( Diver.get.HoverTest() ) {
			_directionArrow.SetActive(false);
			Diver.get.DoHover();	
		} else {
			Diver.get.DoSwim ();
		}		
	}

	public void Reset(){
	}

	void OnDrawGizmos(){		
		if (!Diver.Exists()) {
			return;
		}

		Vector3 origin_ = Diver.get.GetPosition();

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
		
		#if UNITY_EDITOR || UNITY_STANDALONE
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

		_lastControllerPosition = Vector3.zero;

		for (int i = 0; i < Input.touchCount; ++i) {
			_lastControllerPosition += (Vector3)(Input.GetTouch(i).position);
		}

		_lastControllerPosition /= Input.touchCount;
		#endif

		_lastControllerPosition += Vector3.forward * (RenderCamera.get.transform.position - Diver.get.GetPosition()).magnitude;
		_lastControllerPosition = MainCamera.get.GetComponent<Camera>().ScreenToViewportPoint(_lastControllerPosition);
	
		return true;
	}	

	void UpdateDirectionHolder(){
		Vector3 cameraDirection_ = (RenderCamera.get.transform.position - Diver.get.transform.position).normalized;
		

		_directionArrow.transform.rotation = Quaternion.AngleAxis(GetUIAngle(), Vector3.back);
		_directionArrow.transform.position = Diver.get.transform.position + cameraDirection_ * 1.0f;
		
		RaycastHit2D hit_ = Physics2D.Raycast(_directionArrow.transform.position, _directionArrow.transform.rotation * Vector3.down, 3.0f, 1<<LayerMask.NameToLayer("Boundary"));
		float distance_ = 3.0f;
		if ( hit_.collider != null ){
			distance_ = ((Vector2)(_directionArrow.transform.position) - hit_.point).magnitude;
		}

		_directionArrow.transform.GetChild(0).transform.localPosition = Vector3.down * distance_;
	}

	void Update () {
		transform.eulerAngles += Vector3.forward * Time.deltaTime * 60.0f;
		UpdateDirectionHolder(); 

		if ( UpdateViewportPosition() ){	
			Vector3 world_ = RenderCamera.get.GetComponent<Camera>().ViewportToWorldPoint(_lastControllerPosition);
			transform.position = world_;
			SetUIVector();	
			Set();	
		}		
	}
}
