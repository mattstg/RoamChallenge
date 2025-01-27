using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public float height;
    bool isDrawn;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = attribute as ShowIfAttribute;

        System.Type targetType = property.serializedObject.targetObject.GetType();
        bool shouldDraw = true;

        foreach (string s in showIf.boolFieldNames)
        {
            FieldInfo fi = targetType.GetField(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            height = EditorGUI.GetPropertyHeight(property, label, true);

            if (fi != null && fi.FieldType == typeof(bool))
            {
                bool boolValue = (bool)fi.GetValue(property.serializedObject.targetObject);
                shouldDraw = shouldDraw && boolValue;
            }

            PropertyInfo pi = targetType.GetProperty(s, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (pi != null && pi.PropertyType == typeof(bool))
            {
                bool boolValue = (bool)pi.GetValue(property.serializedObject.targetObject);
                shouldDraw = shouldDraw && boolValue;
            }
        }

        isDrawn = shouldDraw;
        if (shouldDraw)
        {
            DrawProperty(position, property, label);
        }
    }

    private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (isDrawn)? height : 0; // base.GetPropertyHeight(property, label);
    }
}
