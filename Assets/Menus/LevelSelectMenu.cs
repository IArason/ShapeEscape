using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class LevelSelectMenu : MonoBehaviour {

    [SerializeField]
    GameObject levelElementPrefab;

    [SerializeField]
    float ySpacing = 50f;

    GameObject[] levelElements;

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        var levels = FileUtils.GetSavedLevels();

        int i = 0;
        levelElements = new GameObject[levels.Count];
        foreach(KeyValuePair<string, FileInfo> pair in levels)
        {
            levelElements[i] = Instantiate(levelElementPrefab);
            levelElements[i].transform.SetParent(transform);
            ((RectTransform)levelElements[i].transform).anchoredPosition = Vector3.down * (i * ySpacing);

            // Add ref to level serializer
            /*
            levelElements[i].GetComponent<Button>().onClick.AddListener(
                () => LevelSerializer.Instance.LoadLevelFromFile(pair.Value));
                */

            levelElements[i].GetComponentInChildren<Text>().text = pair.Key;

            i++;
        }
    }
}
