using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[System.Serializable]
public class SnapPoint{
	public Vector3 offset = Vector3.zero;
	public GameObject snapObject = null;
	public bool snapX = true;
	public bool snapY = true;
	public bool fromLeft = true;
	public bool fromRight = true;
	public bool facing = true;
	public bool backwards = true;
	public SnapManager.SnapType snapType;
	public float facingOffsetX = 0;
}

public class SnapTrigger : MonoBehaviour {
	[SerializeField]
	public List<SnapPoint> _snapPoints;

	public SnapPoint GetSnap(SnapManager.SnapType type){
		return _snapPoints.Find(x => x.snapType == type);
	}
}


