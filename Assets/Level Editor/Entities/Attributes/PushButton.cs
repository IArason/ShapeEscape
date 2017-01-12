using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Trigger))]
public class PushButton : LevelEntityAttribute, ISurfaceAlignable {

    [SerializeField]
    Rigidbody2D pushPlate;

    [SerializeField]
    float triggerDepth = -0.22f; // If it reaches this depth on its own, activate
    [SerializeField]
    float lockedDepth = -0.28f; // Moves to this depth and locks while cooling down

    Trigger trigger;

    bool coolingDown = false;

    /// Menu-modifiable variables
    public float stiffness = 5f;
    public float resetTime = 1f;
    public bool canReset = true;

    protected override void Awake()
    {
        base.Awake();
        trigger = GetComponent<Trigger>();
    }

    void FixedUpdate()
    {
        if (coolingDown) return;

        if(pushPlate.transform.localPosition.y < triggerDepth)
        {
            StartCoroutine(OnActivate());
        }
    }

    IEnumerator OnActivate()
    {
        Debug.Log("Triggered button");
        trigger.OnTrigger();
        coolingDown = true;
        pushPlate.GetComponent<Rigidbody2D>().isKinematic = true;

        // Move plate down and lock it

        var timer = resetTime;
        if (canReset)
        {
            while (timer > 0)
            {
                if (pushPlate.transform.localPosition.y > lockedDepth)
                {
                    pushPlate.transform.localPosition += Vector3.down * 0.25f * Time.fixedDeltaTime;
                }

                timer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while(pushPlate.transform.localPosition.y > lockedDepth)
                pushPlate.transform.localPosition += Vector3.down * 0.25f * Time.fixedDeltaTime;

            yield break;
        }

        pushPlate.isKinematic = false;
        pushPlate.mass = 30f;

        // Wait a bit for it to rebound
        yield return new WaitForSeconds(2f);
        pushPlate.mass = stiffness;

        coolingDown = false;
    }

    public void ReadyForPlay(float stiffness, float resetTime, bool canReset, bool editable)
    {
        this.stiffness = stiffness;
        this.resetTime = resetTime;
        this.canReset = canReset;
        pushPlate.isKinematic = editable;
        pushPlate.mass = stiffness;
    }

    public void AlignToSurface()
    {
        var hits = Physics2D.LinecastAll(transform.position, transform.position - transform.up * 10);
        if (hits.Length != 0)
        {
            RaycastHit2D closest = hits[0];
            foreach (var v in hits)
            {
                if (v.transform.tag != "LevelBlock") continue;

                if (closest.transform.tag != "LevelBlock" ||
                    Vector3.Distance(transform.position, v.point) <
                    Vector3.Distance(transform.position, closest.point))
                    closest = v;
            }
            if (closest.transform.tag != "LevelBlock") return;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, closest.normal);
            transform.position = (Vector3)closest.point + Vector3.forward * transform.position.z;
        }
    }

    #region Serialization

    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        return new SerializedPushButton(stiffness, resetTime, canReset);
    }
    
    public class SerializedPushButton : Serialized
    {
        public float stiffness;
        public float resetTime;
        public bool canReset;

        public SerializedPushButton() { }

        public SerializedPushButton(float stiffness, float resetTime, bool canReset)
        {
            this.stiffness = stiffness;
            this.resetTime = resetTime;
            this.canReset = canReset;
        }

        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            idList[parentID].GetComponent<PushButton>().ReadyForPlay(stiffness, resetTime, canReset, editable);
        }
    }

    #endregion
}
