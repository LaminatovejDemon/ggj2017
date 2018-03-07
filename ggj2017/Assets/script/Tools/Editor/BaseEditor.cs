using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseEditor<T> : Editor  where T: Object{

	public T instance;
	void Init(){
		instance = target as T;
	}

	public sealed override void OnInspectorGUI() {
		Init();
		Initialise();
		OnInspector();
	}

	protected virtual void Initialise(){

	}
	public abstract void OnInspector();
}

