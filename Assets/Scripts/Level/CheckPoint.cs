using UnityEngine;
using System.Collections;
using System;

public class CheckPoint : LevelEntityAttribute, ISurfaceAlignable {

    [SerializeField]
    private static CheckPoint currentCheckpoint;
    
    public Transform checkpointFlag;

    public Texture2D inactiveTex;
    public Texture2D activeTex;

    public AudioClip activatedSound;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Triggerable>().Listen(SetCheckpoint);
    }
    
    public void Deactivate()
    {
        checkpointFlag.GetComponent<Renderer>().material.SetTexture("_MainTex", inactiveTex);
    }

    public void Activate()
    {
        if(activatedSound != null)
            AudioSource.PlayClipAtPoint(activatedSound, transform.position);
        checkpointFlag.GetComponent<Renderer>().material.SetTexture("_MainTex", activeTex);
    }
    

    // Use this for initialization
    public void SetCheckpoint()
    {
        Debug.Log("Set checkpoint");
        if (currentCheckpoint != null)
            currentCheckpoint.Deactivate();
        currentCheckpoint = this;
        Activate();
    }

    public static Vector3 GetSpawnLoc()
    {
        return currentCheckpoint == null ? 
            FindObjectOfType<PlayerSpawner>().transform.position :
            currentCheckpoint.transform.position + currentCheckpoint.transform.up;
    }

    // Undoing handled in context menu button
    public void AlignToSurface()
    {
        var hits = Physics2D.LinecastAll(transform.position, transform.position - transform.up * 10);
        if (hits.Length != 0)
        {
            RaycastHit2D closest = hits[0];
            foreach(var v in hits)
            {
                if (v.transform.tag != "LevelBlock") continue;

                if (closest.transform.tag != "LevelBlock" || 
                    Vector3.Distance(transform.position, v.point) <
                    Vector3.Distance(transform.position, closest.point))
                    closest = v;
            }
            if (closest.transform.tag != "LevelBlock") return;
            
            transform.rotation = Quaternion.LookRotation(Vector3.forward, closest.normal);
            transform.position = (Vector3)closest.point + transform.up + Vector3.forward * transform.position.z;
        }
    }
}