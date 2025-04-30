using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    private const int _width = 50;
    private const int _height = 50;
    private Vector3[] _vertices;
    private int[] _triangles;

    private void Start()
    {
        NewMesh();
    }

    private void GenerateTerrain()
    {
        _vertices = new Vector3[_width * _height];

        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                _vertices[z * _width + x] = new Vector3(x, 0, z);
            }
        }

        for (int i = 0; i < _vertices.Length; i++)
        {
            float scale = 0.1f;
            float amplitude = 5f;
            _vertices[i].y = Mathf.PerlinNoise(_vertices[i].x * scale, _vertices[i].z * scale) * amplitude;
        }
    }

    private void CreateTriangles()
    {
        GenerateTerrain();
        _triangles = new int[(_width - 1) * (_height - 1) * 6];
        int index = 0;

        for (int z = 0; z < _height - 1; z++)
        {
            for (int x = 0; x < _width - 1; x++)
            {
                int bottomLeft = z * _width + x;
                int topLeft = (z + 1) * _width + x;
                int topRight = (z + 1) * _width + (x + 1);
                int bottomRight = z * _width + (x + 1);

                // Triangle 1
                _triangles[index++] = bottomLeft;
                _triangles[index++] = topLeft;
                _triangles[index++] = topRight;

                // Triangle 2
                _triangles[index++] = bottomLeft;
                _triangles[index++] = topRight;
                _triangles[index++] = bottomRight;
            }
        }
    }

    private void NewMesh()
    {
        CreateTriangles();
        Mesh mesh = new Mesh();
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
