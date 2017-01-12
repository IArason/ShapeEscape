using UnityEngine;

public class ColorMultiplier : MonoBehaviour
{
    [SerializeField]
    Color color;
    [Range(0, 1), SerializeField]
    float power;

    [BitStrap.Button]
    void Multiply()
    {
        var mr = GetComponent<MeshRenderer>();
        var sr = GetComponent<SpriteRenderer>();

        if (mr != null) mr.material.color = mr.material.color * (1-power) + color * (power);
        if (sr != null) sr.material.color = sr.material.color * (1-power) + color * (power);

        Debug.Log(mr + " | " + sr);
    }
}
