using System.Collections.Generic;
using UnityEngine;

public class UndoInstantiateNode : UndoAction
{
    List<NodeParentIndexGroup> instantiatedNodes = new List<NodeParentIndexGroup>();

    bool undone = false;

    public UndoInstantiateNode(List<GameObject> objects)
    {
        if (objects.Count == 0) return;

        foreach(var obj in objects)
        {
            var group = new NodeParentIndexGroup();
            group.node = obj.transform;
            group.parent = obj.GetComponent<MoverHandle>().ParentMover.GetComponent<Mover>();
            group.index = group.parent.IndexOf(group.node);
            instantiatedNodes.Add(group);
        }
        UndoManager.Instance.AddAction(this);
    }

    public UndoInstantiateNode(GameObject obj)
    {
        if (obj == null) return;

        var group = new NodeParentIndexGroup();
        group.node = obj.transform;
        group.parent = obj.GetComponent<MoverHandle>().ParentMover.GetComponent<Mover>();
        group.index = group.parent.IndexOf(group.node);
        instantiatedNodes.Add(group);

        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        for (int i = instantiatedNodes.Count - 1; i >= 0; i--)
        {
            instantiatedNodes[i].node.gameObject.SetActive(true);
            instantiatedNodes[i].parent.ReAddNode(instantiatedNodes[i].node, instantiatedNodes[i].index);
        }
        undone = false;
        Debug.Log("Redid duplication of node");
    }

    public override void Undo()
    {
        Debug.Log(instantiatedNodes.Count);
        for (int i = instantiatedNodes.Count - 1; i >= 0; i--)
        {
            Debug.Log(i);
            Debug.Log(instantiatedNodes[i]);
            instantiatedNodes[i].node.gameObject.SetActive(false);
            instantiatedNodes[i].index = instantiatedNodes[i].parent.RemoveNode(instantiatedNodes[i].node);
        }
        undone = true;
        Debug.Log("Undid duplication of node");
    }

    public override void Cull()
    {
        // If it was undone when culled, remove the object.
        Debug.Log("Culled instantiate. Undone: " + undone.ToString());
        if (undone)
        {
            foreach (var g in instantiatedNodes)
            {
                Object.Destroy(g.node.gameObject);
            }
            instantiatedNodes.Clear();
        }
    }

    class NodeParentIndexGroup
    {
        public Transform node;
        public Mover parent;
        public int index;
    }
}
