using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerHandler<T> : MonoBehaviour{

	List<T> _listeners;
	public void RegisterListener(T source)
	{
		if ( _listeners == null ){
			_listeners = new List<T>();
		}

		if (_listeners == null) {
			_listeners = new List<T> ();
		}

		if (_listeners.Contains (source)) {
			return;
		}

		_listeners.Add (source);
		NewListener(source);
	}

	protected delegate void OpenDelegate(T explicitThis);
	protected void NotifyListeners(OpenDelegate openDelegate){
		
		for (int i = 0; i < _listeners.Count; ++i) {
			openDelegate(_listeners[i]);
		}
	}

	NewListenerListener _newListener = null;
	public delegate void NewListenerListener(T source);

	public void RegisterNewListenerListener(NewListenerListener newListener){
		_newListener = newListener;
	}

	protected virtual void OnNewListener(T source){}
	
	void NewListener(T source){
		if ( _newListener != null ){
			_newListener(source);
		} else {
			OnNewListener(source);
		}
	}
}
