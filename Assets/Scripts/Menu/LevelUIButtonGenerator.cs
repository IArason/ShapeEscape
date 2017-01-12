using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class LevelUIButtonGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject levelLoadButtonPrefab;

    [SerializeField]
    GameObject boundaryDefinition;

    [SerializeField]
    float xOffset;
    [SerializeField]
    float ySpacing;

    HashSet<GameObject> buttons = new HashSet<GameObject>();
    
    public void OnClick()
    {
        DestroyButtons();

        var files = FileUtils.GetLevelsInPersistentPath();
        for(int i = 0; i < files.Length; i++)
        {
            var go = (GameObject)Instantiate(levelLoadButtonPrefab);
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(xOffset, -ySpacing * i, 0);
            go.GetComponentInChildren<Text>().text = files[i];
            var levelName = files[i];
            go.GetComponent<Button>().onClick.AddListener(() => LevelSerializer.Instance.LoadLevelFromFile(levelName));
            go.GetComponent<Button>().onClick.AddListener(() => LevelSerializer.Instance.PlayLevel(false));
            go.GetComponent<Button>().onClick.AddListener(() => DestroyButtons());
            buttons.Add(go);
        }
    }

    void Update()
    {

        // If clicked outside, close self.
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // If none of the results contains self, close self.
            if (!results.Any(x => x.gameObject == boundaryDefinition))
            {
                DestroyButtons();
            }
        }
    }

    public void DestroyButtons()
    {
        foreach(GameObject go in buttons)
        {
            Destroy(go);
        }
    }
}
