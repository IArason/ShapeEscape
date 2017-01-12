using System;
using UnityEngine;

public class UndoTriggerLink : UndoAction
{
    // We can use the actual classes as Triggerable only calls AddTarget(self) on Trigger.
    Trigger owner;
    LevelEntity target;

    public UndoTriggerLink(Trigger owner, LevelEntity target)
    {
        this.owner = owner;
        this.target = target;

        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        owner.AddTarget(target, true);
    }

    public override void Undo()
    {
        owner.RemoveTarget(target, true);
    }
}
