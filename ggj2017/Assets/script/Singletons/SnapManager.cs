using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;



public class SnapManager : BaseManager<SnapManager> {
	Snap _lastSnap = null;
	public enum SnapType{
		None,
		SurfaceSit,
		StandJump,
		SitSuface,
		Stand,
	};

	public void ActivateSnap(Snap target){
		if ( target!= null ){
			_lastSnap = target;
		}
	}

	public void DisableSnap(){
		_lastSnap = null;
	}

	public void DisableSnap(Snap target){
		if ( _lastSnap == target ){
			_lastSnap = null;
		}
	}

	public void Reset(){
		_lastSnap = null;
	}
	
	public bool IsSnap(SnapType type){
		if (_lastSnap == null ){
			return false;
		}
		SnapPoint point_ = _lastSnap.GetSnap(type);
		if ( point_ == null ){
			return false;
		}

		bool fromLeft_ = Diver.get.transform.position.x < (_lastSnap.transform.position.x + point_.lookAt.x);
		bool facingLeft_ = DirectionMarker.get.IsDiverFacingLeft();
		bool facing_ = (fromLeft_ == !facingLeft_);
		

		if ((facing_ && !point_.facing) || !(facing_ || point_.backwards) ){
			return false;
		}
		if ((!fromLeft_ && !point_.fromRight) || (fromLeft_ && !point_.fromLeft)) {
			return false;
		}

		if ( DirectionMarker.get.GetSnapUIDot(_lastSnap, point_.lookAt) > 0 ){
			return false;
		}

		return true;
	}

	public Snap GetSnap(){
		return _lastSnap;
	}

}
