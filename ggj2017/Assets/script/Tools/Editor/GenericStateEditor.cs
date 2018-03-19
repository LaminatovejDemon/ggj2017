using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenericState))]
public class GenericStateEditor : BaseEditor<GenericState> {

	enum TF{
		False = 0,
		True = 1,
	};

	protected override void Initialise(){
		// Animation currentAnimation_ = instance.gameObject;
	}

	public override void OnInspector(){
		EditorGUILayout.BeginHorizontal();		
		instance.state = (Diver.state)EditorGUILayout.EnumPopup("Set State", instance.state);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("On Enter");
		EditorGUILayout.BeginHorizontal();
		instance._surfaceSnapModifier = EditorGUILayout.Toggle("Surface Snap", instance._surfaceSnapModifier);
		if ( instance._surfaceSnapModifier ){
			instance._surfaceSnapValue = (TF)(EditorGUILayout.EnumPopup((TF)(instance._surfaceSnapValue? 1 : 0))) == TF.False ? false : true;
		}
		EditorGUILayout.EndHorizontal();
		instance._doTwist = EditorGUILayout.Toggle("Do Twist", instance._doTwist);

		EditorGUILayout.BeginHorizontal();
		instance._surfaceAngleSnapModifier = EditorGUILayout.Toggle("Surface Angle Snap", instance._surfaceAngleSnapModifier);
		if ( instance._surfaceAngleSnapModifier ){
			instance._surfaceAngleSnapValue = (TF)(EditorGUILayout.EnumPopup((TF)(instance._surfaceAngleSnapValue? 1 : 0))) == TF.False ? false : true;
		}
		EditorGUILayout.EndHorizontal();

		if (instance._surfaceAngleSnapValue){
			EditorGUILayout.BeginHorizontal();
			instance._surfaceAngleSnapDataModifier = EditorGUILayout.Toggle("Angle Snap Value", instance._surfaceAngleSnapDataModifier);
			if ( instance._surfaceAngleSnapDataModifier ){
				instance._surfaceAngleSnapDataValue = (Diver.angles)(EditorGUILayout.EnumPopup(instance._surfaceAngleSnapDataValue));
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("During Animation");
		EditorGUILayout.BeginHorizontal();
		instance._matchTargetAngle = EditorGUILayout.Toggle("Match Target Angle", instance._matchTargetAngle);
		if ( instance._matchTargetAngle ){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Angles", GUILayout.MaxWidth(34));
			instance.twistAngle = (Diver.gangles)EditorGUILayout.EnumPopup(instance.twistAngle);
			instance.turnAngle = (Diver.angles)EditorGUILayout.EnumPopup(instance.turnAngle);
			EditorGUILayout.EndHorizontal();
		} 
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		instance._matchTargetPosition = EditorGUILayout.Toggle("Match Target Position", instance._matchTargetPosition);
		if ( instance._matchTargetPosition ){
			instance._snapPointType = (SnapManager.SnapType)EditorGUILayout.EnumPopup("Snap Type", instance._snapPointType);
			GUILayout.FlexibleSpace();
		} 
		EditorGUILayout.EndHorizontal();

		if ( instance._matchTargetPosition || instance._matchTargetAngle ){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Match Range");
			EditorGUILayout.LabelField(instance._matchMin.ToString("0.00"), GUILayout.MaxWidth(30));
			EditorGUILayout.MinMaxSlider(ref instance._matchMin, ref instance._matchMax, 0, 1f);
			EditorGUILayout.LabelField(instance._matchMax.ToString("0.00"), GUILayout.MaxWidth(30));
			EditorGUILayout.EndHorizontal();
		}
	}
}
