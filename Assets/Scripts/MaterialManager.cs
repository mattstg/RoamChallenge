using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager instance;
    private void Awake()
    {
        instance = this;
    }

    
}
