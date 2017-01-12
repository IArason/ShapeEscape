using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;

/// <summary>
/// TODO: Fix the polygon fill
/// TODO: Add a check for whether polygon fill needs refreshing
/// TODO: Clean up debug code
/// </summary>
[RequireComponent(typeof(LevelEntity))]
public class Polygon : LevelEntityAttribute {

    [SerializeField]
    bool debugHandles = false;

    [SerializeField]
    public List<Transform> vertices = new List<Transform>();

    public float meshDepth = 1f;
    public Color color = Color.cyan;

    [SerializeField]
    bool showVisualization = false;

    [SerializeField]
    bool centerOnVertices = true;

    Mesh mesh;

    bool selected = false;

    public void SetVertices(Vector3[] vertices)
    {
        // Store a vertex example
        var vertexObject = Instantiate(this.vertices[0].gameObject);

        for (int i = 0; i < this.vertices.Count;)
        {
            Destroy(this.vertices[i].gameObject);
            this.vertices.RemoveAt(i);
        }
        this.vertices.Clear();
        foreach (Vector3 v in vertices)
        {
            Debug.DrawRay(transform.position, v, Color.green, 5f);

            var tr = Instantiate(vertexObject).transform;
            tr.SetParent(transform);
            tr.localPosition = v;
            this.vertices.Add(tr);
        }

        mesh.triangles = UpdateFaces(mesh.triangles);
        // Clear the example object
        Destroy(vertexObject);
    }

    // Use this for initialization
    protected override void Awake() {
        base.Awake();
        GenerateMesh();
        RefreshVertices();
    }

    protected override void OnPointerEnter()
    {
        if(centerOnVertices)
            UpdateCenter();
    }

