using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class ContextDelete : ContextMenuOption
{
    public override void OnSelect()
    {
        
        base.OnSelect();
        ObjectManipulator.Instance.DestroyObjects(
            owners.Select(x => x.GetComponent<LevelEntity>()).ToList());
    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        if (owners.Any(x => x.GetComponent<LevelEntity>() == null))
        {
            FailVerification();
        }
    }
}