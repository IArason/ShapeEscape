using UnityEngine;
using System.Collections;

public class Jumper : MonoBehaviour {

    [SerializeField]
    SliderJoint2D rightBox;
    [SerializeField]
    SliderJoint2D leftBox;

    [SerializeField]
    float speed = 20f;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Expand();
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Q))
        {
            Shrink();
        }
    }

    public void Expand()
    {
        StopAllCoroutines();
        StartCoroutine(Expand(leftBox));
        StartCoroutine(Expand(rightBox));
    }

    public void Shrink()
    {
        StopAllCoroutines();
        StartCoroutine(Shrink(leftBox));
        StartCoroutine(Shrink(rightBox));
    }

    IEnumerator Expand(SliderJoint2D joint)
    {
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = speed;
        joint.motor = motor;
        while (Mathf.Abs(joint.transform.localPosition.x) < joint.limits.max - 0.01f)
        {
            yield return new WaitForFixedUpdate();
        }
        
        motor.motorSpeed = 0f;
        joint.motor = motor;
    }

    IEnumerator Shrink(SliderJoint2D joint)
    {
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = -speed;
        joint.motor = motor;
        while (Mathf.Abs(joint.transform.localPosition.x) > joint.limits.min + 0.01f)
        {
            yield return new WaitForFixedUpdate();
        }

        motor.motorSpeed = 0f;
        joint.motor = motor;
    }

}