    // Update is called once per frame
    void Update () {
        var check = false;

        foreach (Transform t in vertices)
        {
            if (t.hasChanged)
                check = true;

            t.hasChanged = false;
        }

        if (check)
        {
            if (check) color = Color.cyan;
            else color = Color.red;
            UpdateMesh();
            mesh.triangles = UpdateFaces(mesh.triangles);
            check = false;
        }

        if (showVisualization)
        {
            vertices.Add(vertices[0]);
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                Debug.DrawLine(vertices[i].position, vertices[i + 1].position, IsSelfValid() ? Color.green : Color.red);
            }

            // Clean up
            vertices.RemoveAt(vertices.Count - 1);
        }
	}

    // Moves vertex meshes forward to match terrain width.
    void RefreshVertices()
    {
        foreach(Transform t in vertices)
        {
            t.GetComponentInChildren<Collider2D>().transform.localPosition = Vector3.back * meshDepth * 1.05f / 2;
        }
    }

    void UpdateCenter()
    {
        Vector3 total = Vector3.zero;
        Dictionary<Transform, Transform> vertexParents = new Dictionary<Transform, Transform>();
        foreach(Transform t in vertices)
        {
            vertexParents.Add(t, t.parent);
            t.SetParent(null, true);
            total += t.position;
        }

        total /= vertices.Count;
        total = LevelEditorUtils.GetClosestSnappedToGrid(ObjectManipulator.Instance.gridUnit, total);

        transform.position = total;
        foreach (Transform t in vertices)
        {
            t.SetParent(vertexParents[t]);
        }

    }

    void GenerateMesh()
    {
        // Tris: (2dverts - 1) * 4
            // Strip: 2dverts * 2
            // Faces: (2dverts - 2) * 2
        // Verts: 2dverts * 6
            // Strip: 2dverts * 4
            // Faces: 2dverts * 2

        mesh = new Mesh();

        mesh.vertices = new Vector3[vertices.Count * 6];

        // Set vertex positions
        UpdateMesh();

        
        int[] tris = new int[(vertices.Count - 1) * 4 * 3];
        // Strip
        for (int i = 0; i < vertices.Count; i++)
        {
            tris[i * 6] = i * 4;
            tris[i * 6 + 1] = i * 4 + 2;
            tris[i * 6 + 2] = i * 4 + 3;

            tris[i * 6 + 3] = i * 4;
            tris[i * 6 + 4] = i * 4 + 3;
            tris[i * 6 + 5] = i * 4 + 1;
        }

        UpdateFaces(tris);

        mesh.triangles = tris;

        mesh.normals = new Vector3[mesh.vertices.Length];
        
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Updates the mesh to reflect the new vertex positions
    void UpdateMesh()
    {
        Vector3[] verts = mesh.vertices;

        // Strip - Verified
        for (int i = 2; i < vertices.Count * 4; i++)
        {
            verts[i - 2] = transform.InverseTransformPoint(vertices[i / 4].position) + (i % 2 == 1 ? -1 : 1) * (Vector3.forward * (meshDepth / 2));
        }

        verts[vertices.Count * 4 - 2] = transform.InverseTransformPoint(vertices[0].position) + (Vector3.forward * (meshDepth / 2));
        verts[vertices.Count * 4 - 1] = transform.InverseTransformPoint(vertices[0].position) - (Vector3.forward * (meshDepth / 2));

        // Faces
        // Does each face in sequence, each face getting (2dverts - 2) vertices
        // Don't ever touch this ever
        for (int i = 0; i < vertices.Count; i++)
        {
            verts[i + vertices.Count * 4] = transform.InverseTransformPoint(vertices[i].position) - 
                (Vector3.forward * (meshDepth / 2));
            verts[Mathf.Abs(vertices.Count -1 - i) + vertices.Count + vertices.Count * 4] = 
                transform.InverseTransformPoint(vertices[i].position) + (Vector3.forward * (meshDepth / 2));
        }

        mesh.vertices = verts;

        Vector2[] uvs = new Vector2[mesh.vertexCount];

        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = mesh.vertices[i];
        }

        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        // Update the polygon collider.
        if (GetComponent<PolygonCollider2D>() != null)
            GetComponent<PolygonCollider2D>().points = vertices.Select(x => (Vector2)transform.InverseTransformPoint(x.position)).ToArray();
    }

    int[] UpdateFaces(int[] tris)
    {

        // Polygon fill
        // Front face

        // Don't need the back faces
        /*
        Vector3[] backVerts = mesh.vertices.ToList().GetRange(vertices.Count * 4, vertices.Count).ToArray();
        int[] back = new Triangulator(backVerts).Triangulate();
        // Swap rotation so they face the right way
        for (int i = 0; i < back.Length; i += 3)
        {
            var temp = back[i];
            back[i] = back[i + 2];
            back[i + 2] = temp;
        }
        */

        // Back face
        Vector3[] frontVerts = mesh.vertices.ToList().GetRange(vertices.Count * 4 + vertices.Count, vertices.Count).ToArray();

        int[] front = new Triangulator(frontVerts).Triangulate(vertices.Count * 4);

        // Flip clockwise/cc
        /*
        for (int i = 0; i < front.Length; i += 3)
        {
            var temp = front[i];
            front[i] = front[i + 2];
            front[i + 2] = temp;
        }
        */
        
        // Move the vertex points forward to where the front and back faces are stored
        for (int i = 0; i < front.Length; i++)
        {
            //front[i] += vertices.Count * 4;
            //back[i] += vertices.Count * 4 + vertices.Count;
        }

        // Copy the front and back arrays into the triangle array
        Array.Copy(front, 0, tris, vertices.Count * 6, front.Length);
        //Array.Copy(back, 0, tris, vertices.Count * 6 + front.Length, back.Length);

        return tris;
    }

    bool IsSelfValid()
    {
        // Lazier than lazy -- add first element again as last to complete polygon loop

        vertices.Add(vertices[0]);
        for(int i = 0; i < vertices.Count - 1; i++)
        {
            for(int j = i + 1; j < vertices.Count - 1; j++)
            {
                if (DoLinesIntersect(transform.InverseTransformPoint(vertices[i].position),
                    transform.InverseTransformPoint(vertices[i+1].position),
                    transform.InverseTransformPoint(vertices[j].position),
                    transform.InverseTransformPoint(vertices[j+1].position)))
                {
                    // Clean up
                    vertices.RemoveAt(vertices.Count - 1);
                    return false;
                }
            }
        }

        // Clean up
        vertices.RemoveAt(vertices.Count - 1);
        return true;
    }

#if UNITY_EDITOR
    // Mesh gen debug stuff
    void OnDrawGizmos()
    {
        if (!debugHandles) return;

        Dictionary<Vector3, int> stuff = new Dictionary<Vector3, int>();
        if (mesh == null) return;
        
        for(int i = 0; i < mesh.vertexCount; i++)
        {
            if (stuff.ContainsKey(mesh.vertices[i])) stuff[mesh.vertices[i]] += 1;
            else stuff.Add(mesh.vertices[i], 1);
            Handles.Label(mesh.vertices[i] + mesh.vertices[i] * stuff[mesh.vertices[i]] * 0.05f, i.ToString());

        }
        
        for (int i = 0; i < mesh.triangles.Length; i+=3)
        {
            Vector3 pos = (mesh.vertices[mesh.triangles[i]] + mesh.vertices[mesh.triangles[i + 1]] + mesh.vertices[mesh.triangles[i + 2]]) / 3;

            if (stuff.ContainsKey(pos)) stuff[pos] += 1;
            else stuff.Add(pos, 1);

            Handles.Label(pos + pos * 0.1f * stuff[pos], i.ToString() +"|"+ (i + 1).ToString() + "|" + (i + 2).ToString());
        }
    }
