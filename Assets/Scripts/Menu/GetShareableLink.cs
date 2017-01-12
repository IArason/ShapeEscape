using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GetShareableLink : MonoBehaviour {
    
    [SerializeField]
    InputField field;
    [SerializeField]
    RectTransform popupObject;

    bool open = false;

    string link;

    SharedLevel level;

    // Use this for initialization
    void Start()
    {
        popupObject.localScale = Vector2.zero;
        if (level == null)
            Destroy(transform.parent.gameObject);
    }

    public void SetLevel(SharedLevel level)
    {
        this.level = level;
    }

    public void Open()
    {
        if(open)
        {
            StartCoroutine(CloseRoutine());
            return;
        }
        if (this.link == null)
        {
            if (level != null)
            {
                field.text = "Loading...";
                Lambdas.GetShareableLink(level, (link) =>
                {
                    this.link = link;
                    field.text = link;
                });
            }
            else
            {
                field.text = "N/A";
            }
        }
        else
        {
            field.text = link;
        }
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        open = true;
        while (popupObject.localScale.x < 1)
        {
            popupObject.localScale += Vector3.one * Time.deltaTime * 5;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CloseRoutine()
    {
        open = false;
        while (popupObject.localScale.x > 0)
        {
            popupObject.localScale -= Vector3.one * Time.deltaTime * 5;
            yield return new WaitForEndOfFrame();
        }
    }
}
