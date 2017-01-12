using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContextTweakMenuButton : ContextTweakMenu
{
    [SerializeField]
    Slider stiffness;
    [SerializeField]
    Slider resetTime;
    [SerializeField]
    Toggle canReset;
    [SerializeField]
    Text stiffnessValue;
    [SerializeField]
    Text resetTimeValue;

    PushButton[] buttons;

    public void Awake()
    {
        selectionArea.SetActive(false);
    }

    public void UpdateValues()
    {
        buttons[0].stiffness = stiffness.value;
        buttons[0].resetTime = resetTime.value;
        buttons[0].canReset = canReset.isOn;
        //stiffnessValue.text = buttons[0].stiffness.ToString("00.00");
        resetTimeValue.text = buttons[0].resetTime.ToString("G1");

        resetTime.gameObject.SetActive(canReset.isOn);
        resetTimeValue.gameObject.SetActive(canReset.isOn);

    }

    public override void VerifyAndSetOwner(List<Transform> owners)
    {
        base.VerifyAndSetOwner(owners);
        buttons = owners.Select(x => x.GetComponent<PushButton>()).ToArray();
        if (buttons.Any(x => x == null))
        {
            FailVerification();
            return;
        }

        stiffness.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        resetTime.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);
        canReset.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.Off);

        stiffness.value = buttons[0].stiffness;
        canReset.isOn = buttons[0].canReset;
        resetTime.value = buttons[0].resetTime;
        resetTimeValue.text = buttons[0].resetTime.ToString("G1");
        //stiffnessValue.text = buttons[0].stiffness.ToString("00.00");

        resetTime.gameObject.SetActive(canReset.isOn);
        resetTimeValue.gameObject.SetActive(canReset.isOn);
        
        stiffness.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        resetTime.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        canReset.onValueChanged.SetPersistentListenerState(
            0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
    }
    
    protected override IEnumerator OpenMenuJuice()
    {
        resetTime.gameObject.SetActive(canReset.isOn);
        resetTimeValue.gameObject.SetActive(canReset.isOn);
        StartCoroutine(base.OpenMenuJuice());
        yield return new WaitForEndOfFrame();
    }


}
