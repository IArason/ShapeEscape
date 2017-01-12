using UnityEngine;
using System.Collections;

/// <summary>
/// Used in combination with EditorCamera.
/// </summary>
public class FreeCamera : MonoBehaviour {

    public float panSensitivity = 4f;
    Vector2 oldMousePos;

	// Update is called once per frame
	void Update () {
        //ControllerControls();
        PCControls();
    }

    void PCControls()
    {
        if(Input.GetMouseButton(2))
        {
            TranslateWithMouse(oldMousePos, Input.mousePosition);
        }
        
        oldMousePos = Input.mousePosition;
    }

    
    void ControllerControls()
    {
        Vector2 leftStick = new Vector2(Input.GetAxis("L_XAxis_1"), -Input.GetAxis("L_YAxis_1")) * panSensitivity;

        transform.Translate(((Vector3)leftStick) *
            Time.deltaTime, Space.World);
    }

    void TranslateWithMouse(Vector2 start, Vector2 end)
    {
        var oldRay = Camera.main.ScreenPointToRay(start);
        var newRay = Camera.main.ScreenPointToRay(end);

        var plane = new Plane(-Vector3.forward, Vector3.zero);
        float oldDist;
        float newDist;
        if (plane.Raycast(oldRay, out oldDist) && plane.Raycast(newRay, out newDist))
        {
            // If snap, use mouse position instead of motion delta.
            var delta =
                (newRay.origin + newRay.direction * newDist) -
               (oldRay.origin + oldRay.direction * oldDist);

            transform.position -= delta;
        }
    }
}
