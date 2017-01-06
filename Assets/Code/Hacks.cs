using UnityEngine;
using System.Collections;

public static class Hacks {

    //ROUND TO X DIGITS
	public static float Round(float n, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(n * mult) / mult;
    }

    //ROTATE VECTOR2
    public static Vector2 RotateVector2(Vector2 v, float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        //(xcos - ysin, xsin + ycos)
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
