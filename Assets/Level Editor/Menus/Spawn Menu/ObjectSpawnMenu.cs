using UnityEngine;
using System.Collections;

public class ObjectSpawnMenu : MonoBehaviour {

    [SerializeField]
    Vector3 openDisplacement = new Vector2(-400f, 0f);
    [SerializeField]
    float openDuration = 0.5f;
    
    [SerializeField]
    Transform[] pages;

    bool open = false;
    RectTransform rt;
    Coroutine movementCoroutine;

    Vector2 startPos;

    void Awake()
    {
        rt = (RectTransform)transform;
        startPos = rt.localPosition;
    }

    void Update()
    {
        // If clicked outside the menu, close it
        if(open && Input.GetMouseButtonDown(0) && !LevelEditorUtils.MouseOverUIElement())
        {
            Close();
        }
    }

    /// <summary>
    /// Creates all the menu elements by loading them from Resources/LevelEditor
    /// </summary>
    public void Initialize()
    {

    }

    public void Open()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(TranslateBy(openDisplacement, false));
        open = true;
    }

    public void Close()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(TranslateBy(-openDisplacement, true));

        open = false;
    }

    public void SetPage(int page)
    {
        for(int i = 0; i < pages.Length; i++)
        {
            pages[i].SetSiblingIndex(i);
            pages[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }
        pages[page].SetAsLastSibling();
        pages[page].localPosition -= Vector3.right * 5f;

        if (!open) Open();
    }

    IEnumerator TranslateBy(Vector2 pos, bool closing)
    {
        float timer = 0f;
        var oldPos = rt.anchoredPosition;
        while(openDuration > timer)
        {
            rt.anchoredPosition = Vector2.Lerp(oldPos, oldPos + pos, timer / openDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rt.anchoredPosition = pos + oldPos;

        if(closing)
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].SetSiblingIndex(i);
                pages[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }

        movementCoroutine = null;
    }
}