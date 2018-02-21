using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> : MonoBehaviour where T : Component{

	public static T get{
		get {
			if ( _instance == null ){
				_instance = GameObject.FindObjectOfType<T>();
				if ( _instance == null ){
					GameObject template_ = Resources.Load(typeof(T).ToString()) as GameObject;
					T templateComponent_ = template_ == null ? null : template_.GetComponent<T>();
					_instance = templateComponent_ != null ? GameObject.Instantiate<T>(templateComponent_) : new GameObject().AddComponent<T>();
					_instance.gameObject.name = "#" + typeof(T).ToString();
				}
			}
			return _instance;
		}
		private set {
			_instance = value;
		}
	}

	public static bool Exists() {
		return _instance != null;
	}

	private static T _instance;
}
