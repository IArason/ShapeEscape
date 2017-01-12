using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Keeps track of undoable actions.
/// </summary>
public class UndoManager : Singleton<UndoManager> {

    [SerializeField]
    int maxHistoryLength = 50;

    // 0 is oldest, last is newest.
    public List<UndoAction> history = new List<UndoAction>();
    public int currentIndex = -1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Redo();
        }
    }
    
    public void AddAction(UndoAction action)
    {
        // If not at the end, remove everything from
        // currentindex to end.
        if(currentIndex != history.Count - 1)
        {
            Cull(currentIndex + 1, history.Count - currentIndex - 1);
            history.RemoveRange(currentIndex + 1, history.Count - currentIndex  - 1);
        }

        history.Add(action);
        currentIndex = history.Count - 1;
        Trim();
    }

    public void Clear()
    {
        Cull(0, history.Count);
        currentIndex = -1;
        history = new List<UndoAction>();
    }

    public void Undo()
    {
        if (currentIndex < 0) return;

        history[currentIndex].Undo();
        currentIndex--;
    }

    public void Redo()
    {
        if (currentIndex > history.Count - 2) return;

        currentIndex++;
        history[currentIndex].Redo();
    }

    // Should be called every time the history is modified.
    void Trim()
    {
        if(history.Count > maxHistoryLength)
        {
            Cull(0, 1);
            history.RemoveAt(0);
            currentIndex--;
        }
    }
    
    void Cull(int start, int count)
    {
        for(int i = start; i < start + count; i++)
        {
            history[i].Cull();
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(UndoManager))]
public class UndoManagerEditor : Editor
{
    UndoManager manager;
    bool toggle = false;

    void OnEnable()
    {
        manager = (UndoManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        toggle = EditorGUILayout.Foldout(toggle, "History");
        if (toggle)
        {
            for (int i = 0; i < manager.history.Count; i++)
            {
                var label = i + ": " + manager.history[i].GetType().ToString();
                if (i == manager.currentIndex)
                    label += "  ◀";
                EditorGUILayout.LabelField(label);
            }
        }
    }
}
#endif