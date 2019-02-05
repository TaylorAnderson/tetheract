using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier {

	public static Vector2 Lerp(Vector2 a, Vector2 b, float t) {
        return a + (b - a) * t;
    }

    public static Vector2 QuadraticCurve(Vector2 a, Vector2 b, Vector2 c, float t) {
        Vector2 p0 = Lerp(a, b, t);
        Vector2 p1 = Lerp(b, c, t);
        return Lerp(p0, p1, t);
    }
    public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t) {
        Vector2 p0 = Vector2.Lerp(a, b, t);
        Vector2 p1 = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(p0, p1, t);
    }
    public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t) {
        Vector2 p0 = EvaluateQuadratic(a, b, c, t);
        Vector2 p1 = EvaluateQuadratic(b, c, d, t);
        return Vector2.Lerp(p0, p1, t);
    }
}
