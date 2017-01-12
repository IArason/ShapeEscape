using System.Collections;
using UnityEngine;

public abstract class ContextTweakMenu : ContextMenuOption
{ 
    [SerializeField]
    protected GameObject selectionArea;

    [SerializeField]
    protected Animator openAnimator;

    bool pointerOutOfBounds = false;
    bool open = false;

    Coroutine currentRoutine;

    void Awake()
    {
        selectionArea.SetActive(false);
        openAnimator.transform.localScale = Vector3.zero;
    }

    /* -- Commented areas cause the tweak menu to close when the mouse moves out of bounds.
    void Update()
    {
        if (pointerOutOfBounds && !Input.GetMouseButton(0))
        {
            if (currentRoutine != null) return;

            currentRoutine = StartCoroutine(CloseMenuJuice());
        }
    }
    */

    public override void OnSelect()
    {
        if(currentRoutine != null)
            StopCoroutine(currentRoutine);
        if (!open)
            currentRoutine = StartCoroutine(OpenMenuJuice());
        else
            currentRoutine = StartCoroutine(CloseMenuJuice());
    }

    /*
    public override void OnPointerEnter()
    {
        pointerOutOfBounds = false;
    }

    public override void OnPointerExit()
    {
        pointerOutOfBounds = true;
    }
    */

    protected virtual IEnumerator OpenMenuJuice()
    {
        Debug.Log("Opening");
        selectionArea.SetActive(true);
        openAnimator.SetBool("Open", true);
        yield return new WaitForSeconds(0.5f);
        open = true;
        currentRoutine = null;
    }

    protected virtual IEnumerator CloseMenuJuice()
    {
        pointerOutOfBounds = false;
        Debug.Log("Closing");
        openAnimator.SetBool("Open", false);
        yield return new WaitForSeconds(0.5f);
        open = false;
        selectionArea.SetActive(false);
        currentRoutine = null;
    }

}

