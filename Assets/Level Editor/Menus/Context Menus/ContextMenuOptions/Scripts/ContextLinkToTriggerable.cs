using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ContextLinkToTriggerable : ContextMenuOption
{
    static List<LevelEntity> highlighted;
    /// <summary>
    /// Begins a process to link this triggerable object to a trigger.
    /// </summary>
    public override void OnSelect()
    {
        owners.ForEach(x => x.GetComponent<Trigger>().BeginLink());
    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);

        if (owners.Any(x => x.GetComponent<Trigger>() == null))
        {
            FailVerification();
        }
    }

    public override void OnPointerEnter()
    {
        if (highlighted == null || highlighted.Count == 0 || highlighted[0] == null)
        {
            highlighted = FindObjectsOfType(typeof(Triggerable)).Select(x =>
            ((MonoBehaviour)x).GetComponent<LevelEntity>()).ToList();
            Debug.Log("Got new highlighted list");
        }

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