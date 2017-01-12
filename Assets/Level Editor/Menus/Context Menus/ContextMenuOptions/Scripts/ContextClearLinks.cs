using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContextClearLinks : ContextMenuOption
{
    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);

        if (owners.Any(x => x.GetComponent<Linkable>() == null))
        {
            FailVerification();
        }
    }

    public override void OnSelect()
    {
        base.OnSelect();
        foreach (var owner in owners)
            owner.GetComponent<Linkable>().ClearTargets();
    }
}