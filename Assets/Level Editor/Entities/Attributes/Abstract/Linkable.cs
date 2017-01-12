using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Linkable : LevelEntityAttribute
{
    protected bool drawConnections { get { return hovering || selected; } }
    protected bool hovering = false;
    protected bool selected = false;
    protected bool waitForClick = false;
    protected bool unlinking = false;
    
    protected override void OnPointerEnter() { hovering = true; }
    protected override void OnPointerExit() { hovering = false; }
    protected override void OnSelect() { selected = true; }
    protected override void OnDeselect() { selected = false; }

    public virtual void LateUpdate()
    {
        CheckClick();
        if (drawConnections || waitForClick)
            DrawConnections();
    }

    protected virtual void CheckClick()
    {
        if (waitForClick && Input.GetMouseButtonDown(0))
        {
            var targets = Physics2D.OverlapPointAll(LevelEditorUtils.ScreenTo2DWorldPlane(
                Input.mousePosition)).Select(x => x.GetComponentInParent<LevelEntity>()).ToList();
            
            if (unlinking)
                RemoveFirstTarget(targets);
            else
                AddFirstTarget(targets);
            waitForClick = false;
        }
    }

    /// <summary>
    /// Gets the top valid entity from a list of entities.
    /// </summary>
    public abstract void AddFirstTarget(List<LevelEntity> entities, bool ignoreUndo = false);

    /// <summary>
    /// Same as AddTarget.
    /// </summary>
    public void AddTarget(LevelEntity target, bool ignoreUndo = false)
    {
        AddFirstTarget(new List<LevelEntity>() { target }, ignoreUndo);
    }

    /// <summary>
    /// For redo use only.
    /// </summary>
    public abstract void AddTargets(List<LevelEntity> entities);

    public abstract void RemoveTarget(LevelEntity entity, bool ignoreUndo = false);

    public abstract void RemoveFirstTarget(List<LevelEntity> entities, bool ignoreUndo = false);

    public abstract void RemoveTargets(List<LevelEntity> entity, bool ignoreUndo = false);

    public virtual void BeginLink() { waitForClick = true; unlinking = false; }
    public virtual void BeginUnlink() { waitForClick = true; unlinking = true; }

    public abstract void ClearTargets(bool ignoreUndo = false);

    protected abstract void DrawConnections();

    public abstract List<LevelEntity> GetTargets();
}
