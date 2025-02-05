using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;

[HideLabel]
[System.Serializable]
public class FloatToggle 
{
    [HorizontalGroup(20), HideLabel]
    public bool Enabled;

    [HorizontalGroup]
    [EnableIf("Enabled")]
    [LabelText("@$property.Parent.Parent.NiceName")]
    public float Value;

    static System.Type[] typesToDisplay = TypeCache.GetTypesWithAttribute<ExposeAttributeUI>().OrderBy(m => m.Name).ToArray();
}
