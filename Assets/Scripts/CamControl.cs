using UnityEngine;

public class CamControl : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.position = pos1.position;
            transform.rotation = pos1.rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            transform.position = pos2.position;
            transform.rotation = pos2.rotation;
        }

    }
}
