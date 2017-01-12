using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//using UnityEditor;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [SerializeField]
    float torquePower = 500;
    [SerializeField]
    float torqueAirMultiplier = 0.5f;

    [SerializeField]
    float forcePower = 250;
    [SerializeField]
    float forceAirMultiplier = 0.5f;

    [SerializeField]
    Shape shapeSphere;
    [SerializeField]
    Shape shapeRect;
    [SerializeField]
    Shape shapeCube;


    [SerializeField]
    Material faceMat;
    [SerializeField]
    Material bodyMat;

    [SerializeField]
    Texture2D rectFaceTex;
    [SerializeField]
    Texture2D rectBodyTex;
    [SerializeField]
    Texture2D sphereFaceTex;
    [SerializeField]
    Texture2D sphereBodyTex;
    [SerializeField]
    Texture2D squareFaceTex;
    [SerializeField]
    Texture2D squareBodyTex;

    [SerializeField]
    float growSpeed = 6f;

    [SerializeField]
    GameObject particles;

    [SerializeField]
    float maxVelocity = 50;
    [SerializeField]
    float maxAngVelocity = 1500;
    [SerializeField]
    float jumpClamp = 2f;
    [SerializeField]
    float jumpPowerMultiplier = 2f;
    [SerializeField, Range(0, 1)]
    float rectFriction = 0.5f;




    [SerializeField]
    bool grounded = false;

    [SerializeField]
    GameObject audioPrefab;

    Rigidbody2D rb;
    bool swapping = false;
    Vector2 expectedPos;

    Dictionary<ShapeType, Coroutine> shapeRoutines = new Dictionary<ShapeType, Coroutine>();

    AudioSource source;

    ShapeType currentShape = ShapeType.Cube;

    float dir = 0;

    Vector2 lastCollisionNormal;

    // Use this for initialization
    void Awake()
    {
        //PrefabUtility.ResetToPrefabState(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        // Start with cube
        SwapShapes(currentShape);
        expectedPos = rb.worldCenterOfMass;
        source = GetComponent<AudioSource>();
        //  PrefabUtility.ResetToPrefabState(this.gameObject);

        StartCoroutine(CollisionClearer());
    }

    // Update is called once per frame
    void Update()
    {
        dir = Mathf.RoundToInt(Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("SwapSphere") && currentShape != ShapeType.Sphere && !swapping)
        {
            SwapShapes(ShapeType.Sphere);
        }
        if (Input.GetButtonDown("SwapRect") && currentShape != ShapeType.Rectangle && !swapping)
        {
            SwapShapes(ShapeType.Rectangle);
        }
        if (Input.GetButtonDown("SwapCube") && currentShape != ShapeType.Cube && !swapping)
        {
            SwapShapes(ShapeType.Cube);
        }

        if (Input.GetButtonDown("Reset")) Die();

    }

    void FixedUpdate()
    {

        if (dir == 1)
        {
            rb.AddForce(Vector2.right * forcePower * (grounded ? 1 : forceAirMultiplier), ForceMode2D.Force);
            //rb.AddTorque(-1f * torquePower * (grounded ? 1 : torqueAirMultiplier), ForceMode2D.Force);
            rb.angularVelocity -= torquePower * (grounded ? 1 : torqueAirMultiplier) / Time.fixedDeltaTime;
        }
        if (dir == -1)
        {
            rb.AddForce(Vector2.left * forcePower * (grounded ? 1 : forceAirMultiplier), ForceMode2D.Force);
            //rb.AddTorque(torquePower * (grounded ? 1 : torqueAirMultiplier), ForceMode2D.Force);
            rb.angularVelocity += torquePower * (grounded ? 1 : torqueAirMultiplier) / Time.fixedDeltaTime;
        }

        if (Mathf.Abs(rb.angularVelocity) > maxAngVelocity || rb.velocity.magnitude > maxVelocity)
        {
            Debug.Log("Died at " + rb.angularVelocity);
            Die();
        }

        grounded = false;
    }

    IEnumerator CollisionClearer()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();
            collisions.Clear();
        }
    }

    public void SwapShapes(ShapeType shape)
    {
        if (swapping) return;

        PlayPuff();

        foreach (var p in shapeRoutines)
            if (p.Value != null) StopCoroutine(p.Value);

        shapeRoutines[ShapeType.Sphere] = StartCoroutine(
            shape == ShapeType.Sphere ? Grow(shapeSphere) : Shrink(shapeSphere));
        shapeRoutines[ShapeType.Rectangle] = StartCoroutine(
            shape == ShapeType.Rectangle ? Grow(shapeRect, true) : Shrink(shapeRect));
        shapeRoutines[ShapeType.Cube] = StartCoroutine(
            shape == ShapeType.Cube ? Grow(shapeCube) : Shrink(shapeCube));

        currentShape = shape;
    }

    void PlayPuff()
    {
        // Particles
        Vector3 tempVec;
        tempVec = transform.position + new Vector3(0, 0, -2);
        var go = Instantiate(particles, tempVec, Quaternion.identity) as GameObject;
        go.AddComponent<Rigidbody2D>().velocity = rb.velocity;

        // Sounds
        /*
        if (transformSounds.Length > 0)
            AudioSource.PlayClipAtPoint(transformSounds[UnityEngine.Random.Range(0,
                transformSounds.Length)], transform.position, oneShotSoundVolume);
        */
        PlaySound("transform");
    }

    public void Die()
    {
        /*
        if (deathSounds.Length > 0)
            AudioSource.PlayClipAtPoint(
                deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)], transform.position, oneShotSoundVolume * 0.5f);
                */
        Camera.main.GetComponent<Transition>().DoTransition();

        transform.position = CheckPoint.GetSpawnLoc();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        PlaySound("death");
    }

    IEnumerator Shrink(Shape shape)
    {
        while (shape.transform.localScale.x > 0.2f)
        {
            shape.transform.localScale -= Vector3.one * Time.deltaTime * 6;
            shape.transform.localScale = Vector3.Max(shape.transform.localScale, Vector3.one * 0.2f);
            yield return new WaitForFixedUpdate();
        }
        shape.transform.gameObject.SetActive(false);
       
    }

    IEnumerator Grow(Shape shape, bool bounceCheck = false)
    {
        swapping = true;
        expectedPos = (rb.worldCenterOfMass + rb.velocity * Time.fixedDeltaTime);


        if (shape == shapeCube)
        {
            faceMat.mainTexture = squareFaceTex;
            bodyMat.mainTexture = squareBodyTex;
        }
        else if (shape == shapeSphere)
        {
            faceMat.mainTexture = sphereFaceTex;
            bodyMat.mainTexture = sphereBodyTex;
        }
        else if (shape == shapeRect)
        {
            faceMat.mainTexture = rectFaceTex;
            bodyMat.mainTexture = rectBodyTex;
        }

        shape.transform.gameObject.SetActive(true);

        float growTime = 1f / growSpeed;
        Vector3 initialScale = shape.transform.localScale;
        Vector3 endScale = Vector3.one;

        var angPow = torquePower;
        var linPow = forcePower;
        var mass = rb.mass;
        var linDrag = rb.drag;
        var angDrag = rb.angularDrag;
        var airLinMult = forceAirMultiplier;
        var airAngMult = torqueAirMultiplier;
        for (float timer = 0; timer < growTime; timer += Time.fixedDeltaTime)
        {
            float lerpVal = timer / growTime;

            // RB params
            torquePower = Mathf.Lerp(angPow, shape.angularPower, lerpVal);
            forcePower = Mathf.Lerp(linPow, shape.linearPower, lerpVal);
            rb.mass = Mathf.Lerp(mass, shape.mass, lerpVal);
            rb.drag = Mathf.Lerp(linDrag, shape.linearDrag, lerpVal);
            rb.angularDrag = Mathf.Lerp(angDrag, shape.angularDrag, lerpVal);

            forceAirMultiplier = Mathf.Lerp(airLinMult, shape.airLinearMultiplier, lerpVal);
            torqueAirMultiplier = Mathf.Lerp(airLinMult, shape.airAngularMultiplier, lerpVal);


            // Size
            shape.transform.localScale = Vector3.Lerp(initialScale, endScale, lerpVal);

            yield return new WaitForFixedUpdate();

            if (bounceCheck && (expectedPos - rb.worldCenterOfMass).magnitude > 0.005f)
            {
                BounceCheck();
            }

            expectedPos = (rb.worldCenterOfMass + rb.velocity * Time.fixedDeltaTime);

        }
        shape.transform.localScale = endScale;


        torquePower = shape.angularPower;
        forcePower = shape.linearPower;
        rb.mass = shape.mass;
        rb.drag = shape.linearDrag;
        rb.angularDrag = shape.angularDrag;

        forceAirMultiplier = shape.airLinearMultiplier;
        torqueAirMultiplier = shape.airAngularMultiplier;

        swapping = false;
    }

    void BounceCheck()
    {

        var delta = Vector2.ClampMagnitude(((rb.worldCenterOfMass - expectedPos) /
            Time.fixedDeltaTime) * jumpPowerMultiplier, jumpClamp);

        Debug.DrawRay(transform.position, delta, Color.green, 5f);

        if (Vector2.Angle(transform.right, lastCollisionNormal) < 90)
        {
            var angle = Vector2.Angle(transform.right, lastCollisionNormal);
            Vector3 cross = Vector3.Cross(transform.right, lastCollisionNormal);

            if (cross.z > 0)
                angle = -angle;

            Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, Vector3.forward) * delta, Color.red, 5f);

            angle *= rectFriction;

            delta = Quaternion.AngleAxis(angle, Vector3.forward) * delta;
        }
        else
        {
            var angle = Vector2.Angle(-transform.right, lastCollisionNormal);
            Vector3 cross = Vector3.Cross(-transform.right, lastCollisionNormal);

            if (cross.z > 0)
                angle = -angle;

            Debug.DrawRay(transform.position, Quaternion.AngleAxis(angle, Vector3.forward) * delta, Color.red, 5f);

            angle *= rectFriction;

            delta = Quaternion.AngleAxis(angle, Vector3.forward) * delta;
        }

        Debug.DrawRay(transform.position, delta, new Color(255, 64, 0), 5f);

        rb.velocity = rb.velocity + delta;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        grounded = true;
        Vector2 total = Vector2.zero;
        foreach (ContactPoint2D p in col.contacts)
        {
            total += p.normal;
        }
        lastCollisionNormal = total.normalized;


    }

    void PlaySound(string type) //death, transform
    {
        GameObject PlayerSound = Instantiate(audioPrefab, transform.position, transform.rotation) as GameObject;
        PlayerSound.GetComponent<PlayerSound>().audioGroup = type;
        PlayerSound.transform.SetParent(this.transform, false);
    }

    private List<Collision2D> collisions = new List<Collision2D>();

    bool didCheckThisFrame = false;

    void OnCollisionStay2D(Collision2D coll)
    {
        collisions.Add(coll);

        // cycle through the other collisions and detect what the normal is.
        // if the difference between the normals is more than 90 degrees, the player has been crushed.
        Vector3 new_normal = coll.contacts[0].normal;
        
        foreach (Collision2D existing_coll in collisions)
        {
            Vector3 existing_normal = existing_coll.contacts[0].normal;

            // If normals are opposite and they are moving in opposite directions, die
            float normal_angle = Vector3.Angle(new_normal, existing_normal);
             
            if ((existing_coll.transform.tag == "Crusher" || coll.transform.tag == "Crusher")
                && normal_angle > 100 &&
                ((Vector3.Dot(existing_coll.relativeVelocity.normalized,
                coll.relativeVelocity.normalized) < 0.5f) &&
                ((coll.relativeVelocity - existing_coll.relativeVelocity).magnitude > 0.01f)))
            {
                Debug.Log(normal_angle + " | " + existing_coll.relativeVelocity + " | " +
                coll.relativeVelocity);
                Die();
            }
        }
    }

            


    public enum ShapeType
    {
        Sphere = 0,
        Rectangle = 1,
        Cube = 2
    }

    [Serializable]
    public class Shape
    {
        public Transform transform;
        
        public float angularPower = 1;
        public float linearPower = 1;
        
        public float mass = 1;
        public float linearDrag = 0.05f;
        public float angularDrag = 0.3f;

        public float airLinearMultiplier = 1f;
        public float airAngularMultiplier = 1f;
    }
}
