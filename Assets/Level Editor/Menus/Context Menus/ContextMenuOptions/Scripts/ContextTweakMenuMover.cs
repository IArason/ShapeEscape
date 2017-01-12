using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContextTweakMenuMover : ContextTweakMenu
{
    [SerializeField]
    Toggle interruptable;
    [SerializeField]
    Toggle autoReturn;
    [SerializeField]
    Slider speed;
    [SerializeField]
    Toggle alwaysOn;
    [SerializeField]
    Slider activationDelay;
    [SerializeField]
    Slider pauseTime;

    Mover[] movers;

    public void UpdateValues()
    {
        movers[0].speed = speed.value;
        movers[0].interruptable = interruptable.isOn;
        movers[0].autoReturn = autoReturn.isOn;
        movers[0].activationDelay = activationDelay.value;
        movers[0].stopTime = pauseTime.value;
        movers[0].alwaysOn = alwaysOn.isOn;
    }

    // Speed, interruptable, auto return, always on, activation delay, stop time

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        movers = owners.Select(x => x.GetComponent<Mover>()).ToArray();
        if (movers.Any(x => x == null))
        {
            FailVerification();
            return;
        }

        interruptable.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        activationDelay.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        autoReturn.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        speed.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        alwaysOn.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        pauseTime.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);

        speed.value = movers[0].speed;
        interruptable.isOn = movers[0].interruptable;
        autoReturn.isOn = movers[0].autoReturn;
        activationDelay.value = movers[0].activationDelay;
        pauseTime.value = movers[0].stopTime;
        alwaysOn.isOn = movers[0].alwaysOn;

        interruptable.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        activationDelay.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        autoReturn.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        speed.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        alwaysOn.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        pauseTime.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
    }
}
