using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    public GameObject header;
    public GameObject inputField;
    public GameObject slider;
    public GameObject text;
    public GameObject button;
    public GameObject toggle;

    public VerticalLayoutGroup verticalLayoutGroup;

    public Object Target;

    [ExposeMethodInEditor()]
    public void Test()
    {
        SetTarget(Target);
    }

    public void SetTarget(Object target)
    {
        //Target any field that uses the ExposeAttributeUI attribute
        verticalLayoutGroup.transform.DeleteAllChildren();
        foreach (var field in target.GetType().GetFields())
        {
            var attributes = field.GetCustomAttributes(typeof(ExposeAttributeUI), true);
            if (attributes.Length > 0)
            {
                var attribute = (ExposeAttributeUI)attributes[0];
            }
        }
    }

    private void DrawExposeAttributeUI(FieldInfo fieldInfo, ExposeAttributeUI target)
    {
        //var headerInstance = Instantiate(header, verticalLayoutGroup.transform);
        //headerInstance.GetComponentInChildren<Text>().text = fieldInfo.Name;
        if (fieldInfo.FieldType == typeof(float))
        {
            var sliderInstance = Instantiate(slider, verticalLayoutGroup.transform);
            sliderInstance.GetComponentInChildren<Slider>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
        }
        else if (fieldInfo.FieldType == typeof(int))
        {
            var sliderInstance = Instantiate(slider, verticalLayoutGroup.transform);
            sliderInstance.GetComponentInChildren<Slider>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, (int)value));
        }
        else if (fieldInfo.FieldType == typeof(string))
        {
            var inputFieldInstance = Instantiate(inputField, verticalLayoutGroup.transform);
            inputFieldInstance.GetComponentInChildren<InputField>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
        }
        else if (fieldInfo.FieldType == typeof(bool))
        {
            var toggleInstance = Instantiate(toggle, verticalLayoutGroup.transform);
            toggleInstance.GetComponentInChildren<Toggle>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
        }/*
                else if (fieldInfo.FieldType == typeof(Vector2))
                {
                    var inputFieldInstance = Instantiate(inputField, verticalLayoutGroup.transform);
                    inputFieldInstance.GetComponentInChildren<InputField>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
                }
                else if (fieldInfo.FieldType == typeof(Vector3))
                {
                    var inputFieldInstance = Instantiate(inputField, verticalLayoutGroup.transform);
                    inputFieldInstance.GetComponentInChildren<InputField>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
                }
                else if (fieldInfo.FieldType == typeof(Vector4))
                {
                    var inputFieldInstance = Instantiate(inputField, verticalLayoutGroup.transform);
                    inputFieldInstance.GetComponentInChildren<InputField>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
                }
                else if (fieldInfo.FieldType == typeof(Color))
                {
                    var inputFieldInstance = Instantiate(inputField, verticalLayoutGroup.transform);
                    inputFieldInstance.GetComponentInChildren<InputField>().onValueChanged.AddListener((value) => fieldInfo.SetValue(target, value));
                }*/
        else
        {
            Debug.LogError("Unsupported type: " + fieldInfo.FieldType);
        }
    }


}
