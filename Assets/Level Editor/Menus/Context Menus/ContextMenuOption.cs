using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ContextMenuOption : MonoBehaviour
{
    protected List<Transform> owners;

    /// <summary>
    /// Called by ContextMenuTarget.
    /// Links self to ContextMenuTarget and provides its own type so ContextMenuTarget
    /// can populate a list of types of options it has.
    /// Should be overridden to check for any prerequisite components
    /// and call FailVerification if unsuccessful.
    /// </summary>
    public virtual void VerifyAndSetOwner(List<Transform> owners)
    {
        this.owners = owners;
    }

    public virtual void OnSelect()
    {
        ContextMenu.Instance.CloseMenu();
    }
    
    public virtual void OnPointerEnter() { }

    public virtual void OnPointerExit() { }

    protected void FailVerification()
    {
        Debug.LogError("Failed verification for " + GetType().ToString() + " on " + owners[0].name);
    }
}
