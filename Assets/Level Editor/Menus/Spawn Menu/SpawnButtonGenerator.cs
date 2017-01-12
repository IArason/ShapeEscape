using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpawnButtonGenerator : MonoBehaviour {

    [SerializeField]
    GameObject buttonPrefab;

    [SerializeField]
    string[] resourcePaths;

    [SerializeField]
    [Tooltip("The total width of the menu")]
    float totalHorizontalSpace;
    [SerializeField]
    Vector2 offsets = new Vector2(10, 50);
    [SerializeField]
    RectTransform parentObject;

    void Start()
    {
        List<GameObject> rs = new List<GameObject>();
        foreach(string path in resourcePaths)
        {
            rs.AddRange(Resources.LoadAll<GameObject>(path));
        }

        float width = ((RectTransform)buttonPrefab.transform).rect.width;
        float height = ((RectTransform)buttonPrefab.transform).rect.height;
        float xPos = 0;
        float yPos = 0;
        for(int i = 0; i < rs.Count; i++)
        {
            var spawnable = rs[i].GetComponent<Spawnable>();
            if (spawnable == null)
            {
                Debug.LogWarning(rs[i] + " does not have a spawnable component.");
                rs.RemoveAt(i);
                i--;
                continue;
            }

            var rt = Instantiate(buttonPrefab).GetComponent<RectTransform>();
            rt.SetParent(parentObject, false);
            rt.anchoredPosition = new Vector2(xPos, yPos);
            
            if (spawnable.sprite != null)
            {
                var img = rt.GetComponent<Image>();
                img.sprite = rs[i].GetComponent<Spawnable>().sprite;
            }

            var text = rt.GetComponentInChildren<Text>();
            text.text = spawnable.label != "" ? spawnable.label : "Missing Label";

            var spawnPrefab = rs[i];
            var offset = new Vector2(totalHorizontalSpace, 0);
            rt.GetComponent<Button>().onClick.AddListener(() => LevelEditor.InstantiateObject(spawnPrefab, offset));


            if (xPos + offsets.x + width > totalHorizontalSpace)
            {
                xPos = 0;
                yPos -= offsets.y + height;
            }
            else
            {
                xPos += offsets.x + width;
            }
        }
    }



}
