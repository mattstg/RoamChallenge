using System.Runtime.CompilerServices;
using UnityEngine;

public static class Vector2Extensions
{
    public enum V2AngleMode
    {
        Angle,
        AngleAlt,
        /*ThirdType,
        CCW_AngleAlt,
        ZeroTo360*/
    }

    public static Vector2Int V2Int(this Vector2 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }

    public static Vector3 V3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y));
    }

    public static Vector2 Clamp01(this Vector2 vector)
    {
        return new Vector2(Mathf.Clamp(vector.x, min: 0, max: 1), Mathf.Clamp(vector.y, min: 0, max: 1));
    }

    public static Vector2 Mult(this Vector2 v, Vector2 multBy)
    {
        return new Vector2(v.x * multBy.x, v.y * multBy.y);
    }

    public static Vector3 Rotate(this Vector3 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static bool IsValid(this Vector2 v)
    {
        return !(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNegativeInfinity(v.x) || float.IsNegativeInfinity(v.y) || float.IsPositiveInfinity(v.x) || float.IsPositiveInfinity(v.y));
    }

    public static bool IsValid(this float f)
    {
        return !(float.IsNaN(f) || float.IsNegativeInfinity(f) || float.IsPositiveInfinity(f));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float rad = Mathf.Deg2Rad * degrees;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        float tx = v.x;
        float ty = v.y;
        return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
    }

    public static Vector2 Abs(this Vector2 v)
    {
        return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }

    public static bool ApproxEqual(this Vector2 a, Vector2 b, float tolerance)
    {
        return Vector2.Distance(a, b) < tolerance;
    }
    public static float ToAng(this Vector2 v, V2AngleMode angMode = V2AngleMode.Angle)
    {
        switch (angMode)
        {
            case V2AngleMode.Angle:
                return Angle(v);
            case V2AngleMode.AngleAlt:
                return AngleAlt(v);
            /*case V2AngleMode.CCW_AngleAlt:
                return MathHelper.UnAngleAlt(v);
            case V2AngleMode.ThirdType:
                var vector1 = new Vector2(1, 0); // 12 o'clock == 0°, assuming that y goes from bottom to top
                return Mathf.Rad2Deg*(Mathf.Atan2(v.y, v.x) - Mathf.Atan2(vector1.y, vector1.x));
            case V2AngleMode.ZeroTo360:
                return Mathf.Rad2Deg*Mathf.Atan2(v.y, v.x);
            */
            default:
                Debug.Log("Unhandled switch, returning normal angle mode");
                goto case V2AngleMode.Angle;
        }
    }

	public static float Angle(this Vector2 v)
	{
		return Vector2.Angle(Vector2.right, v);
	}

	public static float SignedAngle(Vector2 v1, Vector2 v2)
	{
		return Vector2.Angle(v1, v2) * Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
	}

    public static int GetThresholdIndex(this Vector2 v2, float value)
    {
        if (value < v2.x)
            return 0;
        else if (value < v2.y)
            return 1;
        else
            return 2;
    }

    public static float SurfaceArea(this Vector2 v)
    {
        return v.x * v.y;
    }

    public static Vector2 SetXSign(this Vector2 v, bool positive)
    {
        return new Vector2(Mathf.Abs(v.x) * MathHelper.BooleanSign(positive), v.y);
    }

    public static bool IsBetween(this Vector2 v, float v2)
    {
        return v2 >= v.x && v2 <= v.y;
    }

    public static float Random(this Vector2 v)
    {
        return UnityEngine.Random.Range(v.x, v.y);
    }

    public static int Random(this Vector2Int v)
    {
        return UnityEngine.Random.Range(v.x, v.y + 1);
    }

    public static int RandomValueInRangeInclusive(this Vector2Int v)
    {
        return UnityEngine.Random.Range(v.x, v.y + 1);  //+1 cause exclusive
    }

    public static int Area(this Vector2Int v)
    {
        return v.x * v.y;
    }
    public static Vector2 GetRandomPosition(this Vector2 v, Vector2 size)
    {
        return new Vector2(UnityEngine.Random.Range(v.x - size.x / 2, v.x + size.x / 2), UnityEngine.Random.Range(v.y - size.y / 2, v.y + size.y / 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AngleAlt(this Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        if (vector.y < 0f)
            angle += 360f;
        return angle % 360f;
    }

}
