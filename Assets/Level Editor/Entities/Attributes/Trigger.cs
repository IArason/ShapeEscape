using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Trigger : Linkable
{
    event Action triggerEvent;

    public override void AddFirstTarget(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        foreach(LevelEntity l in entities)
        {
            if (l == null) continue;

            var triggerable = l.GetComponent<Triggerable>();
            if(triggerable != null && (triggerEvent == null || 
                !triggerEvent.GetInvocationList().Contains((Action)triggerable.OnTriggered)))
            {
                triggerEvent += (Action)triggerable.OnTriggered;
                triggerable.AddTarget(targetEntity);

                if(!ignoreUndo)
                    new UndoTriggerLink(this, l);

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
            var triggerable = l.GetComponent<Triggerable>();
            if (triggerable != null && (triggerEvent == null 
                || !triggerEvent.GetInvocationList().Contains((Action)triggerable.OnTriggered)))
            {
                triggerEvent += (Action)triggerable.OnTriggered;
                triggerable.AddTarget(targetEntity, true);
            }
        }
    }

    public override void RemoveTarget(LevelEntity triggerable, bool ignoreUndo = false)
    {
        var t = triggerable.GetComponent<Triggerable>();
        if (t != null && triggerEvent != null && triggerEvent.GetInvocationList().Contains((Action)t.OnTriggered))
        {
            triggerEvent -= (Action)t.OnTriggered;
            t.RemoveTarget(targetEntity, ignoreUndo);

            if (!ignoreUndo)
                new UndoDestroyTriggerLinks(this, targetEntity);
        }
    }

    public override void RemoveFirstTarget(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        foreach (LevelEntity l in entities)
        {
            var triggerable = l.GetComponent<Triggerable>();
            if (triggerable != null && (triggerEvent != null &&
                triggerEvent.GetInvocationList().Contains((Action)triggerable.OnTriggered)))
            {
                triggerEvent -= (Action)triggerable.OnTriggered;
                triggerable.RemoveTarget(targetEntity);

                UnityEngine.Debug.Log("Success removing first");

                if (!ignoreUndo)
                    new UndoDestroyTriggerLinks(this, l);

                return;
            }
        }
    }

    public override void RemoveTargets(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        foreach (LevelEntity e in entities)
        {
            RemoveTarget(e, true);
        }

        if(!ignoreUndo)
            new UndoDestroyTriggerLinks(this, entities);
    }

    public override void ClearTargets(bool ignoreUndo = false)
    {
        RemoveTargets(GetTargets(), ignoreUndo);
        triggerEvent = null;
    }

    public void OnTrigger()
    {
        Debug.Log("Activated trigger");
        
        if (triggerEvent != null)
            triggerEvent.Invoke();
    }

    protected override void DrawConnections()
    {
        if (triggerEvent != null)
            foreach (LevelEntity t in GetTargets())
            {
                if(t.gameObject.activeInHierarchy)
                    LineDrawer.DrawLine(transform.position, t.transform.position, Color.blue * (selected ? 1f : 0.5f), 2f, true);
            }
        if(waitForClick)
        {
            LineDrawer.DrawLine(transform.position, 
                LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition), 
                Color.blue, 2f, true);
        }
    }

    public override List<LevelEntity> GetTargets()
    {
        if (triggerEvent == null)
            return new List<LevelEntity>();

        return triggerEvent.GetInvocationList().Select(x => ((MonoBehaviour)x.Target).GetComponent<LevelEntity>()).ToList();
    }

    #region Serialization
    
    /// <summary>
    /// Grabs all linked objects and stores their IDs.
    /// </summary>
    /// <param name="objectToID"></param>
    /// <returns></returns>
    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        var ids = new List<int>();
        foreach (LevelEntity e in GetTargets().Where(x => x.isActiveAndEnabled))
        {
            ids.Add(objectToID[e]);
        }

        return new SerializedTrigger(ids);
    }


    public class SerializedTrigger : Serialized
    {
        // Triggerable objects
        public int[] connectedObjects;

        public SerializedTrigger() { }

        public SerializedTrigger(List<int> connectedObjects)
        {
            this.connectedObjects = connectedObjects.ToArray();
        }

        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            var self = idList[parentID].gameObject.GetComponent<Trigger>();

            // Gets all LevelEntities fitting the connectedObjects ids
            var targets = idList.Where(x => connectedObjects.Contains(x.Key)).Select(x => x.Value).ToList();

            self.AddTargets(targets);
        }
    }

    #endregion
}
