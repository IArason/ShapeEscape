using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContextTweakMenuSpinner : ContextTweakMenu
{
    [SerializeField]
    Toggle flippedRotation;
    [SerializeField]
    Slider speed;
    [SerializeField]
    Toggle startOn;

    Spinner[] spinners;
    
    public void UpdateValues()
    {
        spinners[0].flippedRotation = flippedRotation.isOn;
        spinners[0].speed = speed.value;
        spinners[0].startOn = startOn.isOn;
    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        spinners = owners.Select(x => x.GetComponent<Spinner>()).ToArray();
        if (spinners.Any(x => x == null))
        {
            FailVerification();
            return;
        }

        flippedRotation.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        speed.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        startOn.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);

        flippedRotation.isOn = spinners[0].flippedRotation;
        speed.value = spinners[0].speed;
        startOn.isOn = spinners[0].startOn;

        flippedRotation.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        speed.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        startOn.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
    }
}
