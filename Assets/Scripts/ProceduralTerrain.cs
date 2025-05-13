using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct DeformJob : IJobParallelFor
{
    public NativeArray<Vector3> vertices;
    public Vector3 hitPoint;
    public float radius;
    public float strength;

    public void Execute(int index)
    {
        Vector3 v = vertices[index];
        if (Vector3.Distance(v, hitPoint) < radius)
        {
            v.y += strength;
            vertices[index] = v;
        }
    }
}

public class ProceduralTerrain : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] private float _terrainSize = 50f;

    [SerializeField] private int _highResResolution = 50;
    [SerializeField] private int _lowResResolution = 10;

    [Header("Deformation Settings")]
    [SerializeField] private float _radius = 2.5f;
    [SerializeField] private float _deformationStrength = 0.5f;

    [Header("LOD Settings")]
    [SerializeField] private float _lodDistance = 1000f;

    private Mesh _highResMesh;
    private Mesh _lowResMesh;
    private Vector3[] _highResVertices;
    private int[] _highResTriangles;

    private void Start()
    {
        _highResMesh = GenerateMesh(_highResResolution, out _highResVertices, out _highResTriangles);
        _lowResMesh = GenerateMesh(_lowResResolution, out _, out _);

        GetComponent<MeshFilter>().mesh = _highResMesh;
        GetComponent<MeshCollider>().sharedMesh = _highResMesh;
    }

    private void Update()
    {
        DeformTerrain();
        HandleLOD();
    }

    private void HandleLOD()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshCollider mc = GetComponent<MeshCollider>();

        if (distance > _lodDistance)
        {
            if (mf.sharedMesh != _lowResMesh)
            {
                mf.sharedMesh = _lowResMesh;
                mc.sharedMesh = _lowResMesh;
            }
        }
        else
        {
            if (mf.sharedMesh != _highResMesh)
            {
                mf.sharedMesh = _highResMesh;
                mc.sharedMesh = _highResMesh;
            }
        }
    }

    private Mesh GenerateMesh(int resolution, out Vector3[] vertices, out int[] triangles)
    {
        int width = resolution;
        int height = resolution;

        float spacing = _terrainSize / (resolution - 1);
        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float worldX = x * spacing;
                float worldZ = z * spacing;
                float scale = 0.1f;
                float amplitude = 5f;
                float y = Mathf.PerlinNoise(worldX * scale, worldZ * scale) * amplitude;
                vertices[z * width + x] = new Vector3(worldX, y, worldZ);
            }
        }

        int index = 0;
        for (int z = 0; z < height - 1; z++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int bl = z * width + x;
                int tl = (z + 1) * width + x;
                int tr = (z + 1) * width + (x + 1);
                int br = z * width + (x + 1);

                triangles[index++] = bl;
                triangles[index++] = tl;
                triangles[index++] = tr;

                triangles[index++] = bl;
                triangles[index++] = tr;
                triangles[index++] = br;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    private void UpdateHighResMesh()
    {
        _highResMesh.Clear();
        _highResMesh.vertices = _highResVertices;
        _highResMesh.triangles = _highResTriangles;
        _highResMesh.RecalculateNormals();

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshCollider mc = GetComponent<MeshCollider>();

        if (mf.sharedMesh != _highResMesh)
            mf.sharedMesh = _highResMesh;

        mc.sharedMesh = null;
        mc.sharedMesh = _highResMesh;
    }

    private void DeformTerrain()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point;

                NativeArray<Vector3> nativeVertices = new NativeArray<Vector3>(_highResVertices, Allocator.TempJob);

                DeformJob job = new DeformJob
                {
                    vertices = nativeVertices,
                    hitPoint = hitPoint,
                    radius = _radius,
                    strength = _deformationStrength
                };

                JobHandle handle = job.Schedule(nativeVertices.Length, 64);
                handle.Complete();

                nativeVertices.CopyTo(_highResVertices);
                nativeVertices.Dispose();

                UpdateHighResMesh();
            }
        }
    }
}
