using UnityEngine;

public static class MeshUtilities
{
    /// <summary>
    /// Upsets the vertices of <paramref name="mesh"/> by <paramref name="noise"/> 
    /// in <paramref name="direction"/>.
    /// <br></br>Note: <paramref name="direction"/> will not be normalized within.
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="noise"></param>
    /// <param name="direction"></param>
    public static void ApplyNoiseToQuad(Mesh mesh, float[][] noise, Vector3 direction)
    {
        var numVerts = Mathf.Sqrt(mesh.vertexCount);

        Vector3[] vertices = mesh.vertices;

        int vertIndex = 0;
        for (uint y = 0; y < numVerts; y++)
        {
            for (uint x = 0; x < numVerts; x++)
            {
                vertices[vertIndex] += noise[y][x] * direction;
                vertIndex++;
            }
        }

        mesh.vertices = vertices;
    }

    public static Mesh GenerateQuad(uint numVerts, float size, Vector3 forward, bool center = true)
    {
        var mesh = new Mesh();

        // Ensure numVerts is at least 2 (minimum for a quad)
        if (numVerts < 2)
        {
            Debug.LogError("numVerts must be at least 2 to create a quad.");
            return mesh;
        }

        // Default forward direction is Z if none is provided
        if (forward == default)
            forward = Vector3.forward;

        // Create the local basis for the quad
        forward = forward.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        // Handle degenerate case where forward is parallel to up
        if (right == Vector3.zero)
            right = Vector3.Cross(Vector3.forward, forward).normalized;
        Vector3 up = Vector3.Cross(forward, right).normalized;

        int vertCount = (int)(numVerts * numVerts);
        int quadCount = (int)((numVerts - 1) * (numVerts - 1));

        // Create vertex and UV arrays
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[quadCount * 6];

        // Step size for the grid
        float step = size / (numVerts - 1);
        float offset = center ? size / 2.0f : 0.0f;

        // Populate vertices and UVs
        int vertIndex = 0;
        for (uint y = 0; y < numVerts; y++)
        {
            for (uint x = 0; x < numVerts; x++)
            {
                float xPos = x * step - offset;
                float yPos = y * step - offset;

                // Transform the vertex to the quad's local basis
                vertices[vertIndex] = xPos * right + yPos * up;
                uv[vertIndex] = new Vector2((float)x / (numVerts - 1), (float)y / (numVerts - 1));
                vertIndex++;
            }
        }

        // Populate triangles with correct winding order
        int triIndex = 0;
        for (uint y = 0; y < numVerts - 1; y++)
        {
            for (uint x = 0; x < numVerts - 1; x++)
            {
                int topLeft = (int)(y * numVerts + x);
                int topRight = topLeft + 1;
                int bottomLeft = (int)((y + 1) * numVerts + x);
                int bottomRight = bottomLeft + 1;

                // Triangle 1
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topRight;

                // Triangle 2
                triangles[triIndex++] = topRight;
                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = bottomRight;
            }
        }

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}