using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using CielaSpike;

public class MyLevels : MonoBehaviour {

    [SerializeField]
    GameObject levelListPrefab;

    [SerializeField]
    Transform levelParent;

    [SerializeField]
    Button rightArrow;
    [SerializeField]
    Button leftArrow;

    List<SharedLevel> sharedLevels;
    List<Level> levels;

    List<GameObject> listings = new List<GameObject>();

    [SerializeField]
    int page = 0;

    void OnReadyOpen()
    {
        page = 0;
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        Task task;
        string path = Application.persistentDataPath + "/CustomLevels/";

        this.StartCoroutineAsync(LoadLevels(path), out task);
        while (task.State == TaskState.Running)
        {
            yield return new WaitForEndOfFrame();
        }
        SetPage(page);

        Lambdas.GetPlayerLevels((lvls) =>
        {
            sharedLevels = lvls;
            var lst = listings.Select(x => x.GetComponent<LevelInfoData>());
            foreach (SharedLevel level in lvls)
            {
                foreach (LevelInfoData v in lst)
                {
                    if(v.levelNameText.text == level.LevelName)
                    {
                        v.SetLevelData(level);
                    }
                }
            }
        });
    }

    IEnumerator LoadLevels(string path)
    {
        levels = FileUtils.GetAllLevelsInPath(path);
        yield break;
    }

    void SetPage(int page)
    {
        foreach (GameObject g in listings)
            Destroy(g);

        listings = new List<GameObject>();

        for (int i = page * 5; i < Mathf.Min(levels.Count, 5 * page + 5); i++)
        {
            var go = Instantiate(levelListPrefab);
            go.transform.SetParent(levelParent, false);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -130 - (110 * i) + page * 5 * 110);
            
            Level level = levels[i];
            var btn = go.GetComponent<LevelInfoData>();
            btn.SetLevelData(level);
            btn.SetOnPlay(() =>
            {
                LevelSerializer.Instance.PlayLevel(level, false);
            });

            listings.Add(go);
        }
        
        rightArrow.interactable = (levels.Count > page * 5 + 5);
        leftArrow.interactable = (page != 0);
    }

    public void RightArrow()
    {
        page++;
        SetPage(page);
    }

    public void LeftArrow()
    {
        page--;
        SetPage(page);
    }
}
