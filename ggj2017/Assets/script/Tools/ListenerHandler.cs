using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListenerHandler<T> : MonoBehaviour {

	List<T> _animationListeners;
	public void RegisterAnimationListener(T source)
	{
		if ( _animationListeners == null ){
			_animationListeners = new List<T>();
		}

		if (_animationListeners == null) {
			_animationListeners = new List<T> ();
		}

		if (_animationListeners.Contains (source)) {
			return;
		}

		_animationListeners.Add (source);
		OnNewListener(source);
	}

	protected delegate void OpenDelegate(T explicitThis);
	protected void NotifyListeners(OpenDelegate openDelegate){
		
		for (int i = 0; i < _animationListeners.Count; ++i) {
			openDelegate(_animationListeners[i]);
		}
	}

	protected abstract void OnNewListener(T source);
}
