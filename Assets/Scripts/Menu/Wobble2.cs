using UnityEngine;
using System.Collections;

public class Wobble2 : MonoBehaviour
{
    public float scale = 0.1f;
    public float speed = 1.0f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    private Vector3[] baseHeight;

    Mesh mesh;
    Mesh deformMesh;

    void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        deformMesh = Instantiate(mesh);
        GetComponent<SkinnedMeshRenderer>().sharedMesh = deformMesh;
        
    }

    void Update()
    {
        if (baseHeight == null)
            baseHeight = deformMesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
       deformMesh.vertices = vertices;
        deformMesh.RecalculateNormals();
    }
}