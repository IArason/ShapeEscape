using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Triggerable), typeof(Rigidbody2D))]
public class Mover : LevelEntityAttribute
{
    [SerializeField]
    List<Transform> moveTargets;

    int nodeIndex;
    [SerializeField]
    PosRotPair[] nodes;

    Coroutine motionRoutine;
    Coroutine lineVisualsroutine;
    GameObject previewVisuals;

    Rigidbody2D rb;

    bool returning = false;

    [SerializeField]
    GameObject deleteNodeMenuOptionPrefab;
    
    /// Menu-modifiable variables
    [SerializeField] // Velocity in units per second
    public float speed = 1f; 

    [SerializeField] // Can swap directions halfway
    public bool interruptable = false; 

    [SerializeField] // Returns to origin after reaching destination
    public bool autoReturn = false;

    [SerializeField]
    public bool alwaysOn = false;

    [SerializeField] // How long until it can be triggered again
    public float activationDelay = 0f;

    [SerializeField] // How long does it stop at each node
    public float stopTime = 0.5f;
    

    void Start()
    {
        UpdateNodes();
        rb = GetComponent<Rigidbody2D>();
    }

    void UpdateNodes()
    {
        if (moveTargets == null) return;

        // Populate node list
        nodes = new PosRotPair[moveTargets.Count + 1];
        nodes[0] = new PosRotPair(transform.position.XY(), transform.rotation);
        for (int i = 0; i < moveTargets.Count; i++)
        {
            nodes[i + 1] = new PosRotPair(moveTargets[i].position.XY(), moveTargets[i].rotation);
        }
    }

    void FixedUpdate()
    {
        if (alwaysOn && motionRoutine == null && moveTargets == null)
            OnTrigger();

    }

    public Transform AddNode(Transform clone)
    {
        Debug.Log("Adding node");
        var instance = Instantiate(clone.gameObject);
        // Only duplicated nodes may be deleted
        instance.GetComponent<ContextMenuTarget>().AddOption(deleteNodeMenuOptionPrefab);
        instance.transform.SetParent(transform, false);
        instance.transform.position = clone.transform.position + Vector3.one.XY();
        moveTargets.Insert(moveTargets.IndexOf(clone) + 1, instance.transform);
        Debug.Log("Added node");
        return instance.transform;
    }

    // For undo system only
    public void ReAddNode(Transform node, int index)
    {
        moveTargets.Insert(index, node);
    }

    public int IndexOf(Transform node)
    {
        return moveTargets.IndexOf(node);
    }

    public int RemoveNode(Transform node)
    {
        var index = moveTargets.IndexOf(node);
        moveTargets.Remove(node);
        return index;
    }

    protected override void OnSelect()
    {
        base.OnSelect();

        UpdateNodes();

        // If not all nodes are identical
        if (nodes.Length > 1 && !nodes.All(x => x.Equals(nodes[0])))
        {
            previewVisuals = Instantiate(moveTargets[0].gameObject);
            Destroy(previewVisuals.GetComponentInChildren<SpriteRenderer>().gameObject);
            previewVisuals.transform.position = nodes[0].pos + Vector3.back * 3;
            previewVisuals.transform.rotation = nodes[0].rot;

            var visualRB = previewVisuals.AddComponent<Rigidbody2D>();
            visualRB.isKinematic = true;
            nodeIndex = 0;

            StartCoroutine(MoveRoutine(visualRB));
        }
    }
    protected override void OnDeselect()
    {
        StopAllCoroutines();
        returning = false;
        Destroy(previewVisuals);
        base.OnDeselect();
    }

    protected override void OnPointerEnter()
    {
        base.OnPointerEnter();
        lineVisualsroutine = StartCoroutine(ShowLines());
    }

    protected override void OnPointerExit()
    {
        base.OnPointerExit();
        if(lineVisualsroutine != null)
            StopCoroutine(lineVisualsroutine);
    }

