using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseEditModeEditor<T> : BaseEditor<T> where T: MonoBehaviour {
	GUIStyle _toggleButtonNormal = null, _toggleButtonToggled = null;
	protected void EditModeToggle(){

		if ( _toggleButtonNormal == null ){
			_toggleButtonNormal = "Button";
			_toggleButtonToggled = new GUIStyle(_toggleButtonNormal);
			_toggleButtonToggled.normal.background = _toggleButtonToggled.active.background;
		}

		if ( GUILayout.Button(new GUIContent("E", "Run in edit mode"), (instance.runInEditMode ? _toggleButtonToggled : _toggleButtonNormal) )){
				instance.runInEditMode = !instance.runInEditMode;
			}
			if ( instance.runInEditMode ){
				EditorUtility.SetDirty(instance);
			}
	}
}
