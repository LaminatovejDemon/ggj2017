using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class depthColor : MonoBehaviour {

	public Color[] _Colors;
	public Color _actualColor;
	public int _range = 100;
	public int indexFrom_;
	public int indexTo_;
	public float currentValue_;

	void Update () {


		float value_ = -RenderCamera.get.transform.position.y / _range;
		indexFrom_ = Mathf.Max (0, (int)(value_ * _Colors.Length));
		indexTo_ = Mathf.Min (_Colors.Length - 1, (int)(value_ * _Colors.Length) + 1);
		currentValue_ = (value_ * _Colors.Length) - (int)(value_ * _Colors.Length);



		_actualColor = value_ >= 1.0f ? _Colors[indexTo_] : Color.Lerp (_Colors [indexFrom_], _Colors [indexTo_], currentValue_); 
		GetComponent<MeshRenderer>().material.SetColor("_TintColor", _actualColor);
	}
}
