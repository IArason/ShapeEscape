using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UndoDestroy : UndoAction {

    GameObject[] destroyedObjects;
    bool undone = false;

    public UndoDestroy(List<GameObject> objects)
    {
        destroyedObjects = objects.ToArray();
        UndoManager.Instance.AddAction(this);
    }

    public UndoDestroy(GameObject obj)
    {
        destroyedObjects = new GameObject[] { obj };
        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        foreach (GameObject g in destroyedObjects)
        {
            g.SetActive(false);
        }
        undone = false;
    }

    public override void Undo()
    {
        foreach (GameObject g in destroyedObjects)
        {
            g.SetActive(true);
        }
        ObjectManipulator.Instance.SetSelection(destroyedObjects.Select(x => x.GetComponent<LevelEntity>()).ToList(), true);
        Debug.Log("Undid instantiation");
        undone = true;
    }

    public override void Cull()
    {
        if (!undone)
        {
            for (int i = 0; i < destroyedObjects.Length; i++)
            {
                Object.Destroy(destroyedObjects[i]);
            }
            destroyedObjects = new GameObject[0];
            Debug.Log("Cleared destroyed objects");
        }
    }
}
