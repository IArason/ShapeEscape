using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContextTweakMenuCrusher : ContextTweakMenu
{
    [SerializeField]
    Slider speedSlider;
    [SerializeField]
    Slider intervalSlider;
    [SerializeField]
    Slider offsetSlider;
    [SerializeField]
    Toggle startOnButton;


    [SerializeField]
    Text offsetValue;
    [SerializeField]
    Text intervalValue;

    Crusher[] crushers;

    void Awake()
    {
        selectionArea.SetActive(false);
    }
    
    public void UpdateValues()
    {
        foreach (var c in crushers)
        {
            c.interval = intervalSlider.value / 10;
            c.speed = speedSlider.value;
            c.timeOffset = offsetSlider.value * c.interval;
            c.startOn = startOnButton.isOn;
        }

        offsetValue.text = offsetSlider.value.ToString("n1");
        intervalValue.text = crushers[0].interval.ToString("n1");
    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        crushers = owners.Select(x => x.GetComponent<Crusher>()).ToArray();
        if(crushers.Any(x => x == null))
        {
            FailVerification();
        }

        intervalSlider.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        speedSlider.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        offsetSlider.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        startOnButton.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);

        Debug.Log("Updating slider values of " + owners.Count + " crushers");

        speedSlider.value = crushers[0].speed;
        Debug.Log(speedSlider.value + " | " + crushers[0].speed);
        intervalSlider.value = crushers[0].interval * 10;
        offsetSlider.value = crushers[0].timeOffset / crushers[0].interval;
        startOnButton.isOn = crushers[0].startOn;

        offsetValue.text = crushers[0].timeOffset.ToString("n1");
        intervalValue.text = crushers[0].interval.ToString("n1");

        intervalSlider.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        speedSlider.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        offsetSlider.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        startOnButton.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
    }
}
