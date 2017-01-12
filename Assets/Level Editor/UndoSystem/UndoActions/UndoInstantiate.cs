using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UndoInstantiate : UndoAction {

    GameObject[] instantiatedObjects;

    bool undone = false;

    public UndoInstantiate(List<GameObject> objects)
    {
        instantiatedObjects = objects.ToArray();
        UndoManager.Instance.AddAction(this);
    }

    public UndoInstantiate(GameObject obj)
    {
        instantiatedObjects = new GameObject[] { obj };
        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        foreach (GameObject g in instantiatedObjects)
        {
            g.SetActive(true);
        }
        undone = false;
    }

    public override void Undo()
    {
        foreach(GameObject g in instantiatedObjects)
        {
            g.SetActive(false);
        }
        undone = true;
    }

    public override void Cull()
    {
        // If it was undone when culled, remove the object.
        Debug.Log("Culled instantiate. Undone: " + undone.ToString());
        if (undone)
        { 
            foreach (GameObject g in instantiatedObjects)
            {
                Object.Destroy(g);
            }
            instantiatedObjects = new GameObject[0];
        }
    }
}
