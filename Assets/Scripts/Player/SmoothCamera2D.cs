using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    new Camera camera;
    float initialZ;

    public float rbSpeedPredict = 0.2f;

    public float minSpeedThreshold = 1f;
    public float maxSpeedThreshold = 10f;
    public float maxZChange = 10f;
    public float zoomDamp = 1f;

    public bool useZ = false;

    void Awake()
    {
        initialZ = transform.position.z;
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

            Vector3 point = camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position -
                camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)) + (rb != null ?
                (Vector3)rb.velocity * rbSpeedPredict : Vector3.zero);
            if (useZ) delta.z = target.position.z - transform.position.z; 
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

            float newZ = transform.position.z;
            if (rb && !useZ)
            {
                var speed = Vector3.Magnitude(rb.velocity);
                newZ = (Mathf.Max(speed - minSpeedThreshold, 0)) / (maxSpeedThreshold - minSpeedThreshold) * maxZChange;

                newZ = Mathf.Lerp(transform.position.z, initialZ - newZ, Time.deltaTime * zoomDamp);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, useZ ? target.position.z : newZ);
        }
        else
        {
            var t = FindObjectOfType<Player>();
            target = t == null ? null : t.transform;
        }

    }
}