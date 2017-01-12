using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

[RequireComponent(typeof(Triggerable))]
public class Crusher : LevelEntityAttribute, ISurfaceAlignable
{
    [SerializeField]
    AnimationCurve velocityCurve;

    [SerializeField]
    GameObject bottom;
    [SerializeField]
    GameObject piston;
    [SerializeField]
    GameObject top;

    [SerializeField]
    Transform[] linecastPositions;

    [SerializeField]
    float maxLength = 10f;

    [SerializeField]
    float normalizedParticleTime = 0.3f;
    [SerializeField]
    AudioClip crushHit;
    [SerializeField]
    float crushVolume;
    [SerializeField]
    float crushPitch;
    [SerializeField]
    AudioSource crushSource;

    bool runLinecast = false;

    [SerializeField]
    float currentLength = 0;
    Vector2 lastNormal = Vector3.up;

    bool active = false;
    bool gameplay = false;

    Rigidbody2D rb;

    /// Menu-editable variables
    public float interval = 1f;
    public float speed = 5f; // How fast it goes down
    public float timeOffset = 0f;
    public bool startOn = true;

    [SerializeField]
    ParticleSystem[] crushParticles;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(CrushCycle());
        GetComponent<Triggerable>().Listen(OnTrigger);
        rb = bottom.GetComponent<Rigidbody2D>();
        crushSource.clip = crushHit;
        crushSource.volume = crushVolume;
        crushSource.pitch = crushPitch;
    }

    void OnTrigger()
    {
        active = !active;
    }

    protected override void OnSelect()
    {
        active = true;
    }

    protected override void OnDeselect()
    {
        active = false;
    }

    void LateUpdate()
    {
        float shortestCast = maxLength;
        RaycastHit2D shortest = new RaycastHit2D();

        foreach (Transform t in linecastPositions)
        {
            var hits = Physics2D.LinecastAll(t.position.XY(), t.position.XY() - t.up * maxLength);

            Debug.DrawLine(t.position.XY(), t.position.XY() - t.up * maxLength, Color.green);

            if (hits.Length == 0)
            {
                continue;
            }

            shortest = hits[0];

            foreach (var h in hits)
            {
                if (h.collider.gameObject.tag != "LevelBlock") continue;

                Debug.DrawLine(t.position.XY(), h.point, Color.yellow);
                if (Vector2.Distance(h.point, t.position.XY()) < shortestCast)
                {
                    shortestCast = Vector2.Distance(h.point, t.position.XY());
                    lastNormal = h.normal;
                    shortest = h;
                }
            }
        }

        SetLength(shortestCast - linecastPositions[0].transform.localPosition.y);
        Debug.DrawRay(shortest.point, (Vector2)transform.up * shortestCast, Color.red);
    }
    
    IEnumerator CrushCycle()
    {
        yield return new WaitForSeconds(timeOffset);
        float time = interval;
        // Always active coroutine
        while (true)
        {
            if(active)
            {
                time = interval;
                float timer = 0;
                while (timer < 1 / speed)
                {
                    var value = velocityCurve.Evaluate(timer / (1 / speed));

                    rb.MovePosition(transform.TransformPoint(bottom.transform.localPosition.x * Vector3.right +
                        Vector3.down * currentLength * value));
                    piston.transform.localScale =
                        Vector3.one + ((value * currentLength) - 1) * Vector3.up;

                    timer += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();

                    if (timer < normalizedParticleTime / speed && timer + Time.fixedDeltaTime >= normalizedParticleTime / speed)
                    { 
                    // crushSource.Stop();
                    //crushSource.clip = null;
                    //crushSource.clip = crushHit;
                    DoEffects();

                    // Debug.Log("what");
                    crushSource.Play();
                }
                    
                }

                yield return new WaitForSeconds(time - 1/speed);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void AlignToSurface()
    {
        // Not on a surface.
        if (currentLength == maxLength) return;

        var tempGO = new GameObject();
        transform.rotation = Quaternion.identity;

        var oldParent = transform.parent;

        // Place GO at the center of the base
        tempGO.transform.position = bottom.transform.position;

        tempGO.transform.rotation = Quaternion.LookRotation(Vector3.back, transform.up);

        transform.SetParent(tempGO.transform, true);
        transform.rotation = Quaternion.identity;

        tempGO.transform.rotation = Quaternion.LookRotation(Vector3.back, lastNormal);

        transform.SetParent(oldParent, true);

        Destroy(tempGO);
    }

    void SetLength(float length)
    {
        if (length != currentLength)
        {
            currentLength = length;
        }
    }

    public void Activate(float frequency, float length, bool startOn, float speed, bool editable, float timeOffset)
    {
        this.interval = frequency;
        SetLength(length);
        this.startOn = startOn;
        this.speed = speed;
        this.timeOffset = timeOffset;
        //StartCoroutine(CrushCycle());

        // Prevents resizing at playtime
        if (!editable && linecastPositions.Length != 0)
        {
            active = startOn;
            gameplay = !editable;
        }
    }
    void DoEffects()
    {
        foreach (ParticleSystem p in crushParticles)
        {
            p.Play();
        }
    }

    #region Serialization
    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        return new SerializedCrusher(interval, currentLength, startOn, speed, timeOffset);
    }

    public class SerializedCrusher : Serialized
    {
        public float frequency;
        public float length;
        public bool startOn;
        public float speed;
        public float timeOffset;

        public SerializedCrusher()
        { }

        public SerializedCrusher(float frequency, float length, bool startOn, float speed, float timeOffset)
        {
            this.frequency = frequency;
            this.length = length;
            this.startOn = startOn;
            this.speed = speed;
            this.timeOffset = timeOffset;
        }

        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            idList[parentID].GetComponent<Crusher>().Activate(frequency, length, startOn, speed, editable, timeOffset);
        }
    }
    #endregion
}
