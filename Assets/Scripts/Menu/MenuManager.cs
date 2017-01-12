using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MenuManager : Singleton<MenuManager> {

    public List<MenuNamePair> menus;

    RectTransform openMenu;
    bool opening = false;

    void Awake()
    {
        openMenu = menus[0].menuTransform;
        for(int i = 1; i < menus.Count; i++)
        {
            menus[i].menuTransform.anchoredPosition = Vector3.right * Screen.width * 2.2f;
        }
    }

    public void OpenMenu(string menu)
    {
        var open = menus.First(x => x.menuName == menu);
        open.menuTransform.anchoredPosition = Vector3.right * 5000;
        StartCoroutine(SmoothOpen(open));
        StartCoroutine(SmoothClose(openMenu, 0.25f));
    }

    IEnumerator SmoothOpen(MenuNamePair t)
    {
        if (t == null) yield break;
        float time = t.openTime;
        while (opening)
        {
            yield return new WaitForEndOfFrame();
        }
        
        t.menuTransform.SendMessage("OnReadyOpen", SendMessageOptions.DontRequireReceiver);

        yield return new WaitForSeconds(time / 4);
        var start = new Vector2(t.menuTransform.position.x, 0);
        for(float i = 0; i < time; i+= Time.deltaTime)
        {
            float val = 0;
            switch(t.easeType)
            {
                case EaseType.Bounce:
                    val = iTween.easeOutBounce(0, 1, i / time);
                    break;
                case EaseType.Elastic:
                    val = iTween.easeOutElastic(0, 1, i / time);
                    break;
                case EaseType.Quad:
                    val = iTween.easeOutQuad(0, 1, i / time);
                    break;
                case EaseType.Linear:
                    val = iTween.linear(0, 1, i / time);
                    break;
            }

            t.menuTransform.anchoredPosition = Vector2.LerpUnclamped(start, Vector2.zero,
                val);
            
            yield return new WaitForEndOfFrame();
        }
        t.menuTransform.anchoredPosition = Vector2.zero;
        t.menuTransform.SendMessage("OnOpened", SendMessageOptions.DontRequireReceiver);
        opening = false;
        openMenu = t.menuTransform;
    }

    IEnumerator SmoothClose(RectTransform t, float time)
    {
        if (t == null) yield break;
        var start = t.anchoredPosition;
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            t.anchoredPosition = Vector2.LerpUnclamped(start, (Vector2.left * Screen.width * 2.2f),
            iTween.easeInQuad(0, 1, i / time));
            
            yield return new WaitForEndOfFrame();
        }
        t.anchoredPosition = Vector2.left * Screen.width * 2.2f;
        t.SendMessage("OnClosed", SendMessageOptions.DontRequireReceiver);
    }

    [System.Serializable]
    public class MenuNamePair
    {
        public string menuName;
        public RectTransform menuTransform;
        public EaseType easeType = EaseType.Quad;
        public float openTime = 1;
    }

    public enum EaseType
    {
        Bounce,
        Elastic,
        Quad,
        Linear
    }
}
