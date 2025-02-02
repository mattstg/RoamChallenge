using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ExposeAttributeUIRange : PropertyAttribute
{
    public ExposeAttributeUIRange(float min, float max)
    { 
    
    }

    public ExposeAttributeUIRange(int min, int max)
    {

    }

}
