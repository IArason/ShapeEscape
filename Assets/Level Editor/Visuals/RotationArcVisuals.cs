using UnityEngine;
using System.Collections;

public class RotationArcVisuals : MonoBehaviour
{
    [SerializeField]
    int quality = 30;
    Mesh mesh;
    GameObject arcObject;
    public Material material;

    Vector3 initialDir;
    Vector3 targetDir;
    [SerializeField]
    float radius = 5.0f;

    void Start()
    {
        arcObject = new GameObject("RotateArc", new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer) });
        arcObject.transform.position = Vector3.back * 10f;
        arcObject.layer = LayerMask.NameToLayer("RotArc");

        mesh = new Mesh();
        mesh.vertices = new Vector3[4 * quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
        mesh.triangles = new int[3 * 2 * quality];

        Vector3[] normals = new Vector3[4 * quality];
        Vector2[] uv = new Vector2[4 * quality];

        for (int i = 0; i < uv.Length; i++)
            uv[i] = new Vector2(0, 0);
        for (int i = 0; i < normals.Length; i++)
            normals[i] = new Vector3(0, 1, 0);

        mesh.uv = uv;
        mesh.normals = normals;

        mesh.MarkDynamic();

        arcObject.GetComponent<MeshFilter>().mesh = mesh;
        arcObject.GetComponent<MeshRenderer>().material = material;
    }

    void Update()
    {
        var initialDir = this.initialDir;
        if(initialDir == targetDir)
            initialDir = Quaternion.AngleAxis(2f, Vector3.back) * initialDir;

        float angleIncrements = Vector3.Angle(initialDir, targetDir) / quality;

        if (Vector3.Cross(initialDir, targetDir).z > 0)
        {
            //angleIncrements = -angleIncrements;
            initialDir = targetDir;
        }

        Vector3 currentPos = initialDir.normalized;
        Vector3 nextPos = (Quaternion.AngleAxis(angleIncrements, Vector3.back) * initialDir).normalized;

        Vector3[] vertices = new Vector3[4 * quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
        int[] triangles = new int[3 * 2 * quality];

        for (int i = 0; i < quality; i++)
        {
            int a = 4 * i;
            int b = 4 * i + 1;
            int c = 4 * i + 2;
            int d = 4 * i + 3;

            vertices[a] = transform.position;
            vertices[b] = currentPos * radius + transform.position;
            vertices[c] = nextPos * radius + transform.position;
            vertices[d] = transform.position;

            triangles[6 * i] = a;       // Triangle1: abc
            triangles[6 * i + 1] = b;
            triangles[6 * i + 2] = c;
            triangles[6 * i + 3] = c;   // Triangle2: cda
            triangles[6 * i + 4] = d;
            triangles[6 * i + 5] = a;

            currentPos = (Quaternion.AngleAxis(angleIncrements, Vector3.back) * currentPos).normalized;
            nextPos = (Quaternion.AngleAxis(angleIncrements, Vector3.back) * nextPos).normalized;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }

    /// <summary>
    /// Opens the arc 
    /// </summary>
    /// <param name="initialAngle"></param>
    public void EnableArc(Vector2 initialDir)
    {
        arcObject.SetActive(true);
        this.initialDir = initialDir.normalized;
        targetDir = initialDir;
        Update();
    }

    public void SetTargetDir(Vector2 target)
    {
        targetDir = target.normalized;
    }

    public void DisableArc()
    {
        arcObject.SetActive(false);
    }
}
