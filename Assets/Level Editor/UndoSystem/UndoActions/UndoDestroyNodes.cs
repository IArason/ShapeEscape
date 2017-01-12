
using System.Collections.Generic;
using UnityEngine;

public class UndoDestroyNodes : UndoAction
{
    List<NodeParentIndexGroup> destroyedNodes = new List<NodeParentIndexGroup>();
    bool undone = false;

    public UndoDestroyNodes(List<GameObject> objects)
    {
        foreach(var obj in objects)
        {
            var group = new NodeParentIndexGroup();
            group.node = obj.transform;
            group.parent = obj.GetComponent<MoverHandle>().ParentMover.GetComponent<Mover>();
            group.index = group.parent.RemoveNode(obj.transform);
            destroyedNodes.Add(group);
        }
        UndoManager.Instance.AddAction(this);
    }

    public UndoDestroyNodes(GameObject obj)
    {
        var group = new NodeParentIndexGroup();
        group.node = obj.transform;
        group.parent = obj.GetComponent<MoverHandle>().ParentMover.GetComponent<Mover>();
        group.index = group.parent.RemoveNode(obj.transform);
        destroyedNodes.Add(group);

        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        for (int i = 0; i < destroyedNodes.Count; i++)
        {
            destroyedNodes[i].index = destroyedNodes[i].parent.RemoveNode(destroyedNodes[i].node);
            destroyedNodes[i].node.gameObject.SetActive(false);
        }
        undone = false;
    }

    public override void Undo()
    {
        // Populate it backwards to retain proper indices
        for (int i = destroyedNodes.Count - 1; i >= 0; i--)
        {
            destroyedNodes[i].parent.ReAddNode(destroyedNodes[i].node, destroyedNodes[i].index);
            destroyedNodes[i].node.gameObject.SetActive(true);
        }
        undone = true;
    }

    public override void Cull()
    {
        if (!undone)
        {
            for (int i = 0; i < destroyedNodes.Count; i++)
            {
                Object.Destroy(destroyedNodes[i].node);
            }
            destroyedNodes.Clear();
            Debug.Log("Cleared destroyed nodes");
        }
    }

    class NodeParentIndexGroup
    {
        public Transform node;
        public Mover parent;
        public int index;
    }

}
