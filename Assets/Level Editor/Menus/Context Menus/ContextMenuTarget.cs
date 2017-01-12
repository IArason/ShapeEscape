using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ContextMenuTarget : MonoBehaviour {

    [SerializeField]
    List<GameObject> optionPrefabs;

    public List<GameObject> MenuOptions {
        get
        {
            return optionPrefabs;
        }
    }

    public void AddOption(GameObject option)
    {
        if (!optionPrefabs.Contains(option))
            optionPrefabs.Add(option);
    }
}
