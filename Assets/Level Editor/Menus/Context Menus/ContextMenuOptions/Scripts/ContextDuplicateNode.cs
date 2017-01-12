using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContextDuplicateNode : ContextMenuOption
{
    public override void OnSelect()
    {
        base.OnSelect();
        var clones = new List<Transform>();
        Debug.Log(owners[0].GetComponent<MoverHandle>().ParentMover);
        foreach(Transform t in owners)
        {
            clones.Add(t.GetComponent<MoverHandle>().ParentMover.GetComponent<Mover>().AddNode(t));
        }
        new UndoInstantiateNode(clones.Select(x => x.gameObject).ToList());
    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        foreach(var o in owners)
        {
            if (o.GetComponent<MoverHandle>() == null)
                FailVerification();
        }
    }
}
