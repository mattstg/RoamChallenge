using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ShowIfAttribute : PropertyAttribute
{
    public string[] boolFieldNames;

    public ShowIfAttribute(params string[] boolFieldNames)
    {
        this.boolFieldNames = boolFieldNames;
    }
}