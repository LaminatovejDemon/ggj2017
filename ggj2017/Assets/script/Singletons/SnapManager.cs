using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class SnapManager : BaseManager<SnapManager> {
	Snap _lastSnap = null;

	public void ActivateSnap(Snap target){
		if ( target!= null ){
			Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": adding " + target);
			_lastSnap = target;
		} else {
			Debug.LogWarning(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": adding FAILED");
		}
	}

	public void DisableSnap(){
		_lastSnap = null;
	}

	public void DisableSnap(Snap target){
		if ( _lastSnap == target ){
			Debug.Log(this.ToString() + "." + MethodBase.GetCurrentMethod().Name + ": removing " + target); 
			_lastSnap = null;
		}
	}

	public void Reset(){
		_lastSnap = null;
	}
	
	public bool IsSnap(){
		if (_lastSnap == null){
			return false;
		}

		bool fromLeft_ = Diver.get.transform.position.x < _lastSnap.transform.position.x;
		bool facingLeft_ = DirectionMarker.get.IsDiverFacingLeft();
		bool facing_ = (fromLeft_ == !facingLeft_);

		if ((facing_ && !_lastSnap.facing) || !(facing_ || _lastSnap.backwards) ){
			return false;
		}
		if ((!fromLeft_ && !_lastSnap.fromRight) || (fromLeft_ && !_lastSnap.fromLeft)) {
			return false;
		}
		return true;
	}

	public Snap GetSnap(){
		return _lastSnap;
	}

}
