using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class UndoDestroyTriggerLinks : UndoAction
{
    Linkable owner;
    LevelEntity[] targets;

    public UndoDestroyTriggerLinks(Linkable owner, List<LevelEntity> targets)
    {
        // get call stack
        StackTrace stackTrace = new StackTrace();

        // get calling method name
        UnityEngine.Debug.Log(stackTrace.GetFrame(1).GetMethod().Name);


        this.owner = owner;
        this.targets = targets.ToArray();

        if(Validate())
            UndoManager.Instance.AddAction(this);
    }

    public UndoDestroyTriggerLinks(Linkable owner, LevelEntity target)
    {
        // get call stack
        StackTrace stackTrace = new StackTrace();

        // get calling method name
        UnityEngine.Debug.Log(stackTrace.GetFrame(1).GetMethod().Name);

        this.owner = owner;
        targets = new LevelEntity[] { target };

        if (Validate())
            UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        owner.RemoveTargets(targets.ToList(), true);
    }

    public override void Undo()
    {
        owner.AddTargets(targets.ToList());
    }

    bool Validate()
    {
        return !(owner == null || targets == null || targets.Length == 0);
    }
}
