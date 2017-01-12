using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

[RequireComponent(typeof(RotationArcVisuals))]
public class ObjectManipulator : Singleton<ObjectManipulator> {

    // Used by GetNextSelection.
    static Action<List<LevelEntity>> nextSelectCallback;
    static List<LevelEntity> highlightedSelectableEntities = new List<LevelEntity>();

    Tool selectedTool = Tool.Translate;

    bool canTranslate       { get { return !selectedEntities.Any(x => !x.canTranslate);      } }
    bool canRotate          { get { return !selectedEntities.Any(x => !x.canRotate);         } }
    bool mustSnapTranslate  { get { return  selectedEntities.Any(x =>  x.translateSnapOnly); } }
    bool mustSnapRotate     { get { return  selectedEntities.Any(x =>  x.rotateSnapOnly);    } }

    bool dragging = false;
    bool areaSelecting = false;

    bool wasOverUIElement = false;

    // Used to keep track of the not-snapped positions of objects during snap dragging.
    Vector2 dragPos;

    List<Quaternion> startDragRots = new List<Quaternion>();
    List<Vector3> startDragPoses = new List<Vector3>();

    // Entities currently selected.
    List<LevelEntity> selectedEntities = new List<LevelEntity>();

    List<LevelEntity> entitiesUnderMouse = new List<LevelEntity>();

    // Entity being hovered over.
    LevelEntity hoveredEntity;

    // For snapping
    public float gridUnit = 1f;
    public float rotationSnapDegs = 22.5f;
    
    // Click interaction variables
    Vector2 lastMousePos;
    Vector2 lastDownPos = new Vector2(-1000, -1000);
    Vector3 lastDownWorldPos;

    // Area select UI element
    public RectTransform areaSelectSprite;
    
    // Tool-specific update function
    Action ToolUpdate;

    // Draws a visual representation and highlights covered objects.
    Coroutine areaSelectRoutine;

    // Triggered when a vertex is selected.
    // Makes all objects except vertices unselectable until selection is cleared.
    bool inVertexMode = false;

    // Class used to display an arc during rotation.
    RotationArcVisuals arcVisuals;

    void Awake()
    {
        ToolUpdate = UpdateTranslateTool;
        arcVisuals = GetComponent<RotationArcVisuals>();
    }

    void LateUpdate()
    {
        // Makes ToolUpdate null so we don't run into any issues.
        if(nextSelectCallback != null && selectedTool != Tool.Selection)
        {
            DeselectAll(true);
            SetTool(Tool.Selection);
        }

        UpdateMouse();
        if (ToolUpdate != null)
            ToolUpdate();

        if (Input.GetButtonDown("SetToolTranslate"  )) SetTool(Tool.Translate);
        if (Input.GetButtonDown("SetToolRotate"     )) SetTool(Tool.Rotate);
        if (Input.GetButtonDown("SetToolSelect"     )) SetTool(Tool.Selection);

        if (Input.GetButtonDown("Duplicate"))
            DuplicateSelection();

        lastMousePos = Input.mousePosition;
    }

    public static void GetNextSelection(Action<List<LevelEntity>> callback, Type highlightType = null)
    {
        nextSelectCallback = callback;
        if(highlightType != null)
        {
            // Select all LevelEntities with the correct highlight type
            highlightedSelectableEntities = FindObjectsOfType(highlightType).Select(
                x => ((MonoBehaviour)x).GetComponent<LevelEntity>()).Where(x => x != null).ToList();

            
            foreach(LevelEntity e in highlightedSelectableEntities)
            {
                e.HighlightPersistent(true);
            }
            print("Permanent highlighting on objects enabled.");
        }
    }

