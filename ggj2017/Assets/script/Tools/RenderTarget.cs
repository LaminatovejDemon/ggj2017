using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTarget : MonoBehaviour {

	int _resolutionY; 
	public int ResolutionY = 10;

	void UpdateTexture(){

		int target_;

		target_ = (ResolutionY < 1 || ResolutionY > Screen.width) ? Screen.width : ResolutionY;

		if (target_ == _resolutionY) {
			return;
		}
		_resolutionY = target_;

		if (RenderCamera.get.GetComponent<Camera>().targetTexture != null) {
			RenderCamera.get.GetComponent<Camera>().targetTexture.Release ();
		}

		int sizeY = _resolutionY;
		int sizeX = (int)(_resolutionY * MainCamera.get.GetComponent<Camera>().aspect);
		RenderTexture newTexture_ = new RenderTexture (sizeX, sizeY, 8);
		newTexture_.useMipMap = false;
		newTexture_.filterMode = FilterMode.Point;
		RenderCamera.get.GetComponent<Camera>().targetTexture = newTexture_;

		GetComponent<MeshRenderer> ().material.mainTexture = newTexture_;
	}

	// Use this for initialization
	void Start () {
		Vector3 size = transform.localScale;
		size.y = MainCamera.get.GetComponent<Camera>().orthographicSize * 2;
		size.x = MainCamera.get.GetComponent<Camera>().orthographicSize * 2 * MainCamera.get.GetComponent<Camera>().aspect;
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