    IEnumerator ShowLines()
    {
        Start();
        while(true)
        {
            for(int i = 0; i < nodes.Length - 1; i++)
            {
                LineDrawer.DrawLine(nodes[i].pos, nodes[i + 1].pos, Color.green * 0.7f, 2f, true);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void OnTrigger()
    {
        Debug.Log("Was triggered");

        if (motionRoutine == null || interruptable)
        {
            if(motionRoutine != null)
                returning = !returning;

            motionRoutine = StartCoroutine(MoveRoutine(rb));
        }
    }

    // Departing true -> going to go to destination
    IEnumerator MoveRoutine(Rigidbody2D rb)
    {
        Debug.Log("Starting move routine");
        PosRotPair destination;
        PosRotPair origin;

        // Always actives
        while (true)
        {
            if (returning)
            {
                nodeIndex--;

                if (nodeIndex <= 0)
                {
                    returning = false;
                    break;
                }

                origin = nodes[nodeIndex];
                destination = nodes[nodeIndex - 1];
            }
            else
            {
                nodeIndex++;
                if (nodeIndex == nodes.Length)
                {
                    Debug.Log("Node index equal to length");
                    returning = true;
                    if (autoReturn)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }

                }
                origin = nodes[nodeIndex - 1];
                destination = nodes[nodeIndex];
            }
            
            float timeToNode = Vector3.Distance(rb.position, destination.pos.XY()) / speed;
            float timer = 0;

            var oldPos = transform.position;
            while (timer < timeToNode)
            {
                // If parent is moved, stop preview if in edit mode.
                if (this.rb != rb && transform.position != oldPos)
                {
                    Debug.Log("Stopping preview");
                    returning = false;
                    Destroy(previewVisuals);
                    yield break;
                }

                float lerpVal = timer / timeToNode;
                if (lerpVal <= 0.5)
                {
                    lerpVal = 2 * lerpVal * lerpVal;
                }
                else {
                    lerpVal = -1 + (4 - 2 * lerpVal) * lerpVal;
                }
                
                rb.MovePosition(Vector2.Lerp(origin.pos, destination.pos, lerpVal));
                rb.MoveRotation(Quaternion.Slerp(origin.rot, destination.rot, lerpVal).eulerAngles.z);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            rb.MovePosition(destination.pos);
            rb.MoveRotation(destination.rot.eulerAngles.z);
            yield return new WaitForSeconds(stopTime);

            // If parent is moved, stop preview if in edit mode.
            if (this.rb != rb && transform.position != oldPos)
            {
                Debug.Log("Stopping preview");
                returning = false;
                Destroy(previewVisuals);
                yield break;
            }
        }
        motionRoutine = null;
    }

    #region Serialization

    private void Initialize(PosRotPair[] nodes, 
        float speed, 
        float activationDelay,
        float stopTime,
        bool alwaysOn, 
        bool interruptable, 
        bool autoReturn, 
        bool editable
        )
    {
        this.nodes = nodes;

        this.speed = speed;
        this.activationDelay = activationDelay;
        this.stopTime = stopTime;
        this.alwaysOn = alwaysOn;
        this.interruptable = interruptable;
        this.autoReturn = autoReturn;
        
        if (editable)
        {
            // Create clones to use as the new node links
            // There's already one existing moveTarget at [0] so we reuse that
            var original = moveTargets[0];
            moveTargets.Clear();
            moveTargets.Add(original);
            moveTargets[0].position = nodes[1].pos; // Nodes[0] is the parent, [1] is the first move target
            moveTargets[0].rotation = nodes[1].rot;
            Debug.Log(nodes.Length + " | " + moveTargets.Count);
            // Populate the rest
            for (int i = 1; i < moveTargets.Count; i++)
            {
                moveTargets[i] = ((GameObject)Instantiate(original.gameObject, nodes[i+1].pos, nodes[i+1].rot)).transform;
            }
            Debug.Log(nodes.Length + " | " + moveTargets.Count);
            UpdateNodes();
        }
        else
        {
            rb = GetComponent<Rigidbody2D>();
            if (alwaysOn)
                motionRoutine = StartCoroutine(MoveRoutine(rb));
            moveTargets = null;
        }

        GetComponent<Triggerable>().Listen(OnTrigger);
    }
    
    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        List<Transform> targets = new List<Transform>();
        targets.Add(transform);
        targets.AddRange(moveTargets);
        
        return new SerializedMover(
            targets.ToArray(),
            speed, 
            activationDelay, 
            stopTime,
            alwaysOn,
            interruptable,
            autoReturn);
    }

    public class SerializedMover : Serialized
    {
        public SerializedVector3[] poses;
        public SerializedQuaternion[] rots;
        public float speed;
        public float stopTime;
        public bool interruptable;
        public bool alwaysOn;
        public bool autoReturn;
        public float activationDelay;

        public SerializedMover(){}

        public SerializedMover(Transform[] targets, 
            float speed, 
            float activationDelay, 
            float stopTime,
            bool alwaysOn,
            bool interruptable,
            bool autoReturn
            )
        {
            poses = new SerializedVector3[targets.Length];
            rots = new SerializedQuaternion[targets.Length];
            for(int i = 0; i < targets.Length; i++)
            {
                poses[i] = new SerializedVector3(targets[i].position);
                rots[i] = new SerializedQuaternion(targets[i].rotation);
            }

            this.speed = speed;
            this.activationDelay = activationDelay;
            this.stopTime = stopTime;
            this.alwaysOn = alwaysOn;
            this.interruptable = interruptable;
            this.autoReturn = autoReturn;
        }
            
        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            var self = idList[parentID].GetComponent<Mover>();
            PosRotPair[] w = new PosRotPair[poses.Length];
            for(int i = 0; i < poses.Length; i++)
            {
                w[i] = new PosRotPair(poses[i].ToVector(), rots[i].ToQuaternion());
            }
            self.Initialize(w, speed, activationDelay, stopTime, alwaysOn, interruptable, autoReturn, editable);
        }
    }
    #endregion

    [System.Serializable]
    private struct PosRotPair
    {
        public Vector3 pos;
        public Quaternion rot;

        public PosRotPair(Vector3 pos, Quaternion rot)
        {
            this.rot = rot;
            this.pos = pos;
        }
    }
}
