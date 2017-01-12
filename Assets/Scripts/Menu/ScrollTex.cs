using UnityEngine;
using System.Collections;

public class ScrollTex : MonoBehaviour
{
    //public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";

    public bool backAndForth = true;

    Vector2 uvOffset = Vector2.zero;

    void LateUpdate()
    {
        if (backAndForth)
        {
            uvOffset = new Vector2(Mathf.Sin(uvAnimationRate.x * Time.time) / 4, uvAnimationRate.y * Time.deltaTime + uvOffset.y);
        }
        else
        {
            uvOffset += (uvAnimationRate * Time.deltaTime);
        }

        if (GetComponent<Renderer>().enabled)
        {
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset(textureName, uvOffset);
        }
    }
}