using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UVRotateEditor")]
public class UVRotate : MonoBehaviour {
	
	[System.Serializable]
	public class UVSetting{
		public float SpeedX = 0f;
		public float OffsetX = 0f;
		public float SpeedY = 0f;
		public float OffsetY = 0f;
	}
	public int _propertyCount = 0;
	[SerializeField]
	public List<string> _propertyNames = null;
	[SerializeField]
	public List<UVSetting> _uvSettings;

	public void Initialise() {
		if ( _uvSettings == null){
			_uvSettings = new List<UVSetting>(); 
		}
		if ( _propertyNames == null ){
			_propertyNames = new List<string>();
		}
		
		if ( _uvSettings.Count != _propertyCount ){
			_uvSettings.Clear();
			for ( int i = 0; i < _propertyCount; ++i ){
				_uvSettings.Add(new UVSetting());
			}
		}
	}

	// Update is called once per frame
	void Update () {	
		Vector2 textureOffset_ = new Vector2();
		for ( int i = 0; i < _propertyCount; ++i ){
			if ( _uvSettings[i].SpeedX != 0 || _uvSettings[i].SpeedY != 0 ){ 
				Vector2 textureScale_ = GetComponent<MeshRenderer>().sharedMaterial.GetTextureScale(_propertyNames[i]);
;				textureOffset_.x = Mathf.Sin(Time.time * _uvSettings[i].SpeedX) * _uvSettings[i].OffsetX + transform.position.x * textureScale_.x;
				textureOffset_.y = Mathf.Cos(Time.time * _uvSettings[i].SpeedY) * _uvSettings[i].OffsetY + transform.position.y * textureScale_.y;
			 	GetComponent<MeshRenderer>().sharedMaterial.SetTextureOffset(_propertyNames[i], textureOffset_);
			}
		}
	}
}


