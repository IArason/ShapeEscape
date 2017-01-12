using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

public class UndoTransformation : UndoAction
{
    Vector3[] startPoses;
    Vector3[] endPoses;
    Quaternion[] startRots;
    Quaternion[] endRots;
    Transform[] targets;

    // Array constructor
    public UndoTransformation(Transform[] targets,
        Vector3[] startPoses,
        Vector3[] endPoses,
        Quaternion[] startRots,
        Quaternion[] endRots)
    {
        this.targets = targets.Clone() as Transform[];
        this.startPoses = startPoses.Clone() as Vector3[];
        this.endPoses = endPoses.Clone() as Vector3[];
        this.startRots = startRots.Clone() as Quaternion[];
        this.endRots = endRots.Clone() as Quaternion[];
        
        Validate();
    }

    // List constructor
    public UndoTransformation(List<Transform> targets,
    List<Vector3> startPoses,
    List<Vector3> endPoses,
    List<Quaternion> startRots,
    List<Quaternion> endRots)
    {
        this.targets = targets.ToArray();
        this.startPoses = startPoses.ToArray();
        this.endPoses = endPoses.ToArray();
        this.startRots = startRots.ToArray();
        this.endRots = endRots.ToArray();
        
        Validate();
    }

    // Moveable list constructor
    public UndoTransformation(List<LevelEntity> targets,
    List<Vector3> startPoses,
    List<Vector3> endPoses,
    List<Quaternion> startRots,
    List<Quaternion> endRots)
    {
        this.targets = targets.Select(x => x.transform).ToArray();
        this.startPoses = startPoses.ToArray();
        this.endPoses = endPoses.ToArray();
        this.startRots = startRots.ToArray();
        this.endRots = endRots.ToArray();
        
        Validate();
    }

    // Single param constructor
    public UndoTransformation(Transform target,
    Vector3 startPos,
    Vector3 endPos,
    Quaternion startRot,
    Quaternion endRot)
    {
        targets = new Transform[] { target };
        startPoses = new Vector3[] { startPos };
        endPoses = new Vector3[] { endPos };
        startRots = new Quaternion[] { startRot };
        endRots = new Quaternion[] { endRot };

        Validate();
    }


    /// <summary>
    /// Does checks for validity, and if successful, adds self to queue.
    /// </summary>
    void Validate()
    {
        // check if redundant
        if ((startPoses.Equals(endPoses) && startRots.Equals(endRots))) return;

        var lengths = new int[] {targets.Length,
            startPoses.Length,
            endPoses.Length,
            startRots.Length,
            endRots.Length};

        if(lengths.Any(x => x != lengths[0]))
        {
            Debug.Log(targets.Length + " | " +
                startPoses.Length + " | " +
                endPoses.Length + " | " +
                startRots.Length + " | " +
                endRots.Length + " | "
                );
            Debug.LogError("Length mismatch in undo action");
            return;
        }

        // If not, add self to manager.
        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        for(int i = 0; i < targets.Length; i++)
        {
            targets[i].transform.rotation = endRots[i];
            targets[i].transform.position = endPoses[i];

            Debug.Log("Redid translation on " + targets[i].name);
        }
    }

    public override void Undo()
    {

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].transform.rotation = startRots[i];
            targets[i].transform.position = startPoses[i];

            Debug.Log("Undid translation on " + targets[i].name);
        }
    }
}
