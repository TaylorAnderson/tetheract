using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {

    PathCreator creator;
    Path Path {
		get {
            return creator.path;
        }
	}

    const float segmentSelectDistThreshold = 0.1f;
    int selectedSegmentIndex = -1;

    private void OnEnable() {
        creator = (PathCreator)target;
		if (creator.path == null) {
            creator.CreatePath();
        }
    }



    

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Create New")) {
			 Undo.RecordObject(creator, "Create new");
            creator.CreatePath();
        }


        

        bool autoSetControlPoints = GUILayout.Toggle(Path.AutoSetControlPoints, "Auto Set Control Points");

        if (autoSetControlPoints != Path.AutoSetControlPoints) {
            Undo.RecordObject(creator, "Toggle auto set controls");
            Path.AutoSetControlPoints = autoSetControlPoints;
        }

        bool isClosed = GUILayout.Toggle(Path.IsClosed, "Closed");

		if (Path.IsClosed != isClosed) {
            Undo.RecordObject(creator, "Toggle closed");
            Path.IsClosed = isClosed;
        }

        if (EditorGUI.EndChangeCheck()) {
            SceneView.RepaintAll();
        }
    }
    
	void OnSceneGUI() {
        creator.transform.hasChanged = false;
        var move = (Vector2) creator.transform.position - Path.center;
            
        for (int i = 0; i < Path.NumPoints; i++) {
            bool isAnchorPoint = i % 3 == 0;
            if (isAnchorPoint) {
                Path.MovePoint(i, Path[i] + move);
            }
        }
        Path.center = creator.transform.position;

        //drawing
        for (int i = 0; i < Path.NumSegments; i++) {
            Vector2[] points = Path.GetPointsInSegment(i);

			if (creator.displayControlPoints) {
				Handles.color = Color.black;
				Handles.DrawLine(points[1], points[0]);
				Handles.DrawLine(points[2], points[3]);
			}


            Color segmentCol = (i == selectedSegmentIndex && Event.current.shift ? creator.selectedSegmentColor : creator.segmentColor);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2);
        }
		
        for (int i = 0; i < Path.NumPoints; i++) {
            bool isAnchorPoint = i % 3 == 0;
			if (isAnchorPoint || creator.displayControlPoints) {
            	Handles.color = isAnchorPoint ? creator.anchorColor : creator.controlColor;
           	 	Vector2 newPosition = Handles.FreeMoveHandle(Path[i], Quaternion.identity, isAnchorPoint ? creator.anchorDiameter : creator.controlDiameter, Vector2.zero, Handles.CylinderHandleCap);
				//make sure we only go through with the trouble of recording an undo object and calling MovePoint if the path has actually changed
                if (Path[i] != newPosition) {
					Undo.RecordObject(creator, "Move Point");
                	Path.MovePoint(i, newPosition);
            	}
			}
        }


        //input
        Event e = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

		if (e.type == EventType.MouseDown && e.button == 0 && e.shift) {

			if (selectedSegmentIndex != -1) {
                Undo.RecordObject(creator, "Split Segment");
                Path.SplitSegment(mousePos, selectedSegmentIndex);
            }
			else if (!Path.IsClosed) {
				Undo.RecordObject(creator, "Add Segment");
				Path.AddSegment(mousePos);
			}

        }
		if (e.type == EventType.MouseDown && e.button == 1) {
            float minDist = creator.anchorDiameter * 0.5f;
            int closestAnchorIndex = -1;
			for (int i = 0; i < Path.NumPoints; i+=3)
			{
                float dist = Vector2.Distance(mousePos, Path[i]);
				if (dist < minDist) {
                    minDist = dist;
                    closestAnchorIndex = i;
                }
            }

			if (closestAnchorIndex > -1) {
                Undo.RecordObject(creator, "Delete Segment");
                Path.DeleteSegment(closestAnchorIndex);
            }
        }

		if (e.type == EventType.MouseMove) {
			float minDistToSegment = segmentSelectDistThreshold;
			int newSelectedSegmentIndex = -1;
			for (int i = 0; i < Path.NumSegments; i++)
			{
				Vector2[] points = Path.GetPointsInSegment(i);
				float dist = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
				if (dist < minDistToSegment) {
					minDistToSegment = dist;
					newSelectedSegmentIndex = i;
				}

			}

			if (newSelectedSegmentIndex != selectedSegmentIndex) {
                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
		}

		HandleUtility.AddDefaultControl(0);
    }

}
