using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
    
    static MeshRenderer mr;

    static float initialAlpha;

	// Use this for initialization
	void Start () {
        var gridQuad = transform.GetChild(0);
        mr = gridQuad.GetComponent<MeshRenderer>();
        mr.material.SetFloat("_Mode", 2.0f);
        initialAlpha = mr.material.color.a;
	}

    void Update()
    {
        var pos = Camera.main.transform.position;
        pos.z = 0;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.position = pos;
    }
	
    /// <summary>
    /// Sets grid multiplier (powers of two only).
    /// </summary>
    /// <param name="value"></param>
    public static void SetGridTransparency(float value)
    {
        var c = mr.material.color;
        c.a = Mathf.Min(1f / value, initialAlpha);
        mr.material.color = c;
    }
}
