using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneGenerator : MonoBehaviour
{
    [Header("Plane Dimensions")]
    public float width = 10f;
    public float length = 10f;
    public float spaceBetweenVertices = 1f;

    [Header("Material")]
    public Material material;

    public TMP_InputField scaleInput;
    public TMP_InputField heightMultInput;
    public TMP_InputField offsetXInput;
    public TMP_InputField offsetZInput;
    public UnityEngine.UI.Toggle expoInput;


    public float scale => float.Parse(scaleInput.text);
    public float heightMultiplier => float.Parse(heightMultInput.text);
    public float offsetX => float.Parse(offsetXInput.text);
    public float offsetZ => float.Parse(offsetZInput.text);
    public bool exponential => expoInput.isOn;

    [ExposeMethodInEditor()]
    public void GeneratePlane()
    {
        // Ensure valid values
        if (width <= 0 || length <= 0 || spaceBetweenVertices <= 0)
        {
            Debug.LogError("Width, Length, and Space Between Vertices must be greater than 0.");
            return;
        }

        // Calculate number of vertices along each axis
        int verticesX = Mathf.CeilToInt(width / spaceBetweenVertices) + 1;
        int verticesZ = Mathf.CeilToInt(length / spaceBetweenVertices) + 1;

        // Create mesh components
        Vector3[] vertices = new Vector3[verticesX * verticesZ];
        int[] triangles = new int[(verticesX - 1) * (verticesZ - 1) * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        // Generate vertices and UVs
        for (int z = 0; z < verticesZ; z++)
        {
            for (int x = 0; x < verticesX; x++)
            {
                int index = z * verticesX + x;
                vertices[index] = new Vector3(x * spaceBetweenVertices, 0, z * spaceBetweenVertices);
                uv[index] = new Vector2((float)x / (verticesX - 1), (float)z / (verticesZ - 1));
            }
        }

        // Generate triangles
        int triangleIndex = 0;
        for (int z = 0; z < verticesZ - 1; z++)
        {
            for (int x = 0; x < verticesX - 1; x++)
            {
                int topLeft = z * verticesX + x;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + verticesX;
                int bottomRight = bottomLeft + 1;

                // First triangle
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;

                // Second triangle
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        // Create and assign the mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uv
        };
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        

        // Assign the material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        if (material != null)
        {
            meshRenderer.material = material;
        }

        MeshCollider mc = GetComponent<MeshCollider>();
        if(mc)
            GameObject.DestroyImmediate(mc);
        mc = gameObject.AddComponent<MeshCollider>();

    }

    [ExposeMethodInEditor()]
    public void PerlinTheGrid()
    {
        RandomGrid(GetComponent<MeshFilter>().mesh, scale, heightMultiplier, offsetX, offsetZ);
    }

    

    public void RandomGrid(Mesh mesh, float scale, float heightMultiplier, float offsetX, float offsetZ)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null. Cannot modify terrain.");
            return;
        }

        Vector3[] vertices = mesh.vertices; // Get the mesh's vertices

        for (int i = 0; i < vertices.Length; i++)
        {
            float xCoord = (vertices[i].x + offsetX) * scale;
            float zCoord = (vertices[i].z + offsetZ) * scale;

            // Generate Perlin noise value
            float perlinValue = Mathf.PerlinNoise(xCoord, zCoord);

            // Modify the vertex height based on noise value
            vertices[i].y = perlinValue * heightMultiplier * ((exponential)?perlinValue:1) - 1;
        }

        mesh.vertices = vertices; // Apply new vertex positions
        mesh.RecalculateNormals(); // Update normals for proper lighting
        mesh.RecalculateBounds(); // Update the mesh bounds
    }
    // Optional: Draw gizmos to visualize the plane in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        for (float z = 0; z <= length; z += spaceBetweenVertices)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0, 0, z), transform.position + new Vector3(width, 0, z));
        }
        for (float x = 0; x <= width; x += spaceBetweenVertices)
        {
            Gizmos.DrawLine(transform.position + new Vector3(x, 0, 0), transform.position + new Vector3(x, 0, length));
        }
    }
}
