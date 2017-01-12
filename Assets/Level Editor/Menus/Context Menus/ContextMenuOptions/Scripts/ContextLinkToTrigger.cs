using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContextLinkToTrigger : ContextMenuOption
{
    static List<LevelEntity> highlighted;
    /// <summary>
    /// Begins a process to link this triggerable object to a trigger.
    /// </summary>
    public override void OnSelect()
    {
        Debug.Log("Selected");
        base.OnSelect();
        owners.ForEach(x => x.GetComponent<Triggerable>().BeginLink());
    }
    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        if (owners.Any(x => x.GetComponent<Triggerable>() == null))
        {
            FailVerification();
        }
    }

    public override void OnPointerEnter()
    {
        if (highlighted == null || highlighted.Count == 0 || highlighted[0] == null)
            highlighted = FindObjectsOfType(
                typeof(Trigger)).Select(x => ((MonoBehaviour)x).
                GetComponent<LevelEntity>()).ToList();

        foreach (LevelEntity e in highlighted)
        {
            e.OnPointerEnter();
        }
    }

    public override void OnPointerExit()
    {
        foreach (LevelEntity e in highlighted)
        {
            e.OnPointerExit();
        }
    }
}