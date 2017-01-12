using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Community : MonoBehaviour {

    [SerializeField]
    GameObject levelListPrefab;

    [SerializeField]
    Text message;

    [SerializeField]
    RectTransform topLevels;
    [SerializeField]
    RectTransform newLevels;
    
    List<GameObject> newLevelListings = new List<GameObject>();
    
    void OnReadyOpen()
    {
        foreach(var v in GetComponentsInChildren<Button>())
        {
            v.interactable = true;
        }

        message.text = "Sup " + Lambdas.cachedPlayerName + "?";

        Lambdas.GetLevels(5, Lambdas.SortBy.AgeAsc, (levels) =>
        {
            PopulateNew(levels);
        });

        Lambdas.GetLevels(5, Lambdas.SortBy.Popularity, (levels) =>
        {
            PopulateTop(levels);
        });
    }

    void PopulateTop(List<SharedLevel> levels)
    {
        for (int i = 0; i < Mathf.Min(levels.Count, 5); i++)
        {
            var go = Instantiate(levelListPrefab);
            go.transform.SetParent(topLevels, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -140 - (110 * i));

            var level = levels[i];
            var btn = go.GetComponent<LevelInfoData>();
            btn.SetLevelData(level);

            btn.SetOnPlay(() =>
            {
                level.ToLevel((lv) =>
                {
                    if (lv == null)
                    {
                        return;
                    }

                    MenuManager.Instance.OpenMenu("LoadingLevel");
                    LoadingLevel.Instance.SetLoadMessage(level);

                    StartCoroutine(LevelSerializer.Instance.PlaySharedLevel(lv));
                });
            });
        }
    }

    void PopulateNew(List<SharedLevel> levels)
    {
        foreach (GameObject g in newLevelListings)
            Destroy(g);

        for (int i = 0; i < Mathf.Min(levels.Count, 5); i++)
        {
            var go = Instantiate(levelListPrefab);
            go.transform.SetParent(newLevels, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -140 - (110 * i));
            
            var level = levels[i];
            var btn = go.GetComponent<LevelInfoData>();
            btn.SetLevelData(level);

            btn.SetOnPlay(() =>
            {
                level.ToLevel((lv) =>
                {
                    if (lv == null)
                    {
                        return;
                    }

                    MenuManager.Instance.OpenMenu("LoadingLevel");
                    LoadingLevel.Instance.SetLoadMessage(level);
                    
                    StartCoroutine(LevelSerializer.Instance.PlaySharedLevel(lv));
                });
            });

            newLevelListings.Add(go);
        }

    }
}
