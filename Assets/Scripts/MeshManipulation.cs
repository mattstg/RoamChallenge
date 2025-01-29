using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshManipulation : MonoBehaviour
{
    [Header("Brush Settings")]
    public float brushSize = 1f;    // Radius of the brush
    public float opacity = 1f;     // Strength of the raise/lower effect
    [Range(0f, 1f)] public float falloff = 0.5f; // Falloff for strength within the brush radius


    private Mesh mesh;
    //private Vector3[] originalVertices; // Stores the original mesh vertices
    private Vector3[] modifiedVertices; // Stores the modified vertices

    private void StartEdit()
    {
        LinkMesh();
    }

    public void LinkMesh()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        //originalVertices = mesh.vertices;
        modifiedVertices = mesh.vertices;
    }

    private void UpdateMesh()
    {
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();
    }

    /// <summary>
    /// Raises terrain at the given position.
    /// </summary>
    public void RaiseTerrain(Vector3 worldPoint, float deltaTime)
    {
        ModifyTerrain(worldPoint, deltaTime, true);
    }

    /// <summary>
    /// Lowers terrain at the given position.
    /// </summary>
    public void LowerTerrain(Vector3 worldPoint, float deltaTime)
    {
        ModifyTerrain(worldPoint, deltaTime, false);
    }

    private void ModifyTerrain(Vector3 worldPoint, float deltaTime, bool isRaising)
    {
        // Transform world point into local space
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            //Vector3 vertex = originalVertices[i];
            Vector3 vertex = modifiedVertices[i];
            float distance = Vector3.Distance(new Vector3(vertex.x, 0, vertex.z), new Vector3(localPoint.x, 0, localPoint.z));

            if (distance < brushSize)
            {
                // Calculate strength based on falloff
                float strength = opacity * (1 - Mathf.Clamp01(falloff * (distance / brushSize)));

                // Modify vertex height
                float modification = strength * deltaTime * (isRaising ? 1 : -1);
                modifiedVertices[i].y += modification;
            }
        }

        UpdateMesh();
    }

    bool editMode = false;
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if(!editMode)
            {
                editMode = true;
                StartEdit();
            }

            if (Input.GetMouseButton(0)) // Left mouse button to raise terrain
            {
                HandleMouseInput(true);
            }
            else if (Input.GetMouseButton(1)) // Right mouse button to lower terrain
            {
                HandleMouseInput(false);
            }
        }
        else
            editMode = false;
    }

    private void HandleMouseInput(bool isRaising)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit,9999, LayerMask.GetMask("WorldTerrain")))
        {
            if (hit.collider.gameObject == gameObject)
            {
                float deltaTime = Time.deltaTime;
                Vector3 hitPoint = hit.point;

                if (isRaising)
                    RaiseTerrain(hitPoint, deltaTime);
                else
                    LowerTerrain(hitPoint, deltaTime);
            }
        }
    }
}
