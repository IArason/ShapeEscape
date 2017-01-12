using UnityEngine;

[System.Serializable]
public class SerializedVector3
{
    public float xPos;
    public float yPos;
    public float zPos;

    public SerializedVector3() { }

    public SerializedVector3(Vector3 pos)
    {
        xPos = pos.x;
        yPos = pos.y;
        zPos = pos.z;
    }

    public Vector3 ToVector()
    {
        return new Vector3(xPos, yPos, zPos);
    }
}
