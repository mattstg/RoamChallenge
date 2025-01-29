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
        editMode = true;
        LinkMesh();
    }

    [ExposeMethodInEditor()]
    public void EndEdit()
    {
        editMode = false;
        ClampMeshHeightForBuildings(Controllers.GameManager.Instance.chain.GetAllColliders());
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
            if (!editMode)
                StartEdit();

            if (Input.GetMouseButton(0)) // Left mouse button to raise terrain
            {
                HandleMouseInput(true);
            }
            else if (Input.GetMouseButton(1)) // Right mouse button to lower terrain
            {
                HandleMouseInput(false);
            }
        }
        else if (editMode)
            EndEdit();
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

    public void ClampMeshHeightForBuildings(Collider[] buildings, float clearanceOffset = 0.1f)
    {
        LinkMesh();
        if (mesh == null || buildings == null || buildings.Length == 0)
        {
            return;
        }

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = transform.TransformPoint(mesh.vertices[i]); // Convert to world space if needed
            foreach (Collider building in buildings)
            {
                if (IsPointInsideCollider(building, worldVertex))
                {
                    // Lower the vertex height to just below the building
                    float buildingBaseHeight = building.bounds.min.y;
                    Vector3 finalV = vertices[i];
                    Vector3 yValueInLocal = transform.InverseTransformPoint(new Vector3(0, buildingBaseHeight - clearanceOffset, 0));


                    vertices[i].y = yValueInLocal.y; // transform.InverseTransformPoint(buildingBaseHeight - clearanceOffset);
                    break; // No need to check other buildings once adjusted
                }
                else
                {
                    float damp = GetDampingMult(building, worldVertex, distanceAffect, clearanceOffset);//, maxDampFactor);
                    if(damp != 1)
                        vertices[i].y = damp * vertices[i].y;
                }
            }
        }

        mesh.vertices = vertices; // Apply the modified vertices
        mesh.RecalculateNormals(); // Update for lighting
        mesh.RecalculateBounds(); // Ensure correct mesh bounds
    }

    public float distanceAffect = 3;
    public float maxDampFactor = 3;

    private static bool IsPointInsideCollider(Collider collider, Vector3 worldPoint)
    {
        // Get collider world-space boundaries
        Bounds bounds = new Bounds(collider.bounds.center, collider.bounds.size);


        return (worldPoint.x >= bounds.min.x && worldPoint.x <= bounds.max.x) &&
                 (worldPoint.z >= bounds.min.z && worldPoint.z <= bounds.max.z) &&
                 (worldPoint.y >= bounds.min.y); // Only check if the vertex is inside or above
    }

    private static float GetDampingMult(Collider collider, Vector3 worldPoint, float distanceAffect, float clearanceOffset)
    {
        // Get collider world-space boundaries
        Bounds bounds = new Bounds(collider.bounds.center, collider.bounds.size * distanceAffect);
        Vector3 closestPoint = bounds.ClosestPoint(worldPoint);
        float distanceToEdge = Vector3.Distance(worldPoint, closestPoint);
        if(distanceToEdge > distanceAffect)
            return 1;

        float buildingBaseHeight = bounds.min.y - clearanceOffset;

        // Apply a damping factor based on distance (stronger near the building)
        return Mathf.Clamp01(distanceToEdge / distanceAffect);
        //return Mathf.Lerp(worldPoint.y, buildingBaseHeight, dampingFactor);
    }

}
