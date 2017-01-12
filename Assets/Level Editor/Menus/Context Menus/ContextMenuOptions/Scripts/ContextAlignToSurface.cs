using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContextAlignToSurface : ContextMenuOption
{
    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        if (owners.Any(x => x.GetComponent<ISurfaceAlignable>() == null))
        {
            FailVerification();
        }
    }

    public override void OnSelect()
    {
        base.OnSelect();

        List<Vector3> oldPoses = new List<Vector3>();
        List<Quaternion> oldRots = new List<Quaternion>();
        List<Vector3> newPoses = new List<Vector3>();
        List<Quaternion> newRots = new List<Quaternion>();


        foreach (var owner in owners)
        {
            oldRots.Add(owner.rotation);
            oldPoses.Add(owner.position);
            owner.GetComponent<ISurfaceAlignable>().AlignToSurface();
            newRots.Add(owner.rotation);
            newPoses.Add(owner.position);
        }

        new UndoTransformation(owners, oldPoses, newPoses, newRots, oldRots);
    }
}