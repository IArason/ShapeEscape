using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ContextMenu : Singleton<ContextMenu>
{
    [SerializeField]
    GameObject contextMenuRoot;


    [SerializeField]
    Transform tweakButton;
    [SerializeField]
    Transform deleteButton;
    [SerializeField]
    Transform connectButton;
    [SerializeField]
    Transform disconnectButton;
    [SerializeField]
    Transform duplicateButton;
    [SerializeField]
    Transform[] miscButtonLocators;

    // Faded replacements
    [SerializeField]
    GameObject tweakButtonDisabled;
    [SerializeField]
    GameObject deleteButtonDisabled;
    [SerializeField]
    GameObject connectButtonDisabled;
    [SerializeField]
    GameObject disconnectButtonDisabled;
    [SerializeField]
    GameObject duplicateButtonDisabled;
    [SerializeField]
    GameObject[] miscDisabledButtons;
    
    List<GameObject> buttons = new List<GameObject>();

    Vector3 mouseClickPos;
    bool wasOverUIElement;
    bool menuReady = true;
    bool menuOpen = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && LevelEditorUtils.MouseOverUIElement())
            wasOverUIElement = true;

        if ((menuOpen && Input.GetMouseButtonUp(1))
            || (Input.GetMouseButtonUp(0) && !wasOverUIElement))
        {
            CloseMenu();
        }

        if (Input.GetMouseButtonUp(0)) wasOverUIElement = false;

        if (Input.GetMouseButtonUp(1))
        {
            if (!menuReady) return;

            // If above selection group, get selection group and use context menu on that.
            // Otherwise run it on the object under mouse.
            List<ContextMenuTarget> targets;

            if (ObjectManipulator.Instance.IsOverSelectionGroup())
            {
                targets = ObjectManipulator.Instance.GetSelection().
                    Select(x => x.GetComponent<ContextMenuTarget>()).ToList();

                // All targets must have a ContextMenuTarget
                if (targets.Count == 0 || targets.Any(x => x == null)) return;
            }
            else
            {
                // Grabs first menu target.
                var underMouse = Physics2D.OverlapPointAll(
                    LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition))
                    .Select(x => x.GetComponentInParent<ContextMenuTarget>()).Where(x => x != null).
                    ToList();

                var tmp = Physics2D.OverlapPointAll(
                    LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition));

                foreach (Collider2D c in tmp)
                {
                    Debug.Log(c.name);
                }

                if (underMouse.Count == 0)
                    return;

                targets = new List<ContextMenuTarget>() { underMouse[0] };

                ObjectManipulator.Instance.SetSelection(underMouse[0].GetComponent<LevelEntity>());
            }

            mouseClickPos = LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition);
            OpenMenu(targets);
        }

    }

    void LateUpdate()
    {
        contextMenuRoot.transform.position = Camera.main.WorldToScreenPoint(mouseClickPos);
    }

    /// <summary>
    /// Takes a list of context menus
    /// </summary>
    /// <param name="elements"></param>
    public void OpenMenu(List<ContextMenuTarget> elements)
    {
        if (!menuReady) return;

        var buttonTypes = new List<List<GameObject>>();

        // Gather all menu options in a list
        foreach (ContextMenuTarget t in elements)
        {
            buttonTypes.Add(t.MenuOptions);
        }

        buttonTypes = GetSharedByAll(buttonTypes);

        Debug.Log(buttonTypes.Count);
        if (buttonTypes.Count == 0)
            return;

        ResetInactiveButtons();

        buttons.Clear();
        contextMenuRoot.SetActive(true);
        menuReady = false;
        contextMenuRoot.GetComponent<Animator>().SetBool("Open", true);
        for (int i = 0; i < buttonTypes[0].Count; i++)
        {
            buttons.Add(Instantiate(buttonTypes[0][i]));

            if (buttonTypes[0][i].GetComponent<ContextTweakMenu>() != null)
            {
                buttons[i].transform.SetParent(tweakButton, false);
                tweakButtonDisabled.gameObject.SetActive(false);

            }
            else if (buttonTypes[0][i].GetComponent<ContextDelete>() != null)
            {
                buttons[i].transform.SetParent(deleteButton, false);
                deleteButtonDisabled.gameObject.SetActive(false);

            }
            else if (buttonTypes[0][i].GetComponent<ContextDuplicate>() != null ||
                buttonTypes[0][i].GetComponent<ContextDuplicateNode>() != null)
            {
                buttons[i].transform.SetParent(duplicateButton, false);
                duplicateButtonDisabled.gameObject.SetActive(false);

            }
            else if (buttonTypes[0][i].GetComponent<ContextLinkToTrigger>() != null ||
              buttonTypes[0][i].GetComponent<ContextLinkToTriggerable>() != null)
            {
                buttons[i].transform.SetParent(connectButton, false);
                connectButtonDisabled.gameObject.SetActive(false);

            }
            else if (buttonTypes[0][i].GetComponent<ContextClearLinks>() != null)
            {
                buttons[i].transform.SetParent(disconnectButton, false);
                disconnectButtonDisabled.gameObject.SetActive(false);
            }
            else // Non-static buttons
            {
                for(int j = 0; j < miscButtonLocators.Length; j++)
                {
                    if(miscButtonLocators[j].childCount == 1)
                    {
                        buttons[i].transform.SetParent(miscButtonLocators[j], false);
                        miscDisabledButtons[j].SetActive(false);
                        break;
                    }
                }
            }
            
            buttons[i].GetComponent<ContextMenuOption>().VerifyAndSetOwner(
                elements.Select(x => x.transform).ToList());
        }
        menuOpen = true;
    }

    void ResetInactiveButtons()
    {
        tweakButtonDisabled.SetActive(true);
        connectButtonDisabled.SetActive(true);
        disconnectButtonDisabled.SetActive(true);
        duplicateButtonDisabled.SetActive(true);
        deleteButtonDisabled.SetActive(true);
        foreach (var go in miscDisabledButtons)
            go.SetActive(true);
    }

    public void CloseMenu()
    {
        if (!menuOpen)
        {
            return;
        }
        
        menuReady = false;
        contextMenuRoot.GetComponent<Animator>().SetBool("Open", false);
        
        foreach(var b in buttons)
        {
            b.GetComponent<Button>().interactable = false;
        }
        menuOpen = false;
    }

    public void OnMenuReady()
    {
        menuReady = true;
        if(!menuOpen)
            foreach (GameObject g in buttons)
            {
                Destroy(g);
            }
    }

    /// <summary>
    /// Returns a list of lists of context menu options where all elements
    /// not shared by all sub-lists are removed.
    /// Tested and working.
    /// </summary>
    List<List<GameObject>> GetSharedByAll(List<List<GameObject>> options)
    {
        // Populate a list of all types with no duplicates
        HashSet<GameObject> types = new HashSet<GameObject>();
        foreach (List<GameObject> l in options)
        {
            foreach (GameObject o in l)
            {
                types.Add(o);
            }
        }

        foreach (GameObject t in types)
        {
            // If not every sub-list contains an option of type, remove it.
            if (!options.All(x => x.Any(z => z == t)))
            {
                RemoveTypeFromAll(t, ref options);
            }
        }

        return options;
    }

    void RemoveTypeFromAll(GameObject type, ref List<List<GameObject>> options)
    {
        foreach (List<GameObject> l in options)
        {
            l.RemoveAll(x => x == type);
        }
    }
}
