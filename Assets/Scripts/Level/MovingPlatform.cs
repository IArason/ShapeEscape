using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour {

    public MovementNode[] nodes;

    public int currentNode = 0;
    public float currentDuration = 0;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        var lastNode = currentNode - 1;
        lastNode = lastNode < 0 ? nodes.Length - 1 : lastNode;

        if (currentDuration > nodes[currentNode].duration)
        {
            currentNode++;
            currentDuration = 0;
        }
        if (currentNode >= nodes.Length)
        {
            currentNode = 0;
            currentDuration = 0;
        }

        lastNode = currentNode - 1;
        lastNode = lastNode < 0 ? nodes.Length - 1 : lastNode;

        rb.MovePosition(Vector3.Lerp(nodes[lastNode].position.position, 
            nodes[currentNode].position.position, currentDuration / nodes[lastNode].duration));
        rb.MoveRotation(Mathf.Lerp(nodes[lastNode].position.eulerAngles.z,
            nodes[currentNode].position.eulerAngles.z, currentDuration / nodes[lastNode].duration));

        currentDuration += Time.deltaTime;
	}

    [System.Serializable]
    public class MovementNode
    {
        public Transform position;
        public float duration;
    }
}
