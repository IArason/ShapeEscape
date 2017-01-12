using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    public static Vector3 XY(this Vector3 v)
    {
        return new Vector3(v.x, v.y, 0);
    }
}