using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Diver))]
public class DiverEditor : BaseEditor<Diver> {

	protected override void Initialise(){
		if ( instance._angles == null){
			instance._angles = new List<float>();
		}
		while ( instance._angles.Count > (int)Diver.angles.Count ){
			instance._angles.RemoveAt(instance._angles.Count-1);
		}
		while (instance._angles.Count < (int)Diver.angles.Count) {
			instance._angles.Add(0);
		}
	}

	public override void OnInspector(){
		EditorGUILayout.LabelField("Snap Angles [deg]");
		for ( int i = 0; i < instance._angles.Count; ++i){
			instance._angles[i] = EditorGUILayout.FloatField(((Diver.angles)(i)).ToString(), instance._angles[i]); 
		}
		EditorGUILayout.LabelField("Swim " + instance._surfaceSwimmingAngle.ToString("0.000"));
		EditorGUILayout.LabelField("Idle " + instance._surfaceIdleSnapAngle.ToString("0.000"));
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Scene References");
		instance._surface = EditorGUILayout.ObjectField("Water", instance._surface, typeof(Env.Surface), true) as Env.Surface;
		instance._physics = EditorGUILayout.ObjectField("Diver Physics", instance._physics, typeof(DiverPhysics), true) as DiverPhysics;
		instance._bubbles = EditorGUILayout.ObjectField("Bubble Particle", instance._bubbles, typeof(ParticleSystem), true) as ParticleSystem;

		EditorGUILayout.Separator();
		instance._lazyThreshold = EditorGUILayout.FloatField("Lazy Threshold [s]", instance._lazyThreshold);
		instance._zeroDepth_ = EditorGUILayout.FloatField("Zero Lift Depth [m]", instance._zeroDepth_);

		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Stats");
		EditorGUILayout.LabelField("State:         \t" + instance.GetState().ToString() );
		EditorGUILayout.LabelField("Twisted Diver: \t" + (instance._twistedDiver ? "YES" : "NO") );
		EditorGUILayout.LabelField("Max Depth:     \t" + instance._maxDepth );
		EditorGUILayout.LabelField("Treasure Found:\t" + (instance._treasure ? "YES" : "NO") );
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}
}
