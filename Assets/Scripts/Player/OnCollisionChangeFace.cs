using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnCollisionChangeFace : MonoBehaviour {

    public MeshRenderer[] faceMeshes;
    public Texture hurtFace;
    public float hurtTime = 0.5f;
    public float minImpact = 1f;

    bool hurt = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.relativeVelocity.magnitude < minImpact || hurt) return;

        List<Texture> texs = new List<Texture>();
        foreach(MeshRenderer r in faceMeshes)
        {
            texs.Add(r.material.mainTexture);
            r.material.mainTexture = hurtFace;
        }
        StartCoroutine(StopHurtAfter(hurtTime, texs));
        hurt = true;
    }

    IEnumerator StopHurtAfter(float time, List<Texture> texs)
    {
        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for(int i = 0; i < texs.Count; i++)
        {
            faceMeshes[i].material.mainTexture = texs[i];
        }
        hurt = false;
    }
}
