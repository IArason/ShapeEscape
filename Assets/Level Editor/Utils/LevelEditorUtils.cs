using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public static class LevelEditorUtils
{


    /// <summary>
    /// Returns a direction rotated a multiple of <para>degrees</para> from Vector2.up
    /// </summary>
    /// <param name="degrees">Degrees</param>
    /// <param name="dir">Original direction</param>
    /// <returns></returns>
    public static Vector2 GetClosestSnappedDir(float degrees, Vector2 dir)
    {
        List<Vector2> possibleDirs = new List<Vector2>();
        for (float i = 0; i < 360; i += degrees)
        {
            possibleDirs.Add(Quaternion.AngleAxis(i, Vector3.forward) * Vector3.up);
        }
        Vector2 closest = possibleDirs[0];

        foreach (Vector2 d in possibleDirs)
        {
            if (Vector2.Dot(d, dir) > Vector2.Dot(closest, dir)) closest = d;
        }

        return closest;
    }

    /// <summary>
    /// Returns a position snapped to a grid with unit size <para>gridSize</para>
    /// </summary>
    /// <param name="gridSize">The unit size of the grid</param>
    /// <param name="pos">The original position</param>
    /// <returns></returns>
    public static Vector2 GetClosestSnappedToGrid(float gridSize, Vector2 pos)
    {
        float x, y;
        x = Mathf.Round(pos.x * (1 / gridSize));
        y = Mathf.Round(pos.y * (1 / gridSize));
        return new Vector2(x / (1 / gridSize), y / (1 / gridSize));
    }

    public static T GetObjectAtScreenPoint<T>(Vector2 pos, bool ignoreUIButtons = false) where T : MonoBehaviour
    {
        if (!ignoreUIButtons && MouseOverUIElement()) return null;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hit, Mathf.Infinity))
        {
            return hit.transform.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// Checks if the mouse is over a UI button.
    /// </summary>
    public static bool MouseOverUIElement()
    {
        // Prevents nullref exception
        if (EventSystem.current == null) return false;
        
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Caching ScreenToWorldPlane with mouse coords every frame.
    static float timeStamp;
    static Vector3 mousePos;

    /// <summary>
    /// Returns the mouse's intersection with an XY plane at Z zero.
    /// </summary>
    /// <param name="screenPos">The screen position to check at.</param>
    public static Vector2 ScreenTo2DWorldPlane(Vector2 screenPos)
    {
        // Mouse pos is checked a LOT, so we cache it every frame.
        if(screenPos == (Vector2)Input.mousePosition)
        {
            if(timeStamp == Time.time)
            {
                return mousePos;
            }
        }

        var ray = Camera.main.ScreenPointToRay(screenPos);
        
        var plane = new Plane(-Vector3.forward, Vector3.zero);
        float dist;

        if (plane.Raycast(ray, out dist))
        {
            var result = ray.origin + ray.direction * dist;
            if (screenPos == (Vector2)Input.mousePosition)
            {
                timeStamp = Time.time;
                mousePos = result;
            }
            return result;
        }
        return Vector3.zero;
    }

    public static Vector2 GetScreenPositionOfTransform(Transform transform)
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    public static bool IsOnGrid(float gridSize, Vector3 pos)
    {
        float x = pos.x;
        var diffx = Mathf.Abs(x % gridSize);

        float y = pos.y;
        var diffy = Mathf.Abs(y % gridSize);
        return (diffx < 0.0001f || diffx > 0.9999f) && (diffy < 0.0001f || diffy > 0.9999f);
    }

    public static void DrawLineInWorld(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
    {
        pointA = Camera.main.WorldToScreenPoint(pointA);
        pointB = Camera.main.WorldToScreenPoint(pointB);
        ScreenDraw.DrawLine(pointA, pointB, color, width, antiAlias);
    }
    
}
