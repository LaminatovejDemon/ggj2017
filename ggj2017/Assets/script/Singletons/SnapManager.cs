using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class SnapManager : BaseManager<SnapManager> {
	List<SnapTrigger> _activeSnaps = null;
	public enum SnapType{
		None,
		SurfaceSit,
		StandJump,
		SitSuface,
		Stand,
	};

	void Initialise(){
		if ( _activeSnaps != null ){
			return;
		}
		_activeSnaps = new List<SnapTrigger>();
	}

	public void ActivateSnap(SnapTrigger target){
		Initialise();
		if ( !_activeSnaps.Contains(target) ){
			_activeSnaps.Add(target);
		}
	}

	public void DisableSnap(SnapTrigger target){
		_activeSnaps.Remove(target);
	}

	public void Reset(){
		_activeSnaps.Clear();
	}

	static public bool FindSnapType(SnapTrigger p, SnapType t){
		return p._snapPoints.Find(x => x.snapType == t) != null;
	}

	public SnapTrigger GetSnapTrigger(SnapType type){
		return _activeSnaps.Find(x => FindSnapType(x, type));
	}
	
	public bool IsSnap(SnapType type){
		if ( _activeSnaps == null ){
			return false;
		}
		
		SnapTrigger trigger_ = GetSnapTrigger(type);
		if (trigger_ == null ){
			return false;
		}
		SnapPoint point_ = trigger_.GetSnap(type);
		if ( point_ == null ){
			return false;
		}

		bool fromLeft_ = Diver.get.transform.position.x < (trigger_.transform.position.x + point_.facingOffsetX);
		bool facingLeft_ = DirectionMarker.get.IsDiverFacingLeft();
		bool facing_ = (fromLeft_ == !facingLeft_);
		

		if ((facing_ && !point_.facing) || !(facing_ || point_.backwards) ){
			Debug.Log (this.ToString() + "." + MethodBase.GetCurrentMethod() + ": "+ type + " has wrong facing");
			return false;
		}
		if ((!fromLeft_ && !point_.fromRight) || (fromLeft_ && !point_.fromLeft)) {
			Debug.Log (this.ToString() + "." + MethodBase.GetCurrentMethod() + ": "+ type + " has wrong access position");
			return false;
		}

		if ( DirectionMarker.get.GetSnapUIDot(trigger_, point_.facingOffsetX) > 0 ){
			Debug.Log (this.ToString() + "." + MethodBase.GetCurrentMethod() + ": "+ type + " has wrong UI direction");
			return false;
		}

		return true;
	}
}
