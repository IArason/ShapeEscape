using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ContextDeleteNode : ContextDelete
{

    public override void OnSelect()
    {
        ContextMenu.Instance.CloseMenu();
        // Destroys the nodes on its own
        new UndoDestroyNodes(owners.Select(x => x.gameObject).ToList());
    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        if (owners.Any(x => x.GetComponent<MoverHandle>() == null))
        {
            FailVerification();
        }
    }
}
