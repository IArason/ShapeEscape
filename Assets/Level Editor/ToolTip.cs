using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class ToolTip : MonoBehaviour {

    [SerializeField]
    [TextArea]
    string text = "Remember to set up the event trigger properly.";

    [SerializeField]
    float delay = 1f;

    [SerializeField]
    GameObject tooltipPrefab;
    GameObject tooltip;

    void Awake()
    {
        
    }

	public void MouseEnter()
    {
        StartCoroutine(Open());
    }

    void Update()
    {
        if(tooltip != null)
            tooltip.transform.position = Input.mousePosition;
    }

    public void MouseExit()
    {
        StopAllCoroutines();

        if(tooltip != null)
            Destroy(tooltip);
    }

    IEnumerator Open()
    {
        var timer = 0f;
        while(timer < delay)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (tooltip == null)
        {
            if(tooltipPrefab == null)
            {
                Debug.LogError("Prefab for tooltip on " + name + " not set.");
                yield break;
            }
            tooltip = Instantiate(tooltipPrefab);
            Debug.Log(tooltip);
            tooltip.GetComponentInChildren<Text>().text = text;
            tooltip.transform.SetParent(transform);
        }
    }
}
