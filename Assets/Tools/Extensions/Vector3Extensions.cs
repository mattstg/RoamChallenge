using System.Runtime.CompilerServices;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 V2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 SetSign(this Vector3 v, bool setX, bool setY)
    {
        return new Vector3(Mathf.Abs(v.x) * MathHelper.BooleanSign(setX), Mathf.Abs(v.y) * MathHelper.BooleanSign(setY), v.z);
    }

    public static Vector3 SetXSign(this Vector3 v, bool positive)
    {
        return new Vector3(Mathf.Abs(v.x) * MathHelper.BooleanSign(positive), v.y, v.z);
    }

    public static Vector3 SetYSign(this Vector3 v, bool positive)
    {
        return new Vector3(v.x, Mathf.Abs(v.y) * MathHelper.BooleanSign(positive), v.z);
    }

    public static Vector3 FlipSign(this Vector3 v, bool flipX, bool flipY)
    {
        return new Vector3((flipX) ? v.x * -1 : v.x, (flipY) ? v.y * -1 : v.y, v.z);
    }

    public static Vector3 Abs(this Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion EulerToQuat(this Vector3 toQuat)
    {
        Quaternion temp = Quaternion.identity;
        temp.eulerAngles = new Vector3(toQuat.x, toQuat.y, toQuat.z);
        return temp;
    }
}
