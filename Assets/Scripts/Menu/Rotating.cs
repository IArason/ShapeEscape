using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Rotating : MonoBehaviour {

    public float speed = 1;
    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        rb.MoveRotation(-Time.time * 360 * speed);
	}
}
