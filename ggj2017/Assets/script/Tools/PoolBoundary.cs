using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PoolBoundary : MonoBehaviour {

	public Water.Surface _pool;
	public GameObject _bottomPanel;
	public GameObject  _leftPanel;
	public GameObject _rightPanel;
	public GameObject _backPanel;

	public float _thickness = 1;
	public float _spacer = 0.1f;

	void Update(){
		_bottomPanel.transform.localScale = new Vector3(
			_pool._surfaceWidth + _thickness * 2.0f + _spacer * 2.0f,
		 	_thickness, 
			_pool._surfaceHeight + _spacer );
		_bottomPanel.transform.position = _pool.transform.position + 
			Vector3.down * (_pool._depth + _spacer + _bottomPanel.transform.localScale.y * 0.5f) + 
			Vector3.forward * ((_pool._surfaceHeight + _spacer ) * 0.5f );

		_backPanel.transform.localScale = new Vector3(
			_bottomPanel.transform.localScale.x,
			_pool._depth + _spacer,
			_thickness );
		_backPanel.transform.position = _pool.transform.position + 
			Vector3.down * ((_pool._depth + _spacer ) * 0.5f) +
			Vector3.forward * (_pool._surfaceHeight + _spacer + _thickness * 0.5f);

		Vector3 sideOffset_ = Vector3.left * ( (_pool._surfaceWidth + _thickness) * 0.5f + _spacer) +
			Vector3.down * ( (_pool._depth + _spacer) * 0.5f ) +
			Vector3.forward * ( (_pool._surfaceHeight + _spacer) * 0.5f );	

		_leftPanel.transform.localScale = new Vector3(
			_thickness,
			_pool._depth + _spacer,
		 	_pool._surfaceHeight + _spacer );
		_leftPanel.transform.position = _pool.transform.position + sideOffset_;
		_rightPanel.transform.localScale = _leftPanel.transform.localScale;
		sideOffset_.x = -sideOffset_.x;
		_rightPanel.transform.position = _pool.transform.position + sideOffset_;
	}
}
