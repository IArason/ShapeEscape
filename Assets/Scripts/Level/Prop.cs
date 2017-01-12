using UnityEngine;
using System.Collections;

public class Prop : MonoBehaviour {

    Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.position;
    }

	void OnCollisionEnter2D(Collision2D col)
    {

    }

    public void Kill()
    {
        transform.position = originalPos;
    }
}
