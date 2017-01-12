using UnityEngine;
using System.Collections;

/// <summary>
/// Smoothly follows a given target.
/// </summary>
public class EditorCamera : MonoBehaviour
{
    [SerializeField]
    public float dampTime = 0.15f;
    [SerializeField]
    float zoomSensitivity = 8f;
    [SerializeField]
    float zoomDamp = 0.05f;
    [SerializeField]
    FreeCamera target;
    [SerializeField]
    float maxZoom = 5f;
    [SerializeField]
    float minZoom = 80f;

    [SerializeField]
    float gridZoomTransparencyCoefficient = 6f;

    Vector3 moveVelocity = Vector3.zero;
    float zoomVelocity = 0;
    new Camera camera;
    float initialZ;
    float targetZoom;

    void Awake()
    {
        initialZ = transform.position.z;
        camera = GetComponent<Camera>();
        targetZoom = camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly follow target
        if (target)
        {
            Vector3 point = camera.WorldToViewportPoint(target.transform.position);
            Vector3 delta = target.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref moveVelocity, dampTime);
        }

        // Zoom
        float triggers = (Input.GetAxis("TriggersR_1") - Input.GetAxis("TriggersL_1")) * zoomSensitivity;
        float mouseScroll = Input.mouseScrollDelta.y * zoomSensitivity;

        targetZoom -= (triggers + mouseScroll) *
            Mathf.Sqrt(Mathf.Abs(targetZoom)) * zoomSensitivity;

        targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);

        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetZoom, ref zoomVelocity, zoomDamp);

        Grid.SetGridTransparency(camera.orthographicSize / gridZoomTransparencyCoefficient);
    }
}