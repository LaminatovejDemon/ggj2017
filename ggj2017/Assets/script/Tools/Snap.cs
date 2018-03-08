using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[System.Serializable]
public class SnapPoint{
	public Vector3 offset;
	public bool fromLeft;
	public bool fromRight;
	public bool facing;
	public bool backwards;
	public SnapManager.SnapType snapType;
	public Vector3 lookAt;
}

public class Snap : MonoBehaviour {
	[SerializeField]
	public List<SnapPoint> _snapPoints;

	public SnapPoint GetSnap(SnapManager.SnapType type){
		return _snapPoints.Find(x => x.snapType == type);
	}
}


