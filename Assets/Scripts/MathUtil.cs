using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public static Vector2 RadiansToVector2 (float radians) {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
    public static Vector2 DegreesToVector2 (float degrees) {
        return RadiansToVector2(degrees * Mathf.Deg2Rad);
    }

    //Maps the value from the first range to the second range.  For example, values 0.5, 0, 1, 0, 100 would produce 50.
    public static float map (float value, float fromMin, float fromMax, float toMin, float toMax) {
        return toMin + ((value - fromMin) / (fromMax - fromMin)) * (toMax - toMin);
    }
}
