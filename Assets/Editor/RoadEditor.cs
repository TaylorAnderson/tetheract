using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor {

    RoadCreator creator;

	private void OnEnable() {
        creator = (RoadCreator)target;
    }
	override public void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Collider")) {
            creator.GenerateCollider();
        }
	}
	private void OnSceneGUI() {
		if (creator.autoUpdate && Event.current.type == EventType.Repaint) {
            creator.UpdateRoad();
        }
	}
}
