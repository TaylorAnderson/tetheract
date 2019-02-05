using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path {

    [SerializeField]
    //this will contain both anchor and control points
    List<Vector2> points;
    
    [SerializeField]
    bool isClosed;

    [SerializeField]
    bool autoSetControlPoints;

    public Vector2 center;

    public Path(Vector2 center) {
        this.center = center;
        //building two anchors and two control points
        points = new List<Vector2> {
            center + Vector2.left, 
            center + (Vector2.left + Vector2.up)    *0.5f, 
            center + (Vector2.right + Vector2.down) *0.5f,
            center + Vector2.right
        };
    }

    //just giving the edtor an easy way to access the points.
    public Vector2 this[int i] {
        get {
            return points[i];
        }
    }

    public int NumPoints {
        get { return points.Count; }
    }

    //for the first segment we have four points (two anchors, two controls), but for subsequent segments we have three points
    //because we only add another control point for the last anchor, and then an anchor and a control point.
    //this could be done more literally with (points.Count-4)/3 + 1 + (isClosed ? 1 : 0).  
    //but we can actually get what we want simpler by just dividing points count by 3 and flooring the result.
    public int NumSegments {
        get {
            return points.Count/3;
        }
    }

    public Vector2 LastAnchor {
        get {
            return points[points.Count - 1];
        }
    }
    public Vector2 LastControl {
        get {
            return points[points.Count - 2];
        }
    }
    public bool AutoSetControlPoints {
        get {
            return autoSetControlPoints;
        }
        set {
            if (autoSetControlPoints != value) {
                autoSetControlPoints = value;
                if (autoSetControlPoints) {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public bool IsClosed {
        get {
            return isClosed;
        }
        set {
            if (isClosed != value) {
                isClosed = value;

                if (isClosed) {

                    Vector2 firstAnchor = points[0];
                    Vector2 firstControl = points[1];
                    Vector2 nextControl = LastAnchor + (LastAnchor - LastControl);
                    //give the last anchor a second control point
                    points.Add(nextControl);
                    //give the first anchor a second control point
                    points.Add(firstAnchor * 2 - firstControl);

                    if (autoSetControlPoints) {
                        AutoSetAnchorControlPoints(0);
                        AutoSetAnchorControlPoints(points.Count-3);
                    }
                }
                else {
                    //remove the control points we created to close the path
                    points.RemoveRange(points.Count - 2, 2);

                    if (autoSetControlPoints) {
                        AutoSetStartAndEndControls();
                    }
                }
            }
        }
    }

    public void AddSegment(Vector2 anchorPos) {
        Vector2 nextControl = LastAnchor + (LastAnchor - LastControl);
        //first control, attached to the last anchor
        points.Add(nextControl);
        //second control point, attached to new anchor
        points.Add((nextControl + anchorPos) * 0.5f);
        //new anchor
        points.Add(anchorPos);

        if (autoSetControlPoints) {
            AutoSetAllEffectedControlPoints(points.Count - 1);
        }
    }

    public void SplitSegment(Vector2 anchorPos, int segmentIndex) {
        points.InsertRange(segmentIndex * 3 + 2, new Vector2[] { Vector2.zero, anchorPos, Vector2.zero });
        if (autoSetControlPoints) {
            AutoSetAllEffectedControlPoints(segmentIndex * 3 + 3);
        }
        else {
            AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
        }
    }

    public Vector2[] GetPointsInSegment(int i) {
        int segmentBegin = i * 3;
        return new Vector2[] {
            points[segmentBegin],
            points[segmentBegin+1],
            points[segmentBegin+2],
            //the anchor point here could exist on the other end of the array if we have a closed path
            points[LoopIndex(segmentBegin+3)]
        };
    }

    public void MovePoint(int i, Vector2 pos) {
        Vector2 deltaMove = pos - points[i];

        if (autoSetControlPoints) {
            AutoSetAllEffectedControlPoints(i);
        }

        if (i % 3 == 0 || !autoSetControlPoints) {
            points[i] = pos;
            if (i % 3 == 0) {
                //we're moving an anchor point, so also move its control points
                if (i + 1 < points.Count || (isClosed)) { points[LoopIndex(i + 1)] += deltaMove; }
                if (i - 1 >= 0 || isClosed) { points[LoopIndex(i - 1)] += deltaMove; }
            }
            else {
                //we're moving a control point
                bool nextPointIsAnchor = (i + 1) % 3 == 0;
                int correspondingControlIndex = nextPointIsAnchor ? i + 2 : i - 2;
                int anchorIndex = LoopIndex(nextPointIsAnchor ? i + 1 : i - 1);

                //if we have an in-range corresponding control point,
                //move it so that the two control points always form a straight line.
                if (IsInRange(correspondingControlIndex)) {
                    float dist = (points[anchorIndex] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                    Vector2 dir = (points[anchorIndex] - pos).normalized;
                    points[LoopIndex(correspondingControlIndex)] = points[anchorIndex] + dir * dist;
                }
            }
        }

    }
    private int LoopIndex(int i) {
        //we add points.Count to handle negative values
        return (i + points.Count) % points.Count;
    }

    void AutoSetAllEffectedControlPoints(int updatedAnchorIndex) {
        //only update the anchor and its neighbours
        for (int i = updatedAnchorIndex-3; i <= updatedAnchorIndex + 3; i+=3)
        {
            if (IsInRange(i)) {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }
        AutoSetAnchorControlPoints(updatedAnchorIndex);
    }
    void AutoSetAllControlPoints() {
        for (int i = 0; i < points.Count; i+=3)
        {
            AutoSetAnchorControlPoints(i);
        }
        AutoSetStartAndEndControls();
    }

    //to set the control points to be perpendicular to the angle this anchor makes with its neighbours.
    //(makes the curve nice)
    void AutoSetAnchorControlPoints(int anchorIndex) {
        Vector2 anchorPos = points[anchorIndex];
        Vector2 direction = Vector2.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0 || isClosed) {
            //get the first anchor offset
            Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            direction += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
        
        if (anchorIndex + 3 < points.Count || isClosed) {
            //get the second anchor offset
            Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
            direction -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }
        direction.Normalize();

        
        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (IsInRange(controlIndex)) {
                points[LoopIndex(controlIndex)] = anchorPos + direction * neighbourDistances[i] * 0.5f;
            }
        }
    }

    //for start and end control points, we it halfway between the anchor point and the next control point
    void AutoSetStartAndEndControls() {
        if (!isClosed) {
            points[1] = (points[0] + points[2]) * 0.5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3])*0.5f;
        }
    }

    bool IsInRange(int index) {
        return (index >= 0 && index < points.Count) || isClosed;
    }

    public void DeleteSegment(int anchorIndex) {
        if (NumSegments > 2 || (!isClosed && NumSegments > 1)) {
            if (anchorIndex == 0) {
                if (isClosed) {
                    points[points.Count - 1] = points[2];
                }
                points.RemoveRange(0, 3);

            }
            else if (anchorIndex == points.Count - 1 && !isClosed) {
                points.RemoveRange(anchorIndex - 2, 3);
            }
            else {
                points.RemoveRange(anchorIndex - 1, 3);
            }
        }
    }
    public Vector2[] CalculateEvenlySpacedPoints (float spacing, float resolution = 1) {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(points[0]);
        Vector2 previousPoint = points[0];
        float distSinceLastEvenPoint = 0;
        for (int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++)
        {
            Vector2[] p = GetPointsInSegment(segmentIndex);
            float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength/2f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            int whileLoopSafety = 100;
            while (t<=1) {
                t += 1f / divisions;
                whileLoopSafety--;
                Vector2 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                distSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                while (distSinceLastEvenPoint >= spacing) {
                    float overshootDist = distSinceLastEvenPoint - spacing;
                    Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDist;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    distSinceLastEvenPoint = overshootDist;
                    previousPoint = newEvenlySpacedPoint;
                }
                previousPoint = pointOnCurve;
            }
        }
        return evenlySpacedPoints.ToArray();
    }
    
}
