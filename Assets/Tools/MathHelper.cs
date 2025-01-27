using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MathHelper  {


    #region Lightning Methods
    //Lightning methods are approximations of the actual methods that are waaaaaaay faster than thier normal alternatives... Enough to be worth

    //this is a stupid fast Acos approximation with maximum 10.31 degrees error
    const float maxValue_Acos = 1f;
    const float minValue_Acos = -1f;
    const float Acos_Range = (maxValue_Acos - minValue_Acos);
    static float maxInputValue_Acos = Acos_Range * Acos_Range;
    const float resolution_Acos = 0.01f;
    static float[] acosCache;

    static void FillAcosCache()
    {
        int values = (int)(maxInputValue_Acos / resolution_Acos);
        acosCache = new float[values];
        for (int i = 0; i < values; i++)
        {
            float toAcos = (i * resolution_Acos) + minValue_Acos;
            acosCache[i] = Mathf.Acos(toAcos);
        }
       //TestAcosFaster();
    }

    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static float LightningAcos(float x)
    {
        //this is fast... but has some error... may not be accurate enough for some cases (ie. extremely shalow win cones)
        if (x > maxValue_Acos)
            x = maxValue_Acos;
        else if (x < minValue_Acos)
            x = minValue_Acos;
        try
        {
            return acosCache[(int)((x - minValue_Acos) / resolution_Acos)];
        }
        catch (System.Exception e)
        {
            Debug.Log("x: " + x + " index: " + (int)((x - minValue_Acos) / resolution_Acos));
            throw e;
        }
    }

    /*public static void TestAcosFaster(int tests = 100000)
    {
        long ticks = System.DateTime.UtcNow.Ticks;
        for (int i = 0; i < tests; i++)
        {
            float value = Random.Range(-1f, 1f);
            LightningAcos(value);
        }

        long lightningDuration = (System.DateTime.UtcNow.Ticks - ticks);
        Debug.Log(lightningDuration  + " ticks for LightningAcos");

        ticks = System.DateTime.UtcNow.Ticks;
        for (int i = 0; i < tests; i++)
        {
            float value = Random.Range(-1f, 1f);
            Mathf.Acos(value);
        }
        long normalAcos = (System.DateTime.UtcNow.Ticks - ticks);
        Debug.Log(normalAcos + " ticks for normal Acos");

        if(lightningDuration < normalAcos)
        {
            Debug.Log("Lightning faster by " + (normalAcos - lightningDuration) + " ticks");
        }
        else
        {
            Debug.Log("Normal faster by " + (lightningDuration - normalAcos) + " ticks");
        }
    }*/

    public static float Sqrt(float x)
    {
        return Mathf.Sqrt(x);
    }

    public static void FillCache()
    {
        FillAcosCache();
    }

    #endregion
    const float epsilon = 0.001f;

    #region Elipse Stuff
    //https://www.youtube.com/watch?v=Or3fA-UjnwU
    public static Vector2 GetPointOnEllipseByAngle(float ellipseWidth, float ellipseHeight, float angle)
    {
        return new Vector2(Mathf.Sin(angle) * ellipseWidth, Mathf.Cos(angle) * ellipseHeight);
    }
    #endregion

    public static bool IsApproximately(this float a, float b)
    {
        float difference = a - b;
        return difference > -epsilon && difference < epsilon;
    }
    
    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    public static Vector2 DegreeToVector2(float degree, float length)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad) * length;
    }
    public static float AreaOfCircle(float radius)
    {
        return Mathf.Pow(radius, 2) * Mathf.PI;
    }

    //Returns true/false if found intercept, use out to get value -- This seems pricy
    public static bool CalculateIntercept(Vector2 src, Vector2 dst, float v, Vector2 dstVelo, out Vector2 interceptPt)
    {
        float tx = dst.x - src.x;
        float ty = dst.y - src.y;
        float tvx = dstVelo.x;
        float tvy = dstVelo.y;

        // Get quadratic equation components
        var a = tvx * tvx + tvy * tvy - v * v;
        var b = 2 * (tvx * tx + tvy * ty);
        var c = tx * tx + ty * ty;

        // Solve quadratic
        Vector2 sol;

        if (quad(a, b, c, out sol))
        {
            var t0 = sol[0];
            var t1 = sol[1];
            var t = Mathf.Min(t0, t1);
            if (t < 0) t = Mathf.Max(t0, t1);
            if (t > 0)
            {
                sol = new Vector2(dst.x + dstVelo.x * t, dst.y + dstVelo.y * t);
                interceptPt = sol;
                return true; //found intercept
            }
        }
        interceptPt = sol;
        return false; //failed no intercept
    }

    /**
     * Return solutions for quadratic
     */
    static bool quad(float a, float b, float c, out Vector2 result)
    {
        Vector2 sol = new Vector2();
        if (Mathf.Abs(a) < 1e-6)
        {
            if (Mathf.Abs(b) < 1e-6)
            {
                if (Mathf.Abs(c) < 1e-6)
                    sol = new Vector2(0, 0);
                else
                {
                    sol = new Vector2(0, 0);
                    //Debug.LogError("This was spouse to make the interception null?");
                    result = sol;
                    return false;
                }
            }
            else
            {
                sol = new Vector2(-c / b, -c / b);
            }
        }
        else
        {
            var disc = b * b - 4 * a * c;
            if (disc >= 0)
            {
                disc = Mathf.Sqrt(disc);
                a = 2 * a;
                sol = new Vector2((-b - disc) / a, (-b + disc) / a);
            }
        }
        result = sol;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 DegreeToVector2Normalized(float degree)
    {
        return (RadianToVector2(degree * Mathf.Deg2Rad) * 0.5f) + new Vector2(0.5f, 0.5f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 RotateV2(Vector2 v, float deg)
    {
        float sin = Mathf.Sin(deg * Mathf.Deg2Rad);
        float cos = Mathf.Cos(deg * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 RotateV3(Vector3 v, float deg)
    {
        float sin = Mathf.Sin(deg * Mathf.Deg2Rad);
        float cos = Mathf.Cos(deg * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        v.z = 0;
        return v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SigmoidVelocity(float progress)
    {
        if (progress >= 0 && progress <= 1)
            return -4 * Mathf.Abs(progress - 0.5f) + 2;
        else
            return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sigmoid(float progress)
    {
        if (progress <= 0)
            return 0;
        else if (progress >= 1)
            return 1;
        else if (progress <= 0.5f)
            return progress * SigmoidVelocity(progress) / 2;
        else
            return 0.5f + ((progress - 0.5f) * (SigmoidVelocity(progress) + 2) / 2);
    }

    const float MaxValue = 1E10f;
    const float MinValue = MaxValue * -1f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsAbsurd(this float t)
    {
        return t > MaxValue || t < MinValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAbsurd(this Vector2 t)
    {
        return IsAbsurd(t.x) || IsAbsurd(t.y);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(float v, float min, float max, bool inclusive = true)
    {
        if (!inclusive && (v == min || v == max))
            return false;
        else
            return (v >= min && v <= max);
    }

    /// <summary>
    /// Returns 1 or -1 (True or False)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BooleanSign(bool b)
    {
        return (b) ? 1 : -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CoinFlip()
    {
        return Random.Range(0, 2) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IncreaseLoopingValueWithMod(int v, int modV)
    {
        return (v + 1) % modV;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GreatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LowestCommonMultiple(params int[] numbers)
    {
        if (numbers.Length == 1)
            return numbers[0];

        int runninglcm = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
            runninglcm = (runninglcm / GreatestCommonFactor(runninglcm, numbers[i])) * numbers[i];
        return runninglcm;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion FaceObject(Vector2 startingPosition, Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - startingPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion FaceObject(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetObjectRotation(Transform fakeHand, Vector2 vecAng)
    {
        float ang = vecAng.normalized.Angle() + 270;
        if (ang < 0)
            ang += 360;
        fakeHand.localEulerAngles = new Vector3(0, 0, ang);
    }

    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion EulerZToQuat(float z)
    {
        return new Vector3(0, 0, z).EulerToQuat();
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandomElemFromArr<T>(T[] arr, bool allowNull = false)
    {
        //In a perfect world, the array does not have null elements. We do not live in that world. Tool safety with error logging
        if (arr == null || arr.Length == 0)
        {
            Debug.LogError("Error, GetRandomElemFromArr was passed a null or empty array, returning a default t");
            return (T)System.Activator.CreateInstance(typeof(T));  //This creates a default instance, for example, if an int, 0, if a class, that classes default constructor, this is not gaureteened to work, this is an attempted catch
        }

        int length = arr.Length;
        int randIndex = UnityEngine.Random.Range(0, length);
        if (allowNull || arr[randIndex] != null)
            return arr[randIndex];
        else
        {
            for (int attempts = 0; attempts < 100; attempts++)
            {
                T toRet = arr[randIndex];
                if (toRet != null)
                    return toRet;
                else
                    randIndex = UnityEngine.Random.Range(0, length);
            }
            Debug.LogError("Get random elem from array could not randomly find a non-null element after many attempts, returning the first element, which may be null");
            return arr[0];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetArea(this Collider2D collider)
    {
        if (collider is CircleCollider2D circle)
        {
            return Mathf.Pow(circle.radius, 2) * Mathf.PI;
        }
        else if (collider is BoxCollider2D box)
        {
            return box.size.x * box.size.y;
        }
        else if (collider is PolygonCollider2D polygon)
        {
            return GetSurfaceArea(polygon.points);
        }
        else if (collider is CapsuleCollider2D capsule)
        {
            Vector2 size = capsule.size;
            return Mathf.Pow(size.x / 2f, 2f) * Mathf.PI + (size.y - size.x) * size.x;
        }
        Debug.LogWarning("Unsupported collider type " + collider.gameObject);
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetSurfaceArea(Vector3[] vetices)
    {
        float xSum = 0;
        float ySum = 0;
        int numOfVertices = vetices.Length;
#if UNITY_EDITOR
        if (numOfVertices <= 0)
            Debug.LogError("whoa there, less/equal than 0 vertices");
#endif
        for (int i = 0; i < numOfVertices - 1; i++)
        {
            xSum += vetices[i].x * vetices[i + 1].y;
            ySum += vetices[i].y * vetices[i + 1].x;
        }

        xSum += vetices[numOfVertices - 1].x * vetices[0].y;
        ySum += vetices[numOfVertices - 1].y * vetices[0].x;

        return Mathf.Abs((xSum - ySum) / 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetSurfaceArea(Vector2[] vetices)
    {
        float xSum = 0;
        float ySum = 0;
        int numOfVertices = vetices.Length;
#if UNITY_EDITOR
        if (numOfVertices <= 0)
            Debug.LogError("whoa there, less/equal than 0 vertices");
#endif
        for (int i = 0; i < numOfVertices - 1; i++)
        {
            xSum += vetices[i].x * vetices[i + 1].y;
            ySum += vetices[i].y * vetices[i + 1].x;
        }

        xSum += vetices[numOfVertices - 1].x * vetices[0].y;
        ySum += vetices[numOfVertices - 1].y * vetices[0].x;

        return Mathf.Abs((xSum - ySum) / 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetSurfaceArea(Mesh mesh)
    {
        return GetSurfaceArea(mesh.vertices);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color BuildRandomSaturatedColour()
    {
        //fully saturated colours always have one maxed channel, one min channel, and one channel with any value
        float[] channels = new float[3]; //indices 0, 1, 2, correspond to r, g, b respectively

        int maxSatIndex = Random.Range(0, 3);  //this next code block determines which channels are min, max, and random
        int minSatIndex = Random.Range(0, 3);
        while (minSatIndex == maxSatIndex)
            minSatIndex = Random.Range(0, 3);
        int randSatIndex = Random.Range(0, 3);
        while (randSatIndex == maxSatIndex || randSatIndex == minSatIndex)
            randSatIndex = Random.Range(0, 3);

        float randSat = Random.Range(0f, 1f); //value of the random channel

        channels[maxSatIndex] = 1; //this block populates the array according to the determined indices
        channels[minSatIndex] = 0;
        channels[randSatIndex] = randSat;

        Color color = new Color(channels[0], channels[1], channels[2]); //builds colour from array
        return color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SpliceCamelCase(string str, string separator)
    {
        string newStr = str;
        for (int i = str.Length - 1; i > 0; i--)
            if (char.IsUpper(str[i]) && !char.IsWhiteSpace(str[i - 1]) && !char.IsUpper(str[i - 1]))
                newStr = newStr.Insert(i, separator);
        return newStr;
    }
}
