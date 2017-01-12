using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Functions which require triggering will register here.
/// OnTriggered is called by Trigger objects.
/// </summary>
public class Triggerable : Linkable {

    // Event for actions on trigger.
    private event Action OnTriggeredEvent;

    // Used purely for link visualization.
    [SerializeField]
    private List<LevelEntity> triggers = new List<LevelEntity>();

    /// Menu editable buttons
    [SerializeField]
    bool toggleButton = true;

    [SerializeField] // Should only be displayed when toggleButton is false
    bool resetOnRespawn = false;

    protected override void Awake()
    {
        base.Awake();
        // Just in case
        isSerializable = false;
        foreach(var t in triggers)
        {
            t.GetComponent<Trigger>().AddTarget(GetComponent<LevelEntity>(), true);
        }
    }

    public override void AddFirstTarget(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        foreach (LevelEntity l in entities)
        {
            if (l == null) continue;

            var trigger = l.GetComponentInParent<Trigger>();
            
            if(trigger != null)
            {
                trigger.AddTarget(targetEntity, ignoreUndo);
                if(!triggers.Contains(l))
                {
                    triggers.Add(l);
                }
                return;
            }
        }
    }

    /// <summary>
    /// For redo use only.
    /// </summary>
    public override void AddTargets(List<LevelEntity> entities)
    {
        foreach (LevelEntity l in entities)
        {
            AddTarget(l, true);
        }
    }

    public override void RemoveTarget(LevelEntity entity, bool ignoreUndo = false)
    {
        if (triggers.Contains(entity))
        {
            entity.GetComponent<Trigger>().RemoveTarget(targetEntity);
            triggers.Remove(entity);

            if (!ignoreUndo)
                new UndoDestroyTriggerLinks(this, entity);
        }
    }

    public override void RemoveFirstTarget(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        foreach (LevelEntity l in entities)
        {
            var trigger = l.GetComponent<Trigger>();
            if (triggers.Contains(l))
            {
                trigger.RemoveTarget(targetEntity, ignoreUndo);
                triggers.Remove(l);
                return;
            }
        }
    }

    public override void RemoveTargets(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        foreach(LevelEntity e in entities)
        {
            RemoveTarget(e.GetComponent<LevelEntity>(), true);
        }

        if (!ignoreUndo)
            new UndoDestroyTriggerLinks(this, entities);
    }

    public override void ClearTargets(bool ignoreUndo = false)
    {
        if (!ignoreUndo)
            new UndoDestroyTriggerLinks(this, triggers);

        for (; triggers.Count > 0;)
        {
            triggers[0].GetComponent<Trigger>().RemoveTarget(targetEntity, true);
        }
    }


    public void OnTriggered()
    {
        if (gameObject.activeInHierarchy && OnTriggeredEvent != null)
            OnTriggeredEvent.Invoke();
    }

    public void Listen(Action callback)
    {
        OnTriggeredEvent += callback;
    }

    public void StopListen(Action callback)
    {
        OnTriggeredEvent -= callback;
    }
    
    protected override void DrawConnections()
    {
        foreach (LevelEntity t in triggers)
        {
            LineDrawer.DrawLine(transform.position, t.transform.position, Color.blue * (selected ? 1f : 0.5f), 2f, true);
        }
        if (waitForClick)
        {
            LineDrawer.DrawLine(transform.position,
                LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition),
                Color.blue, 2f, true);
        }
    }

    public override List<LevelEntity> GetTargets()
    {
        return triggers;
    }
}
