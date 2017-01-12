using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CheckboxSmile : MonoBehaviour {

    [SerializeField]
    Animator animator;
    [SerializeField]
    Toggle toggle;

    void Awake()
    {
        UpdateValue();
    }

    public void UpdateValue()
    {
        animator.SetBool("On", toggle.isOn);
    }
}