    void UpdateMouse()
    { 
        // Prevents clicking through UI elements
        if (LevelEditorUtils.MouseOverUIElement())
        {
            if (Input.GetMouseButtonDown(0) && LevelEditorUtils.MouseOverUIElement())
                wasOverUIElement = true;

            if (hoveredEntity != null)
            {
                hoveredEntity.OnPointerExit();
                hoveredEntity = null;
            }
            return;
        }
        if (wasOverUIElement)
        {
            if (Input.GetMouseButtonUp(0))
                wasOverUIElement = false;

            if (hoveredEntity != null)
            {
                hoveredEntity.OnPointerExit();
                hoveredEntity = null;
            }
            return;
        }

        if (inVertexMode)
        {
            entitiesUnderMouse = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(
                (Input.GetMouseButton(0) ? lastDownPos : (Vector2)Input.mousePosition)
                )).
                Select(x => (LevelEntity)x.GetComponentInParent<Vertex>()).Where(x => x != null).ToList();
        }
        else
        {
            entitiesUnderMouse = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(
                (Input.GetMouseButton(0) ? lastDownPos : (Vector2)Input.mousePosition)
                )).
                Select(x => x.GetComponentInParent<LevelEntity>()).Where(x => x != null).ToList();
        }

        // Hovering outline
        // Ignore if nothing to outline, selected object is under mouse, or dragging
        if (entitiesUnderMouse.Count == 0 || 
            entitiesUnderMouse.Intersect(selectedEntities).Any() ||
            dragging || areaSelecting)
        {
            if (hoveredEntity != null)
            {
                hoveredEntity.OnPointerExit();
                hoveredEntity = null;
            }
        }
        else if (hoveredEntity != entitiesUnderMouse[0])
        {
            if (hoveredEntity != null)
                hoveredEntity.OnPointerExit();
            hoveredEntity = entitiesUnderMouse[0];
            hoveredEntity.OnPointerEnter();
        }

        
        // TODO: This is probably never used
        // Return early if something is waiting on selection callback.
        if(nextSelectCallback != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                nextSelectCallback(entitiesUnderMouse);
                nextSelectCallback = null;

                foreach (LevelEntity e in highlightedSelectableEntities)
                {
                    e.HighlightPersistent(false);
                }

                print("Permanent highlighting on objects disabled.");
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            lastDownPos = Input.mousePosition;
            lastDownWorldPos = LevelEditorUtils.ScreenTo2DWorldPlane(lastDownPos);
        }

        if(Input.GetMouseButton(0) && !(areaSelecting || dragging))
        {
            if (Vector2.Distance(lastDownPos, Input.mousePosition) > 3)
            {
                if (entitiesUnderMouse.Count == 0 || Input.GetButton("AdditiveSelect"))
                {
                    StartAreaSelection(lastDownWorldPos);
                }
                // If button's down and has been dragged a nonzero distance, begin drag
                else
                {
                    var entitiesAtOriginalPoint = Physics2D.OverlapPointAll(LevelEditorUtils.ScreenTo2DWorldPlane(lastDownPos)).
                        Select(x => x.GetComponentInParent<LevelEntity>()).Where(x => x != null).ToList();
                    
                    // If the object isn't already selected, select it. 
                    if (!selectedEntities.Intersect(entitiesAtOriginalPoint).Any())
                    {
                        SetSelection(entitiesAtOriginalPoint[0]);
                    }

                    ResetUpDir(Vector2.up);
                    StartDrag();
                }
            }
        }

        if(!Input.GetMouseButton(0) && (areaSelecting || dragging))
        {
            StopDrag();
            StopAreaSelection();
        }

        if(Input.GetMouseButtonUp(0))
        {
            // If clicking in approx the same place
            if(Vector2.Distance(lastDownPos, Input.mousePosition) < 3)
            {
                // Deselect all if clicking empty space
                if (entitiesUnderMouse.Count != 0)
                {
                    // If shift, flip select
                    if (Input.GetButton("AdditiveSelect"))
                    {
                        if (selectedEntities.Contains(entitiesUnderMouse[0]))
                        {
                            Deselect(entitiesUnderMouse[0]);
                        }
                        else
                        {
                            AddSelect(entitiesUnderMouse[0]);
                        }
                    }
                    else
                    {
                        SetSelection(entitiesUnderMouse[0]);
                    }
                }
                else
                {
                    DeselectAll();
                }
            }
            else
            {
                StopDrag();
                StopAreaSelection();
            }
        }
    }

    #region Selection

    #region Public
    
    /// <summary>
    /// Resets selection.
    /// </summary>
    public void PrepareForUndo()
    {
        StopDrag();
    }

    /// <summary>
    /// Sets the selection.
    /// </summary>
    public void SetSelection(LevelEntity selection, bool ignoreUndo = false)
    {
        var oldSelection = selectedEntities.ToArray();

        DeselectAll(true);
        AddSelect(selection, true);
        
        if (!ignoreUndo)
        {
            new UndoSelection(this, oldSelection, selectedEntities);
        }
    }

    /// <summary>
    /// Sets the selection.
    /// </summary>
    public void SetSelection(List<LevelEntity> selection, bool ignoreUndo = false)
    {
        var oldSelection = selectedEntities.ToArray();
        DeselectAll(true);
        foreach (LevelEntity m in selection)
        {
            AddSelect(m, true);
        }
        if (!ignoreUndo)
        {
            new UndoSelection(this, oldSelection, selectedEntities);
        }
    }

    public void ClearSelection()
    {
        DeselectAll(true);
    }
    
    /// <summary>
    /// Returns a list of all selected entities.
    /// </summary>
    public List<LevelEntity> GetSelection()
    {
        return selectedEntities;
    }

    #endregion

    #region Private

    void AddSelect(LevelEntity v, bool ignoreUndo = false)
    {
        if (selectedEntities.Contains(v) || v == null) return;

        // Enable vertex mode when a vertex is selected.
        if (!inVertexMode && v.GetType() == typeof(Vertex))
        {
            StartVertexMode(new List<LevelEntity>() { v }, true);
            return;
        }

        var oldSelection = selectedEntities.ToArray();
        
        if (v == null) return;
        
        v.OnSelect();
        selectedEntities.Add(v);

        if (!ignoreUndo)
        {
            new UndoSelection(this, oldSelection, selectedEntities);
        }

        UpdateCenter(false);

        RefreshTool();
    }

    void AddSelect(List<LevelEntity> v, bool ignoreUndo = false)
    {
        if (v == null || v.Count == 0) return;

        var oldSelection = selectedEntities.ToArray();

        foreach (LevelEntity e in v)
        {
            AddSelect(e, true);
        }

        if(!ignoreUndo)
        { 
            new UndoSelection(this, oldSelection, selectedEntities);
        }
    }

    void Deselect(LevelEntity v, bool ignoreUndo = false)
    {
        if (!selectedEntities.Contains(v)) return;

        var oldSelection = selectedEntities.ToArray();

        v.transform.SetParent(null, true);

        v.OnDeselect();

        selectedEntities.Remove(v);

        if (!ignoreUndo)
        {
            new UndoSelection(this, oldSelection, selectedEntities);
        }

        if (inVertexMode && selectedEntities.Count == 0)
        {
            StopVertexMode();
        }

        UpdateCenter(false);
    }
    
    void DeselectAll(bool ignoreUndo = false)
    {
        var oldSelection = selectedEntities.ToArray();
        for (; 0 < selectedEntities.Count;)
        {
            Deselect(selectedEntities[0], true);
        }
        if (!ignoreUndo)
        {
            new UndoSelection(this, oldSelection, selectedEntities);
        }
    }

    void StartDrag()
    {
        dragging = true;
        
        // Used to undo
        startDragPoses.Clear();
        startDragRots.Clear();

        for (int i = 0; i < selectedEntities.Count; i++)
        {
            startDragPoses.Add(selectedEntities[i].transform.position);
            startDragRots.Add(selectedEntities[i].transform.rotation);
        }

        if (selectedTool == Tool.Rotate)
        {
            var target = LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition) - GetTargetCenter();

            if (mustSnapRotate || !Input.GetButton("DisableSnap"))
            {
                target = LevelEditorUtils.GetClosestSnappedDir(rotationSnapDegs, target) * target.magnitude;
            }

            arcVisuals.EnableArc(target);
        }
        
        dragPos = GetTargetCenter();
    }

    void StopDrag()
    {
        if (!dragging) return;

        dragging = false;
        List<Vector3> endDragPoses = new List<Vector3>();
        List<Quaternion> endDragRots = new List<Quaternion>();
        for (int i = 0; i < selectedEntities.Count; i++)
        {
            endDragPoses.Add(selectedEntities[i].transform.position);
            endDragRots.Add(selectedEntities[i].transform.rotation);
        }

        arcVisuals.DisableArc();

        // New collections needed as lists are pasesd by reference
        new UndoTransformation(selectedEntities,
            startDragPoses,
            endDragPoses,
            startDragRots,
            endDragRots);

        dragPos = Vector3.one * -1000;
    }


    // Updates the tool's position relative to the selected cluster
    void UpdateCenter(bool snapTargets)
    {
        if (selectedEntities.Count == 0) return;

        if (selectedTool == Tool.Translate)
        {
            if ((mustSnapTranslate || !Input.GetButton("DisableSnap")) && snapTargets)
            {
                SetCenter(LevelEditorUtils.GetClosestSnappedToGrid(gridUnit, GetTargetCenter()));
            }
            else
            {
                SetCenter(GetTargetCenter());
            }
        }

        if (selectedTool == Tool.Rotate)
        {
            // Find the center of the objects and
            SetCenter(GetTargetCenter());

            // use the mouse's relative direction as a baseline
            ResetUpDir(LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition) - (Vector2)transform.position);
        }
    }

    /// <summary>
    /// Initializes vertex mode.
    /// </summary>
    /// <param name="vertices">The vertex selected to enable this mode.</param>
    void StartVertexMode(List<LevelEntity> vertices, bool ignoreUndo = false)
    {
        inVertexMode = true;

        var oldSelection = selectedEntities.ToArray();

        DeselectAll(ignoreUndo);

        AddSelect(vertices, ignoreUndo);
    }

    void StopVertexMode()
    {
        inVertexMode = false;
    }

    // Sets the center of the tool
    void SetCenter(Vector3 center)
    {
        foreach (LevelEntity m in selectedEntities)
        {
            m.transform.SetParent(null, true);
        }
        transform.position = center;
        foreach (LevelEntity m in selectedEntities)
        {
            m.transform.SetParent(transform, true);
        }
    }

    /// <summary>
    /// For use with a single selected object only.
    /// Centers tool on object and snaps to grid.
    /// </summary>
    void SnapToGrid()
    {
        if (selectedEntities.Count != 1) return;

        selectedEntities[0].transform.SetParent(null, true);
        transform.position = LevelEditorUtils.GetClosestSnappedToGrid(gridUnit, transform.position);
        selectedEntities[0].transform.SetParent(transform, true);
        selectedEntities[0].transform.localPosition = new Vector3(0, 0, selectedEntities[0].transform.localPosition.z);
    }
    
    void ResetUpDir(Vector2 upDir)
    {
        foreach (LevelEntity m in selectedEntities)
        {
            m.transform.SetParent(null, true);
        }

        transform.rotation = Quaternion.LookRotation(Vector3.forward, upDir);

        foreach (LevelEntity m in selectedEntities)
        {
            m.transform.SetParent(transform, true);
        }
    }

    void StartAreaSelection(Vector3 start)
    {
        areaSelecting = true;
        areaSelectRoutine = StartCoroutine(RunAreaSelection(start));
        areaSelectSprite.gameObject.SetActive(true);
    }

    void StopAreaSelection()
    {
        areaSelecting = false;
        areaSelectSprite.gameObject.SetActive(false);
    }

    IEnumerator RunAreaSelection(Vector3 start)
    {
        LevelEntity[] allEntities;
        if(inVertexMode)
        {
            allEntities = FindObjectsOfType<Vertex>();
        }
        else
        {
            allEntities = FindObjectsOfType<LevelEntity>();
        }
        List<LevelEntity> inArea = new List<LevelEntity>();
        while(areaSelecting)
        {
            var end = LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition);
            inArea = GetEntitiesInArea(start, end, allEntities);
            foreach (LevelEntity e in allEntities)
            {
                if(inArea.Contains(e))
                {
                    // Prevents vertex highlighting out of vertex mode.
                    if (inVertexMode ^ e.GetType() == typeof(Vertex)) continue;
                        e.OnPointerEnter();
                }
                else
                {
                    e.OnPointerExit();
                }
            }

            // Draw selection
            var worldStart = Camera.main.WorldToScreenPoint(start);
            var worldEnd = Camera.main.WorldToScreenPoint(end);

            Vector3 max, min;

            max.x = Mathf.Max(worldStart.x, worldEnd.x);
            max.y = Mathf.Max(worldStart.y, worldEnd.y);
            min.x = Mathf.Min(worldStart.x, worldEnd.x);
            min.y = Mathf.Min(worldStart.y, worldEnd.y);

            areaSelectSprite.anchoredPosition = new Vector2(
                min.x + (max.x - min.x) / 2,
                min.y + (max.y - min.y) / 2);
            areaSelectSprite.sizeDelta = new Vector2(max.x - min.x, max.y - min.y);

            yield return null;
        }

        // Check if everything selected is a vertex. 
        // If it isn't, clear out vertices.
        // If it is, enable vertex mode.
        if (!inVertexMode && inArea.Count != 0)
        {
            if (inArea.Any(x => x.GetType() != typeof(Vertex)))
            {
                inArea = inArea.Where(x => x.GetType() != typeof(Vertex)).ToList();
            }
            else
            {
                StartVertexMode(inArea, true);
            }
        }

        var oldSelection = selectedEntities.ToArray();
        // Set selection to objects in square.
        if(Input.GetButton("AdditiveSelect"))
        {
            AddSelect(inArea, true);
        }
        else
        {
            SetSelection(inArea, true);
        }
        new UndoSelection(this, oldSelection, selectedEntities);
    }

    List<LevelEntity> GetEntitiesInArea(Vector3 start, Vector3 end, LevelEntity[] entities)
    {
        Vector2 max, min;
        max.x = Mathf.Max(start.x, end.x);
        max.y = Mathf.Max(start.y, end.y);
        min.x = Mathf.Min(start.x, end.x);
        min.y = Mathf.Min(start.y, end.y);

        List<LevelEntity> results = new List<LevelEntity>();
        foreach(LevelEntity e in entities)
        {
            var c = e.transform.position;
            if (c.x < max.x && c.y < max.y && c.x > min.x && c.y > min.y)
            {
                results.Add(e);
            }
        }

        return results;
    }

    #endregion

    #endregion

    #region Tool Update Functions

    void UpdateTranslateTool()
    {

        // If we let go of free move before mouse button is released, we snap.
        if (Input.GetButtonUp("DisableSnap") && Input.GetMouseButton(0))
        {
            if (selectedEntities.Count == 1)
                SnapToGrid();
            dragPos = GetTargetCenter();
        }

        if (dragging)
        {
            TranslateWithMouse(lastMousePos,
                Input.mousePosition,
                !Input.GetButton("DisableSnap") || mustSnapTranslate);

            if (selectedEntities.Count == 1 && !Input.GetButton("DisableSnap")) SnapToGrid();

        }
    }
    
    void UpdateRotationTool()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateCenter(true);
        }

        // Set self to a snapped up dir so snaps won't be off center
        if (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && Input.GetButtonUp("DisableSnap")))
        {
            if (selectedEntities.Count == 1 && entitiesUnderMouse.Contains(selectedEntities[0]))
            {
                var oldRot = selectedEntities[0].transform.rotation;
                ResetUpDir(LevelEditorUtils.GetClosestSnappedDir(rotationSnapDegs, transform.up));
                selectedEntities[0].transform.rotation = Quaternion.LookRotation(Vector3.forward,
                    LevelEditorUtils.GetClosestSnappedDir(rotationSnapDegs, selectedEntities[0].transform.up));

                if (oldRot != selectedEntities[0].transform.rotation)
                {
                    new UndoTransformation(
                        selectedEntities[0].transform,
                        selectedEntities[0].transform.position,
                        selectedEntities[0].transform.position,
                        oldRot,
                        selectedEntities[0].transform.rotation);
                }
            }
        }

        if (dragging)
        {
            RotateWithMouse(Input.mousePosition, !Input.GetButton("DisableSnap"));

            var target = LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition) - GetTargetCenter();
            if(mustSnapRotate || !Input.GetButton("DisableSnap"))
            {
                target = LevelEditorUtils.GetClosestSnappedDir(rotationSnapDegs, target) * target.magnitude;
            }

            arcVisuals.SetTargetDir(target);
        }
    }

    #endregion

    #region Translation

    void TranslateWithMouse(Vector2 start, Vector2 end, bool snap)
    {
        var oldPos = LevelEditorUtils.ScreenTo2DWorldPlane(start);
        var newPos = LevelEditorUtils.ScreenTo2DWorldPlane(end);


        if (newPos != Vector2.zero && oldPos != Vector2.zero)
        {
            // If snap, use mouse position instead of motion delta.
            var delta = newPos - oldPos;

            if (snap)
            {
                dragPos += delta;
                MoveTo(dragPos);
            }
            else
            {
                Translate(delta);
            }
        }
    }

    void Translate(Vector2 delta)
    {
        var oldPos = transform.position;
        transform.position = (Vector2)transform.position + delta;
    }

    void MoveTo(Vector2 pos)
    {
        transform.position = LevelEditorUtils.GetClosestSnappedToGrid(gridUnit, pos);
    }

    #endregion

    #region Rotation

    void RotateWithMouse(Vector3 mousePos, bool snap)
    {
        SetUpDir(LevelEditorUtils.ScreenTo2DWorldPlane(mousePos) - (Vector2)transform.position, snap);
    }

    void SetUpDir(Vector2 up, bool snap)
    {
        var finalRot = up;
        if (snap) finalRot = LevelEditorUtils.GetClosestSnappedDir(rotationSnapDegs, finalRot);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, finalRot);
    }

    #endregion

    #region Instantiation

    /// <summary>
    /// Spawns a prefab snapped at center screen
    /// </summary>


    public void DuplicateSelection()
    {
        // Validate selection
        if (selectedEntities.Any(x => !x.canDuplicate))
        {
            DeselectAll();
            return;
        }

        List<GameObject> oldSelection = selectedEntities.Select(x => x.gameObject).ToList();
        List<GameObject> newSelection = new List<GameObject>();

        DeselectAll();

        foreach(GameObject g in oldSelection)
        {
            var go = Instantiate(g);
            newSelection.Add(go);

            // Add small offset from original object
            go.transform.position = g.transform.position + (Vector3.up + Vector3.right) * 2;
        }

        new UndoInstantiate(newSelection);

        SetSelection(newSelection.Select(x => x.GetComponent<LevelEntity>()).ToList(), true);
    }

    #endregion

    // Returns the average center of the cluster
    Vector2 GetTargetCenter()
    {
        if (selectedEntities.Count == 0) return Vector3.zero;

        Vector2 totalPos = Vector2.zero;

        foreach (LevelEntity m in selectedEntities)
        {
            totalPos += (Vector2)m.transform.position;
        }

        var finalPos = totalPos / selectedEntities.Count;

        if (mustSnapTranslate || !Input.GetButton("DisableSnap"))
            finalPos = LevelEditorUtils.GetClosestSnappedToGrid(gridUnit, finalPos);

        return finalPos;
    }

    void SetTool(Tool tool)
    {
        Debug.Log("Set tool to " + tool.ToString());
        selectedTool = tool;
        switch(tool)
        {
            case Tool.Rotate:
                ToolUpdate = UpdateRotationTool;
                break;
            case Tool.Translate:
                ToolUpdate = UpdateTranslateTool;
                break;
            case Tool.Selection:
                ToolUpdate = null;
                return;
        }
        RefreshTool();
    }

    void DestroySelected()
    {
        // Vertices may not be destroyed.
        if (inVertexMode)
        {
            DeselectAll();
            return;
        }

        new UndoDestroy(selectedEntities.Select(x => x.gameObject).ToList());
        foreach (LevelEntity m in selectedEntities)
        {
            m.gameObject.SetActive(false);
        }
        DeselectAll(true);
    }

    public void DestroyObject(LevelEntity entity, bool ignoreUndo = false)
    {
        if (!ignoreUndo)
            new UndoDestroy(new List<GameObject>() { entity.gameObject });
        entity.gameObject.SetActive(false);
        Deselect(entity, true);
    }

    public void DestroyObjects(List<LevelEntity> entities, bool ignoreUndo = false)
    {
        if(!ignoreUndo)
            new UndoDestroy(entities.Select(x => x.gameObject).ToList());
        foreach(var e in entities)
        {
            DestroyObject(e, true);
        }
    }

    /// <summary>
    /// Checks if mouse is currently above a selected element.
    /// </summary>
    public bool IsOverSelectionGroup()
    {
        return selectedEntities.Intersect(entitiesUnderMouse).Any();
    }

    /// <summary>
    /// Verifies that a tool is valid for use considering selection.
    /// </summary>
    void RefreshTool()
    {
        if (!canRotate && selectedTool == Tool.Rotate)
            SetTool(Tool.Translate);
        else if (!canTranslate && selectedTool == Tool.Translate)
            SetTool(Tool.Selection);
    }

    enum Tool
    {
        Selection,
        Rotate,
        Translate
    }
}
