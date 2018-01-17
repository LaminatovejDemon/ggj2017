using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTarget : MonoBehaviour {

	int _resolutionY; 
	public int ResolutionY = 10;

	public Camera _targetCamera;
	public Camera _renderCamera;

	void UpdateTexture(){

		int target_;

		target_ = (ResolutionY < 1 || ResolutionY > Screen.width) ? Screen.width : ResolutionY;

		if (target_ == _resolutionY) {
			return;
		}
		_resolutionY = target_;

		if (_renderCamera.targetTexture != null) {
			_renderCamera.targetTexture.Release ();
		}

		int sizeY = _resolutionY;
		int sizeX = (int)(_resolutionY * _targetCamera.aspect);
		RenderTexture newTexture_ = new RenderTexture (sizeX, sizeY, 8);
		newTexture_.useMipMap = false;
		newTexture_.filterMode = FilterMode.Point;
		_renderCamera.targetTexture = newTexture_;

		GetComponent<MeshRenderer> ().material.mainTexture = newTexture_;
	}

	// Use this for initialization
	void Start () {
		Vector3 size = transform.localScale;
		size.y = _targetCamera.orthographicSize * 2;
		size.x = _targetCamera.orthographicSize * 2 * _targetCamera.aspect;
		transform.localScale = size;

		UpdateTexture();
	}
	
	// Update is called once per frame
	void Update () {
		if (ResolutionY != _resolutionY) {
			UpdateTexture ();
		}
	}
}
