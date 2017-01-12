using UnityEngine;

[System.Serializable]
public class SerializedQuaternion
{
    public float xRot;
    public float yRot;
    public float zRot;
    public float wRot;

    public SerializedQuaternion() { }

    public SerializedQuaternion(Quaternion rot)
    {
        xRot = rot.x;
        yRot = rot.y;
        zRot = rot.z;
        wRot = rot.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(xRot, yRot, zRot, wRot);
    }
}