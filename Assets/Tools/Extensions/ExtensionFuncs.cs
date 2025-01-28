using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class IfEditor
{
    public static void Do(Action a)
    {
#if UNITY_EDITOR
        a.Invoke();
#endif
    }

}

/// <remarks>
/// Things to add to upgrade debug extension system
/// Toggle run during build (by default runs all, attempt 0 expensive if off)
/// Toggle active ones during build (Numpad)
/// Toggle output to file
/// </remarks>

public static class ExtensionFuncs
{

    static System.Random rnd = new System.Random();

    //https://stackoverflow.com/questions/6771917/why-cant-i-preallocate-a-hashsett
    private static class HashSetDelegateHolder<T>
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
        public static MethodInfo InitializeMethod { get; } = typeof(HashSet<T>).GetMethod("Initialize", Flags);
    }

    public static void SetCapacity<T>(this HashSet<T> hs, int capacity)
    {
        HashSetDelegateHolder<T>.InitializeMethod.Invoke(hs, new object[] { capacity });
    }

    public static HashSet<T> GetHashSet<T>(int capacity)
    {
        var hashSet = new HashSet<T>();
        hashSet.SetCapacity(capacity);
        return hashSet;
    }

    public static int Sign(this bool b, bool positiveIsNegative = false)
    {
        if (!positiveIsNegative)
            return (b) ? 1 : -1;
        else
            return (b) ? -1 : 1;

    }

    //I have to do this so often, so I made an ext
    public static void SafeAdd<T, G>(this Dictionary<T, List<G>> d, T key, G value)
    {
        if (!d.ContainsKey(key))
            d.Add(key, new List<G>());
        d[key].Add(value);
    }

    public static void DeleteAllChildren(this Transform t)
    {
        Transform[] childarr = t.ChildrenArray();
        foreach (Transform child in childarr)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static Bounds GetTotalColiBoundsOfGameObject(this GameObject g, bool includeTriggers)
    {
        Bounds finalBounds;
        Collider2D[] allColis = (includeTriggers)? g.GetComponentsInChildren<Collider2D>(true) : g.GetComponentsInChildren<Collider2D>(true).Where(c => !c.isTrigger).ToArray();
        if (allColis.Length >= 1)
        {
            finalBounds = allColis[0].bounds;
        }
        else
        {
            finalBounds = new Bounds();
            UnityEngine.Debug.Log("Object has no colliders to get bounds from, if it is a prefab resource being loaded in edit, they do not have colliders initialized yet, (Unity thing)"); //If you are working directly on a resource and not on something in a scene
            //Also if it only has trigger colliders and your not using includeTriggers
        }


        foreach (Collider2D pcoli in allColis)
        {
            finalBounds.Encapsulate(pcoli.bounds);
        }

        return finalBounds;
    }

    public static Bounds GetTotalSpriteBoundsOfGameObject(this GameObject g)
    {
        Bounds finalBounds;
        SpriteRenderer[] allSr = g.GetComponentsInChildren<SpriteRenderer>(true);
        if (allSr.Length > 1)
        {
            finalBounds = allSr[0].bounds;
        }
        else
        {
            finalBounds = new Bounds();
            UnityEngine.Debug.Log("Object has no sprite renderers");
        }


        foreach (SpriteRenderer pcoli in allSr)
        {
            finalBounds.Encapsulate(pcoli.bounds);
        }

        return finalBounds;
    }

    public static T ToEnum<T>(this string s) where T : struct, IConvertible, IComparable, IFormattable
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("String conversion to enum failed, T was not enum type, must be an enum.");
        }
        T toRet;
        if (Enum.TryParse<T>(s, out toRet))
        {
            return toRet;
        }
        else
        {
            System.Type st = typeof(T);
            UnityEngine.Debug.LogError($"ERROR, Conversion from string to enum for string:{s} and enumType:{st} failed!! Returning default value for enum");
            return default(T);
        }

    }

    public static FieldInfo[] GetFieldInfosOfAttribute(this System.Type thisType, System.Type attrType)
    {
        return thisType.GetFields(ReflectionHelper.BindingFlags).Where(fi => fi.IsDefined(attrType, false)).ToArray();
    }




    public static List<string> GetStringListOfEnums(this System.Type t)
    {
        if (!t.IsEnum)
        {
            UnityEngine.Debug.LogError("Get string list of enums called on not an enum");
            return new List<string>();
        }
        else
            return Enum.GetNames(t).ToList();
    }

    public static Bounds GetBoundsFromPoints(this Vector2[] points)
    {
        Vector2 botLeft = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 topRight = new Vector2(-float.MaxValue, -float.MaxValue);

        for (int i = 0; i < points.Length; i++)
        {
            botLeft.x = Mathf.Min(botLeft.x, points[i].x);
            botLeft.y = Mathf.Min(botLeft.y, points[i].y);
            topRight.x = Mathf.Max(topRight.x, points[i].x);
            topRight.y = Mathf.Max(topRight.y, points[i].y);
        }

        return new Bounds((botLeft + topRight) / 2f, topRight - botLeft);
    }

    public static Vector2 GetCenterFromPoints(this Vector2[] points, int startIndex, int endIndex)
    {
        float a_x = float.MaxValue;
        float a_y = float.MaxValue;
        float b_x = -float.MaxValue;
        float b_y = -float.MaxValue;

        for (int i = startIndex; i <= endIndex; i++)
        {
            Vector2 point = points[i];
            a_x = Mathf.Min(a_x, point.x);
            a_y = Mathf.Min(a_y, point.y);
            b_x = Mathf.Max(b_x, point.x);
            b_y = Mathf.Max(b_y, point.y);
        }

        return new Vector2((a_x + b_x) / 2f, (a_y + b_y) / 2f);
    }

    public static Vector3 GetCenterFromPoints(this Vector3[] points, int startIndex, int endIndex)
    {
        float a_x = float.MaxValue;
        float a_y = float.MaxValue;
        float b_x = -float.MaxValue;
        float b_y = -float.MaxValue;

        for (int i = startIndex; i <= endIndex; i++)
        {
            Vector3 point = points[i];
            a_x = Mathf.Min(a_x, point.x);
            a_y = Mathf.Min(a_y, point.y);
            b_x = Mathf.Max(b_x, point.x);
            b_y = Mathf.Max(b_y, point.y);
        }

        return new Vector3((a_x + b_x) / 2f, (a_y + b_y) / 2f, 0f);
    }

    public static Vector2 GetCenterOfPoints(this List<Vector2> vectors)
    {
        Vector2 center = new Vector2();
        foreach (Vector2 v in vectors)
            center += v;
        center /= vectors.Count;
        return center;
    }

    //This function only points to children and does not include the parent
    //This function can retrieve children if they were destroyed in the same frame!! So does foreach apparently tho...
    public static Transform[] ChildrenArray(this Transform t)
    {
        return t.Cast<Transform>().ToArray();
    }

    public static Transform ChildByName(this Transform t, string _name)
    {
        return t.ChildrenArray().FirstOrDefault(tchild => tchild.name == _name);
    }

    public static Transform ChildBySubStringName(this Transform t, string _name)
    {
        return t.ChildrenArray().FirstOrDefault(tchild => tchild.name.Contains(_name));
    }

    public static void SetAllLayersInAllChildrenRecursive(this Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++)
            t.GetChild(i).SetAllLayersInAllChildrenRecursive(layer);
    }

    public static Transform[] ChildernByName(this Transform t, string _name)
    {
        return t.ChildrenArray().Where(childName => childName.name == _name).ToArray();
    }

    public static bool IntToBool(this int i)
    {
        return (i != 0);
    }

    public static int BoolToInt(this bool b)
    {
        return (b) ? 1 : 0;
    }

    public static Transform FindChildBySlashSeperateSearch(this Transform t, string searchTerm)
    {
        Transform toRet = null;
        try
        {
            string[] searchTerms = searchTerm.Split('/');
            for (int i = 0; i < searchTerm.Length; i++)
            {
                toRet = toRet.Find(searchTerms[i]);
            }
        }
        catch
        {
            UnityEngine.Debug.LogError($"Failed to find the requested child {searchTerm} in the object {t.name}, returning null");
        }
        return toRet;
    }

    public static T[] GetAllCompRecursive<T>(this Transform t) where T : Component
    {
        List<T> toFill = new List<T>();
        _GetAllCompRecursive<T>(t, toFill);
        return toFill.ToArray();
    }

    private static void _GetAllCompRecursive<T>(this Transform t, List<T> allComp) where T : Component
    {
        if (t.childCount > 0)
            foreach (Transform child in t)
                _GetAllCompRecursive<T>(child, allComp);
        T compToAdd = t.GetComponent<T>();
        if (compToAdd)
            allComp.Add(compToAdd);
    }

    public static Vector2 SumTotal(this List<Vector2> toSum)
    {
        Vector2 toRet = Vector2.zero;
        foreach (Vector2 v2 in toSum)
            toRet += v2;
        return toRet;
    }

    public static List<string> GetParentsNames(this GameObject g, string stopAt)
    {
        //All the dumb gameobject casting is because the string name of transform includes the word (UnityEngine.Transform)
        //I could also do a string replace, but I did this

        List<string> toRet = new List<string>();
        GameObject currentPtr = g;
        while (currentPtr.transform.parent && currentPtr.transform.parent.gameObject.name != stopAt)  //transform name gives bad name
        {
            currentPtr = currentPtr.transform.parent.gameObject;
            toRet.Add(currentPtr.name);
        }
        return toRet;
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        //if (source == null) throw new ArgumentNullException("source");
        //if (action == null) throw new ArgumentNullException("action");

        foreach (T item in source)
        {
            try
            {
                action(item);
            }
            catch(System.Exception e)
            {
                if(item != null)
                    UnityEngine.Debug.Log("Warning, failure in Foreach on item " + item.ToString() + " e: " + e.ToString());
                else
                    UnityEngine.Debug.Log("Warning, failure in Foreach on item, wall null, e: " + e.ToString());
            }

        }
    }

    public static bool TrueForAll<T>(this IEnumerable<T> source, Predicate<T> action)
    {
        foreach (T item in source)
            if (!action(item))
                return false;
        return true;
    }

    public static T[] Append<T>(this T[] array, T item)
    {
        T[] result = new T[array.Length + 1];
        array.CopyTo(result, 0);
        result[array.Length] = item;
        return result;
    }

    public static T[] RemoveAtIndex<T>(this T[] source, int indexToRemove)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (indexToRemove < 0 || indexToRemove >= source.Length)
            throw new ArgumentOutOfRangeException(nameof(indexToRemove), "Index is out of range.");

        T[] result = new T[source.Length - 1];

        for (int i = 0, j = 0; i < source.Length; i++)
        {
            if (i == indexToRemove) continue; // Skip the index to remove
            result[j++] = source[i];
        }

        return result;
    }

    public static T[] RemoveAtIndices<T>(this T[] source, int[] indicesToRemove)
    {
        UnityEngine.Debug.Log("WARNING. GPT wrote this and I never tested it. Run a test in some simple mono script awake with a few collections first before removing this warning please");
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (indicesToRemove == null)
            throw new ArgumentNullException(nameof(indicesToRemove));

        // Order indices from highest to lowest
        var orderedIndices = indicesToRemove.OrderByDescending(i => i).Distinct().ToArray();

        // Validate indices
        if (orderedIndices.Any(i => i < 0 || i >= source.Length))
            throw new ArgumentOutOfRangeException(nameof(indicesToRemove), "One or more indices are out of range.");

        // Create a new array and copy elements, skipping the ones to be removed
        T[] result = new T[source.Length - orderedIndices.Length];
        int sourceIndex = 0, resultIndex = 0, removeIndex = 0;

        while (sourceIndex < source.Length)
        {
            if (removeIndex < orderedIndices.Length && sourceIndex == orderedIndices[removeIndex])
            {
                // Skip this index
                sourceIndex++;
                removeIndex++;
            }
            else
            {
                result[resultIndex] = source[sourceIndex];
                sourceIndex++;
                resultIndex++;
            }
        }

        return result;
    }

    public static int[] GetIndexArray(this int value, bool inclusive)
    {
        if (value < 0 || (value == 0 && !inclusive))
            throw new ArgumentException("Value must be positive and non-zero if exclusive.");

        int length = inclusive ? value + 1 : value;
        int[] result = new int[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = i;
        }

        return result;
    }


    public delegate G ParsingDelg<T, G>(T elem);

    public static G[] CollectionFromForEach<T, G>(this IEnumerable<T> source, ParsingDelg<T, G> parsingDelg)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (parsingDelg == null) throw new ArgumentNullException("action");

        List<G> toRet = new List<G>();

        foreach (T item in source)
        {
            toRet.Add(parsingDelg(item));
        }
        return toRet.ToArray();
    }

    public delegate bool conditionalDelg<T>(T element);

    public static List<T> GetAll<T>(this IEnumerable<T> source, Predicate<T> action)
    {
        List<T> toReturn = new List<T>();
        foreach (T t in source)
        {
            if (action.Invoke(t))
                toReturn.Add(t);
        }
        return toReturn;
    }

    public static bool IsMaskContainedInThisLayerMask(this int lm, string layerName)
    {
        int n = LayerMask.NameToLayer(layerName);
        return lm == (lm | (1 << n)); //self explainatory
    }

    public static bool IsMaskContainedInThisLayerMask(this int lm, int layer)
    {
        return lm == (lm | (1 << layer)); //see above
    }

    public static string ToBinaryString(this Enum flagsEnum)
    {
        var sb = new System.Text.StringBuilder();
        var enumType = flagsEnum.GetType();

        foreach (Enum value in Enum.GetValues(enumType))
        {
            if (flagsEnum.HasFlag(value))
            {
                sb.Append("1");
            }
            else
            {
                sb.Append("0");
            }
            sb.Append(" ");
            sb.Append(Enum.GetName(enumType, value));
            sb.Append(", ");
        }

        return sb.ToString().TrimEnd(',', ' ');
    }

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("GetCopyOfFailed: " + e.ToString());

                } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddCopyComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
   
    public static T GetRandomElement<T>(this System.Collections.Generic.ICollection<T> iCollec)
    {
        if (iCollec.Count <= 0)
            return default(T);
        return iCollec.ElementAt(UnityEngine.Random.Range(0, iCollec.Count));
    }

    public static int GetIndexOf<T>(this T[] arr, T elem)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(elem))
            {
                return i;
            }
        }
        return -1;
    }

    public static T GetRandom<T>(this T[] v)
    {
        if (v.Length <= 0)
        {
            UnityEngine.Debug.LogError("GetRandom was given an array of length less than 0");
            return default(T);
        }
        return v[UnityEngine.Random.Range(0, v.Length)];
    }


    //Randomizes and produces no garbage, so the original array is altered.
    public static void RandomizeNoGarbage<T>(ref T[] selection)
    {
        int selectionLength = selection.Length;
        for (int i = 0; i < selectionLength - 1; i++)
        {
            int rand = UnityEngine.Random.Range(i, selectionLength);
            T temp = selection[i];

            selection[i] = selection[rand];
            selection[rand] = temp;
        }
    }

    public static bool IsArrayAllEqual(this float[] arr, float valueCompare)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != valueCompare)
            {
                return false;
            }
        }
        return true;
    }

    public static T[] GetXRandomUniqueElements<T>(this T[] v, int numToGet)
    {
        if (numToGet > v.Length)
        {
            UnityEngine.Debug.Log($"Asked for more random elements({numToGet}) than in the array({v.Length}), returned the entire array");
            return v.ToArray();
        }
        if (numToGet == v.Length)
        {
            return v.ToArray();
        }

        T[] shuffled = v.ToArray(); //Clone array so don't shuffle original

        for (int i = shuffled.Length - 1; i > 0; i--)
        {

            // Pick a random index
            // from 0 to i
            int j = UnityEngine.Random.Range(0, i + 1);

            // Swap arr[i] with the
            // element at random index
            T temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }

        //Once shuffled, I now need to truncate and return an array of length numToGet

        T[] toRet = new T[numToGet];
        for (int i = 0; i < numToGet; i++)
        {
            toRet[i] = shuffled[i];
        }
        return toRet;
    }

    public static Vector2 GetSpritePivot(this Sprite sprite)
    {
        Bounds bounds = sprite.bounds;
        var pivotX = -bounds.center.x / bounds.extents.x / 2 + 0.5f;
        var pivotY = -bounds.center.y / bounds.extents.y / 2 + 0.5f;

        return new Vector2(pivotX, pivotY);
    }

    public static List<T> RandomizeOrder<T>(this List<T> v)
    {
        var result = v.OrderBy(item => rnd.Next());
        return result.ToList();
    }
    public static void Shuffle<T>(this System.Random rng, T[] array)
    {
        int n = array.Length;
        Shuffle(rng, array, n);
    }

    public static void Shuffle<T>(this System.Random rng, T[] array, int count)
    {
        int n = count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    /// <summary>
    /// EX) Given a percent range of .5 and a value of 10 will return a value between 5 and 15, Given zero, it is the same value
    /// </summary>
    /// <param name="v"></param>
    /// <param name="withinPercentRange"></param>
    /// <returns></returns>
    public static float RandomizeByPercent(this float v, float withinPercentRange)
    {
        if (withinPercentRange == 0)
            return v;
        return v + UnityEngine.Random.Range(-v * withinPercentRange, v * withinPercentRange);
    }

    public static int IncreaseLoopingValue(this int v, int modV)
    {
        return (v + 1) % modV;
    }

    public static int DecreaseLoopingValue(this int v, int modV)
    {
        return (v + modV - 1) % modV;
    }


    public static string[] CollectionToStringArray<T>(this System.Collections.Generic.ICollection<T> v)
    {
        string[] toRet = new string[v.Count];
        int i = 0; //cannot use for, for this situtation
        foreach (T elem in v)
        {
            toRet[i] = elem.ToString();
            i++;
        }
        return toRet;
    }

    public static string OutputDictAsString<T, G>(this Dictionary<T, G> toOut)
    {
        List<string> stringPairs = new List<string>();
        return toOut.CollectionFromForEach((kv) => { return (kv.Key.ToString() + ":" + kv.Value.ToString()); }).CollectionToString();
    }

    public static string CollectionToString<T>(this System.Collections.Generic.ICollection<T> v)
    {
        return StringArrayToString(CollectionToStringArray<T>(v));
    }

    public static string StringArrayToString(this string[] v, string seperationCharacter = ",")
    {
        string toRet = "";
        foreach (string elem in v)
        {
            toRet += elem + seperationCharacter;
        }

        if (string.IsNullOrEmpty(toRet))
            return toRet;
        return toRet.Substring(0, toRet.Length - 1); //remove last ","
    }

    public static Vector2 AngToV2(this float v)
    {
        return MathHelper.DegreeToVector2(v);
    }

    //Converts a value from one enum to another. If the value does not exist in the target enum, returns the default value (0)
    public static TTarget ConvertTo<TSource, TTarget>(this TSource sourceValue, TTarget defaultValue) where TSource : Enum where TTarget : Enum
    {
        string sourceName = sourceValue.ToString();

        // Try to parse the source name in the target enum
        if (Enum.TryParse(typeof(TTarget), sourceName, out var result) && Enum.IsDefined(typeof(TTarget), result))
        {
            return (TTarget)result;
        }
        else
            return defaultValue; //Return default if no match was found
    }

    //JointLimitState2D

    public static AnimationCurve Copy(this AnimationCurve curve)
    {
        Keyframe[] keys = curve.keys;
        int numKeys = keys.Length;

        // Create a new array to hold the copied keyframes
        Keyframe[] newKeys = new Keyframe[numKeys];

        // Copy each keyframe from the original curve to the new array
        for (int i = 0; i < numKeys; i++)
        {
            Keyframe originalKey = keys[i];

            // Create a new Keyframe using the same time and value as the original keyframe
            Keyframe newKey = new Keyframe(originalKey.time, originalKey.value);

            // Copy additional properties of the keyframe if needed (e.g., inTangent, outTangent)

            // Add the new keyframe to the new array
            newKeys[i] = newKey;
        }

        // Create a new AnimationCurve using the copied keyframes
        AnimationCurve newCurve = new AnimationCurve(newKeys);

        // Copy any additional properties of the original curve to the new curve if needed
        newCurve.preWrapMode = curve.preWrapMode;
        newCurve.postWrapMode = curve.postWrapMode;

        return newCurve;
    }

    public static List<string> GetActiveFlags(this Enum value)
    {
        List<string> activeFlags = new List<string>();

        foreach (Enum flag in Enum.GetValues(value.GetType()))
        {
            if (value.HasFlag(flag) && Convert.ToInt32(flag) != 0)
            {
                activeFlags.Add(flag.ToName());
            }
        }

        return activeFlags;
    }
   
    public static T GetKeyForValue<T, G>(this Dictionary<T, G> dict, G value)
    where G : IEquatable<G>
    {
        foreach (var pair in dict)
        {
            if (pair.Value.Equals(value))
            {
                return pair.Key;
            }
        }
        throw new Exception("Value not found in dictionary");
    }


    //We can use this when we know the enum starts at 0, and incremements without gaps
    public static T CycleEnum<T>(this T enumValue, bool forward = true) where T : Enum
    {
        int count = Enum.GetValues(typeof(T)).Length;
        int index = (int)(object)enumValue;
        int nextIndex = (forward) ? IncreaseLoopingValue(index, count) : DecreaseLoopingValue(index, count);
        return (T)(object)nextIndex;
    }

    //This one allows for cycling when there may be gaps or not starting at 0
    public static T CycleEnumSafe<T>(this T enumValue) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        int currentIndex = Array.IndexOf(values, enumValue);
        int nextIndex = (currentIndex + 1) % values.Length;
        return values[nextIndex];
    }

    public static int FindHighestTrueIndex(this bool[] arr)
    {
        for (int i = arr.Length - 1; i >= 0; i--)
        {
            if (arr[i])
            {
                return i;
            }
        }
        return -1;
    }

    public static string GetResolutionString(this Resolution res)
    {
        return $"{res.width}x{res.height} @ {res.refreshRateRatio.value:F2}Hz";
    }
    //we dont check for empty string because many dicts have empty "" string
    
}