#endif

    /// <summary>
    /// Checks if two lines intersect
    /// </summary>
    static bool DoLinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        // Checks if lines share a vertex, then if they are the same
        Vector2[] list = { p1, p2, p3, p4 };

        bool duplicates = false;
        for(int i = 0; i < list.Length; i++)
        {
            for(int j = i + 1; j < list.Length; j++)
            if(list[i].Equals(list[j]))
            {
                duplicates = true;
                break;
            }
        }

        // If any two points are the same
        if (duplicates)
        {
            // Both lines are identical and overlap
            if ((p1 == p3 && p2 == p4) || (p1 == p4 && p3 == p2))
                return true;
            return false;
        }

        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;

        float alphaNumerator = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator = a.x * c.y - a.y * c.x;
        float betaDenominator = a.y * b.x - a.x * b.y;

        bool doIntersect = true;

        if (alphaDenominator == 0 || betaDenominator == 0)
        {
            doIntersect = false;
        }
        else {

            if (alphaDenominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                {
                    doIntersect = false;

                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
            {
                doIntersect = false;
            }

            if (doIntersect && betaDenominator > 0)
            {
                if (betaNumerator < 0 || betaNumerator > betaDenominator)
                {
                    doIntersect = false;
                }
            }
            else if (betaNumerator > 0 || betaNumerator < betaDenominator)
            {
                doIntersect = false;
            }
        }
        

        return doIntersect;
    }

    public class Triangulator
    {
        private List<Vector3> m_points = new List<Vector3>();

        public void initTriangulator(List<Vector3> points, Vector3 normal)
        {
            var quad = Quaternion.FromToRotation(normal, Vector3.forward);

            foreach (var v in points)
                m_points.Add(quad * v);
        }

        public Triangulator(Vector3[] points)
        {
            m_points = new List<Vector3>(points);
        }

        public int[] Triangulate(int offset)
        {
            var indices = new List<int>();

            var n = m_points.Count;
            if (n < 3)
                return indices.ToArray();

            var V = new int[n];
            if (Area() > 0)
            {
                for (var v = 0; v < n; v++)
                    V[v] = v;
            }
            else {
                for (var v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            var nv = n;
            var count = 2 * nv;
            var m = 0;
            for (var v = nv - 1; nv > 2;)
            {
                if ((count--) <= 0)
                    return indices.ToArray();

                var u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                var w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V))
                {
                    int a;
                    int b;
                    int c;
                    int s;
                    int t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(offset + a);
                    indices.Add(offset + b);
                    indices.Add(offset + c);
                    m++;
                    s = v;
                    for (t = v + 1; t < nv; t++)
                    {
                        V[s] = V[t];
                        s++;
                    }
                    nv--;
                    count = 2 * nv;
                }
            }

            return indices.ToArray();
        }

        private float Area()
        {
            int n = m_points.Count;
            float A = 0.0f;
            int q = 0;
            for (var p = n - 1; q < n; p = q++)
            {
                var pval = m_points[p];
                var qval = m_points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return (A * 0.5f);
        }

        private bool Snip(int u, int v, int w, int n, int[] V)
        {
            int p;
            var A = m_points[V[u]];
            var B = m_points[V[v]];
            var C = m_points[V[w]];

            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                var P = m_points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float ax;
            float ay;
            float bx;
            float by;
            float cx;
            float cy;
            float apx;
            float apy;
            float bpx;
            float bpy;
            float cpx;
            float cpy;
            float cCROSSap;
            float bCROSScp;
            float aCROSSbp;

            ax = C.x - B.x; ay = C.y - B.y;
            bx = A.x - C.x; by = A.y - C.y;
            cx = B.x - A.x; cy = B.y - A.y;
            apx = P.x - A.x; apy = P.y - A.y;
            bpx = P.x - B.x; bpy = P.y - B.y;
            cpx = P.x - C.x; cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp > 0.0) && (bCROSScp > 0.0) && (cCROSSap > 0.0));
        }
    }

    #region Serialization

    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        return new SerializedPolygon(vertices.ToArray());
    }

    public class SerializedPolygon : Serialized
    {
        public SerializedVector3[] vertices;

        public SerializedPolygon() { }

        public SerializedPolygon(Transform[] vertices)
        {
            this.vertices = new SerializedVector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[i] = new SerializedVector3(vertices[i].localPosition);
            }
        }

        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            var poly = idList[parentID].gameObject.GetComponent<Polygon>();
            poly.SetVertices(vertices.Select(x => x.ToVector()).ToArray());
        }
    }

    #endregion
}
