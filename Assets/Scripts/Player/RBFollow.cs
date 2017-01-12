using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class RBFollow : MonoBehaviour {

    public Transform target;
    Rigidbody2D rb;
	
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void FixedUpdate () {
        rb.MovePosition(target.position);
	}
}
