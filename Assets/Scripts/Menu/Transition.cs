using UnityEngine;
using System.Collections;

public class Transition : MonoBehaviour {

    public GameObject[] transitonPrefabs;
    public float duration = 0.2f;
    public float speed = 1;

    [SerializeField]
    bool introOnAwake = false;

    void Awake()
    {
        if (introOnAwake) IntroTransition();
    }
    
    public void DoTransition()
    {
        StartCoroutine(Grow(duration/2, true));
    }

    [BitStrap.Button]
    public void OutroTransition()
    {
        StartCoroutine(Grow(duration / 2, false));
    }

    public void IntroTransition()
    {
        StartCoroutine(Shrink(duration / 2));
    }

    IEnumerator Grow(float duration, bool outToo)
    {
        var go = Instantiate(transitonPrefabs[Random.Range(0, transitonPrefabs.Length)]);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.forward;
        go.transform.localScale = Vector3.zero;
        var timer = 0f;
        while(timer < duration)
        {
            yield return new WaitForEndOfFrame();
            go.transform.localScale = Vector3.one * 8 * (timer / duration);
            go.transform.localEulerAngles += Vector3.forward * Time.deltaTime * speed * 360;
            timer += Time.deltaTime;
        }
        if (outToo)
            StartCoroutine(Shrink(go, duration));
    }

    IEnumerator Shrink(GameObject go, float duration)
    {
        var timer = duration;
        while (go.transform.localScale.x > 0)
        {
            timer -= Time.deltaTime;
            go.transform.localScale = Vector3.one * 8 * (timer / duration);
            go.transform.localEulerAngles += Vector3.forward * Time.deltaTime * speed * 360;
            yield return new WaitForEndOfFrame();
        }
        Destroy(go);
    }

    IEnumerator Shrink(float duration)
    {
        var go = Instantiate(transitonPrefabs[Random.Range(0, transitonPrefabs.Length)]);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.forward;
        go.transform.localScale = Vector3.one * 12;
        var timer = duration;
        while (go.transform.localScale.x > 0)
        {
            timer -= Time.deltaTime;
            go.transform.localScale = Vector3.one * 12 * (timer / duration);
            go.transform.localEulerAngles += Vector3.forward * Time.deltaTime * speed * 360;
            yield return new WaitForEndOfFrame();
        }
        Destroy(go);
    }
}
