using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Triggerable), typeof(Rigidbody2D))]
public class Spinner : LevelEntityAttribute
{
    public bool flippedRotation = false; // false CW, true CCW
    public float speed = 1;
    public bool startOn = true;

    Rigidbody2D rb;
    float currentSpeed = 0;
    bool isSpinning = false;

    Coroutine accelRoutine;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        GetComponent<Triggerable>().Listen(ToggleOn);
    }

    void FixedUpdate()
    {
        if (accelRoutine == null)
            currentSpeed = isSpinning ? speed : 0;

        rb.MoveRotation(rb.rotation - Time.fixedDeltaTime * currentSpeed * 360 * (flippedRotation ? -1 : 1));
    }

    protected override void OnSelect()
    {
        ToggleOn();
    }

    protected override void OnDeselect()
    {
        ToggleOn();
    }

    void ToggleOn()
    {
        if (!gameObject.activeInHierarchy) return;

        if (accelRoutine != null)
        {
            StopCoroutine(accelRoutine);
            isSpinning = !isSpinning;
        }
        accelRoutine = StartCoroutine(Accelerate());
    }

    IEnumerator Accelerate()
    {
        var timer = speed / 2f;

        var oldSpeed = currentSpeed;

        while(timer > 0)
        {
            // Lerping counting down to 0, so (max, min)
            if(isSpinning)
                currentSpeed = Mathf.Lerp(0, oldSpeed, timer / (speed / 2f));
            else
                currentSpeed = Mathf.Lerp(speed, oldSpeed, timer / (speed / 2f));

            timer -= Time.fixedDeltaTime * (isSpinning ? 1 : 0.5f);
            yield return new WaitForFixedUpdate();
        }

        currentSpeed = isSpinning ? 0 : speed;

        isSpinning = !isSpinning;
        accelRoutine = null;
    }

    public void SetParams(float speed, bool flippedRotation, bool startOn, bool editable)
    {
        this.speed = speed;
        this.flippedRotation = flippedRotation;
        this.startOn = startOn;

        if (!editable && startOn)
            ToggleOn();
    }

    #region Serialization

    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        return new SerializedSpinner(speed, flippedRotation, startOn);
    }

    public class SerializedSpinner : Serialized
    {
        public float speed;
        public bool flippedRotation;
        public bool startOn;

        public SerializedSpinner() { }

        public SerializedSpinner(float speed, bool flippedRotation, bool startOn)
        {
            this.speed = speed;
            this.flippedRotation = flippedRotation;
            this.startOn = startOn;
        }

        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            var self = idList[parentID].GetComponent<Spinner>();
            self.SetParams(speed, flippedRotation, startOn, editable);
        }
    }


    #endregion
}
